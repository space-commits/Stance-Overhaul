using RealismCommonLib.Utils;
using StanceOverhaul.Enums;
using UnityEngine;
using static RealismCommonLib.Plugin;
using static StanceOverhaul.Plugin;

namespace StanceOverhaul.Stances;

public class HighReady : StanceBase
{
    public override EStanceType StanceType => EStanceType.HighReady;

    public override Vector3Curve EnterPositionCurve => _enterPos;
    public override Vector3Curve EnterRotationCurve => _enterRot;

    public override Vector3Curve ExitPositionCurve => _exitPos;
    public override Vector3Curve ExitRotationCurve => _exitRot;

    public override float BlendThreshold(EStanceType nextStance)
    {
        switch (nextStance)
        {
            case EStanceType.ActiveAiming:
                return PluginConfig.test3.Value;
            case EStanceType.LowReady:
                return 0.75f;
            case EStanceType.LeftShoulder:
                return PluginConfig.test3.Value;
            case EStanceType.PatrolStance:
                return PluginConfig.test3.Value;
            case EStanceType.ShortStock:
                return PluginConfig.test3.Value;
            default:
                return 0.2f;
        }
    }

    public override float BaseSpeed(EStanceType? previousStance)
    {
        switch (previousStance)
        {
            case EStanceType.ActiveAiming:
                return PluginConfig.test4.Value;
            case EStanceType.LowReady:
                return 3f;
            case EStanceType.LeftShoulder:
                return PluginConfig.test4.Value;
            case EStanceType.PatrolStance:
                return PluginConfig.test4.Value;
            case EStanceType.ShortStock:
                return PluginConfig.test4.Value;
            default:
                return 3f;
        }
    }

    public override float TransitionSpeedModifier(EStanceType? nextStance)
    {
        switch (nextStance)
        {
            case EStanceType.ActiveAiming:
                return PluginConfig.test4.Value;
            case EStanceType.LowReady:
                return 0.6f;
            case EStanceType.LeftShoulder:
                return PluginConfig.test4.Value;
            case EStanceType.PatrolStance:
                return PluginConfig.test4.Value;
            case EStanceType.ShortStock:
                return PluginConfig.test4.Value;
            default:
                return 1f;
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
 