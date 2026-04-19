using StanceOverhaul.Controllers.StateControllers;
using StanceOverhaul.Enums;
using StanceOverhaul.Events;
using StanceOverhaul.Stances;
using System.Linq;
using UnityEngine;
using static RealismCommonLib.Plugin;
using static StanceOverhaul.Plugin;

namespace StanceOverhaul
{
    internal class StanceInputHandler : IControllerHelper
    {
        private StanceState _stanceState;
        private IStance? _storedStance;

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

        }

        public void StanceInputUpdate()
        {
        }

        public void OnWeaponSwap()
        {
            if (!PluginConfig.RememberStanceItem.Value && !PlayerStateInstance.WeaponIsReady)
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
                if (_stanceState.CurrentStance?.IsActive == true) 
                    _storedStance = _stanceState.CurrentStance;

                _stanceState.CancelAll();
            }
            else if (_storedStance != null)
            {
                ToggleStance(_storedStance);
                _storedStance = null;
            }
        }

        //TODO: call this from an aim event
        //TODO: this may need a rework
        private void ToggleStance(
            IStance? targetStance,
            bool setCurrentToStoredStance = false,
            bool setStoredStanceAsNone = false)
        {
            if (targetStance == null) return;

            ModLogger.LogWarning("requesting");
            _stanceState.RequestStance(targetStance);

            if (setCurrentToStoredStance && _storedStance != null)
                _stanceState.RequestStance(_storedStance);

            if (setStoredStanceAsNone)
                _storedStance = null;
        }

        //TODO: call this from an aim event
        public void TogglePatrolStance()
        {
            ModLogger.LogWarning("toggle patrol");
            ToggleStance(StanceControllerInstance.PatrolStance, false, true);
        }

        private void ToggleLeftShoulder()
        {
            ModLogger.LogWarning("toggle left shoulder");
            ToggleStance(StanceControllerInstance.LeftShoulder, false, true);
        }

        private void ToggleHighReady()
        {
            //ToggleStance(StanceControllerInstance.HighReady, true);
        }

        private void ToggleShortStock()
        {
            //ToggleStance(StanceControllerInstance.ShortStock, true);
        }

        private void ToggleLowReady()
        {
            //ToggleStance(StanceControllerInstance.LowReady, true);
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
            if (_stanceState.CurrentStance?.StanceType == EStance.Melee)
                return;

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

