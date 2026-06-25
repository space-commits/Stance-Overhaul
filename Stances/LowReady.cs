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
            case EStanceType.ActiveAiming:
                return 0f;
            case EStanceType.HighReady:
                return 0.1f;
            case EStanceType.LeftShoulder:
                return 0.5f;
            case EStanceType.PatrolStance:
                return PluginConfig.test1.Value;
            case EStanceType.ShortStock:
                return 0f;
            default:
                return 0.2f;
        }
    }

    public override float TransitionFromModifier(EStanceType? previousStance)
    {
        switch (previousStance)
        {
            case EStanceType.ActiveAiming:
                return 3f;
            case EStanceType.HighReady:
                return 3f;
            case EStanceType.LeftShoulder:
                return 3f;
            case EStanceType.PatrolStance:
                return PluginConfig.test2.Value;
            case EStanceType.ShortStock:
                return 2f;
            default:
                return 3f;
        }
    }

    public override float TransitionToSpeedModifier(EStanceType? nextStance)
    {
        switch (nextStance)
        {
            case EStanceType.ActiveAiming:
                return 1.5f;
            case EStanceType.HighReady:
                return 1.25f;
            case EStanceType.LeftShoulder:
                return 1.8f;
            case EStanceType.PatrolStance:
                return PluginConfig.test3.Value;
            case EStanceType.ShortStock:
                return 1f;
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
