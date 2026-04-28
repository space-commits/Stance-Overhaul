using RealismCommonLib.Utils;
using StanceOverhaul.Enums;
using UnityEngine;
using static RealismCommonLib.Plugin;
using static StanceOverhaul.Plugin;

namespace StanceOverhaul.Stances;

public class HighReady : StanceBase
{
    public override EStance StanceType => EStance.HighReady;

    public override Vector3Curve EnterPositionCurve => _enterPos;
    public override Vector3Curve EnterRotationCurve => _enterRot;

    public override Vector3Curve ExitPositionCurve => _exitPos;
    public override Vector3Curve ExitRotationCurve => _exitRot;

    public override float BlendThreshold => 0.5f; 
    public override float BaseSpeed => 3f; 

    private readonly Vector3Curve _enterPos;
    private readonly Vector3Curve _enterRot;

    private readonly Vector3Curve _exitPos;
    private readonly Vector3Curve _exitRot;

    public HighReady()
    {
        _enterPos = RealismCommonLib.Utils.CurveDrawer.GetCurve("high_ready_enter_position");
        _enterRot = RealismCommonLib.Utils.CurveDrawer.GetCurve("high_ready_enter_rotation");

        _exitPos = RealismCommonLib.Utils.CurveDrawer.GetCurve("high_ready_exit_position");
        _exitRot = RealismCommonLib.Utils.CurveDrawer.GetCurve("high_ready_exit_rotation");
    }
}
 