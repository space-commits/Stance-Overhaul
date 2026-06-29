using RealismCommonLib.Utils;
using StanceOverhaul.Enums;
using UnityEngine;
using static RealismCommonLib.Plugin;
using static StanceOverhaul.Plugin;

namespace StanceOverhaul.Stances;

public class ActiveAim : StanceBase
{
    public override EStanceType StanceType => EStanceType.ActiveAiming;
    public override EStaminaMode StaminaMode => EStaminaMode.Drain;
    public override float StaminaRate => 0.075f;

    public override Vector3Curve EnterPositionCurve => _enterPos;
    public override Vector3Curve EnterRotationCurve => _enterRot;

    public override Vector3Curve ExitPositionCurve => _exitPos;
    public override Vector3Curve ExitRotationCurve => _exitRot;

    public override float BlendIntoThreshold(EStanceType nextStance)
    {
        switch (nextStance)
        {
            case EStanceType.LowReady: return PluginConfig.ActiveAimBlendThresholdLowReady.Value;
            case EStanceType.HighReady: return PluginConfig.ActiveAimBlendThresholdHighReady.Value;
            case EStanceType.LeftShoulder: return PluginConfig.ActiveAimBlendThresholdLeftShoulder.Value;
            case EStanceType.PatrolStance: return PluginConfig.ActiveAimBlendThresholdPatrol.Value;
            case EStanceType.ShortStock: return PluginConfig.ActiveAimBlendThresholdShortStock.Value;
            default: return 0.2f;
        }
    }

    public override float TransitionFromModifier(EStanceType? previousStance)
    {
        switch (previousStance)
        {
            case EStanceType.LowReady: return PluginConfig.ActiveAimTransitionFromLowReady.Value;
            case EStanceType.HighReady: return PluginConfig.ActiveAimTransitionFromHighReady.Value;
            case EStanceType.LeftShoulder: return PluginConfig.ActiveAimTransitionFromLeftShoulder.Value;
            case EStanceType.PatrolStance: return PluginConfig.ActiveAimTransitionFromPatrol.Value;
            case EStanceType.ShortStock: return PluginConfig.ActiveAimTransitionFromShortStock.Value;
            default: return 3f;
        }
    }

    public override float TransitionToSpeedModifier(EStanceType? nextStance)
    {
        switch (nextStance)
        {
            case EStanceType.LowReady: return PluginConfig.ActiveAimTransitionToLowReady.Value;
            case EStanceType.HighReady: return PluginConfig.ActiveAimTransitionToHighReady.Value;
            case EStanceType.LeftShoulder: return PluginConfig.ActiveAimTransitionToLeftShoulder.Value;
            case EStanceType.PatrolStance: return PluginConfig.ActiveAimTransitionToPatrol.Value;
            case EStanceType.ShortStock: return PluginConfig.ActiveAimTransitionToShortStock.Value;
            default: return 1f;
        }
    }

    private readonly Vector3Curve _enterPos;
    private readonly Vector3Curve _enterRot;

    private readonly Vector3Curve _exitPos;
    private readonly Vector3Curve _exitRot;

    public ActiveAim()
    {
        _enterPos = RealismCommonLib.Utils.CurveDrawer.GetCurve("active_enter_position")!;
        _enterRot = RealismCommonLib.Utils.CurveDrawer.GetCurve("active_enter_rotation")!;

        _exitPos = RealismCommonLib.Utils.CurveDrawer.GetCurve("active_exit_position")!;
        _exitRot = RealismCommonLib.Utils.CurveDrawer.GetCurve("active_exit_rotation")!;
    }
}

