using RealismCommonLib.Utils;
using StanceOverhaul.Enums;
using UnityEngine;
using static RealismCommonLib.Plugin;
using static StanceOverhaul.Plugin;

namespace StanceOverhaul.Stances;

public class LowReady : StanceBase
{
    public override EStanceType StanceType => EStanceType.LowReady;

    public override Vector3Curve EnterPositionCurve => _enterPos;
    public override Vector3Curve EnterRotationCurve => _enterRot;

    public override Vector3Curve ExitPositionCurve => _exitPos;
    public override Vector3Curve ExitRotationCurve => _exitRot;

    public override float BlendIntoThreshold(EStanceType nextStance)
    {
        switch (nextStance)
        {
            case EStanceType.ActiveAiming: return PluginConfig.LowReadyBlendThresholdActiveAim.Value;
            case EStanceType.HighReady: return PluginConfig.LowReadyBlendThresholdHighReady.Value;
            case EStanceType.LeftShoulder: return PluginConfig.LowReadyBlendThresholdLeftShoulder.Value;
            case EStanceType.PatrolStance: return PluginConfig.LowReadyBlendThresholdPatrol.Value;
            case EStanceType.ShortStock: return PluginConfig.LowReadyBlendThresholdShortStock.Value;
            default: return 0.2f;
        }
    }

    public override float TransitionFromModifier(EStanceType? previousStance)
    {
        switch (previousStance)
        {
            case EStanceType.ActiveAiming: return PluginConfig.LowReadyTransitionFromActiveAim.Value;
            case EStanceType.HighReady: return PluginConfig.LowReadyTransitionFromHighReady.Value;
            case EStanceType.LeftShoulder: return PluginConfig.LowReadyTransitionFromLeftShoulder.Value;
            case EStanceType.PatrolStance: return PluginConfig.LowReadyTransitionFromPatrol.Value;
            case EStanceType.ShortStock: return PluginConfig.LowReadyTransitionFromShortStock.Value;
            default: return 3f;
        }
    }

    public override float TransitionToSpeedModifier(EStanceType? nextStance)
    {
        switch (nextStance)
        {
            case EStanceType.ActiveAiming: return PluginConfig.LowReadyTransitionToActiveAim.Value;
            case EStanceType.HighReady: return PluginConfig.LowReadyTransitionToHighReady.Value;
            case EStanceType.LeftShoulder: return PluginConfig.LowReadyTransitionToLeftShoulder.Value;
            case EStanceType.PatrolStance: return PluginConfig.LowReadyTransitionToPatrol.Value;
            case EStanceType.ShortStock: return PluginConfig.LowReadyTransitionToShortStock.Value;
            default: return 1f;
        }
    }

    private readonly Vector3Curve _enterPos;
    private readonly Vector3Curve _enterRot;

    private readonly Vector3Curve _exitPos;
    private readonly Vector3Curve _exitRot;

    public LowReady()
    {
        _enterPos = RealismCommonLib.Utils.CurveDrawer.GetCurve("low_ready_enter_position")!;
        _enterRot = RealismCommonLib.Utils.CurveDrawer.GetCurve("low_ready_enter_rotation")!;

        _exitPos = RealismCommonLib.Utils.CurveDrawer.GetCurve("low_ready_exit_position")!;
        _exitRot = RealismCommonLib.Utils.CurveDrawer.GetCurve("low_ready_exit_rotation")!;
    }
}
