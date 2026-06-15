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

    public override float BlendThreshold(EStanceType nextStance)
    {
        switch (nextStance)
        {
            case EStanceType.LowReady:
                return 0.25f;
            case EStanceType.HighReady:
                return PluginConfig.test7.Value;
            case EStanceType.LeftShoulder:
                return PluginConfig.test7.Value;
            case EStanceType.PatrolStance:
                return PluginConfig.test7.Value;
            case EStanceType.ShortStock:
                return PluginConfig.test7.Value;
            default:
                return 0.2f;
        }
    }

    public override float BaseSpeed(EStanceType? previousStance)
    {
        switch (previousStance)
        {
            case EStanceType.LowReady:
                return 0.6f;
            case EStanceType.HighReady:
                return PluginConfig.test8.Value;
            case EStanceType.LeftShoulder:
                return PluginConfig.test8.Value;
            case EStanceType.PatrolStance:
                return PluginConfig.test8.Value;
            case EStanceType.ShortStock:
                return PluginConfig.test8.Value;
            default:
                return 3f;
        }
    }

    public override float TransitionSpeedModifier(EStanceType? nextStance)
    {
        switch (nextStance)
        {
            case EStanceType.LowReady:
                return PluginConfig.test8.Value;
            case EStanceType.HighReady:
                return PluginConfig.test8.Value;
            case EStanceType.LeftShoulder:
                return PluginConfig.test8.Value;
            case EStanceType.PatrolStance:
                return PluginConfig.test8.Value;
            case EStanceType.ShortStock:
                return PluginConfig.test8.Value;
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

