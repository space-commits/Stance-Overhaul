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
            StanceInputEvents.OnActiveAimKeyDown += OnActiveAimKeyDown;
            StanceInputEvents.OnActiveAimKeyUp += OnActiveAimKeyUp;
            StanceInputEvents.ToggleMelee += ToggleMelee;
            StanceInputEvents.ToggleLeftShoulder += ToggleLeftShoulder;
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
            StanceInputEvents.OnActiveAimKeyDown -= OnActiveAimKeyDown;
            StanceInputEvents.OnActiveAimKeyUp -= OnActiveAimKeyUp;
            StanceInputEvents.ToggleMelee -= ToggleMelee;
            StanceInputEvents.ToggleLeftShoulder -= ToggleLeftShoulder;
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
            bool isActiveAim = _stanceState.CurrentStanceType == EStance.ActiveAiming && !AimStateInstance.IsAiming;
            bool keepStance =
                rememberStance
                || isActiveAim
                || _stanceState.CurrentStanceType == EStance.LeftShoulder
                || _stanceState.CurrentStanceType == EStance.ShortStock
                || _stanceState.CurrentStanceType == EStance.PistolCompressed;



            ModLogger.LogWarning("stance " + _stanceState.CurrentStanceType);

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
            if (AimStateInstance.IsAiming)
            {
                if (_stanceState.ActiveStance != null)
                {
                    _wasInterruptedByADS = true;
                }

                _stanceState.CancelAll();
            }
            else
            {
                // ADS released
                TryRestoreStoredStance();
            }
        }

        private void TryRestoreStoredStance()
        {
            if (!_wasInterruptedByADS)
                return;

            _wasInterruptedByADS = false;

            if (_storedStance == null)
                return;

            ModLogger.LogWarning($"Restoring stance: {_storedStance.StanceType}");
            _stanceState.RequestStance(_storedStance);
        }

        private bool CanBeStored(IStance stance)
        {
            return stance.StanceType != EStance.ActiveAiming
                && stance.StanceType != EStance.Melee;
        }

        //TODO: call this from an aim event
        //TODO: this may need a rework
        private void ToggleStance(
            IStance? targetStance,
            bool remember = true)
        {
            if (targetStance == null) return;

            ModLogger.LogWarning("requesting");
            _stanceState.RequestStance(targetStance);

            if (remember && CanBeStored(targetStance))
            {
                _storedStance = targetStance;
                ModLogger.LogWarning($"Stored stance: {_storedStance.StanceType}");
            }

            /*            if (setCurrentToStoredStance && _storedStanceInput != null)
                            _stanceState.RequestStance(_storedStanceInput);

                        if (setStoredStanceAsNone)
                            _storedStanceInput = null;*/
        }

        //TODO: call this from an aim event
        public void TogglePatrolStance()
        {
            ModLogger.LogWarning("toggle patrol");
            ToggleStance(StanceControllerInstance.PatrolStance, false);
        }

        private void ToggleLeftShoulder()
        {
            ModLogger.LogWarning("toggle left shoulder");
            ToggleStance(StanceControllerInstance.LeftShoulder, true);
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
            //ToggleStance(StanceControllerInstance.ShortStock, true);
        }

        public void ToggleActiveAim()
        {
            //ToggleStance(StanceControllerInstance.ActiveAiming);
            /*            if (_stanceState
                            .CurrentStance?.StanceType != EStance.ActiveAiming)
                            ToggleStance(_storedStance);*/
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

