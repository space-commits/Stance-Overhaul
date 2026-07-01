# Player Movement Speed — Analysis & Modification Plan

## Status: Analysis Complete

---

## Overview

This document analyses how EFT determines and drives player walk and sprint speed, mapping every property involved in the pipeline from backend config → physical state → movement context → animator. It then outlines the available interception points and their trade-offs for a mod wishing to modify movement speed.

---

## System Architecture

The speed pipeline has three layers:

```
BackendConfigSettingsClass  ← global constants (curves, limits)
        ↓
BasePhysicalClass / PlayerPhysicalClass  ← per-player physical state, weight, overweight, stamina
        ↓
MovementContext  ← speed limits, smoothed movement speed, sprint speed
        ↓
PlayerAnimator  ← animator float parameters that drive actual root motion
        ↓
CharacterController.Move()  ← applied to world position each frame
```

---

## Walk Speed Pipeline

### 1. `MaxSpeed` (the absolute walk ceiling)

Defined on `MovementContext`:

```csharp
public float MaxSpeed =>
    GClass2298.Evaluate(
        Singleton<BackendConfigSettingsClass>.Instance.WalkSpeed,  // Vector2(0.7f, 1.0f)
        (float)SkillManager.Strength.SummaryLevel / 60f           // normalized 0..1
    );
```

`GClass2298.Evaluate(curve, t)` is a simple `Mathf.Lerp(curve.x, curve.y, t)`. So:
- At Strength level 0: `MaxSpeed = 0.7`
- At Strength level 60: `MaxSpeed = 1.0`

This is a **normalized movement speed** — it feeds the animator, not a world-space m/s value. Root motion converts this into actual displacement.

### 2. Weight → `WalkSpeedLimit`

Computed in `BasePhysicalClass.OnWeightUpdated()`:

```csharp
WalkSpeedLimit = 1f - WalkSpeedOverweightLimits.InverseLerp(totalWeight);
```

Where `WalkSpeedOverweightLimits` defaults to `Vector2(45f, 80f)` kg (backend config, scaled by `CarryingWeightRelativeModifier` from skills/health effects). This gives:
- Below 45 kg: `WalkSpeedLimit = 1.0` (no penalty)
- At 80 kg: `WalkSpeedLimit = 0.0` (no movement)

`WalkSpeedLimit` is then applied in `MovementContext.WeightRelatedValuesUpdated()`:

```csharp
float walkSpeedLimit = _player.Physical.WalkSpeedLimit;
RemoveStateSpeedLimit(Player.ESpeedLimit.Weight);
if (walkSpeedLimit < 1f)
    AddStateSpeedLimit(walkSpeedLimit * MaxSpeed, Player.ESpeedLimit.Weight);
```

### 3. `StateSpeedLimit` (the composite limit)

`MovementContext.StateSpeedLimit` is the **minimum** of all active `SpeedLimits` dictionary entries:

```csharp
// method_4() — recalculated when SpeedLimitIsDirty
foreach (var limit in SpeedLimits)
    if (limit.Value <= StateSpeedLimit)
        StateSpeedLimit = limit.Value;
```

All `ESpeedLimit` causes:

| Cause | Default | Set by |
|-------|---------|--------|
| `SurfaceNormal` | `1.0f` | Always present |
| `Weight` | `WalkSpeedLimit * MaxSpeed` | `WeightRelatedValuesUpdated()` |
| `Aiming` | configurable | `SetAimingSlowdown()` |
| `Fall` | post-jump penalty | `OnJumpEnd()` — **only affects `StateSprintSpeedLimit`** |
| `Swamp` | `0.2f` | `OnEnterObstacle()` when swamp present |

**Note:** The `Fall` limit goes into `StateSprintSpeedLimit`, not `StateSpeedLimit`. Sprint and walk use separate composite limits.

### 4. `ClampedSpeed`

```csharp
public float ClampedSpeed => Mathf.Clamp(CharacterMovementSpeed, 0f, StateSpeedLimit);
```

`CharacterMovementSpeed` is set by `SetCharacterMovementSpeed()`:

```csharp
public virtual void SetCharacterMovementSpeed(float speed, bool force = false)
{
    CharacterMovementSpeed = Mathf.Clamp(speed, 0f, MaxSpeed);
    UpdateCovertEfficiency(ClampedSpeed);
    RelativeSpeed = CharacterMovementSpeed / MaxSpeed;
    if (force)
    {
        CharacterMovementSpeed = speed;
        SmoothedCharacterMovementSpeed = ClampedSpeed;
    }
}
```

So `ClampedSpeed` is doubly clamped: first to `MaxSpeed`, then to `StateSpeedLimit`.

### 5. `SmoothedCharacterMovementSpeed` → animator

The `RunStateClass` (`method_1`) lerps `SmoothedCharacterMovementSpeed` toward `ClampedSpeed` using `WalkInertia`. Once `SmoothedCharacterMovementSpeed` is set, it calls:

```csharp
PlayerAnimatorSetCharacterMovementSpeed(SmoothedCharacterMovementSpeed);
```

Which routes to `PlayerAnimator_1.SetCharacterMovementSpeed(value)` — this sets the animator float that drives blend trees and root motion scale.

### Walk Speed Formula (summary)

```
effectiveWalkSpeed = Lerp(0.7, 1.0, Strength/60)   ← MaxSpeed
                   * min(all active SpeedLimits)     ← StateSpeedLimit (includes weight, aim slowdown, swamp)
```

---

## Sprint Speed Pipeline

Sprint and walk share `MaxSpeed` but diverge from there.

### 1. `SprintingSpeed` (sprint ceiling from strength)

```csharp
public float SprintingSpeed =>
    GClass2298.Evaluate(
        Singleton<BackendConfigSettingsClass>.Instance.SprintSpeed,  // Vector2(0.4f, 2.0f)
        (float)SkillManager.Strength.SummaryLevel / 60f
    );
```

- At Strength 0: `SprintingSpeed = 0.4`
- At Strength 60: `SprintingSpeed = 2.0`

### 2. `BasePhysicalClass.SprintSpeed` (overweight penalty)

```csharp
public float SprintSpeed =>
    Mathf.Lerp(1f, StaminaParameters.SprintSpeedLowerLimit, Float_3);
// Float_3 = SprintOverweightLimits.InverseLerp(totalWeight) — 0 below 20kg, 1 at 40kg
// SprintSpeedLowerLimit = 0.4 (default)
```

So at max overweight for sprint: `SprintSpeed = 0.4`.

### 3. Sprint acceleration — `SprintAcceleration()` in `MovementContext`

Called each frame in `SprintStateClass.ManualAnimatorMoveUpdate`:

```csharp
public void SprintAcceleration(float deltaTime)
{
    float accelRate = _player.Physical.SprintAcceleration * deltaTime;
    float targetSpeed = (_player.Physical.SprintSpeed * SprintingSpeed + 1f)
                        * StateSprintSpeedLimit;

    // Inertia/rotation penalty — reduces target speed during rapid turning
    float inertiaMult = Mathf.Max(
        EFTHardSettings.Instance.sprintSpeedInertiaCurve.Evaluate(Mathf.Abs(AverageRotationX.Average)),
        EFTHardSettings.Instance.sprintSpeedInertiaCurve.Evaluate(2.1474836E+09f) * (2f - _player.Physical.Inertia)
    );
    targetSpeed = Mathf.Clamp(targetSpeed * inertiaMult, 0.1f, targetSpeed);

    SprintSpeed = Mathf.Clamp(
        SprintSpeed + accelRate * Mathf.Sign(targetSpeed - SprintSpeed),
        0.01f, targetSpeed
    );
}
```

`SprintSpeed` (the `MovementContext` property) is then fed to the animator via `PlayerAnimator_1.SetCharacterSprintSpeed(SprintSpeed_1)`.

### 4. `StateSprintSpeedLimit` (post-jump and weight limit for sprint)

```csharp
// method_4():
if (SpeedLimits.ContainsKey(Player.ESpeedLimit.Fall))
    StateSprintSpeedLimit = SpeedLimits[Player.ESpeedLimit.Fall];
```

The `OnJumpEnd()` post-landing penalty sets this limit with a timed duration via `ChangeSpeedLimit(num2, Player.ESpeedLimit.Fall, duration)`. It only affects sprint, not walk.

### 5. `SpeedLimiter` (`GClass2175`) — CharacterController physical speed cap

`CreateSpeedLimiter()` is called during initialization (for vaulting states). It sets `CharacterController.SpeedLimit` directly:

```csharp
// method_1() in GClass2175:
float limit = Lerp(stateLimits.MinSpeed, stateLimits.MaxSpeed, strengthLevel / maxLevel)
            * Lerp(SprintSpeedLowerLimit, 1f, walkSpeedLimit);
CharacterController.SpeedLimit = limit;
```

This is a **physics-layer cap** on the CharacterController — it prevents the character from exceeding a world-space m/s value regardless of what the animator tries to do. Vaulting states use this to control transition speeds.

### Sprint Speed Formula (summary)

```
targetSprintSpeed = Lerp(1.0, 0.4, overweightFraction)   ← Physical.SprintSpeed
                  * Lerp(0.4, 2.0, Strength/60)           ← SprintingSpeed
                  * inertiaTurningPenalty                  ← rotation-based mult
                  * StateSprintSpeedLimit                  ← post-jump / weight limit
```

The animator `SprintSpeed` parameter gradually ramps up from 0 to this target at rate `SprintAcceleration`.

---

## Pre-Sprint Phase (`PreSprintAcceleration`)

Before the sprint animation fully activates (while still in `RunState` with sprint requested), `MovementContext.PreSprintAcceleration()` is called each frame:

```csharp
public virtual void PreSprintAcceleration(float deltaTime)
{
    if (MovementDirection.y < 0.1f)
    {
        EnableSprint(false);
        return;
    }
    if (SmoothedCharacterMovementSpeed < 1f)
    {
        float rate = _player.Physical.PreSprintAcceleration;
        SmoothedCharacterMovementSpeed = Mathf.Clamp(
            SmoothedCharacterMovementSpeed + deltaTime * rate, 0f, 1f);
        CharacterMovementSpeed = SmoothedCharacterMovementSpeed;
        RaiseChangeSpeedEvent();
    }
}
```

`PreSprintAcceleration` is derived from `Inertia` (weight-based). Heavy players take longer to reach `SmoothedCharacterMovementSpeed = 1.0` before the sprint animator triggers.

---

## Key Derived Values & Their Sources

| Value | Type | Source | Effect |
|-------|------|--------|--------|
| `MaxSpeed` | `float` | `BackendConfigSettingsClass.WalkSpeed` curve + Strength | Ceiling for `CharacterMovementSpeed` |
| `SprintingSpeed` | `float` | `BackendConfigSettingsClass.SprintSpeed` curve + Strength | Animator sprint blend target |
| `Physical.SprintSpeed` | `float` | Overweight vs `SprintOverweightLimits` | Multiplier applied to `SprintingSpeed` |
| `WalkSpeedLimit` | `float 0..1` | Weight vs `WalkSpeedOverweightLimits` | Added to `SpeedLimits[Weight]` |
| `StateSpeedLimit` | `float` | Minimum of all `SpeedLimits` entries | Clamps `ClampedSpeed` |
| `StateSprintSpeedLimit` | `float` | `SpeedLimits[Fall]` entry | Caps sprint target speed |
| `CharacterMovementSpeed` | `float` | Clamped to `MaxSpeed` by `SetCharacterMovementSpeed` | Source for `ClampedSpeed` |
| `ClampedSpeed` | `float` | `Clamp(CharacterMovementSpeed, 0, StateSpeedLimit)` | What animator actually receives |
| `SmoothedCharacterMovementSpeed` | `float` | Lerped toward `ClampedSpeed` per-frame via `WalkInertia` | Animator walk blend float |
| `SprintSpeed` (context property) | `float` | Ramped by `SprintAcceleration()` | Animator sprint blend float |
| `Inertia` | `float 0..1` | Weight vs `BaseInertiaLimits` | Affects ramp rates, acceleration curves |
| `WalkInertia` | `float` | `InertiaSettings.WalkInertia` evaluated at `Inertia` | Smoothing time constant for walk |
| `SprintAcceleration` (physical) | `float` | `SprintAccelerationLimits.InverseLerp(Inertia)` | Rate of sprint speed ramp-up |

---

## Existing Patches in This Workspace

`RealismCommonLib/src/Patches/PlayerPatches.cs` contains two experimental patches (currently commented out / test config gated):

### `ApplyMotionPatch` — scale the motion vector directly
Patches `MovementState.ApplyMotion(ref Vector3 motion)` as prefix, multiplying `motion` by a config multiplier before the CharacterController move is applied. This affects world-space displacement **after** all speed limit logic — a direct multiplier on the final movement vector.

### `ClampedSpeedPatch` — scale the `ClampedSpeed` property
Patches the `ClampedSpeed` getter as postfix, multiplying `__result` by a config value. Since `ClampedSpeed` feeds into both the animator float and the pre-sprint acceleration check, this is a high-level intercept that respects the weight/aim slowdown system but multiplies on top of it.

---

## Potential Modification Approaches

### Approach A — `MaxSpeed` Override (Postfix on getter)

**Target:** `MovementContext.MaxSpeed` getter  
**How:** Postfix patch → multiply `__result`  
**Effect:** Scales the ceiling used by both `SetCharacterMovementSpeed` and `CharacterMovementSpeed` clamping. Weight penalties and `StateSpeedLimit` still apply relative to this new ceiling.  
**Risk:** Low. All downstream speed limit logic still operates correctly since it references `MaxSpeed` only as an input. However, `UpdateCharacterControllerSpeedLimit` calls `SetCharacterMovementSpeed(RelativeSpeed * MaxSpeed)` every state change — the multiplier applies there automatically.

```csharp
[PatchPostfix]
private static void Postfix(MovementContext __instance, ref float __result)
{
    if (!player.IsYourPlayer) return;
    __result *= PluginConfig.WalkSpeedMult.Value;
}
```

### Approach B — `ClampedSpeed` Override (Postfix on getter)

**Target:** `MovementContext.ClampedSpeed` getter  
**How:** Postfix patch → multiply `__result`  
**Effect:** Applied after all speed limit math. Scales what the animator receives. Does not affect how the game calculates weight penalties relative to `MaxSpeed`, so the "feel" of the speed limit system is preserved.  
**Risk:** Low. The already-tested approach in `PlayerPatches.cs`. Does not affect `CharacterController.SpeedLimit` (physics layer), so at very high multipliers the CC cap may become the constraint.

### Approach C — `ApplyMotion` Vector Scale (Prefix patch)

**Target:** `MovementState.ApplyMotion(ref Vector3 motion)`  
**How:** Prefix patch → `motion *= multiplier`  
**Effect:** Scales the raw delta-position applied to the CharacterController each frame. Bypasses all animator speed floats — the character visually animates at the same speed but covers more/less ground. This creates a desync between animation and movement (sliding) at high multipliers.  
**Risk:** Medium. Desync between animation and root motion at extreme values. Not suitable for meaningful speed changes — better for subtle tweaks.

### Approach D — `SprintingSpeed` Override (Postfix on getter)

**Target:** `MovementContext.SprintingSpeed` getter  
**How:** Postfix patch → multiply `__result`  
**Effect:** Directly scales the sprint target speed ceiling. The inertia/turning penalty and `StateSprintSpeedLimit` still apply on top.  
**Risk:** Low. Sprint-only change. Separate from walk.

### Approach E — `AddStateSpeedLimit` / Custom `ESpeedLimit` Cause

**Target:** `MovementContext.AddStateSpeedLimit()` + call it with a custom cause  
**How:** Stance or state handler calls `movementContext.AddStateSpeedLimit(limit, cause)` / `RemoveStateSpeedLimit(cause)` at appropriate events  
**Effect:** Adds a persistent speed limit that participates cleanly in the composite `StateSpeedLimit` computation. Automatically interacts with all other limits (walk, aim, swamp). Timed versions auto-expire via `ProcessSpeedLimits`.  
**Risk:** Low. The cleanest integration point for stance-dependent speed changes (e.g. slow walk during LowReady, faster movement during PatrolStance). Requires access to `MovementContext` — available via `PlayerStateInstance.Player.MovementContext`.

```csharp
// In a stance OnEnter:
var ctx = PlayerStateInstance.Player?.MovementContext;
ctx?.AddStateSpeedLimit(0.85f * ctx.MaxSpeed, Player.ESpeedLimit.Aiming);

// In a stance OnExit:
ctx?.RemoveStateSpeedLimit(Player.ESpeedLimit.Aiming);
```

**Note:** The game already uses `ESpeedLimit.Aiming` for the ADS slow. If stance-based speed changes are needed, use an unused cause value or patch in a new cause to avoid collision.

### Approach F — `GClass2175.method_1` Override (SpeedLimiter speed cap)

**Target:** `GClass2175.Update(EPlayerState, int, float)` or `method_1`  
**How:** Postfix patch → modify the returned/assigned `Speed` value  
**Effect:** Modifies `CharacterController.SpeedLimit` — the physics-layer world-speed cap. Only active during vaulting/climbing states when `SpeedLimiter` is non-null. Has no effect during normal walk/sprint since the SpeedLimiter is `null` then.  
**Risk:** Medium. Narrow scope — only relevant for vault/climb.

### Approach G — `SetCharacterMovementSpeed` Override (Prefix/Postfix)

**Target:** `MovementContext.SetCharacterMovementSpeed(float, bool)`  
**How:** Postfix or prefix, modify the `characterMovementSpeed` parameter  
**Effect:** Intercepts at the root of all walk speed assignment. All calling paths (state transitions, `UpdateCharacterControllerSpeedLimit`, `ResetSpeedAfterSprint`) will pass through.  
**Risk:** Medium. Called frequently from many paths. A prefix that scales the parameter will multiply before the `Clamp(0, MaxSpeed)` inside the method — at high multipliers it will be clamped back to `MaxSpeed` unless `MaxSpeed` is also patched.

---

## Recommended Strategy for Stance-Based Speed Changes

For the Stance-Overhaul project, the cleanest integration is **Approach E** (custom speed limits via `AddStateSpeedLimit`/`RemoveStateSpeedLimit`) combined with **Approach D** (postfix on `SprintingSpeed`) for sprint-specific changes.

### Design

1. Each stance that affects speed declares properties:
   ```csharp
   virtual float WalkSpeedMultiplier { get; } // default 1.0 = no change
   virtual float SprintSpeedMultiplier { get; } // default 1.0 = no change
   ```

2. A `StanceMovementHandler` subscribes to `StanceEvents.OnStanceEntered` / `OnStanceExited`:
   - On enter: if `WalkSpeedMultiplier != 1.0`, call `AddStateSpeedLimit(MaxSpeed * mult, cause)` (can reuse `ESpeedLimit.Aiming` if no conflict, or a postfix patch on `ClampedSpeed`)
   - On exit: remove the limit

3. For sprint, either:
   - Patch `SprintingSpeed` getter to apply `StanceSprintMultiplier` when a sprint-affecting stance is active
   - Or postfix `SprintAcceleration()` to clamp `SprintSpeed` to a lower target

### Suggested Stance Speed Values

| Stance | Walk Mult | Sprint Mult | Rationale |
|--------|-----------|-------------|-----------|
| HighReady | 1.0 | 0.9 | Slight sprint penalty, weapon raised |
| LowReady | 1.0 | 1.0 | Minimal restriction |
| PatrolStance | 1.0 | 1.0 | Relaxed carry, no penalty |
| ShortStock | 0.95 | 0.9 | Weapon compressed, slightly restricted |
| ActiveAiming | — | — | Handled by game's `SetAimingSlowdown` |

---

## Implementation Notes

### Accessing `MovementContext` from handlers

```csharp
var ctx = PlayerStateInstance.Player?.MovementContext;
if (ctx == null) return;
```

### Accessing `ESpeedLimit` values

`Player.ESpeedLimit` is an enum defined inside the `Player` class in `Assembly-CSharp`. Values already used by the game:

```
SurfaceNormal, Fall, Swamp, Aiming, Weight
```

To avoid collision, add a postfix patch on `ClampedSpeed` for custom stance speed limits rather than injecting a new dictionary entry.

### Thread safety

`SpeedLimits` is a `Dictionary` accessed only from the main Unity thread (Update). Patch calls should similarly be made from the main thread (from `RunOnUpdate` or event handlers triggered by game callbacks). No locking is required.

### `UpdateCharacterControllerSpeedLimit` — state change invalidation

This is called on every state transition (`OnStateChanged` → `ProcessStateEnter`). It calls `SetCharacterMovementSpeed(RelativeSpeed * MaxSpeed)`, which will erase any manual `CharacterMovementSpeed` override. Stance-based speed changes made via `AddStateSpeedLimit` are immune to this because `StateSpeedLimit` is recomputed from the dictionary, which persists across state changes. Speed changes made by directly setting `CharacterMovementSpeed` will not survive state transitions.

---

## Out of Scope

- AI movement speed — AI players use simplified movement context (`SimplifiedMovementContext` or `IsBot = true`). The weight limits are set to `9000–10000 kg` ranges effectively disabling them for AI.
- Mounted / stationary weapon states — `IsInMountedState` blocks normal movement; speed is irrelevant.
- Prone movement speed — driven by same `ClampedSpeed` path but with a different inertia profile (`ProneSpeedAccelerationRange`). Multipliers on `ClampedSpeed` or `ApplyMotion` would affect prone too.
- Platform movement (`PlatformMotion`) — additive to `CharacterController.Move`, unrelated to the speed pipeline.
