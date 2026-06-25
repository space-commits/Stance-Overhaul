using RealismCommonLib.Events;
using StanceOverhaul.Enums;
using StanceOverhaul.Stances;
using StanceOverhaul.State;
using static RealismCommonLib.Plugin;
using static StanceOverhaul.Plugin;

namespace StanceOverhaul.Handlers.StanceInput
{
    internal class StanceInputHandler : IControllerHelper
    {
        private IStance? _storedStance;
        private bool _wasInterruptedByADS;
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

        public void SubscribeToEvents()
        {
            RealismCommonLib.Events.PlayerEvents.OnWeaponDraw += OnWeaponSwap;
            RealismCommonLib.Events.PlayerEvents.AimStateChanged += OnADSToggled;
            RealismCommonLib.Events.PlayerEvents.OnShotFired += OnShotFired;
            StanceInputEvents.TogglePatrolStance += TogglePatrolStance;
            StanceInputEvents.ToggleHighReady += ToggleHighReady;
            StanceInputEvents.ToggleLowReady += ToggleLowReady;
            StanceInputEvents.ToggleShortStock += ToggleShortStock;
            StanceInputEvents.ToggleActiveAim += ToggleActiveAim;
/*            StanceInputEvents.OnActiveAimKeyDown += OnActiveAimKeyDown;
            StanceInputEvents.OnActiveAimKeyUp += OnActiveAimKeyUp;*/
            StanceInputEvents.ToggleMelee += ToggleMelee;
            InputEvents.ToggleLeftStanceInput += ToggleLeftShoulder;
            StanceInputEvents.OnAttemptedToFireFromStance += OnAttemptedToFireFromStance;
            //StanceInputEvents.ToggleMounting += ToggleMounting; TODO: decide if will override BSG mounting
        }

        public void UnSubscribeToEvents()
        {
            RealismCommonLib.Events.PlayerEvents.OnWeaponDraw -= OnWeaponSwap;
            RealismCommonLib.Events.PlayerEvents.AimStateChanged -= OnADSToggled;
            RealismCommonLib.Events.PlayerEvents.OnShotFired -= OnShotFired;
            StanceInputEvents.TogglePatrolStance -= TogglePatrolStance;
            StanceInputEvents.ToggleHighReady -= ToggleHighReady;
            StanceInputEvents.ToggleLowReady -= ToggleLowReady;
            StanceInputEvents.ToggleShortStock -= ToggleShortStock;
            StanceInputEvents.ToggleActiveAim -= ToggleActiveAim;
/*            StanceInputEvents.OnActiveAimKeyDown -= OnActiveAimKeyDown;
            StanceInputEvents.OnActiveAimKeyUp -= OnActiveAimKeyUp;*/
            StanceInputEvents.ToggleMelee -= ToggleMelee;
            InputEvents.ToggleLeftStanceInput -= ToggleLeftShoulder;
            StanceInputEvents.OnAttemptedToFireFromStance -= OnAttemptedToFireFromStance;
        }

        public void StanceInputUpdate()
        {
        }

        public void OnWeaponSwap()
        {
            if (!PluginConfig.RememberStanceItem.Value && !PlayerStateInstance.WeaponIsReady)
                _stanceState.CancelAll();
        }

        public void OnAttemptedToFireFromStance() 
        {
            _stanceState.CancelAll();
        }

        public void OnShotFired()
        {
            bool rememberStance = PluginConfig.RememberStanceFiring.Value && AimStateInstance.IsAiming;
            bool isActiveAim = _stanceState.ActiveStanceType == EStanceType.ActiveAiming && !AimStateInstance.IsAiming;
            bool keepStance =
                rememberStance
                || isActiveAim
                || _stanceState.ActiveStanceType == EStanceType.LeftShoulder
                || _stanceState.ActiveStanceType == EStanceType.ShortStock
                || _stanceState.ActiveStanceType == EStanceType.PistolCompressed;



            ModLogger.LogWarning("stance " + _stanceState.ActiveStanceType);

            ModLogger.LogWarning("keepStance " + keepStance);

            ModLogger.LogWarning("rememberStance " + rememberStance);

            if (!keepStance)
            {
                _stanceState.CancelAll();
                _storedStance = null;
            }
        }

        public void OnActiveAimKeyDown()
        {
            //ToggleStance(StanceControllerInstance.ActiveAiming);
        }

        public void OnActiveAimKeyUp()
        {
            //ToggleStance(_storedStance);
        }

        //TODO: this may need a rework
        //maybe stances hould sub to ADS toggle and pause themselves, or handle cancelling themselves
        public void OnADSToggled()
        {
            if (AimStateInstance.IsAiming && _stanceState.ActiveStance?.StanceType != EStanceType.LeftShoulder)
            {
                if (_stanceState.ActiveStance != null)
                    _wasInterruptedByADS = true;

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

        private bool TryRestoreStoredStanceAfterADS()
        {
            if (!_wasInterruptedByADS)
                return false;

            _wasInterruptedByADS = false;

            if (_storedStance == null)
                return false;

            ModLogger.LogWarning($"Restoring stance: {_storedStance.StanceType}");
            _stanceState.RequestStance(_storedStance);
            return true;
        }

        private bool TryRestoreActiveAimAfterADS() 
        {
            _storedStance = null;
            _aimedFromActiveAim = false;

            if (_stanceState.ActiveStance?.StanceType == EStanceType.ActiveAiming)
                return false;

            ModLogger.LogWarning($"Restoring Active Aim stance");
            _stanceState.RequestStance(StanceControllerInstance.ActiveAim);
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
            bool remember = false,
            bool forgetPrevious = false)
        {
            if (targetStance == null) return;

            ModLogger.LogWarning("requesting");

            if (remember)
            {
                _storedStance = 
                    !IsTogglingActiveStance(targetStance.StanceType) ?
                    targetStance : null;
            }

            if (forgetPrevious)
                _storedStance = null;

            ModLogger.LogWarning("_storedStance after toggle is " + (_storedStance?.StanceType));

            _stanceState.RequestStance(targetStance);
        }

        //TODO: call this from an aim event
        public void TogglePatrolStance()
        {
            ModLogger.LogWarning("toggle patrol");
            ToggleStance(StanceControllerInstance.PatrolStance, forgetPrevious: true);
        }

        private void ToggleLeftShoulder()
        {
            ModLogger.LogWarning("toggle left shoulder");
            ToggleStance(StanceControllerInstance.LeftShoulder, forgetPrevious: true);
        }

        private void ToggleHighReady()
        {
            ModLogger.LogWarning("toggle high");
            ToggleStance(StanceControllerInstance.HighReady, true);
        }

        private void ToggleLowReady()
        {
            ModLogger.LogWarning("toggle low");
            ToggleStance(StanceControllerInstance.LowReady, true);
        }

        private void ToggleShortStock()
        {
            ToggleStance(StanceControllerInstance.ShortStock, false);
        }

        public void ToggleActiveAim()
        {
            ModLogger.LogWarning("toggle active");

            bool activeAimActive =
               _stanceState.ActiveStanceType == EStanceType.ActiveAiming;

            if (activeAimActive)
            {
                ModLogger.LogWarning("active aim is active");
                ModLogger.LogWarning("_storedStance " + (_storedStance?.StanceType));
                if (_storedStance != null)
                    ToggleStance(_storedStance);
                else
                    ToggleStance(StanceControllerInstance.ActiveAim, forgetPrevious: true);
            }
            else 
            {
                ToggleStance(StanceControllerInstance.ActiveAim);
            }

        }

        public void ToggleMelee()
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

