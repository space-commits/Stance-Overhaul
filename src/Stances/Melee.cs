using EFT.Animations;
using EFT;
using StanceOverhaul.Enums;
using System.Collections;
using UnityEngine;
using static RealismCommonLib.Plugin;
using StanceOverhaul.Controllers;
using System;
using RealismCommonLib.Utils;

namespace StanceOverhaul.Stances
{

/*    public class Melee : StanceBase
    {
        public override EStance StanceType => EStance.LeftShoulder;

        public override Vector3Curve EnterPositionCurve => _enterPos;
        public override Vector3Curve EnterRotationCurve => _enterRot;

        public override Vector3Curve ExitPositionCurve => _exitPos;
        public override Vector3Curve ExitRotationCurve => _exitRot;

        public override float BlendThreshold => 0.5f;
        public override float BaseSpeed => 1f;

        private readonly Vector3Curve _enterPos;
        private readonly Vector3Curve _enterRot;

        private readonly Vector3Curve _exitPos;
        private readonly Vector3Curve _exitRot;

        public Melee()
        {
            _enterPos = RealismCommonLib.Utils.CurveDrawer.GetCurve("left_enter_position");
            _enterRot = RealismCommonLib.Utils.CurveDrawer.GetCurve("left_enter_rotation");

            _exitPos = RealismCommonLib.Utils.CurveDrawer.GetCurve("left_exit_position");
            _exitRot = RealismCommonLib.Utils.CurveDrawer.GetCurve("left_exit_rotation");
        }
    }*/

    //TODO: change to use animation curves
    /*    public class PatrolStance : IStance
        {
            public bool DoStance { get; set; } = false;

            public bool HasReset { get; private set; } = false;

            public bool HasCompleted { get; private set; } = false;

            public bool ReadyToTransitionState { get; private set; } = false;

            public Vector3 CurrentPosition { get; private set; } = Vector3.zero;

            public Quaternion CurrentRotation { get; private set; } = Quaternion.identity;

            public EStance StanceType 
            { 
                get
                {
                    return EStance.PatrolStance;
                }
            }

            private StanceController _stanceController;
            private ProceduralWeaponAnimation _pwa;

            private Vector3 _currentRot = Vector3.zero;

            private Vector3 _riflePatrolPos = new Vector3(0.2f, 0.025f, 0.1f);
            private Vector3 _riflePatrolRot = new Vector3(0.05f, -0.05f, -0.5f);
            private Vector3 _pistolPatrolPos = new Vector3(0.05f, 0f, 0f);
            private Vector3 _pistolPatrolRot = new Vector3(0.1f, -0.1f, -0.1f);

            public PatrolStance(StanceController stanceController)
            {
                _stanceController = stanceController;
                _pwa = PlayerStateInstance.Player.ProceduralWeaponAnimation;
            }

            public void StanceUpdate()
            {
                DoPatrolStance();
            }

            public void DoPatrolStance()
            {
                Vector3 targetPos = DoStance ? Vector3.zero : WeaponStateInstance.TreatAsPistol ? _pistolPatrolPos : _riflePatrolPos;
                CurrentPosition = Vector3.Lerp(CurrentPosition, targetPos, 5.5f * Time.deltaTime);

                Vector3 targetRot = DoStance ? Vector3.zero : WeaponStateInstance.TreatAsPistol ? _pistolPatrolRot : _riflePatrolRot;
                _currentRot = Vector3.Lerp(_currentRot, targetRot, 5.5f * Time.deltaTime);
                CurrentRotation = Quaternion.Euler(_currentRot);

                float distanceToComplete = Vector3.Distance(CurrentPosition, targetPos);
                float distanceToReset = Vector3.Distance(CurrentPosition, targetPos);

                if (distanceToComplete <= 0.05f)
                    HasCompleted = true;
                else 
                    HasCompleted = false;

                if (distanceToComplete <= 0.05f)
                    HasReset = true;
                else
                    HasReset = false;

                if (HasCompleted || HasReset)
                    ReadyToTransitionState = false;
            }*/
}
