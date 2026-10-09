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
    // Flags so overlapping interrupts (e.g. ADS while reloading) can start and end independently
    [Flags]
    internal enum EStanceInterruptType
    {
        None = 0,
        ADS = 1,
        Reload = 2
    }

    internal class StanceInputHandler : ISubSystem
    {
        // After ADS ends, wait this long before bringing the stance back. If the aim state flickers while the
        // ADS transition settles, the stance is never re-requested, so it can't flash on screen.
        private const float RestoreAfterADSDelay = 0.1f;

        private IStance? _stanceThatWasToggledOriginally;
        private IStance? _stanceBeforeInterrupt;
        private IStance? _stanceBeforeActiveAim;

        private EStanceInterruptType _interruptType = EStanceInterruptType.None;
        private float _restoreCountdown;

        private bool IsInterrupted => _interruptType != EStanceInterruptType.None;

        //in future can expand on this if want to force low ready, combine with ShouldForceADefaultStance or similar?
        private IStance? EffectiveRememberedStance =>
            DefaultStance ?? _stanceThatWasToggledOriginally;

        //the stance the player is in, including one that ADS has cancelled.
        // ADS cancels the active stance, so ActiveStance on its own is null while aiming.
        private IStance? StanceInEffect =>
            _stanceState.ActiveStance ?? EffectiveRememberedStance;

        //should always return null if there's no reaon to force a stance
        private IStance? DefaultStance
        {
            get
            {
                return WeaponStateInstance.TreatAsPistol ? StanceControllerInstance.PistolCompress : null;
            }
        }

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
            if (_restoreCountdown <= 0f)
                return;

            _restoreCountdown -= deltaTime;

            if (_restoreCountdown <= 0f)
                RestoreStance(EffectiveRememberedStance);
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
            StanceInputEvents.OnActiveAimKeyDown += EnterActiveAim;
            StanceInputEvents.OnActiveAimKeyUp += ExitActiveAim;
            StanceInputEvents.ToggleMelee += ToggleMelee;
            InputEvents.ToggleLeftStanceInput += ToggleLeftShoulder;
            StanceInputEvents.OnAttemptedToFireFromStance += OnAttemptedToFireFromStance;
            StanceEvents.OnStanceReloadReset += ResetReloadState;
            StanceEvents.OnStanceReload += CheckIfReloadInterruptsStance;
            StanceEvents.OnTransformsInit += OnWeaponInit;
            StanceEvents.OnStationaryWeaponOperated += CancelStancesAndResetState;
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
            StanceInputEvents.OnActiveAimKeyDown -= EnterActiveAim;
            StanceInputEvents.OnActiveAimKeyUp -= ExitActiveAim;
            StanceInputEvents.ToggleMelee -= ToggleMelee;
            InputEvents.ToggleLeftStanceInput -= ToggleLeftShoulder;
            StanceInputEvents.OnAttemptedToFireFromStance -= OnAttemptedToFireFromStance;
            StanceEvents.OnStanceReloadReset -= ResetReloadState;
            StanceEvents.OnStanceReload -= CheckIfReloadInterruptsStance;
            StanceEvents.OnTransformsInit -= OnWeaponInit;
            StanceEvents.OnStationaryWeaponOperated -= CancelStancesAndResetState;
        }

        private void RequestStance(IStance stance)
        {
            if (!IsInterrupted)
            {
                _stanceState.RequestStance(stance);
            }
        }

        private void CheckIfReloadInterruptsStance()
        {
            // StanceInEffect rather than ActiveStance: a reload can start while ADS has the stance put away
            var stance = StanceInEffect;
            if (stance == null)
                return;

            if (stance.ReloadTypesThatPauseStance.Contains(StanceControllerInstance.CurrentReloadType))
            {
                _interruptType |= EStanceInterruptType.Reload;
                _stanceBeforeInterrupt = stance;
                InterruptStances();
            }
        }

        private void ResetReloadState()
        {
            if (!_interruptType.HasFlag(EStanceInterruptType.Reload))
                return;

            _interruptType &= ~EStanceInterruptType.Reload;
            RestoreStance(_stanceBeforeInterrupt);
            _stanceBeforeInterrupt = null;
        }

        private void OnWeaponSwap()
        {
            if (WeaponStateInstance.TreatAsPistol)
            {
                TryInitializePisolStance();
            }
            else
            {
                CancelStancesAndResetState();
            }

            ModLogger.LogWarning($"OnWeaponSwap: ActiveStance={_stanceState?.ActiveStance?.StanceType}, weapon is pistol={WeaponStateInstance.TreatAsPistol}");

        }

        private void OnSwappedToItem()
        {
            if (PluginConfig.RememberStanceItem.Value || WeaponStateInstance.TreatAsPistol)
                _stanceBeforeInterrupt = EffectiveRememberedStance;

            InterruptStances();
        }

        private void OnSwappedBackToGun()
        {
            if (PluginConfig.RememberStanceItem.Value || WeaponStateInstance.TreatAsPistol)
                RestoreStance(_stanceBeforeInterrupt);

            _stanceBeforeInterrupt = null;
        }

        private void OnWeaponInit()
        {
            if (WeaponStateInstance.TreatAsPistol)
                TryInitializePisolStance();
            else if (_stanceState.ActiveStanceType == EStanceType.PistolCompress)
                CancelAndForgetStances();

            ModLogger.LogWarning($"OnWeaponInit: ActiveStance={_stanceState?.ActiveStance?.StanceType}, weapon is pistol={WeaponStateInstance.TreatAsPistol}");
        }

        private void TryInitializePisolStance()
        {
            if (_stanceState.ActiveStanceType != EStanceType.PistolCompress) //&& !IsInterrupted && _stanceBeforeInterrupt is null
            {
                ModLogger.LogWarning($"toggle pistol");

                ToggleStance(StanceControllerInstance.PistolCompress);
            }
        }

        // Cancels the active stance and drops everything that would bring one back.
        // Leaves interrupts alone: firing during ADS must not end the ADS interrupt.
        private void CancelAndForgetStances()
        {
            _stanceState.CancelAll();
            _stanceThatWasToggledOriginally = null;
            _stanceBeforeInterrupt = null;
            _stanceBeforeActiveAim = null;
        }

        // Full reset, for weapon swaps
        private void CancelStancesAndResetState()
        {
            CancelAndForgetStances();
            _interruptType = EStanceInterruptType.None;
            _restoreCountdown = 0f;
        }

        // Cancels the stance but remembers nothing new: the caller keeps whatever it wants restored
        private void InterruptStances()
        {
            _stanceState.CancelAll();
            _stanceThatWasToggledOriginally = null;
        }

        // The one path for putting a stance back after an interruption (ADS, reload, item, leaving active aim).
        // Unlike ToggleStance it never un-toggles. It records the stance before checking for interrupts, so if
        // something else is still interrupting, whichever interrupt ends last brings the stance back.
        private void RestoreStance(IStance? stance)
        {
            if (stance == null)
                return;

            if (DefaultStance != null && _stanceState.ActiveStance != DefaultStance)
            {
                RequestStance(DefaultStance);
                return;
            }

            // Active aim is always remembered across interrupts, regardless of its RememberStance flag
            _stanceThatWasToggledOriginally =
                (stance.RememberStance || stance.StanceType == EStanceType.ActiveAiming) ? stance : null;

            if (_stanceState.ActiveStance?.StanceType != stance.StanceType)
                RequestStance(stance);
        }

        private void AssessStanceOnShotAttempt()
        {
            if (PluginConfig.RememberStanceFiring.Value && AimStateInstance.IsAiming)
                return;

            // While ADS the stance is put away (ActiveStance is null), so also look at the one waiting to come back
            if (StanceInEffect?.BlocksFiring != true)
                return;

            CancelAndForgetStances();

            if (DefaultStance != null)
                RequestStance(DefaultStance);
        }

        private void OnShotFired()
        {
            AssessStanceOnShotAttempt();
        }

        private void OnAttemptedToFireFromStance()
        {
            AssessStanceOnShotAttempt();
        }

        private void OnADSToggled()
        {
            if (AimStateInstance.IsAiming)
                OnADSStarted();
            else
                OnADSEnded();
        }

        private void OnADSStarted()
        {
            // Aiming again before the stance came back: drop the pending restore
            _restoreCountdown = 0f;

            var active = _stanceState.ActiveStance;

            // Left shoulder is compatible with ADS, leave it alone
            if (active?.StanceType == EStanceType.LeftShoulder)
                return;

            // High Ready -> Active Aim -> ADS: forget High Ready, ADS ends back in Active Aim
            if (active?.StanceType == EStanceType.ActiveAiming)
                _stanceBeforeActiveAim = null;

            _interruptType |= EStanceInterruptType.ADS;
            _stanceState.CancelAll();
        }

        private void OnADSEnded()
        {
            _interruptType &= ~EStanceInterruptType.ADS;

            // Restore is deferred to RunOnUpdate so a flicker in the aim state can't re-request the stance
            _restoreCountdown = _stanceState.IsTransitioning ? 0f : RestoreAfterADSDelay;
        }

        private bool IsTogglingActiveStance(EStanceType stance)
        {
            return _stanceState.ActiveStance?.StanceType == stance;
        }

        // Only reached from user input. Restores go through RestoreStance.
        private void ToggleStance(
            IStance? targetStance,
            bool forgetPrevious = false)
        {
            if (targetStance == null || IsInterrupted) return;

            _stanceBeforeActiveAim = null;

            if (DefaultStance != null && _stanceState.ActiveStance != DefaultStance)
            {
                RequestStance(DefaultStance);
                return;
            }

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

        private void TogglePatrolStance()
        {
            ToggleStance(StanceControllerInstance.PatrolStance, forgetPrevious: true);
        }

        private void ToggleLeftShoulder()
        {
            if (WeaponStateInstance.TreatAsPistol)
                return;

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
            if (_stanceState.ActiveStanceType == EStanceType.ActiveAiming) ExitActiveAim();
            else EnterActiveAim();
        }

        private void EnterActiveAim()
        {
            if (IsInterrupted) return;
            if (_stanceState.ActiveStanceType == EStanceType.ActiveAiming) return;

            _stanceBeforeActiveAim = EffectiveRememberedStance;
            _stanceThatWasToggledOriginally = StanceControllerInstance.ActiveAim;
            RequestStance(StanceControllerInstance.ActiveAim);
        }

        private void ExitActiveAim()
        {
            var toRestore = _stanceBeforeActiveAim;
            _stanceBeforeActiveAim = null;
            _stanceThatWasToggledOriginally = null;

            // Active aim was put away by a reload/item: what comes back is the stance we had before it
            if (_stanceBeforeInterrupt?.StanceType == EStanceType.ActiveAiming)
                _stanceBeforeInterrupt = toRestore;

            // Already cancelled (e.g. by ADS), nothing to switch away from
            if (_stanceState.ActiveStanceType != EStanceType.ActiveAiming)
                return;

            if (toRestore != null)
                RestoreStance(toRestore);
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
