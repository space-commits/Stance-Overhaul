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
        StanceEvents.OnStanceExited += OnStanceExited;

        OnIdle();
    }

    public void RunOnDestroy()
    {
        StanceEvents.OnStanceEntered -= OnStanceEntered;
        StanceEvents.OnStanceExited -= OnStanceExited;
        _rate = 0f;
        _freeze = false;
    }

    public void RunOnUpdate(float deltaTime)
    {
        if (!PluginConfig.EnableStanceStamChanges.Value) return;

        var physical = PlayerStateInstance.Player?.Physical;
        if (physical == null) return;
        
        if (_rate > 0f)
        {
            physical.HandsStamina.Current = Mathf.Min(physical.HandsStamina.Current + _rate * deltaTime, physical.HandsStamina.TotalCapacity);
        }
        else if (_rate < 0f)
        {
            physical.HandsStamina.Current = Mathf.Max(physical.HandsStamina.Current + _rate * deltaTime, 0f);
        }

        if (_freeze)
        {
            // Perpetually gate SelfRestoration without touching drain consumptions.
            physical.HandsStamina.DisableRestoration = Time.time + 1f;
        }
    }

    private void OnStanceEntered(IStance stance)
    {
        switch (stance.StaminaMode)
        {
            case EStaminaMode.Regen:
                _rate = ComputeRegenRate(stance.StaminaRate);
                _freeze = false;
                break;

            case EStaminaMode.Drain:
                _rate = -ComputeDrainRate(stance.StaminaRate);
                _freeze = true;
                break;

            case EStaminaMode.Freeze:
                _rate = 0f;
                _freeze = true;
                break;

            default: // Neutral
                _rate = 0f;
                _freeze = false;
                break;
        }
    }

    private void OnStanceExited()
    {
        OnIdle();
    }

    private void OnIdle()
    {
        if (PluginConfig.EnableIdleStamDrain.Value)
        {
            _rate = -ComputeIdleDrainRate();
            _freeze = true;
        }
        else
        {
            _rate = 0f;
            _freeze = false;
        }
    }

    private float ComputeRegenRate(float baseRate)
    {
        float bullpup = WeaponStateInstance.IsBullpup ? 1.15f : 1f;
        // ErgoFactor is the ergo penalty: higher value = worse ergo → less regen.
        float ergoScale = Mathf.Clamp01(1f - (WeaponStateInstance.ErgoFactor * bullpup / 100f));
        // Phase 7: * HealthStateInstance.HealthStamRegenFactor
        return baseRate * ergoScale;
    }

    private float ComputeDrainRate(float baseRate)
    {
        float bullpup = WeaponStateInstance.IsBullpup ? 0.5f : 1f;
        float ergoScale = WeaponStateInstance.ErgoFactor * bullpup;
        // Phase 7: * ((1f - HealthStateInstance.HealthStamRegenFactor) + 1f)
        //          * (1f - SkillStateInstance.StrengthSkillAimBuff)
        return baseRate * ergoScale * PluginConfig.IdleStamDrainModi.Value;
    }

    private float ComputeIdleDrainRate()
    {
        // 0.1 pt/sec base idle drain, scaled by ergo and config modifier.
        // Phase 7: add health/skill factors as in ComputeDrainRate.
        float bullpup = WeaponStateInstance.IsBullpup ? 0.6f : 1f;
        float ergoScale = WeaponStateInstance.ErgoFactor * bullpup;
        return 0.1f * ergoScale * PluginConfig.IdleStamDrainModi.Value;
    }


}