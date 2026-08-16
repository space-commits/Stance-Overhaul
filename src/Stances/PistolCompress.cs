using RealismCommonLib.Utils;
using StanceOverhaul.Enums;
using UnityEngine;
using static RealismCommonLib.Plugin;
using static StanceOverhaul.Plugin;

namespace StanceOverhaul.Stances;

public class PistolCompress : StanceBase
{
    public override EStanceType StanceType => EStanceType.PistolCompress;
    public override EStaminaMode StaminaMode => EStaminaMode.Regen;

    public override Vector3Curve EnterPositionCurve => _enterPos;
    public override Vector3Curve EnterRotationCurve => _enterRot;

    public override Vector3Curve ExitPositionCurve => _exitPos;
    public override Vector3Curve ExitRotationCurve => _exitRot;

    public override float StaminaRate => PluginConfig.PistolStaminaRate.Value;
    public override float WalkSpeedBonus => PluginConfig.PistolWalkSpeedBonus.Value;
    public override float SprintAccelBonus => PluginConfig.PistolSprintAccelBonus.Value;
    public override float HipfireBonus => PluginConfig.PistolHipfireBonus.Value;
    public override bool RememberStance => true;

    public override AnimationCurve ExitAimSpeedCurve { get; } = new AnimationCurve
            (
                new Keyframe { time = 0f, value = 0f },
                new Keyframe { time = 0.15f, value = 0.05f },
                new Keyframe { time = 0.5f, value = 0.1f },
                new Keyframe { time = 0.75f, value = 0.15f },
                new Keyframe { time = 0.85f, value = 0.25f },
                new Keyframe { time = 1f, value = 1f }
            );

    public override AnimationCurve EnterAimSpeedCurve { get; } = new AnimationCurve
            (
                new Keyframe { time = 0f, value = 0f },
                new Keyframe { time = 0.15f, value = 0.05f },
                new Keyframe { time = 0.5f, value = 0.1f },
                new Keyframe { time = 0.75f, value = 0.15f },
                new Keyframe { time = 0.85f, value = 0.25f },
                new Keyframe { time = 1f, value = 1f }
            );

    public override float BlendIntoThreshold(EStanceType nextStance)
    {
        switch (nextStance)
        {
            default: return 0.0f;
        }
    }

    public override float TransitionFromModifier(EStanceType? previousStance)
    {
        float speed = PluginConfig.PistolSpeedModifier.Value;
        switch (previousStance)
        {
            case EStanceType.None: speed *= PluginConfig.PistolTransitionFromIdle.Value; return speed;
            default: return speed;
        }
    }

    public override float TransitionToSpeedModifier(EStanceType? nextStance)
    {
        float speed = PluginConfig.PistolSpeedModifier.Value;
        switch (nextStance)
        {
            case EStanceType.None: speed *= PluginConfig.PistolTransitionToIdle.Value; return speed;
            default: return speed;
        }
    }

    private readonly Vector3Curve _enterPos;
    private readonly Vector3Curve _enterRot;

    private readonly Vector3Curve _exitPos;
    private readonly Vector3Curve _exitRot;

    public PistolCompress()
    {
        _enterPos = CurveDrawer.GetCurve("pistol_compress_enter_position")!;
        _enterRot = CurveDrawer.GetCurve("pistol_compress_enter_rotation")!;

        _exitPos = CurveDrawer.GetCurve("pistol_compress_exit_position")!;
        _exitRot = CurveDrawer.GetCurve("pistol_compress_exit_rotation")!;
    }
}