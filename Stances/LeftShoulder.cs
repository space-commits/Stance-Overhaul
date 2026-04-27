using RealismCommonLib.Utils;
using StanceOverhaul.Enums;
using UnityEngine;
using static RealismCommonLib.Plugin;
using static StanceOverhaul.Plugin;

namespace StanceOverhaul.Stances;

public class LeftStance : StanceBase
{
    public override EStance StanceType => EStance.LeftShoulder;

    public override Vector3Curve EnterPositionCurve => _enterPos;
    public override Vector3Curve EnterRotationCurve => _enterRot;

    public override Vector3Curve ExitPositionCurve => _exitPos;
    public override Vector3Curve ExitRotationCurve => _exitRot;

    public override float BlendThreshold => 0.15f;
    public override float BaseSpeed => 3f;

    private readonly Vector3Curve _enterPos;
    private readonly Vector3Curve _enterRot;

    private readonly Vector3Curve _exitPos;
    private readonly Vector3Curve _exitRot;

    public LeftStance()
    {
        _enterPos = RealismCommonLib.Utils.CurveDrawer.GetCurve("left_enter_position");
        _enterRot = RealismCommonLib.Utils.CurveDrawer.GetCurve("left_enter_rotation");

        _exitPos = RealismCommonLib.Utils.CurveDrawer.GetCurve("left_exit_position");
        _exitRot = RealismCommonLib.Utils.CurveDrawer.GetCurve("left_exit_rotation");
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