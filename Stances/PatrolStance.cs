using RealismCommonLib.Utils;
using StanceOverhaul.Enums;
using UnityEngine;
using static RealismCommonLib.Plugin;
using static StanceOverhaul.Plugin;

namespace StanceOverhaul.Stances;

public class PatrolStance : StanceBase
{
    public override EStanceType StanceType => EStanceType.PatrolStance;

    public override Vector3Curve EnterPositionCurve => _enterPos;
    public override Vector3Curve EnterRotationCurve => _enterRot;

    public override Vector3Curve ExitPositionCurve => _exitPos;
    public override Vector3Curve ExitRotationCurve => _exitRot;

    public override float BlendIntoThreshold(EStanceType nextStance)
    {
        switch (nextStance)
        {
            case EStanceType.ActiveAiming:
                return PluginConfig.test1.Value;
            case EStanceType.LowReady:
                return PluginConfig.test1.Value;
            case EStanceType.LeftShoulder:
                return PluginConfig.test1.Value;
            case EStanceType.PatrolStance:
                return PluginConfig.test1.Value;
            case EStanceType.ShortStock:
                return PluginConfig.test1.Value;
            default:
                return 0.2f;
        }
    }

    public override float TransitionFromModifier(EStanceType? previousStance)
    {
        switch (previousStance)
        {
            case EStanceType.ActiveAiming:
                return PluginConfig.test2.Value;
            case EStanceType.LowReady:
                return PluginConfig.test2.Value;
            case EStanceType.LeftShoulder:
                return PluginConfig.test2.Value;
            case EStanceType.PatrolStance:
                return PluginConfig.test2.Value;
            case EStanceType.ShortStock:
                return PluginConfig.test2.Value;
            default:
                return 3f;
        }
    }

    public override float TransitionToSpeedModifier(EStanceType? nextStance)
    {
        switch (nextStance)
        {
            case EStanceType.ActiveAiming:
                return PluginConfig.test3.Value;
            case EStanceType.LowReady:
                return PluginConfig.test3.Value;
            case EStanceType.LeftShoulder:
                return PluginConfig.test3.Value;
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

    public PatrolStance() 
    {
        _enterPos = RealismCommonLib.Utils.CurveDrawer.GetCurve("patrol_enter_position")!;
        _enterRot = RealismCommonLib.Utils.CurveDrawer.GetCurve("patrol_enter_rotation")!;

        _exitPos = RealismCommonLib.Utils.CurveDrawer.GetCurve("patrol_exit_position")!;
        _exitRot = RealismCommonLib.Utils.CurveDrawer.GetCurve("patrol_exit_rotation")!;
    }
}
 
