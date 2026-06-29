# Stance Stamina System — Implementation Plan

## Status: Phases 1–5 Complete

---

## Overview

The stance system needs to drive `HandsStamina` state changes in response to which stance is active. The goal is a clean, event-driven design where each stance **declares its own stamina behaviour** (drain vs. regen and at what rate), and a central `StanceStaminaHandler` responds to stance lifecycle events rather than polling every frame.

**Scope**: Only the five currently implemented stances that meaningfully affect stamina: `HighReady`, `LowReady`, `ShortStock`, `PatrolStance`, `ActiveAiming`. `LeftShoulder` and no-stance (idle) are out of scope except for the idle drain config toggle.

---

## Legacy Code Analysis (`StanceStaminaHandler.cs`)

The old `SetStanceStamina` method ran every frame and had the following problems:

### What it did (requirements extracted)
- **Regen stances**: `HighReady`, `LowReady`, `PatrolStance`, `ShortStock` — arm stamina regenerates.
- **Drain stances**: `ActiveAiming`, idle (if `EnableIdleStamDrain` config is on).
- **Drain overrides regen**: The old check for `AimStateInstance.IsAiming` was a **workaround** — because the old system never reset stance state when aiming, it had to manually check aim state to suppress regen. **This is no longer needed**: `StanceInputHandler.OnADSToggled()` now calls `_stanceState.CancelAll()` when aiming begins, so `ActiveStance` will already be `null` by the time the stamina system could observe it.
- **`player.Physical.Aim(float mass)`** called with `1f` to activate the game's arm stamina consumption, `0f` to deactivate. Called on state *change* only, not every frame.
- **`HandsStamina.Multiplier`** set to scale the drain/regen rate, factoring in weapon ergo, health, skills, and config.
- **Change detection** via storing previous booleans — fragile and verbose; to be replaced by events.

### What it referenced but was not implemented (discard)
- `EStance` (old enum, replaced by `EStanceType`)
- `IsMounting`, `IsBracing`, `IsDoingTacSprint`, `FiringStateInstance.IsFiringFromStance` — unimplemented systems
- `DidWeaponSwap`, `HaveResetStamDrain`, `CanResetAimDrain` — manual dirty flags, unnecessary with events

---

## Requirements for the New System

### Functional Requirements

1. **Each relevant stance defines its own stamina mode** (drain or regen) and base rate.
2. **No active stance (idle)**: neutral by default. Optional drain if `EnableIdleStamDrain` config is true.
3. **Aiming cancels the stance** (`CancelAll()` is already called by `StanceInputHandler.OnADSToggled()`), so when aiming begins, `ActiveStance` becomes `null` → stamina naturally falls to idle/neutral. **No explicit aim-state check needed in the stamina handler.**
4. **Direct `HandsStamina.Current` modification**: regen and drain are applied by adding/subtracting a computed rate per frame in `RunOnUpdate`. No consumption system interaction required.
5. **Regen suppression ("freeze")**: when regen should be suppressed without causing drain, write `HandsStamina.DisableRestoration = Time.time + 1f` each frame. This blocks `SelfRestoration` while leaving all game drain consumptions (vault, melee, etc.) working at full rate.
6. **Weapon ergonomics** scale both drain and regen rates.
7. **Config toggle**: `EnableIdleStamDrain` — when true, no-stance state drains rather than doing nothing.

### Non-Functional Requirements

- **Event-driven**: `StanceStaminaHandler` reacts to stance lifecycle events. No per-frame polling.
- **Stances own their data**: Mode and rate are virtual properties on `StanceBase`, not a switch in the handler.
- **Clean lifecycle**: Subscribe in `RunOnAwake`, unsubscribe in `RunOnDestroy`.

---

## Proposed Stamina Mode Design

### `EStaminaMode` enum

```csharp
public enum EStaminaMode
{
    Neutral, // do nothing — game handles HandsStamina fully
    Regen,   // add rate * dt to Current each frame (clamped to TotalCapacity)
    Drain,   // subtract rate * dt from Current each frame (clamped to 0)
    Freeze   // write DisableRestoration = Time.time + 1f each frame — suppresses SelfRestoration
             // while leaving all game drain consumptions (vault, melee, aim) unaffected
}
```

### Stamina data on `IStance` / `StanceBase`

Each stance exposes:

```csharp
// What this stance does to HandsStamina while active
virtual EStaminaMode StaminaMode { get; }

// Base rate in stamina points per second (positive; direction is implied by StaminaMode)
virtual float StaminaRate { get; }
```

Default in `StanceBase`: `StaminaMode = EStaminaMode.Neutral`, `StaminaRate = 0f`.

Per-stance overrides (implemented stances only):

| Stance       | Mode    | Base Rate |
|--------------|---------|-----------|
| HighReady    | Regen   | 1.85      |
| LowReady     | Regen   | 2.4       |
| PatrolStance | Regen   | 4.0       |
| ShortStock   | Regen   | 1.3       |
| ActiveAiming | Drain   | 0.075     |

---

## Event Sources Required

The stamina handler only needs to react to **stance lifecycle** — no aim, sprint, or firing events required, because those already flow through `CancelAll()` → stance exit.

| Event | Source | Purpose |
|-------|--------|---------|
| `StanceEvents.OnStanceEntered(IStance)` | `StanceBase.OnEnter()` | Apply the entering stance's stamina mode |
| `StanceEvents.OnStanceExited` | `StanceBase.OnExit()` (or `StanceState`) | Stance gone → apply idle mode |

### Why `OnEnter`/`OnExit` are sufficient

- When a stance becomes active, `stance.OnEnter()` fires → raise `OnStanceEntered` with the stance → handler applies that stance's `StaminaMode` and `StaminaRate`.
- When a stance exits (including exit triggered by `CancelAll()` which is called on ADS, weapon swap, shot fired), `stance.OnExit()` fires → raise `OnStanceExited` → handler applies idle mode (neutral or drain per config).
- No need to track aim state, sprint state, or any other context.

> **Note**: `StanceEvents` is a new static event class to be created. `StanceBase.OnEnter()` and `OnExit()` are already virtual hooks that all stance classes use — they just need to raise these events.

---

## `StanceStaminaHandler` Responsibilities (new design)

1. Store a `_rate` float (stamina points/sec; positive = regen, negative = drain, `0` = neutral) and a `_freeze` bool.
2. Subscribe to `StanceEvents.OnStanceEntered` and `StanceEvents.OnStanceExited` in `RunOnAwake`.
3. On `OnStanceEntered(stance)`: read `stance.StaminaMode` and `stance.StaminaRate`, set `_rate` and `_freeze` accordingly.
4. On `OnStanceExited`: if `EnableIdleStamDrain`, set `_rate` to the negative idle drain rate; otherwise set `_rate = 0f` and `_freeze = false`.
5. `RunOnUpdate(dt)`:
   - If `_rate > 0` (regen): `HandsStamina.Current = Mathf.Min(Current + _rate * dt, TotalCapacity)`
   - If `_rate < 0` (drain): `HandsStamina.Current = Mathf.Max(Current + _rate * dt, 0f)`
   - If `_freeze`: `HandsStamina.DisableRestoration = Time.time + 1f`
   - If `_rate == 0 && !_freeze`: do nothing
6. `ComputeRate(baseRate)`: `baseRate * ergoScale * configModifier` (health/skill scaling deferred to later phases).
7. No `Multiplier` modification. No `Aim()` calls. No consumption registration.

---

---

## Phase 2 — Game Stamina System Analysis & Mod Strategy

### The Consumption System (`GClass773` + `GClass774`)

The game drives all stamina changes through a **consumption list** pattern. Each stamina pool (`Stamina`, `HandsStamina`, `Oxygen`) is a `GClass774` that owns a list of active `GClass773` consumption objects. Every frame `GClass774.Process(dt)` is called to tick the pool.

**`GClass773`** — a consumption descriptor:
- `Delta` — lazy-evaluated `float` function: the drain rate per second
- `PrimaryTarget` — flags which pool(s) to drain (`Base` / `Hands` / `Oxygen`)
- `AllowsRestoration` — if `false`, blocks natural self-restoration for `Downtime` seconds after the consumption stops
- `Downtime` — cooldown before regen resumes after drain stops
- `SetActive(physical, bool)` — true: calls `AddConsumption` on the target pool(s); false: invokes the cleanup action to remove it

**`GClass774.Process(dt)`** — the stamina tick (simplified):
```
// 1. Sum all active Delta values
totalDrain = sum of all Consumptions[i].Delta

// 2. If any non-restorable drain is active, block regen
if (anyNonRestorableDrain) DisableRestoration = now + Downtime + (1 - normalizedValue)

// 3. Apply drain (Multiplier scales drain)
Current -= totalDrain * dt * Multiplier

// 4. Apply BuffRestoration (always runs, ignores DisableRestoration)
Current += BuffRestoration * dt

// 5. Apply SelfRestoration (gated by DisableRestoration; Multiplier scales regen)
if (now > DisableRestoration)
    Current += SelfRestoration * dt * Multiplier
```

### The `Aim` Consumption — What `mass` Actually Means

`Consumptions[EConsumptionType.Aim]`:
- `PrimaryTarget = EConsumptionTarget.Hands` — only drains `HandsStamina`
- `AllowsRestoration = false` — blocks regen after aim stops
- `Delta = Class436.method_10()`:

```csharp
return Mathf.Sqrt(Float_5)          // sqrt of mass (Float_5 = the mass arg passed to Aim())
    * StaminaParameters.AimDrainRate  // global base drain rate from backend config
    * armDamageMult                   // 1x / 1.5x / 2x for arm damage
    * Float_6[pose]                   // AimConsumptionByPose array (standing/crouch/prone)
    * (1 - StrengthBuffAimFatigue)    // skill reduction
    * Single_0                        // (1 / overweight factor)
    * mountingMult;                   // bipod/mounting reduction
```

`Float_5` stores the `mass` passed to `Aim(float mass)`. The formula uses `Mathf.Sqrt(mass)` — so passing `1f` (as legacy code did) always gives `√1 = 1`, removing the weapon's mass influence entirely. **The game passes the actual weapon's ergonomics-derived mass here** — this is the correct value to preserve.

**The legacy code's mistake**: calling `Aim(1f)` bypassed the weapon mass calculation, producing a flat drain rate instead of ergo-dependent drain.

### `HandsStamina.Multiplier` — What It Actually Does

`Multiplier` is applied in `Process(dt)` to **both drain and regen**:
- `Current -= totalDrain * dt * Multiplier` — scales drain up/down
- `Current += SelfRestoration * dt * Multiplier` — scales regen up/down equally

Setting `Multiplier = 2.4f` means 2.4× faster regen **and** 2.4× faster drain from any currently active consumptions. During a regen stance (where ADS is cancelled so the Aim consumption is inactive), there are no significant active consumptions, so only the regen path is affected — which is the intended outcome.

Setting `Multiplier = 0f` completely halts both drain and regen (freezes the pool at its current value). This is the correct "neutral" behaviour.

Setting `Multiplier = 1f` restores vanilla behaviour.

### Interference Risk: `Aim()` Called by Game

The game's own `FirearmController` calls `player.Physical.Aim(weaponMass)` when the player aims. **Stances are already cancelled (`CancelAll()`) before ADS activates**, so there is no simultaneous stance + ADS state. The game's `Aim()` will not conflict with our stamina state as long as we reset `Multiplier` to `1f` on stance exit (before ADS restores the stance).

### Proposed Mod Strategy (Final)

#### The Multiplier Problem

`GClass774.Multiplier` scales **both drain and regen** in every code path:
- `Process(dt)`: `Current -= totalDelta * dt * Multiplier` (continuous drain)
- `Consume(consumption)`: `num = consumption.Delta.Value * Multiplier` (one-shot drain)

This means raising `Multiplier` for regen also multiplies **any Hands-targeting consumption that fires while the stance is active**:

| Consumption | Targets Hands? | Affected by our Multiplier? |
|-------------|---------------|---------------------------|
| Wound (bullet hit) | **No** — `Base` only | No |
| Melee (player punches) | Yes | Yes — unintended |
| StandUp | Yes | Yes — unintended |
| Prone (continuous) | Yes | Yes — unintended |
| FastWeaponSwitch | Yes | Yes — unintended |
| VaultHands | Yes | Yes — unintended |
| ClimbHands | Yes | Yes — unintended |

A player in PatrolStance (Multiplier = 4.0) who vaults would drain 4× more arm stamina than normal. **Do not use `Multiplier` for regen or drain.**

#### Correct Approach: Direct `Current` Modification

Modify `HandsStamina.Current` directly each frame in `RunOnUpdate`:
- No `Multiplier` involvement — zero side effects on vault/melee/standup drain
- No `Aim()` / `Float_5` contamination
- No consumption registration or cleanup required

| State | Mechanism |
|-------|-----------|
| Regen stance | `Current += rate * dt` per frame, clamped to `TotalCapacity` |
| Drain stance (`ActiveAiming`) | `Current -= rate * dt` per frame, clamped to `0f` |
| Freeze (regen suppressed, drain allowed) | `DisableRestoration = Time.time + 1f` per frame |
| Idle, `EnableIdleStamDrain = false` | Do nothing — game handles it fully |
| Idle, `EnableIdleStamDrain = true` | `Current -= idleRate * dt` per frame |

#### Regen Suppression via `DisableRestoration`

`HandsStamina.DisableRestoration` is a `public float` storing a `Time.time` threshold. The game writes it after non-restorable drain consumptions stop (the Downtime mechanic) to delay regen resumption. Setting it to `Time.time + 1f` each frame perpetually blocks `SelfRestoration`.

`DisableRestoration` has no effect on drain — `Consume()` doesn't check it, and the consumption list drains independently. Vault, melee, and aim drain remain fully functional while regen is frozen.

In vanilla, `HandsStamina.BuffRestoration` is always `0f`, so blocking `SelfRestoration` via `DisableRestoration` effectively freezes all natural regen.

This "Freeze" mode is available for future use (e.g. sprinting while a stance was recently active) but is not required for the five implemented stances — stances are already cancelled by `CancelAll()` before sprint/ADS activates.

#### What We Must NOT Do
- Do not use `HandsStamina.Multiplier` — scales all Hands-targeting consumptions (vault, melee, fast-switch, prone, stand-up) as an unintended side effect
- Do not set `Multiplier = 0f` to freeze — also blocks `Consume()` one-shot drains via early return
- Do not call `Aim(1f)` — corrupts `Float_5`, breaks the game's drain delta on subsequent ADS
- Do not leave any `Current` modification active or `DisableRestoration` perpetually written after `RunOnDestroy`

---

## Player Reference

The handler needs `Player` at event-response time. Cache it from `PlayerStateInstance.Player` in `RunOnAwake` — the same pattern used by other handlers in this project.

---

## Out of Scope (deferred to later phases)

- Regular `Stamina` (leg stamina) — old code only touched `HandsStamina`.
- Mounting/bracing stamina overrides — depends on those systems being implemented.
- Prone state override.
- `HoldBreath` drain interaction.
- Health and skill scaling of rates (`HealthStateInstance.HealthStamRegenFactor`, `SkillStateInstance.StrengthSkillAimBuff`) — wire in Phase 6.

---

## Phase Roadmap

| Phase | Work |
|-------|------|
| **1** | *(complete)* Context, requirements, legacy code analysis |
| **2** | *(complete)* Game stamina system analysis; mechanism design (direct `Current` + `DisableRestoration` freeze) |
| **3** | *(complete — see below)* `EStaminaMode` enum; `StaminaMode`/`StaminaRate` on `IStance`/`StanceBase`; per-stance overrides |
| **4** | *(complete — see below)* `StanceEvents` static class; raise events from `StanceBase.OnEnter()`/`OnExit()` |
| **5** | *(complete — see below)* Rewrite `StanceStaminaHandler` |
| **6** | Config integration (`EnableIdleStamDrain`, `IdleStamDrainModi`); ergo scaling — **already wired in Phase 5** |
| **7** | Health and skill rate scaling — stubs marked in `StanceStaminaHandler` |
| **8** | Testing, tuning of base rates; cleanup of old dead code in `StanceStaminaHandler` |

---

## Phase 3 — `EStaminaMode` + Stamina Data on Stances

### New file: `Enums/EStaminaMode.cs`

```csharp
namespace StanceOverhaul.Enums;

public enum EStaminaMode
{
    Neutral, // do nothing — game handles HandsStamina fully
    Regen,   // add rate * dt to Current each frame (clamped to TotalCapacity)
    Drain,   // subtract rate * dt from Current each frame (clamped to 0)
    Freeze   // write DisableRestoration = Time.time + 1f each frame — suppresses SelfRestoration
             // while leaving all game drain consumptions (vault, melee, aim) unaffected
}
```

### Changes to `IStance.cs`

Add after `StanceType`:

```csharp
using StanceOverhaul.Enums;

public EStaminaMode StaminaMode { get; }
public float StaminaRate { get; }
```

### Changes to `StanceBase.cs`

Add using and virtual defaults after `StanceType`:

```csharp
using StanceOverhaul.Enums;

public virtual EStaminaMode StaminaMode => EStaminaMode.Neutral;
public virtual float StaminaRate => 0f;
```

### Per-stance overrides

Add both lines directly after `StanceType` in each class.

| Stance | `StaminaMode` | `StaminaRate` |
|--------|--------------|--------------|
| `HighReady` | `Regen` | `1.85f` |
| `LowReady` | `Regen` | `2.4f` |
| `PatrolStance` | `Regen` | `4.0f` |
| `ShortStock` | `Regen` | `1.3f` |
| `ActiveAim` | `Drain` | `0.075f` |

> **Note:** These rates were `Multiplier` multipliers in the old system. They are now additive stamina pts/sec on top of natural regen and will need re-tuning in Phase 8.

Example (`HighReady.cs`):
```csharp
public override EStanceType StanceType => EStanceType.HighReady;
public override EStaminaMode StaminaMode => EStaminaMode.Regen;
public override float StaminaRate => 1.85f;
```

---

## Phase 4 — `StanceEvents` + Wire `StanceBase`

### New file: `Events/StanceEvents.cs`

```csharp
using RealismCommonLib.Events;
using StanceOverhaul.Stances;
using System;

namespace StanceOverhaul.Events;

public static class StanceEvents
{
    /// <summary>Raised when a stance's OnEnter() fires. Carries the entering stance.</summary>
    public static event Action<IStance>? OnStanceEntered;

    /// <summary>Raised when a stance's OnExit() fires. Handler should revert to idle behaviour.</summary>
    public static event Action? OnStanceExited;

    internal static void RaiseOnStanceEntered(IStance stance)
    {
        BaseEventHandler.RaiseEvent(OnStanceEntered, stance);
    }

    internal static void RaiseOnStanceExited()
    {
        BaseEventHandler.RaiseEvent(OnStanceExited);
    }
}
```

### Changes to `StanceBase.OnEnter` / `OnExit`

No stance overrides these hooks, so changes are confined to `StanceBase`. Add using and update the two virtual methods:

```csharp
using StanceOverhaul.Events;

public virtual void OnEnter()
{
    StanceEvents.RaiseOnStanceEntered(this);
}

public virtual void OnExit()
{
    StanceEvents.RaiseOnStanceExited();
}
```

Any future stance that overrides `OnEnter`/`OnExit` must call `base.OnEnter()` / `base.OnExit()` to preserve event propagation.

---

## Phase 5 — Rewrite `StanceStaminaHandler`

Full replacement of `Handlers/StanceStaminaHandler.cs`. All legacy code is discarded.

**Why no `Multiplier` or `Aim()` calls:** see the Multiplier Problem section in Phase 2.

**Rate units:** `_rate` is stamina points per second added to or subtracted from `HandsStamina.Current`. The sign encodes direction; `StaminaRate` on stances is always positive.

**Player access:** `PlayerStateInstance.Player?.Physical` via `static using RealismCommonLib.Plugin` — no caching needed, safe per-frame.

```csharp
using StanceOverhaul.Enums;
using StanceOverhaul.Events;
using StanceOverhaul.Handlers;
using StanceOverhaul.Stances;
using UnityEngine;
using static RealismCommonLib.Plugin;

namespace StanceOverhaul.Controllers.StateControllers;

public class StanceStaminaHandler : IControllerHelper
{
    // Positive = regen pts/sec, negative = drain pts/sec, 0 = do nothing.
    private float _rate = 0f;
    // When true, write DisableRestoration each frame to suppress SelfRestoration.
    private bool _freeze = false;

    public void RunOnAwake()
    {
        StanceEvents.OnStanceEntered += OnStanceEntered;
        StanceEvents.OnStanceExited  += OnStanceExited;
    }

    public void RunOnDestroy()
    {
        StanceEvents.OnStanceEntered -= OnStanceEntered;
        StanceEvents.OnStanceExited  -= OnStanceExited;
        _rate   = 0f;
        _freeze = false;
    }

    public void RunOnUpdate(float deltaTime)
    {
        if (!PluginConfig.EnableStanceStamChanges.Value) return;

        var physical = PlayerStateInstance.Player?.Physical;
        if (physical == null) return;

        var hs = physical.HandsStamina;

        if (_rate > 0f)
        {
            hs.Current = Mathf.Min(hs.Current + _rate * deltaTime, hs.TotalCapacity);
        }
        else if (_rate < 0f)
        {
            hs.Current = Mathf.Max(hs.Current + _rate * deltaTime, 0f);
        }

        if (_freeze)
        {
            // Perpetually gate SelfRestoration without touching drain consumptions.
            hs.DisableRestoration = Time.time + 1f;
        }
    }

    // ── event handlers ────────────────────────────────────────────────────────

    private void OnStanceEntered(IStance stance)
    {
        switch (stance.StaminaMode)
        {
            case EStaminaMode.Regen:
                _rate   = ComputeRegenRate(stance.StaminaRate);
                _freeze = false;
                break;

            case EStaminaMode.Drain:
                _rate   = -ComputeDrainRate(stance.StaminaRate);
                _freeze = false;
                break;

            case EStaminaMode.Freeze:
                _rate   = 0f;
                _freeze = true;
                break;

            default: // Neutral
                _rate   = 0f;
                _freeze = false;
                break;
        }
    }

    private void OnStanceExited()
    {
        if (PluginConfig.EnableIdleStamDrain.Value)
        {
            _rate   = -ComputeIdleDrainRate();
            _freeze = false;
        }
        else
        {
            _rate   = 0f;
            _freeze = false;
        }
    }

    // ── rate computation ──────────────────────────────────────────────────────
    // Phase 7: add HealthStateInstance.HealthStamRegenFactor and
    //          SkillStateInstance.StrengthSkillAimBuff where noted.

    private float ComputeRegenRate(float baseRate)
    {
        float bullpup   = WeaponStateInstance.IsBullpup ? 1.05f : 1f;
        // ErgoFactor is the ergo penalty: higher value = worse ergo → less regen.
        float ergoScale = Mathf.Clamp01(1f - (WeaponStateInstance.ErgoFactor * bullpup / 100f));
        // Phase 7: * HealthStateInstance.HealthStamRegenFactor
        return baseRate * ergoScale;
    }

    private float ComputeDrainRate(float baseRate)
    {
        float bullpup   = WeaponStateInstance.IsBullpup ? 0.4f : 1f;
        float ergoScale = WeaponStateInstance.ErgoFactor * bullpup;
        // Phase 7: * ((1f - HealthStateInstance.HealthStamRegenFactor) + 1f)
        //          * (1f - SkillStateInstance.StrengthSkillAimBuff)
        return baseRate * ergoScale * PluginConfig.IdleStamDrainModi.Value;
    }

    private float ComputeIdleDrainRate()
    {
        // 0.1 pt/sec base idle drain, scaled by ergo and config modifier.
        // Phase 7: add health/skill factors as in ComputeDrainRate.
        float bullpup   = WeaponStateInstance.IsBullpup ? 0.4f : 1f;
        float ergoScale = WeaponStateInstance.ErgoFactor * bullpup;
        return 0.1f * ergoScale * PluginConfig.IdleStamDrainModi.Value;
    }
}
```

### What is NOT changed by Phases 3–5

| File | Reason |
|------|--------|
| `StanceController.cs` | Already calls `OnEnter`/`OnExit` through `StanceState` |
| `StanceInputHandler.cs` | `CancelAll()` → `OnExit()` → `OnStanceExited` fires automatically |
| `StanceState.cs` | Already wires stance lifecycle hooks |
| `PluginConfig.cs` | `EnableStanceStamChanges`, `EnableIdleStamDrain`, `IdleStamDrainModi` all exist |
