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

    public override float BlendThreshold(EStanceType nextStance)
    {
        switch (nextStance)
        {
            case EStanceType.ActiveAiming:
                return PluginConfig.test1.Value;
            case EStanceType.HighReady:
                return 0.25f;
            case EStanceType.LeftShoulder:
                return 0.6f;
            case EStanceType.PatrolStance:
                return PluginConfig.test1.Value;
            case EStanceType.ShortStock:
                return PluginConfig.test1.Value;
            default:
                return 0.2f;
        }
    }

    public override float BaseSpeed(EStanceType? previousStance)
    {
        switch (previousStance)
        {
            case EStanceType.ActiveAiming:
                return PluginConfig.test2.Value;
            case EStanceType.HighReady:
                return 3f;
            case EStanceType.LeftShoulder:
                return 3f;
            case EStanceType.PatrolStance:
                return PluginConfig.test2.Value;
            case EStanceType.ShortStock:
                return PluginConfig.test2.Value;
            default:
                return 3f;
        }
    }

    public override float TransitionSpeedModifier(EStanceType? nextStance)
    {
        switch (nextStance)
        {
            case EStanceType.ActiveAiming:
                return PluginConfig.test3.Value;
            case EStanceType.HighReady:
                return PluginConfig.test3.Value;
            case EStanceType.LeftShoulder:
                return 2f;
            case EStanceType.PatrolStance:
                return PluginConfig.test3.Value;
            case EStanceType.ShortStock:
                return PluginConfig.test3.Value;
            default:
                return 1f;
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
