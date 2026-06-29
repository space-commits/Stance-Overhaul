using RealismCommonLib.Utils;
using StanceOverhaul.Enums;
using UnityEngine;
using static RealismCommonLib.Plugin;
using static StanceOverhaul.Plugin;

namespace StanceOverhaul.Stances;

public class HighReady : StanceBase
{
    public override EStanceType StanceType => EStanceType.HighReady;
    public override EStaminaMode StaminaMode => EStaminaMode.Regen;
    public override float StaminaRate => 1.85f;

    public override Vector3Curve EnterPositionCurve => _enterPos;
    public override Vector3Curve EnterRotationCurve => _enterRot;

    public override Vector3Curve ExitPositionCurve => _exitPos;
    public override Vector3Curve ExitRotationCurve => _exitRot;

    public override float BlendIntoThreshold(EStanceType nextStance)
    {
        switch (nextStance)
        {
            case EStanceType.ActiveAiming: return PluginConfig.HighReadyBlendThresholdActiveAim.Value;
            case EStanceType.LowReady: return PluginConfig.HighReadyBlendThresholdLowReady.Value;
            case EStanceType.LeftShoulder: return PluginConfig.HighReadyBlendThresholdLeftShoulder.Value;
            case EStanceType.PatrolStance: return PluginConfig.HighReadyBlendThresholdPatrol.Value;
            case EStanceType.ShortStock: return PluginConfig.HighReadyBlendThresholdShortStock.Value;
            default: return 0.2f;
        }
    }

    public override float TransitionFromModifier(EStanceType? previousStance)
    {
        switch (previousStance)
        {
            case EStanceType.ActiveAiming: return PluginConfig.HighReadyTransitionFromActiveAim.Value;
            case EStanceType.LowReady: return PluginConfig.HighReadyTransitionFromLowReady.Value;
            case EStanceType.LeftShoulder: return PluginConfig.HighReadyTransitionFromLeftShoulder.Value;
            case EStanceType.PatrolStance: return PluginConfig.HighReadyTransitionFromPatrol.Value;
            case EStanceType.ShortStock: return PluginConfig.HighReadyTransitionFromShortStock.Value;
            default: return 3f;
        }
    }

    public override float TransitionToSpeedModifier(EStanceType? nextStance)
    {
        switch (nextStance)
        {
            case EStanceType.ActiveAiming: return PluginConfig.HighReadyTransitionToActiveAim.Value;
            case EStanceType.LowReady: return PluginConfig.HighReadyTransitionToLowReady.Value;
            case EStanceType.LeftShoulder: return PluginConfig.HighReadyTransitionToLeftShoulder.Value;
            case EStanceType.PatrolStance: return PluginConfig.HighReadyTransitionToPatrol.Value;
            case EStanceType.ShortStock: return PluginConfig.HighReadyTransitionToShortStock.Value;
            default: return 1f;
        }
    }

    private readonly Vector3Curve _enterPos;
    private readonly Vector3Curve _enterRot;

    private readonly Vector3Curve _exitPos;
    private readonly Vector3Curve _exitRot;

    public HighReady()
    {
        _enterPos = RealismCommonLib.Utils.CurveDrawer.GetCurve("high_ready_enter_position")!;
        _enterRot = RealismCommonLib.Utils.CurveDrawer.GetCurve("high_ready_enter_rotation")!;

        _exitPos = RealismCommonLib.Utils.CurveDrawer.GetCurve("high_ready_exit_position")!;
        _exitRot = RealismCommonLib.Utils.CurveDrawer.GetCurve("high_ready_exit_rotation")!;
    }
}
 