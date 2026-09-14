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

        //in future can expand on this if want to force low ready, combine with ShouldForceADefaultStance or similar?
        private IStance? EffectiveRememberedStance =>
            DefaultStance ?? _stanceThatWasToggledOriginally;

        //should always return null if there's no reaon to force a stance
        private IStance? DefaultStance
        {
            get
            {
                return WeaponStateInstance.TreatAsPistol ? StanceControllerInstance.PistolCompress : null;
            }
        }

        private EStanceInterruptType _interruptType = EStanceInterruptType.None;

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
            StanceEvents.OnTransformsInit += OnWeaponInit;
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
            StanceEvents.OnTransformsInit -= OnWeaponInit;
        }

        private void RequestStance(IStance stance)
        {
            if (_interruptType == EStanceInterruptType.None)
            {
                _stanceState.RequestStance(stance);
            }
        }

        private void CheckIfReloadInterruptsStance()
        {
            if (_stanceState.ActiveStance == null)
                return;

            ModLogger.LogWarning("CheckIfReloadInterruptsStance");

            if (_stanceState.ActiveStance.ReloadTypesThatPauseStance.Contains(StanceControllerInstance.CurrentReloadType))
            {
                ModLogger.LogWarning("cancel reload");
                _interruptType = EStanceInterruptType.Reload;
                _stanceBeforeInterrupt = _stanceState.ActiveStance;
                InterruptStances();
            }
        }

        private void ResetReloadState()
        {
            if (_stanceBeforeInterrupt != null && _interruptType == EStanceInterruptType.Reload)
            {
                ModLogger.LogWarning("restore stance after reload");
                _interruptType = EStanceInterruptType.None;
                ToggleStance(_stanceBeforeInterrupt);
                _stanceBeforeInterrupt = null;
            }
        }

        private void OnWeaponSwap()
        {
            ModLogger.LogWarning($"OnWeaponSwap");
            if (WeaponStateInstance.TreatAsPistol)
                return;

            CancelStancesAndResetState();
        }

        private void OnSwappedToItem()
        {
            if (PluginConfig.RememberStanceItem.Value || WeaponStateInstance.TreatAsPistol)
                _stanceBeforeInterrupt = EffectiveRememberedStance;

            InterruptStances();
        }

        private void OnSwappedBackToGun()
        {
            if ((PluginConfig.RememberStanceItem.Value || WeaponStateInstance.TreatAsPistol) && _stanceBeforeInterrupt != null)
            {
                ToggleStance(_stanceBeforeInterrupt);
            }

            _stanceBeforeInterrupt = null;
        }

        private void OnWeaponInit()
        {
            ModLogger.LogWarning($"OnWeaponInit");

            if (!WeaponStateInstance.TreatAsPistol)
            {
                CancelStancesAndResetState();
            }
            else
                TryInitializePisolStance();
        }

        private void TryInitializePisolStance()
        {
            ModLogger.LogWarning($"active stance: {_stanceState.ActiveStanceType}, interrupt: {_interruptType}, stance before interrupt: {_stanceBeforeInterrupt?.StanceType}");

            if (_stanceState.ActiveStanceType != EStanceType.PistolCompress && _interruptType == EStanceInterruptType.None && _stanceBeforeInterrupt is null)
            {
                ToggleStance(StanceControllerInstance.PistolCompress);
            }
        }

        private void CancelStancesAndResetState()
        {
            _stanceState.CancelAll();
            _stanceThatWasToggledOriginally = null;
            _interruptType = EStanceInterruptType.None;
            _stanceBeforeInterrupt = null;
        }

        private void InterruptStances()
        {
            _stanceState.CancelAll();
            _stanceThatWasToggledOriginally = null;
        }

        private void AssessStanceOnShotAttempt()
        {
            bool rememberStanceWhenAiming = PluginConfig.RememberStanceFiring.Value && AimStateInstance.IsAiming;

            bool cancelStance = !rememberStanceWhenAiming && _stanceState?.ActiveStance?.BlocksFiring == true;

            if (cancelStance)
            {
                CancelStancesAndResetState();

                if (DefaultStance != null)
                    RequestStance(DefaultStance);
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

        //TODO: this may need a rework
        //maybe stances hould sub to ADS toggle and pause themselves, or handle cancelling themselves
        private void OnADSToggled()
        {
            if (AimStateInstance.IsAiming && _stanceState.ActiveStance?.StanceType != EStanceType.LeftShoulder)
            {
                if (_stanceState.ActiveStance != null)
                    _interruptType = EStanceInterruptType.ADS;

                _stanceState.CancelAll();
            }
            else
            {
                TryRestoreStoredStanceAfterADS();
            }
        }

        private void TryRestoreStoredStanceAfterADS()
        {
            _interruptType = EStanceInterruptType.None;

            //if a default stance is enforced, should always go back to it after ADS.
            //this becomes problematic if, for example, forced to low ready but player is allowed to use and ADS from other stances


            if (DefaultStance != null && EffectiveRememberedStance?.StanceType != _stanceState.ActiveStance?.StanceType)
            {
                ModLogger.LogWarning($"DefaultStance {DefaultStance?.StanceType}, restore Active stance {_stanceState.ActiveStance?.StanceType}, effective: {EffectiveRememberedStance?.StanceType}");

                ToggleStance(EffectiveRememberedStance);
                return;
            }

            if (_stanceState.ActiveStance?.StanceType == _stanceThatWasToggledOriginally?.StanceType)
                return;

            ModLogger.LogWarning($"_stanceThatWasToggledOriginally {DefaultStance?.StanceType}");
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

            ModLogger.LogWarning("toggle stance");

            if (DefaultStance != null && _stanceState.ActiveStance != DefaultStance)
            {
                ModLogger.LogWarning("toggle DefaultStance");
                RequestStance(DefaultStance);
                return;
            }

            if (targetStance.RememberStance)
            {
                ModLogger.LogWarning("RememberStance");
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

        private void ToggleActiveAim()
        {
            bool activeAimActive = _stanceState.ActiveStanceType == EStanceType.ActiveAiming;

            if (!activeAimActive)
            {
                _stanceBeforeInterrupt = EffectiveRememberedStance;
                RequestStance(StanceControllerInstance.ActiveAim);
            }
            else
            {
                var toRestore = _stanceBeforeInterrupt;
                _stanceBeforeInterrupt = null;

                if (toRestore != null)
                    ToggleStance(toRestore);
                else
                    _stanceState.CancelAll();
            }
        }

        private void OnActiveAimKeyDown()
        {
            if (_stanceState.ActiveStanceType == EStanceType.ActiveAiming)
                return;

            _stanceBeforeInterrupt = EffectiveRememberedStance;
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

