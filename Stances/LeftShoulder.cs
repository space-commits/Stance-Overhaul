using RealismCommonLib.Utils;
using StanceOverhaul.Enums;
using UnityEngine;
using static RealismCommonLib.Plugin;
using static StanceOverhaul.Plugin;

namespace StanceOverhaul.Stances;

public class LeftStance : StanceBase
{
    public override EStance StanceType => EStance.LeftShoulder;

    float _progress;
    float _speed = 5f;

    protected override void OnWeaponStateReset() => Unpause();
    protected override void OnInternalMagReload() => PauseStanceOnReload();
    protected override void OnQuickMagReload() => PauseStanceOnReload();
    protected override void OnMagReload() => PauseStanceOnReload();
    protected override void OnCheckChamber() => Pause();
    protected override void OnRechamber() => Pause();
    protected override void OnMalfFix() => Pause();

    private void PauseStanceOnReload()
    {
        if (!ReloadStateInstance.IsInReloadOpertation) return;
        Pause();
    }

    private void Pause()
    {
        PauseStance = true;
    }

    private void Unpause()
    {
        PauseStance = false;
    }

    public override void Enter()
    {
        base.Enter();
        _progress = 0f;
    }

    protected override bool UpdateEnter(float dt)
    {
        if (base.PauseStance) DoStanceExitAnimation(dt);
        else DoStanceAnimation(dt);

        return MathUtils.IsGreaterThanOrEqualTo(_progress, 1f);
    }

    protected override void UpdateActive(float dt)
    {
        if (base.PauseStance) DoStanceExitAnimation(dt);
        else DoStanceAnimation(dt); //called here to allow re-entering stance after pausing
    }

    protected override bool UpdateExit(float dt)
    {
        DoStanceExitAnimation(dt);

        //TODO: have switch statement with differnt blend values for different stances
        //if transitioning to another stance, blend out until fully exited, if toggling off without another stance, blend back to default values
        float threshold = StanceControllerInstance.NextStanceType != EStance.None ? PluginConfig.test19.Value : 0f;

        CanTransition = MathUtils.IsLessThanOrEqualTo(_progress, threshold);

        return MathUtils.IsLessThanOrEqualTo(_progress, 0f);
    }

    private void DoStanceAnimation(float dt)
    {
        _progress += dt * PluginConfig.test1.Value;
        _progress = Mathf.Clamp01(_progress);

        var rotCurve = new Vector3Curve(PluginConfig.test11.Value, PluginConfig.test12.Value, PluginConfig.test13.Value);
        StanceRotation = rotCurve.Evaluate(_progress);

        var posCurve = new Vector3Curve(PluginConfig.test14.Value, PluginConfig.test15.Value, PluginConfig.test16.Value);
        StancePosition = posCurve.Evaluate(_progress);

        SetCanExit(true); //while active, can transition to another stance at any time
    }

    private void DoStanceExitAnimation(float dt)
    {
        _progress -= dt * PluginConfig.test1.Value;
        _progress = Mathf.Clamp01(_progress);

        var rotCurve = new Vector3Curve(PluginConfig.test11.Value, PluginConfig.test12.Value, PluginConfig.test13.Value);
        StanceRotation = rotCurve.Evaluate(_progress);

        var posCurve = new Vector3Curve(PluginConfig.test14.Value, PluginConfig.test15.Value, PluginConfig.test16.Value);
        StancePosition = posCurve.Evaluate(_progress);
    }
}









/*using EFT.Animations;
using StanceOverhaul.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using StanceOverhaul.Enums;
using static RealismCommonLib.Plugin;
using RealismCommonLib.Utils;
using EFT;

namespace StanceOverhaul.Stances
{
    public class LeftShoulder : IStance
    {
        public bool DoStance { get; set; } = false;

        public bool HasReset { get; private set; } = false;

#warning implement check for this
        public bool HasCompleted { get; private set; } = false;

#warning implement check for this
        public bool ReadyToTransitionState { get; private set; } = false;

#warning this is equivalent to DoStance, decide if controller should determine this or stance class

        public Vector3 CurrentPosition { get; private set; } = Vector3.zero;

        public Quaternion CurrentRotation { get; private set; } = Quaternion.identity;

        public EStance StanceType
        {
            get
            {
                return EStance.LeftShoulder;
            }
        }

        private StanceController _stanceController;
        private ProceduralWeaponAnimation _pwa;

        public bool _isLeftStanceResetState = false;
        private float _leftStanceTime = 0f;
        private Vector3 _leftStanceRotation;

        private Vector3 _leftStancePistolRotaitonTarget = new Vector3(0f, -10f, 0f);
        private Vector3 _leftStancePistolPositionTarget = new Vector3(0f, -0.02f, 0f);
        private Vector3 _leftStanceRifleRotaitonTarget = new Vector3(0f, -10f, 0f);
        private Vector3 _leftStanceRiflePositionTarget = new Vector3(0f, 0f, 0f);
        private Vector3 _leftStanceVelocity = Vector3.zero;
        private float _leftStanceProgress = 0f;
        private float _leftStanceTargetX;

        private AnimationCurve _leftRotationXCurve = new AnimationCurve(
            new Keyframe(0, 0f),
            new Keyframe(0.25f, -2f),
            new Keyframe(0.5f, -5f),
            new Keyframe(0.75f, -1.5f),
            new Keyframe(1, 0f)
        );

        private AnimationCurve _leffPosZCurve = new AnimationCurve(
            new Keyframe(0, 0f),
            new Keyframe(0.15f, 0.1f),
            new Keyframe(0.3f, 0.075f),
            new Keyframe(0.5f, 0.1f),
            new Keyframe(0.65f, 0.05f),
            new Keyframe(0.7f, 0.025f),
            new Keyframe(0.9f, -0.045f),
            new Keyframe(1, 0f)
        );

        private AnimationCurve _leffPosZCurveReturn = new AnimationCurve(
            new Keyframe(0, 0f),
            new Keyframe(0.15f, -0.05f),
            new Keyframe(0.3f, 0.025f),
            new Keyframe(0.5f, 0.05f),
            new Keyframe(0.65f, 0.075f),
            new Keyframe(0.7f, 0.05f),
            new Keyframe(0.9f, 0.1f),
            new Keyframe(1, 0f)
            );


        public LeftShoulder(StanceController stanceController)
        {
            _stanceController = stanceController;
            _pwa = PlayerStateInstance.Player.ProceduralWeaponAnimation;
        }

        public void StanceUpdate()
        {
            DoLeftShoulder();
        }

        private void DoLeftShoulder()
        {
            float stanceMulti = _stanceController.StanceSpeedMultiplier();
            float curveModifier = _stanceController.TreatWeaponAsPistolStance ? 0.05f : 4.5f;
            float baseSpeed = Mathf.Clamp((1f - stanceMulti) + 1f, 0.05f, 1.5f);
            float speed = AimStateInstance.IsAiming ? baseSpeed * 0.22f : baseSpeed * 0.22f;

            //position
            var xTarget = _leftStancePistolRotaitonTarget.x + PluginConfig.LeftShoulderOffset.Value;
            var position = DoStance
                ? new Vector3(xTarget, _leftStancePistolRotaitonTarget.y, _leftStancePistolRotaitonTarget.z + (_leffPosZCurve.Evaluate(_leftStanceProgress) * curveModifier))
                : new Vector3(0f, 0f, _leffPosZCurveReturn.Evaluate(_leftStanceProgress) * curveModifier);

            if (DoStance)
            {
                _leftStanceTargetX = xTarget;
                _leftStanceTime = 0f;
                _isLeftStanceResetState = false;
            }
            else
            {
                _leftStanceTime += Time.deltaTime;
                if (_leftStanceTime <= 0.5f)
                {
                    _isLeftStanceResetState = true;
                }
                else
                {
                    _isLeftStanceResetState = false;
                }
            }

            CurrentPosition = Vector3.SmoothDamp(CurrentPosition, position, ref _leftStanceVelocity, speed, 0.55f, Time.deltaTime);

            _leftStanceProgress = Mathf.InverseLerp(0f, _leftStanceTargetX, CurrentPosition.x);

            if (MathUtils.AreFloatsEqual(_leftStanceProgress, 0f) && !DoStance)
                HasReset = true;
            else 
                HasReset = false;

            //moving towards 1, and is left shoulder
            bool isTransitionignLeft = DoStance && MathUtils.IsLessThan(_leftStanceProgress, 0.99f);
            bool isTransitioningRight = (_isLeftStanceResetState || _stanceController.TargetStance != EStance.LeftShoulder) && MathUtils.IsGreaterThan(_leftStanceProgress, 0.01f);

            if (AimStateInstance.IsAiming && (isTransitionignLeft || isTransitioningRight))
            {
                _stanceController.InterruptAim();
            }
            if (!isTransitionignLeft && !isTransitioningRight)
            {
                _stanceController.UnInterruptAim();
            }

            //rotation
            var rotation = DoStance && !AimStateInstance.IsAiming ? _leftStancePistolRotaitonTarget : Vector3.zero;
            rotation.x += _leftRotationXCurve.Evaluate(_leftStanceProgress);

            _leftStanceRotation = Vector3.Lerp(_leftStanceRotation, rotation, stanceMulti * 2.5f * Time.deltaTime);
            CurrentRotation = Quaternion.Euler(_leftStanceRotation);
        }
    }
}
*/