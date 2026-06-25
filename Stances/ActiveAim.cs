using RealismCommonLib.Utils;
using StanceOverhaul.Enums;
using UnityEngine;
using static RealismCommonLib.Plugin;
using static StanceOverhaul.Plugin;

namespace StanceOverhaul.Stances;

public class ActiveAim : StanceBase
{
    public override EStanceType StanceType => EStanceType.ActiveAiming;

    public override Vector3Curve EnterPositionCurve => _enterPos;
    public override Vector3Curve EnterRotationCurve => _enterRot;

    public override Vector3Curve ExitPositionCurve => _exitPos;
    public override Vector3Curve ExitRotationCurve => _exitRot;

    public override float BlendIntoThreshold(EStanceType nextStance)
    {
        switch (nextStance)
        {
            case EStanceType.LowReady:
                return 0.15f;
            case EStanceType.HighReady:
                return 0f;
            case EStanceType.LeftShoulder:
                return 0f;
            case EStanceType.PatrolStance:
                return PluginConfig.test4.Value;
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
            case EStanceType.LowReady:
                return 1.15f;
            case EStanceType.HighReady:
                return 3f;
            case EStanceType.LeftShoulder:
                return 3f;
            case EStanceType.PatrolStance:
                return PluginConfig.test5.Value;
            case EStanceType.ShortStock:
                return 2.25f;
            default:
                return 3f;
        }
    }

    public override float TransitionToSpeedModifier(EStanceType? nextStance)
    {
        switch (nextStance)
        {
            case EStanceType.LowReady:
                return 0.75f;
            case EStanceType.HighReady:
                return 1f;
            case EStanceType.LeftShoulder:
                return 0.1f;
            case EStanceType.PatrolStance:
                return PluginConfig.test6.Value;
            case EStanceType.ShortStock:
                return 0.1f;
            default:
                return 1f;
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

