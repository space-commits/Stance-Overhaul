using EFT.Animations;
using EFT;
using EFT.InventoryLogic;
using UnityEngine;
using System.Collections.Generic;
using StanceOverhaul.Enums;
using StanceOverhaul.Events;
using RealismCommonLib.Utils;
using RealismCommonLib.ModifierHandlers;
using static StanceOverhaul.Plugin;
using static RealismCommonLib.Plugin;
using System;
using StanceOverhaul.Stances;
using RealismCommonLib.Events;

namespace StanceOverhaul.SubSystem.Animator
{
    internal class LeftHandAnimaor : ISubSystem
    {
        private const float LeftHandDamping = 0.85f;
        private const float LeftHandReturnSpeed = 0.06f;

        //private bool _canRemoveLeftHandForStance;

        private DelayTimer _leftHandResetTimer = new DelayTimer(0.5f);

        public Vector3 LeftHandMarkerPosition
        {
            get
            {
                return LeftHandMarkerGrip.transform.position;
            }
        }
        public Quaternion LeftHandMarkerRotation
        {
            get
            {
                return LeftHandMarkerGrip.transform.rotation;
            }
        }

        public Vector3 LeftHandPositionTargetOffset { get; private set; } = Vector3.zero;
        public Vector3 LeftHandRotationTargetOffset { get; private set; } = Vector3.zero;

        public bool OverrideLeftHand { get; private set; }

        private bool DoOffsets
        {
            get
            {
                return
                    PlayerStateInstance.WeaponIsReady &&
                    !AimStateInstance.IsAiming &&
                    !PlayerStateInstance.IsMounting &&
                    !PlayerStateInstance.IsSprinting &&
                    !PlayerStateInstance.IsUsingStationaryWeapon;
                //&& PlayerStateInstance.Player.LeftHandInteractionTarget == null;
            }
        }


        public void RunOnAwake()
        {
            StanceEvents.OnStanceEntered += StanceRemoveLeftHand;
            StanceEvents.OnStanceExitedRef += StanceAttachLeftHand;
            // PWAEvents.OnUpdateWeaponVariablesWithFc += InitTargets;
            // StanceEvents.OnStanceReloadReset += ResetTimers;
            // StanceEvents.OnStanceCheckAmmo += StartCheckAmmoTimer;
            // StanceEvents.OnStanceChamberCheck += StartChamberCheckTimer;
            // StanceEvents.OnStanceChamber += StartRechamberTimer;
            // StanceEvents.OnTransformsInitFC += SetBaseWeaponOffsetPosition;

            SetSpringValues();
        }

        public void RunOnDestroy()
        {
            StanceEvents.OnStanceEntered -= StanceRemoveLeftHand;
            StanceEvents.OnStanceExitedRef -= StanceAttachLeftHand;
            // PlayerEvents.OnPlayerInitRef -= InitTargets;
            // StanceEvents.OnStanceReloadReset -= ResetTimers;
            // StanceEvents.OnStanceCheckAmmo -= StartCheckAmmoTimer;
            // StanceEvents.OnStanceChamberCheck -= StartChamberCheckTimer;
            // StanceEvents.OnStanceChamber -= StartRechamberTimer;
            // StanceEvents.OnTransformsInitFC -= SetBaseWeaponOffsetPosition;
        }

        public void RunOnUpdate(float deltaTime)
        {
            if (LeftHandMarkerGO == null)
                return;

            UpdateTimers();
            UpdateLeftHandTargets();
            UpdateTransforms();
        }

        private void UpdateTimers()
        {
            _leftHandResetTimer.Update();
        }

        private void ResetTimers()
        {
            _leftHandResetTimer.Stop();
        }

        private void StartLeftHandTimer()
        {
            _leftHandResetTimer.Start();
        }

        private void StanceRemoveLeftHand(IStance stance)
        {
            ModLogger.LogWarning("on");
            if (stance.StanceType == EStanceType.PatrolStance)
            {
                // PlayerStateInstance.Player.LeftHandInteractionTarget = LeftHandMarkerGrip;
                // PlayerStateInstance.Player.ThirdIkWeight.Target = 1f;

                // PlayerStateInstance.Player.MovementContext.SetIKInteraction(LeftHandMarkerGrip);

                //OverrideLeftHand = true;
                PlayerStateInstance.Player.HandsAnimator.SetInventory(true);
            }

            //_canRemoveLeftHandForStance = true;
        }

        private void StanceAttachLeftHand(IStance stance)
        {
            ModLogger.LogWarning("off");
            // if (PlayerStateInstance.Player.LeftHandInteractionTarget == LeftHandMarkerGrip)
            // {
            //     PlayerStateInstance.Player.MovementContext.SetIKInteraction(null);

            //PlayerStateInstance.Player.LeftHandInteractionTarget = null;
            if (stance.StanceType == EStanceType.PatrolStance)
            {
                PlayerStateInstance.Player.HandsAnimator.SetInventory(false);
            }
            //  PlayerStateInstance.Player.ThirdIkWeight.Target = 0f;

            //     //OverrideLeftHand = true;
            // }

            //_canRemoveLeftHandForStance = false;
        }

        private void SetSpringValues()
        {
            StanceControllerInstance.OffsetPositionSpring.ReturnSpeed = StanceControllerInstance.StatsHandlerInstance.GetSpringReturnSpeed(LeftHandDamping);
            StanceControllerInstance.OffsetRotationSpring.ReturnSpeed = StanceControllerInstance.StatsHandlerInstance.GetSpringReturnSpeed(LeftHandReturnSpeed);

            StanceControllerInstance.OffsetPositionSpring.Damping = StanceControllerInstance.StatsHandlerInstance.GetSpringDamping(LeftHandDamping);
            StanceControllerInstance.OffsetRotationSpring.Damping = StanceControllerInstance.StatsHandlerInstance.GetSpringDamping(LeftHandReturnSpeed);
        }

        // public void InitTargets(Player.FirearmController fc)
        // {
        //     ModLogger.LogWarning("=======player init");

        //     LeftHandMarkerGO = new GameObject();
        //     LeftHandMarkerGO.transform.SetParent(fc.Transform.Original);
        // }

        private void HandlePistolPatrolStanceOffset()
        {
            if (WeaponStateInstance.TreatAsPistol && StanceControllerInstance.CurrentStanceType == EStanceType.PatrolStance)
            {
                LeftHandPositionTargetOffset += new Vector3(PluginConfig.test1.Value, PluginConfig.test2.Value, PluginConfig.test3.Value);
                LeftHandRotationTargetOffset += new Vector3(PluginConfig.test4.Value, PluginConfig.test5.Value, PluginConfig.test6.Value);
            }
        }

        //non-stance related rotational and postion changes for immersion
        private void UpdateLeftHandTargets()
        {
            LeftHandPositionTargetOffset = Vector3.zero;
            LeftHandRotationTargetOffset = Vector3.zero;

            if (DoOffsets)
            {
                HandlePistolPatrolStanceOffset();
                PlayerStateInstance.Player.ThirdIkWeight.Speed = PluginConfig.test8.Value;
            }
        }

        private void UpdateTransforms()
        {
            //should be lerped
            // LeftHandMarkerGrip.transform.localPosition = LeftHandPositionTargetOffset;
            // LeftHandMarkerGrip.transform.localRotation = Quaternion.Euler(LeftHandRotationTargetOffset);

            LeftHandMarkerGrip.transform.localPosition = Vector3.Lerp(LeftHandMarkerGrip.transform.localPosition, LeftHandPositionTargetOffset, Time.deltaTime * PluginConfig.test7.Value);
            LeftHandMarkerGrip.transform.localRotation = Quaternion.Lerp(LeftHandMarkerGrip.transform.localRotation, Quaternion.Euler(LeftHandRotationTargetOffset), Time.deltaTime * PluginConfig.test7.Value);
        }
    }
}
