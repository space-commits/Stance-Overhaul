using RealismCommonLib.Utils;
using StanceOverhaul.Enums;
using UnityEngine;
using static RealismCommonLib.Plugin;
using static StanceOverhaul.Plugin;

namespace StanceOverhaul.Stances;

public class LowReady : StanceBase
{
    public override EStance StanceType => EStance.LowReady;

    public override Vector3Curve EnterPositionCurve => _enterPos;
    public override Vector3Curve EnterRotationCurve => _enterRot;

    public override Vector3Curve ExitPositionCurve => _exitPos;
    public override Vector3Curve ExitRotationCurve => _exitRot;

    public override float BlendThreshold => 0.2f;
    public override float BaseSpeed => 3.5f;

    private readonly Vector3Curve _enterPos;
    private readonly Vector3Curve _enterRot;

    private readonly Vector3Curve _exitPos;
    private readonly Vector3Curve _exitRot;

    public LowReady()
    {
        _enterPos = RealismCommonLib.Utils.CurveDrawer.GetCurve("low_ready_enter_position");
        _enterRot = RealismCommonLib.Utils.CurveDrawer.GetCurve("low_ready_enter_rotation");

        _exitPos = RealismCommonLib.Utils.CurveDrawer.GetCurve("low_ready_exit_position");
        _exitRot = RealismCommonLib.Utils.CurveDrawer.GetCurve("low_ready_exit_rotation");
    }
}
