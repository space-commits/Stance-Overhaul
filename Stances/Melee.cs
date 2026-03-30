using EFT.Animations;
using EFT;
using StanceOverhaul.Enums;
using System.Collections;
using UnityEngine;
using static RealismCommonLib.Plugin;
using StanceOverhaul.Controllers;
using System;

namespace StanceOverhaul.Stances
{

    public class Melee : StanceBase
    {
        public override EStance StanceType => EStance.Melee;

        private Vector3 _currentPos;

        float _progress;
        float _speed = 5f;

        private bool _isHoldingBackMelee = false;

        public bool IsReadyForBayonetCharge
        {
            get
            {
                return (_isHoldingBackMelee);
            }
        }

        public override void Enter()
        {
            base.Enter();
            _progress = 0f;
        }

        public override void TryExit(bool force = false)
        { 
            base.TryExit();
            //no reset here - we reverse from current progress
        }


        private Vector3 GetPatrolPos()
        {
            return WeaponStateInstance.TreatAsPistol
                ? new Vector3(0.05f, 0f, 0f)
                : new Vector3(0.2f, 0.025f, 0.1f);
        }

        protected override bool UpdateEnter(float dt)
        {
            _progress += dt * _speed;
            _progress = Mathf.Clamp01(_progress);

            //float t = curve.Evaluate(_progress);, then pass this to Lerp.

            _currentPos = Vector3.Lerp(Vector3.zero, GetPatrolPos(), _progress);

            ApplyOffset();

            return _progress >= 1f;
        }

        protected override void UpdateActive(float dt)
        {
            ApplyOffset();
        }

        protected override bool UpdateExit(float dt)
        {
            _progress -= dt * _speed;
            _progress = Mathf.Clamp01(_progress);

            _currentPos = Vector3.Lerp(Vector3.zero, GetPatrolPos(), _progress);

            ApplyOffset();

            return _progress <= 0f;
        }

        private void ApplyOffset() 
        {
            //spring stuff goes here, or set property that state controller has access to
        }
    }

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
