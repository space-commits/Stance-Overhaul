using RealismCommonLib.Utils;
using StanceOverhaul.Enums;
using UnityEngine;
using static RealismCommonLib.Plugin;
using static StanceOverhaul.Plugin;

namespace StanceOverhaul.Stances;

public class PatrolStance : StanceBase
{
    public override EStance StanceType => EStance.PatrolStance;

    float _progress;
    float _speed = 5f;

    protected override void OnInternalMagReload() => CancelStanceOnReload();
    protected override void OnQuickMagReload() => CancelStanceOnReload();
    protected override void OnMagReload() => CancelStanceOnReload();
    protected override void OnCheckChamber() => CancelStanceOnManip();
    protected override void OnRechamber() => CancelStanceOnManip();
    protected override void OnMalfFix() => CancelStanceOnManip();

    private void CancelStanceOnReload()
    {
        if (!ReloadStateInstance.IsInReloadOpertation) return;
        CancelStanceOnManip();
    }

    private void CancelStanceOnManip()
    {
        TryExit(true);
    }

    public override void Enter()
    {
        base.Enter();
        _progress = 0f;
    }

    protected override bool UpdateEnter(float dt)
    {
        if (PauseStance) DoStanceExitAnimation(dt);
        else DoStanceAnimation(dt);

        return MathUtils.IsGreaterThanOrEqualTo(_progress, 1f);
    }

    protected override void UpdateActive(float dt)
    {
        if (PauseStance) DoStanceExitAnimation(dt);
        else DoStanceAnimation(dt); //called here to allow re-entering stance after pausing
    }

    protected override bool UpdateExit(float dt)
    {
        DoStanceExitAnimation(dt);

        //TODO: have switch statement with differnt blend values for different stances
        //if transitioning to another stance, blend out until fully exited, if toggling off without another stance, blend back to default values
        float threshold =  StanceControllerInstance.NextStanceType != EStance.None ? PluginConfig.test18.Value : 0f;

        CanTransition = MathUtils.IsLessThanOrEqualTo(_progress, threshold);

        return MathUtils.IsLessThanOrEqualTo(_progress, 0f); 
    }

    private void DoStanceAnimation(float dt) 
    {
        _progress += dt * PluginConfig.test1.Value;
        _progress = Mathf.Clamp01(_progress);

        var rotCurve = new Vector3Curve(PluginConfig.test3.Value, PluginConfig.test4.Value, PluginConfig.test5.Value);
        StanceRotation = rotCurve.Evaluate(_progress);

        var posCurve = new Vector3Curve(PluginConfig.test6.Value, PluginConfig.test7.Value, PluginConfig.test8.Value);
        StancePosition = posCurve.Evaluate(_progress);

        SetCanExit(true); //while active, can transition to another stance at any time
    }

    private void DoStanceExitAnimation(float dt) 
    {
        _progress -= dt * PluginConfig.test1.Value;
        _progress = Mathf.Clamp01(_progress);

        var rotCurve = new Vector3Curve(PluginConfig.test3.Value, PluginConfig.test4.Value, PluginConfig.test5.Value);
        StanceRotation = rotCurve.Evaluate(_progress);

        var posCurve = new Vector3Curve(PluginConfig.test6.Value, PluginConfig.test7.Value, PluginConfig.test8.Value);
        StancePosition = posCurve.Evaluate(_progress);
    }
}

