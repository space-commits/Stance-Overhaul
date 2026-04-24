using RealismCommonLib.Utils;
using StanceOverhaul.Enums;
using UnityEngine;
using static RealismCommonLib.Plugin;
using static StanceOverhaul.Plugin;

namespace StanceOverhaul.Stances;

public class PatrolStance2 : StanceBase2
{
    public PatrolStance2() 
    {
        /*        PositionCurve = new Vector3Curve(PluginConfig.test6.Value, PluginConfig.test7.Value, PluginConfig.test8.Value);
                RotationCurve = new Vector3Curve(PluginConfig.test3.Value, PluginConfig.test4.Value, PluginConfig.test5.Value);*/

        PositionCurve = RealismCommonLib.Utils.CurveDrawer.GetCurve("Curve_0");
        RotationCurve = RealismCommonLib.Utils.CurveDrawer.GetCurve("Curve_1");
    }
}
 
