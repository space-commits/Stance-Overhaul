using RealismCommonLib.Utils;
using StanceOverhaul.Enums;
using UnityEngine;
using static RealismCommonLib.Plugin;
using static StanceOverhaul.Plugin;

namespace StanceOverhaul.Stances;

public class ShortStock : StanceBase
{
    public override EStanceType StanceType => EStanceType.ShortStock;

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
                return 0f;
            case EStanceType.LowReady:
                return 0f;
            case EStanceType.PatrolStance:
                return PluginConfig.test4.Value;
            case EStanceType.LeftShoulder:
                return 0f;
            default:
                return 0.4f;
        }
    }

    public override float TransitionFromModifier(EStanceType? previousStance)
    {
        switch (previousStance)
        {
            case EStanceType.ActiveAiming:
                return 2.25f;
            case EStanceType.HighReady:
                return 0.65f;
            case EStanceType.LowReady:
                return 2.5f;
            case EStanceType.PatrolStance:
                return PluginConfig.test5.Value;
            case EStanceType.LeftShoulder:
                return 1.55f;
            default:
                return 2.25f;
        }
    }

    public override float TransitionToSpeedModifier(EStanceType? nextStance)
    {
        switch (nextStance)
        {
            case EStanceType.ActiveAiming:
                return 0.1f;
            case EStanceType.HighReady:
                return 1.35f;
            case EStanceType.LowReady:
                return 0.8f;
            case EStanceType.PatrolStance:
                return PluginConfig.test6.Value;
            case EStanceType.LeftShoulder:
                return 0.5f;
            default:
                return 1f;
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