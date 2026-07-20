using StanceOverhaul.Enums;
using StanceOverhaul.Events;
using StanceOverhaul.Stances;
using UnityEngine;
using static RealismCommonLib.Plugin;
using static StanceOverhaul.Plugin;

namespace StanceOverhaul.Handlers;

public class StanceStaminaHandler : IControllerHelper
{
    private const float BaseIdleDrainRate = 0.1f;

    // Positive = regen pts/sec, negative = drain pts/sec, 0 = do nothing.
    private float _rate = 0f;

    // When true, write DisableRestoration each frame to suppress SelfRestoration.
    private bool _freeze = false;

    public void RunOnAwake()
    {
        StanceEvents.OnStanceEntered += CheckState;
        StanceEvents.OnStanceExited += OnStateChanged;
        WeaponStateInstance.OnWeaponStateChanged += OnStateChanged;
        RealismCommonLib.Events.PlayerEvents.OnAnyItemEquipped += OnStateChanged;
    }

    public void RunOnDestroy()
    {
        StanceEvents.OnStanceEntered -= CheckState;
        StanceEvents.OnStanceExited -= OnStateChanged;
        WeaponStateInstance.OnWeaponStateChanged -= OnStateChanged;
        RealismCommonLib.Events.PlayerEvents.OnAnyItemEquipped -= OnStateChanged;
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

    private void OnStateChanged()
    {
        CheckState(StanceControllerInstance?.CurrentStance);
    }

    private void CheckState(IStance? stance)
    {
        switch (stance?.StaminaMode)
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
                _rate = PluginConfig.EnableIdleStamDrain.Value ? -ComputeDrainRateIdle() : 0f;
                _freeze = PluginConfig.EnableIdleStamDrain.Value ? true : false;
                break;
        }

        //ModLogger.LogWarning($"CheckState: stance={stance?.StanceType}, staminaMode={stance?.StaminaMode}, staminaRate={stance?.StaminaRate}, rate={_rate}, freeze={_freeze}");
    }

    private float ComputeRegenRate(float baseRate)
    {
        return StanceControllerInstance.StatsHandlerInstance.GetRegenerationRate(baseRate);
    }

    private float ComputeDrainRate(float baseRate)
    {
        return StanceControllerInstance.StatsHandlerInstance.GetDrainRate(baseRate) * PluginConfig.IdleStamDrainModi.Value;
    }

    /// <summary>
    /// For idle stam drain, no stance factor
    /// </summary>
    private float ComputeDrainRateIdle()
    {
        return StanceControllerInstance.StatsHandlerInstance.GetDrainRate(BaseIdleDrainRate) * PluginConfig.IdleStamDrainModi.Value;
    }
}