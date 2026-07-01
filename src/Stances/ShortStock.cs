using RealismCommonLib.Utils;
using StanceOverhaul.Enums;
using UnityEngine;
using static RealismCommonLib.Plugin;
using static StanceOverhaul.Plugin;

namespace StanceOverhaul.Stances;

public class ShortStock : StanceBase
{
    public override EStanceType StanceType => EStanceType.ShortStock;
    public override EStaminaMode StaminaMode => EStaminaMode.Regen;

    public override Vector3Curve EnterPositionCurve => _enterPos;
    public override Vector3Curve EnterRotationCurve => _enterRot;

    public override Vector3Curve ExitPositionCurve => _exitPos;
    public override Vector3Curve ExitRotationCurve => _exitRot;

    public override float StaminaRate => PluginConfig.ShortStockStaminaRate.Value;
    public override float WalkSpeedBonus => PluginConfig.ShortStockWalkSpeedBonus.Value;
    public override float SprintAccelBonus => PluginConfig.ShortStockSprintAccelBonus.Value;

    public override AnimationCurve ExitAimSpeedCurve { get; } = new AnimationCurve
            (
                new Keyframe { time = 0f, value = 0f },
                new Keyframe { time = 0.7f, value = 0.0f },
                new Keyframe { time = 0.85f, value = 0.1f },
                new Keyframe { time = 0.95f, value = 0.25f },
                new Keyframe { time = 1f, value = 1f }
            );

    public override float BlendIntoThreshold(EStanceType nextStance)
    {
        switch (nextStance)
        {
            case EStanceType.ActiveAiming: return PluginConfig.ShortStockBlendThresholdActiveAim.Value;
            case EStanceType.HighReady: return PluginConfig.ShortStockBlendThresholdHighReady.Value;
            case EStanceType.LowReady: return PluginConfig.ShortStockBlendThresholdLowReady.Value;
            case EStanceType.PatrolStance: return PluginConfig.ShortStockBlendThresholdPatrol.Value;
            case EStanceType.LeftShoulder: return PluginConfig.ShortStockBlendThresholdLeftShoulder.Value;
            default: return 0.4f;
        }
    }

    public override float TransitionFromModifier(EStanceType? previousStance)
    {
        switch (previousStance)
        {
            case EStanceType.ActiveAiming: return PluginConfig.ShortStockTransitionFromActiveAim.Value;
            case EStanceType.HighReady: return PluginConfig.ShortStockTransitionFromHighReady.Value;
            case EStanceType.LowReady: return PluginConfig.ShortStockTransitionFromLowReady.Value;
            case EStanceType.PatrolStance: return PluginConfig.ShortStockTransitionFromPatrol.Value;
            case EStanceType.LeftShoulder: return PluginConfig.ShortStockTransitionFromLeftShoulder.Value;
            default: return PluginConfig.ShortStockTransitionFromIdle.Value;
        }
    }

    public override float TransitionToSpeedModifier(EStanceType? nextStance)
    {
        switch (nextStance)
        {
            case EStanceType.ActiveAiming: return PluginConfig.ShortStockTransitionToActiveAim.Value;
            case EStanceType.HighReady: return PluginConfig.ShortStockTransitionToHighReady.Value;
            case EStanceType.LowReady: return PluginConfig.ShortStockTransitionToLowReady.Value;
            case EStanceType.PatrolStance: return PluginConfig.ShortStockTransitionToPatrol.Value;
            case EStanceType.LeftShoulder: return PluginConfig.ShortStockTransitionToLeftShoulder.Value;
            default: return 1f;
        }
    }

    private readonly Vector3Curve _enterPos;
    private readonly Vector3Curve _enterRot;

    private readonly Vector3Curve _exitPos;
    private readonly Vector3Curve _exitRot;

    public ShortStock()
    {
        _enterPos = CurveDrawer.GetCurve("short_enter_position")!;
        _enterRot = CurveDrawer.GetCurve("short_enter_rotation")!;

        _exitPos = CurveDrawer.GetCurve("short_exit_position")!;
        _exitRot = CurveDrawer.GetCurve("short_exit_rotation")!;
    }
}