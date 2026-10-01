using EFT;
using UnityEngine;
using StanceOverhaul.Enums;
using StanceOverhaul.Events;
using RealismCommonLib.Utils;
using static StanceOverhaul.Plugin;
using static RealismCommonLib.Plugin;
using StanceOverhaul.Stances;

namespace StanceOverhaul.SubSystem.Animator
{
    internal class LeftHandAnimaor : ISubSystem
    {
        private DelayTimer _leftHandTimer = new DelayTimer(0.8f);
        private DelayTimer _propSoundBlockTimer = new DelayTimer(2f);
        private bool _overrideLeftHand = false;
        private bool _blockSoundEffect = false;
        private bool _updateOverride = false;

        private GameObject _leftHandMarkerGO;
        private GripPose _leftHandMarkerGrip;

        public bool BlockLeftHandSFX
        {
            get
            {
                return _blockSoundEffect;
            }
        }

        public Vector3 LeftHandMarkerPosition
        {
            get
            {
                return _leftHandMarkerGrip.transform.position;
            }
        }
        public Quaternion LeftHandMarkerRotation
        {
            get
            {
                return _leftHandMarkerGrip.transform.rotation;
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
            if (_leftHandMarkerGO == null)
                return;

            UpdateLeftHandTargets();
            UpdateTransforms();
        }

        private void OnPlayerLoad(Player player)
        {
            _leftHandMarkerGO = new GameObject("ProceduralLeftHandTarget");
            _leftHandMarkerGO.transform.SetParent(player.gameObject.transform, false);

            _leftHandMarkerGrip = _leftHandMarkerGO.AddComponent<GripPose>();
            _leftHandMarkerGrip.Hand = GripPose.EHand.Left;
            _leftHandMarkerGrip.GripType = GripPose.EGripType.Common;
            _leftHandMarkerGrip.DontCache = true;
            _leftHandMarkerGrip.transform.localPosition = Vector3.zero;
            _leftHandMarkerGrip.transform.localRotation = Quaternion.identity;
        }

        private void StanceRemoveLeftHand(IStance stance)
        {
            if (stance.StanceType == EStanceType.PatrolStance && WeaponStateInstance.TreatAsPistol)
            {
                PlayerStateInstance.Player.LeftHandInteractionTarget = _leftHandMarkerGrip;
                _leftHandTimer.Start();
                _propSoundBlockTimer.Start();
                _overrideLeftHand = true;
                _updateOverride = false;
            }
        }

        private void StanceAttachLeftHand(IStance stance)
        {
            if (stance.StanceType == EStanceType.PatrolStance && WeaponStateInstance.TreatAsPistol)
            {
                PlayerStateInstance.Player.HandsAnimator.ShowCompass(false);
                _propSoundBlockTimer.Start();
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

            if (!_propSoundBlockTimer.Update())
            {
                _blockSoundEffect = true;
            }
            else
            {
                _blockSoundEffect = false;
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
            _leftHandMarkerGrip.transform.localPosition = Vector3.Lerp(_leftHandMarkerGrip.transform.localPosition, LeftHandPositionTargetOffset, Time.deltaTime * PluginConfig.test9.Value);
            _leftHandMarkerGrip.transform.localRotation = Quaternion.Lerp(_leftHandMarkerGrip.transform.localRotation, Quaternion.Euler(LeftHandRotationTargetOffset), Time.deltaTime * PluginConfig.test9.Value);

            if (PlayerStateInstance.PWA.HandsContainer.WeaponRoot != _leftHandMarkerGO.transform.parent)
            {
                _leftHandMarkerGO.transform.SetParent(PlayerStateInstance.PWA.HandsContainer.TrackingTransform, false);
            }

        }
    }
}
