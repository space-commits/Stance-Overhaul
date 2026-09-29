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
        private DelayTimer _leftHandTimer = new DelayTimer(0.8f);
        private bool _overrideLeftHand = false;
        private bool _updateOverride = false;

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
            }
        }


        public void RunOnAwake()
        {
            StanceEvents.OnStanceEntered += StanceRemoveLeftHand;
            StanceEvents.OnStanceExitedRef += StanceAttachLeftHand;
            StanceEvents.OnPlayerLoadRef += OnPlayerLoad;
            // PWAEvents.OnUpdateWeaponVariablesWithFc += InitTargets;
            // StanceEvents.OnStanceReloadReset += ResetTimers;
            // StanceEvents.OnStanceCheckAmmo += StartCheckAmmoTimer;
            // StanceEvents.OnStanceChamberCheck += StartChamberCheckTimer;
            // StanceEvents.OnStanceChamber += StartRechamberTimer;
            // StanceEvents.OnTransformsInitFC += SetBaseWeaponOffsetPosition;
        }

        public void RunOnDestroy()
        {
            StanceEvents.OnStanceEntered -= StanceRemoveLeftHand;
            StanceEvents.OnStanceExitedRef -= StanceAttachLeftHand;
            StanceEvents.OnPlayerLoadRef -= OnPlayerLoad;
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

            UpdateLeftHandTargets();
            UpdateTransforms();
        }

        private void OnPlayerLoad(Player player)
        {
            LeftHandMarkerGO = new GameObject("ProceduralLeftHandTarget");
            LeftHandMarkerGO.transform.SetParent(player.gameObject.transform, false);

            LeftHandMarkerGrip = LeftHandMarkerGO.AddComponent<GripPose>();
            LeftHandMarkerGrip.Hand = GripPose.EHand.Left;
            LeftHandMarkerGrip.GripType = GripPose.EGripType.Alternative;
            LeftHandMarkerGrip.DontCache = true;
            LeftHandMarkerGrip.transform.localPosition = Vector3.zero;
            LeftHandMarkerGrip.transform.localRotation = Quaternion.identity;
        }

        private void StanceRemoveLeftHand(IStance stance)
        {
            if (stance.StanceType == EStanceType.PatrolStance && WeaponStateInstance.TreatAsPistol)
            {
                PlayerStateInstance.Player.LeftHandInteractionTarget = LeftHandMarkerGrip;
                _leftHandTimer.Start();
                _overrideLeftHand = true;
                _updateOverride = false;
            }
        }

        private void StanceAttachLeftHand(IStance stance)
        {
            if (stance.StanceType == EStanceType.PatrolStance && WeaponStateInstance.TreatAsPistol)
            {
                PlayerStateInstance.Player.HandsAnimator.ShowCompass(false);
                _overrideLeftHand = false;
                _updateOverride = true;
            }
        }

        private void SetDefaultPosition()
        {
            LeftHandPositionTargetOffset += new Vector3(PluginConfig.test1.Value, PluginConfig.test2.Value, PluginConfig.test3.Value);
            LeftHandRotationTargetOffset += new Vector3(PluginConfig.test4.Value, PluginConfig.test5.Value, PluginConfig.test6.Value);
        }

        private void UpdateLeftHandTargets()
        {
            LeftHandPositionTargetOffset = Vector3.zero;
            LeftHandRotationTargetOffset = Vector3.zero;

            if (DoOffsets)
            {
                SetDefaultPosition();
            }

            if (_leftHandTimer.Update())
            {
                _updateOverride = true;
                PlayerStateInstance.Player.HandsAnimator.ShowCompass(true);
            }

            if (_updateOverride)
            {
                var speed = _overrideLeftHand ? PluginConfig.test7.Value : PluginConfig.test8.Value;
                PlayerStateInstance.Player.ThirdIkWeight.Value = Mathf.MoveTowards(PlayerStateInstance.Player.ThirdIkWeight.Value, _overrideLeftHand ? 1f : 0f, speed * Time.deltaTime);
            }

            _leftHandTimer.Duration = PluginConfig.test10.Value;

        }

        private void UpdateTransforms()
        {
            LeftHandMarkerGrip.transform.localPosition = Vector3.Lerp(LeftHandMarkerGrip.transform.localPosition, LeftHandPositionTargetOffset, Time.deltaTime * PluginConfig.test9.Value);
            LeftHandMarkerGrip.transform.localRotation = Quaternion.Lerp(LeftHandMarkerGrip.transform.localRotation, Quaternion.Euler(LeftHandRotationTargetOffset), Time.deltaTime * PluginConfig.test9.Value);

            if (PlayerStateInstance.PWA.HandsContainer.WeaponRoot != LeftHandMarkerGO.transform.parent)
            {
                LeftHandMarkerGO.transform.SetParent(PlayerStateInstance.PWA.HandsContainer.TrackingTransform, false);
            }

        }
    }
}
