using System;
using System.Linq;
using RealismCommonLib.Events;
using StanceOverhaul.Events;
using StanceOverhaul.Enums;
using StanceOverhaul.Stances;
using StanceOverhaul.State;
using static RealismCommonLib.Plugin;
using static StanceOverhaul.Plugin;

namespace StanceOverhaul.SubSystem.StanceInput
{
    internal enum EStanceInterruptType
    {
        None,
        ADS,
        Reload
    }

    internal class StanceInputHandler : ISubSystem
    {
        private IStance? _stanceThatWasToggledOriginally;
        private IStance? _stanceBeforeInterrupt;
        private EStanceInterruptType _interruptType = EStanceInterruptType.None;
        private bool _aimedFromActiveAim;

        private StanceState _stanceState;
        public StanceInputHandler(StanceState stanceState)
        {
            _stanceState = stanceState;
        }

        public void RunOnAwake()
        {
            SubscribeToEvents();
        }

        public void RunOnDestroy()
        {
            UnSubscribeToEvents();
        }

        public void RunOnUpdate(float deltaTime)
        {
            StanceInputUpdate();
        }

        private void SubscribeToEvents()
        {
            PlayerEvents.OnWeaponSwap += OnWeaponSwap;
            PlayerEvents.OnSwappedFromItemToGun += OnSwappedBackToGun;
            PlayerEvents.OnSwappedFromGunToItem += OnSwappedToItem;
            PlayerEvents.AimStateChanged += OnADSToggled;
            PlayerEvents.OnShotFired += OnShotFired;
            StanceInputEvents.TogglePatrolStance += TogglePatrolStance;
            StanceInputEvents.ToggleHighReady += ToggleHighReady;
            StanceInputEvents.ToggleLowReady += ToggleLowReady;
            StanceInputEvents.ToggleShortStock += ToggleShortStock;
            StanceInputEvents.ToggleActiveAim += ToggleActiveAim;
            StanceInputEvents.OnActiveAimKeyDown += OnActiveAimKeyDown;
            StanceInputEvents.OnActiveAimKeyUp += OnActiveAimKeyUp;
            StanceInputEvents.ToggleMelee += ToggleMelee;
            InputEvents.ToggleLeftStanceInput += ToggleLeftShoulder;
            StanceInputEvents.OnAttemptedToFireFromStance += OnAttemptedToFireFromStance;
            StanceEvents.OnStanceReloadReset += ResetReloadState;
            StanceEvents.OnStanceReload += CheckIfReloadInterruptsStance;
            //StanceInputEvents.ToggleMounting += ToggleMounting; TODO: decide if will override BSG mounting
        }

        private void UnSubscribeToEvents()
        {
            PlayerEvents.OnWeaponSwap -= OnWeaponSwap;
            PlayerEvents.OnSwappedFromItemToGun -= OnSwappedBackToGun;
            PlayerEvents.OnSwappedFromGunToItem -= OnSwappedToItem;
            PlayerEvents.AimStateChanged -= OnADSToggled;
            PlayerEvents.OnShotFired -= OnShotFired;
            StanceInputEvents.TogglePatrolStance -= TogglePatrolStance;
            StanceInputEvents.ToggleHighReady -= ToggleHighReady;
            StanceInputEvents.ToggleLowReady -= ToggleLowReady;
            StanceInputEvents.ToggleShortStock -= ToggleShortStock;
            StanceInputEvents.ToggleActiveAim -= ToggleActiveAim;
            StanceInputEvents.OnActiveAimKeyDown -= OnActiveAimKeyDown;
            StanceInputEvents.OnActiveAimKeyUp -= OnActiveAimKeyUp;
            StanceInputEvents.ToggleMelee -= ToggleMelee;
            InputEvents.ToggleLeftStanceInput -= ToggleLeftShoulder;
            StanceInputEvents.OnAttemptedToFireFromStance -= OnAttemptedToFireFromStance;
            StanceEvents.OnStanceReloadReset -= ResetReloadState;
            StanceEvents.OnStanceReload -= CheckIfReloadInterruptsStance;
        }

        private void StanceInputUpdate()
        {
            UpdatePistolDefaultStance();
        }

        private void UpdatePistolDefaultStance()
        {
            bool conditionsMet =
                WeaponStateInstance.TreatAsPistol
                && PlayerStateInstance.WeaponIsReady
                && !PlayerStateInstance.IsUsingStationaryWeapon
                && !AimStateInstance.IsAiming
                && !PlayerStateInstance.IsSprinting
                && !PlayerStateInstance.IsInventoryOpen;

            bool alreadyActive = _stanceState.ActiveStanceType == EStanceType.PistolCompress;

            if (conditionsMet && _stanceState.IsIdle && !alreadyActive)
            {
                RequestStance(StanceControllerInstance.PistolCompress);
            }
        }

        private void RequestStance(IStance stance)
        {
            if (_interruptType == EStanceInterruptType.None)
                _stanceState.RequestStance(stance);
        }


        private void CheckIfReloadInterruptsStance()
        {
            if (_stanceState.ActiveStance == null)
                return;

            if (_stanceState.ActiveStance.ReloadTypesThatPauseStance.Contains(StanceControllerInstance.CurrentReloadType))
            {
                _interruptType = EStanceInterruptType.Reload;
                _stanceBeforeInterrupt = _stanceState.ActiveStance;
                InterruptStances();
            }
        }

        private void ResetReloadState()
        {
            if (_stanceBeforeInterrupt != null && _interruptType == EStanceInterruptType.Reload)
            {
                _interruptType = EStanceInterruptType.None;
                ToggleStance(_stanceBeforeInterrupt);
                _stanceBeforeInterrupt = null;
            }
        }

        private void OnWeaponSwap()
        {
            CancelStances();
        }

        private void OnSwappedToItem()
        {
            if (PluginConfig.RememberStanceItem.Value)
                _stanceBeforeInterrupt = _stanceThatWasToggledOriginally;

            InterruptStances();
        }

        private void OnSwappedBackToGun()
        {
            if (PluginConfig.RememberStanceItem.Value && _stanceBeforeInterrupt != null)
            {
                ToggleStance(_stanceBeforeInterrupt);
            }

            _stanceBeforeInterrupt = null;
        }

        private void CancelStances()
        {
            _stanceState.CancelAll();
            _aimedFromActiveAim = false;
            _stanceThatWasToggledOriginally = null;
            _interruptType = EStanceInterruptType.None;
            _stanceBeforeInterrupt = null;
        }

        private void InterruptStances()
        {
            _stanceState.CancelAll();
            _aimedFromActiveAim = false;
            _stanceThatWasToggledOriginally = null;
        }

        private void AssessStanceOnShotAttempt()
        {
            bool rememberStance = PluginConfig.RememberStanceFiring.Value && AimStateInstance.IsAiming;
            bool isActiveAim = _stanceState.ActiveStanceType == EStanceType.ActiveAiming && !AimStateInstance.IsAiming;
            bool keepStance =
                rememberStance
                || isActiveAim
                || _stanceState.ActiveStanceType == EStanceType.LeftShoulder
                || _stanceState.ActiveStanceType == EStanceType.ShortStock
                || _stanceState.ActiveStanceType == EStanceType.PistolCompress;

            if (!keepStance)
            {
                CancelStances();
            }
        }

        private void OnShotFired()
        {
            AssessStanceOnShotAttempt();
        }

        private void OnAttemptedToFireFromStance()
        {
            AssessStanceOnShotAttempt();
        }

        private void OnActiveAimKeyDown()
        {
            if (_stanceState.ActiveStanceType == EStanceType.ActiveAiming)
                return;

            _stanceBeforeInterrupt = _stanceThatWasToggledOriginally;
            RequestStance(StanceControllerInstance.ActiveAim);
        }

        private void OnActiveAimKeyUp()
        {
            var toRestore = _stanceBeforeInterrupt;
            _stanceBeforeInterrupt = null;

            if (_stanceState.ActiveStanceType != EStanceType.ActiveAiming)
                return;

            if (toRestore != null)
                ToggleStance(toRestore);
            else
                _stanceState.CancelAll();
        }

        //TODO: this may need a rework
        //maybe stances hould sub to ADS toggle and pause themselves, or handle cancelling themselves
        private void OnADSToggled()
        {
            if (WeaponStateInstance.TreatAsPistol)
            {
                _stanceState.CancelAll();
                return;
            }

            if (AimStateInstance.IsAiming && _stanceState.ActiveStance?.StanceType != EStanceType.LeftShoulder)
            {
                if (_stanceState.ActiveStance != null)
                    _interruptType = EStanceInterruptType.ADS;

                if (_stanceState.ActiveStance?.StanceType == EStanceType.ActiveAiming)
                    _aimedFromActiveAim = true;

                _stanceState.CancelAll();
            }
            else
            {
                // ADS released
                if (_aimedFromActiveAim)
                    TryRestoreActiveAimAfterADS();
                else
                    TryRestoreStoredStanceAfterADS();
            }
        }

        private void TryRestoreStoredStanceAfterADS()
        {
            if (_interruptType != EStanceInterruptType.ADS)
                return;

            _interruptType = EStanceInterruptType.None;

            if (_stanceThatWasToggledOriginally == null)
                return;

            ToggleStance(_stanceThatWasToggledOriginally);
        }

        private bool TryRestoreActiveAimAfterADS()
        {
            _aimedFromActiveAim = false;

            if (_stanceState.ActiveStance?.StanceType == EStanceType.ActiveAiming)
                return false;

            RequestStance(StanceControllerInstance.ActiveAim);
            return true;
        }

        private bool IsTogglingActiveStance(EStanceType stance)
        {
            return _stanceState.ActiveStance?.StanceType == stance;
        }

        //TODO: call this from an aim event
        //TODO: this may need a rework
        private void ToggleStance(
            IStance? targetStance,
            bool forgetPrevious = false)
        {
            if (targetStance == null || _interruptType != EStanceInterruptType.None) return;

            if (targetStance.RememberStance)
            {
                _stanceThatWasToggledOriginally =
                    !IsTogglingActiveStance(targetStance.StanceType) ?
                    targetStance : null;
            }

            if (forgetPrevious)
                _stanceThatWasToggledOriginally = null;

            RequestStance(targetStance);
        }

        //TODO: call this from an aim event
        private void TogglePatrolStance()
        {
            ToggleStance(StanceControllerInstance.PatrolStance, forgetPrevious: true);
        }

        private void ToggleLeftShoulder()
        {
            ToggleStance(StanceControllerInstance.LeftShoulder, forgetPrevious: true);
        }

        private void ToggleHighReady()
        {
            ToggleStance(StanceControllerInstance.HighReady);
        }

        private void ToggleLowReady()
        {
            ToggleStance(StanceControllerInstance.LowReady);
        }

        private void ToggleShortStock()
        {
            ToggleStance(StanceControllerInstance.ShortStock);
        }

        private void TogglePistolCompress()
        {
            ToggleStance(StanceControllerInstance.PistolCompress);
        }

        private void ToggleActiveAim()
        {
            bool activeAimActive =
               _stanceState.ActiveStanceType == EStanceType.ActiveAiming;

            if (activeAimActive)
            {
                if (_stanceThatWasToggledOriginally != null)
                    ToggleStance(_stanceThatWasToggledOriginally);
                else
                    ToggleStance(StanceControllerInstance.ActiveAim, forgetPrevious: true);
            }
            else
            {
                ToggleStance(StanceControllerInstance.ActiveAim);
            }

        }

        private void ToggleMelee()
        {
            /*   if (_stanceState.CurrentStance?.StanceType == EStance.Melee)
                   return;*/

            //ToggleStance(StanceControllerInstance.Melee);

            /*StanceControllerInstance.MeleeHitSomething = false;*/
        }

        /*       public void ToggleMounting() 
               {
                   ToggleStance(StanceControllerInstance.Mounting);
               }
        
         
                 private void OnToggleStepOut()
        {
            IsMounting = false;
        }

        private void OnChangeStance()
        {
            IsMounting = false;
        }

        private void OnToggleBipod()
        {
            IsMounting = false;
        }
         
         */
    }
}

