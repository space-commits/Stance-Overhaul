using StanceOverhaul.Enums;
using StanceOverhaul.Events;
using System.Linq;
using UnityEngine;
using static RealismCommonLib.Plugin;

namespace StanceOverhaul.Controllers.StateControllers
{
    public class StanceInputState : IStateController
    {
        private StanceController _stanceController;

        private const float _clickDelay = 0.2f;
        private float _doubleClickTime = 0f;
        private bool _clickTriggered = true;
        private int _stanceIndex = 0;

        private bool _activeAimWasTriggered;

        private bool StanceInputBlocked
        {
            get
            {
                return AimStateInstance.IsAiming
                    || _stanceController.ShouldBlockAllStances
                    || PlayerStateInstance.IsSprinting
                    || PlayerStateInstance.IsInventoryOpen;
            }
        }

        public StanceInputState(StanceController stanceController)
        {
            _stanceController = stanceController;
        }

        public void RunOnAwake()
        {
        }

        public void RunOnDestroy()
        {
        }

        public void RunOnUpdate()
        {
            StanceStateUpdate();
        }

        public void SubscribeToEvents()
        {
            StanceInputEvents.TogglePatrolStance += TogglePatrolStance;
            StanceInputEvents.ToggleHighReady += ToggleHighReady;
            StanceInputEvents.ToggleLowReady += ToggleLowReady;
            StanceInputEvents.ToggleShortStock += ToggleShortStock;
            StanceInputEvents.ToggleActiveAim += ToggleActiveAim;
            StanceInputEvents.OnActiveAimKeyDown += OnActiveAimKeyDown;
            StanceInputEvents.OnActiveAimKeyUp += OnActiveAimKeyUp;
            StanceInputEvents.ToggleMelee += ToggleMelee;
            RealismCommonLib.Events.PlayerEvents.AimStateChanged += OnADSToggled;
            RealismCommonLib.Events.PlayerEvents.OnShotFired += OnShotFired;
        }

        public void StanceStateUpdate()
        {
            if (PlayerStateInstance.WeaponIsReady && PlayerStateInstance.IsUsingStationaryWeapon)
            {
                _stanceController.StanceDampingTimer();

                _stanceController.MeleeCooldownTimer();

                CheckForPatrolInput();

                if (!WeaponStateInstance.TreatAsPistol)
                {

                    CheckStanceCycle();
                    CheckScrollInput();

                    CheckForActimeAimInput();
                    CheckForHighReadyInput();
                    CheckForLowReadyInput();
                    CheckForShortStockInput();
                    CheckForMeleeInput();
                }

 
            }

            CheckIfShouldForceLowReady();

            CheckIfShouldCancelOnWeaponSwap();
        }

        //TODO: this should not be done in update
        public void CheckIfShouldCancelOnWeaponSwap() 
        {
            if (_stanceController.DidWeaponSwap || !PluginConfig.RememberStanceItem.Value && !GameStateInstance.WeaponIsReady || !GameStateInstance.PlayerIsReady)
            {
                _stanceController.CancelAllStances();
                _stanceController.IsMounting = false;
                _stanceIndex = 0;
                _stanceController.DidWeaponSwap = false;
                _stanceController.AimingInterrupted = false;
                _stanceController.ResetStanceStamina();
            }
        }

        public void CheckIfShouldForceLowReady() 
        {
            if (_stanceController.ShouldForceLowReady)
            {
                _stanceController.StanceBlender.Target = 1f;
                _stanceController.CurrentStance = EStance.LowReady;
                _stanceController.StoredStance = EStance.LowReady;
            }
        }

        public void OnShotFired() 
        {
            bool rememberStance = PluginConfig.RememberStanceFiring.Value && AimStateInstance.IsAiming;
            bool isActiveAim = _stanceController.CurrentStance == EStance.ActiveAiming && !AimStateInstance.IsAiming;
            bool keepStance =
                rememberStance
                || isActiveAim
                || _stanceController.CurrentStance == EStance.LeftShoulder
                || _stanceController.CurrentStance == EStance.ShortStock
                || _stanceController.CurrentStance == EStance.PistolCompressed;

            if (!keepStance)
            {
                _stanceController.CancelAllStances();
            }
        }

        public void CheckStanceCycle()
        {
            if (StanceInputBlocked) return;

            if (Input.GetKeyUp(PluginConfig.CycleStancesKeybind.Value.MainKey))
            {
                if (Time.time <= _doubleClickTime)
                {
                    _clickTriggered = true;
                    _stanceController.StanceBlender.Target = 0f;
                    _stanceIndex = 0;
                    _stanceController.CancelAllStances();
                }
                else
                {
                    _clickTriggered = false;
                    _doubleClickTime = Time.time + _clickDelay;
                }
            }
            else if (!_clickTriggered)
            {
                if (Time.time > _doubleClickTime)
                {
                    _stanceController.StanceBlender.Target = 1f;
                    _clickTriggered = true;
                    _stanceIndex++;
                    _stanceIndex = _stanceIndex > 3 ? 1 : _stanceIndex;
                    _stanceController.CurrentStance = (EStance)_stanceIndex;
                    _stanceController.StoredStance = _stanceController.CurrentStance;
                }
            }
        }

        public void CheckScrollInput()
        {
            if (!PluginConfig.UseMouseWheelStance.Value || StanceInputBlocked) return;

            //TODO: get actual player keybind
            bool isHoldingMagSelect = Input.GetKey(KeyCode.R);
            bool isHoldingHeightStance = Input.GetKey(KeyCode.C);
            bool isHoldingKeyModifier = Input.GetKey(KeyCode.C);

            if (Input.GetKey(PluginConfig.StanceWheelComboKeyBind.Value.MainKey) && PluginConfig.UseMouseWheelPlusKey.Value || !PluginConfig.UseMouseWheelPlusKey.Value && !Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftAlt))
            {
                float scrollDelta = Input.mouseScrollDelta.y;
                if (scrollDelta != 0f)
                {
                    HandleScrollInput(scrollDelta);
                }
            }
        }

        private void HandleScrollInput(float scrollIncrement)
        {
            if (scrollIncrement == -1)
            {
                if (_stanceController.CurrentStance == EStance.HighReady)
                {
                    ToggleHighReady();
                }
                else if (_stanceController.CurrentStance != EStance.LowReady && _stanceController.HasResetHighReady)
                {
                    ToggleLowReady();
                }
            }
            if (scrollIncrement == 1 && _stanceController.CurrentStance != EStance.HighReady)
            {
                if (_stanceController.CurrentStance == EStance.LowReady && !_stanceController.HealthConditionForcesLowReady)
                {
                    ToggleLowReady();
                }
                else if (_stanceController.CurrentStance != EStance.HighReady && _stanceController.HasResetLowReady)
                {
                    ToggleHighReady();
                }
            }
        }

        public void CheckForPatrolInput()
        {
            if (StanceInputBlocked) return;

            if (Input.GetKeyDown(PluginConfig.PatrolKeybind.Value.MainKey) && PluginConfig.PatrolKeybind.Value.Modifiers.All(Input.GetKey))
            {
                StanceInputEvents.RaiseTogglePatrolStance();
            }
        }

        public void CheckForLowReadyInput()
        {
            bool isForcedLowReady = _stanceController.CurrentStance == EStance.LowReady && _stanceController.ShouldForceLowReady;
            if (StanceInputBlocked || isForcedLowReady) return;

            if (Input.GetKeyDown(PluginConfig.LowReadyKeybind.Value.MainKey) && PluginConfig.LowReadyKeybind.Value.Modifiers.All(Input.GetKey))
            {
                StanceInputEvents.RaiseToggleLowReady();
            }
        }

        //TODO: in future raise event to trigger high ready attempt animation if low ready is forced due to injury
        public void CheckForHighReadyInput()
        {
            bool isForcedLowReady = _stanceController.CurrentStance == EStance.HighReady && _stanceController.ShouldForceLowReady;
            if (StanceInputBlocked || isForcedLowReady) return;

            if (Input.GetKeyDown(PluginConfig.HighReadyKeybind.Value.MainKey) && PluginConfig.HighReadyKeybind.Value.Modifiers.All(Input.GetKey))
            {
                StanceInputEvents.RaiseToggleHighReady();
            }
        }

        public void CheckForShortStockInput()
        {
            if (StanceInputBlocked) return;

            if (Input.GetKeyDown(PluginConfig.ShortStockKeybind.Value.MainKey) && PluginConfig.ShortStockKeybind.Value.Modifiers.All(Input.GetKey))
            {
                StanceInputEvents.RaiseToggleShortStock();
            }
        }

        public void CheckForMeleeInput()
        {
            if (StanceInputBlocked) return;

            if (_stanceController.MeleeIsToggleable && Input.GetKeyDown(PluginConfig.MeleeKeybind.Value.MainKey) && PluginConfig.MeleeKeybind.Value.Modifiers.All(Input.GetKey))
            {
                StanceInputEvents.RaiseToggleMelee();
            }
        }

        public void CheckForActimeAimInput()
        {
            if (StanceInputBlocked) return;

            //TODO: get actual player keybind
            bool activeAimOverridesAds = (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKey(KeyCode.Mouse1)) && _stanceController.AdsIsBlocked;
            bool keyIsHeld = Input.GetKey(PluginConfig.ActiveAimKeybind.Value.MainKey) && PluginConfig.ActiveAimKeybind.Value.Modifiers.All(Input.GetKey);
            bool activeAimTriggered = activeAimOverridesAds || keyIsHeld;

            if (!PluginConfig.ToggleActiveAim.Value)
            {
                if (!_activeAimWasTriggered && activeAimTriggered)
                    StanceInputEvents.RaiseHoldActiveAimKeyDown();

                if (_activeAimWasTriggered && !activeAimTriggered)
                    StanceInputEvents.RaiseHoldActiveAimKeyUp();
            }
            else
            {
                if (!_activeAimWasTriggered && activeAimTriggered)
                    StanceInputEvents.RaiseToggleActiveAim();
            }

            _activeAimWasTriggered = activeAimTriggered;
        }

        //TODO: call this from an aim event
        private void ToggleStance(EStance targetStance, bool setBlenderTarget = false, bool setStoredStanceAsCurrent = false)
        {        
            if (_stanceController.CurrentStance == targetStance) _stanceController.CurrentStance = EStance.None;
            else _stanceController.CurrentStance = targetStance;

            if (setStoredStanceAsCurrent)
                _stanceController.StoredStance = _stanceController.CurrentStance;

            if (setBlenderTarget)
                _stanceController.StanceBlender.Target = _stanceController.StanceBlender.Target == 0f ? 1f : 0f;
        }

        //TODO: call this from an aim event
        public void TogglePatrolStance()
        {
            AudioControllerInstance.PlayADSSound(5f * PluginConfig.StanceSfxModifier.Value, false);
            ToggleStance(EStance.PatrolStance, false, false);
            _stanceController.StoredStance = EStance.None;
            _stanceController.StanceBlender.Target = 0f;
        }

        private void ToggleLeftShoulder()
        {
            AudioControllerInstance.PlayADSSound(5f * PluginConfig.StanceSfxModifier.Value, false);
            ToggleStance(EStance.LeftShoulder, false, false);
            _stanceController.StoredStance = EStance.None;
            _stanceController.StanceBlender.Target = 0f;
            PlayerStateInstance.ResetProceduralState();
        }

        private void ToggleHighReady()
        {
            ToggleStance(EStance.HighReady, setStoredStanceAsCurrent: true);
        }

        private void ToggleShortStock()
        {
            ToggleStance(EStance.ShortStock, setStoredStanceAsCurrent: true);
        }

        private void ToggleLowReady()
        {
            ToggleStance(EStance.LowReady, setStoredStanceAsCurrent: true);
        }

        public void ToggleActiveAim()
        {
            ToggleStance(EStance.ActiveAiming);
            if (_stanceController.CurrentStance != EStance.ActiveAiming)
            {
                _stanceController.CurrentStance = _stanceController.StoredStance;
            }
        }

        public void OnActiveAimKeyDown()
        {
            _stanceController.StanceBlender.Target = 1f;
            _stanceController.CurrentStance = EStance.ActiveAiming;
        }

        public void OnActiveAimKeyUp()
        {
            _stanceController.StanceBlender.Target = 0f;
            _stanceController.CurrentStance = _stanceController.StoredStance;
        }

        public void ToggleMelee() 
        {
            _stanceController.IsMounting = false;
            _stanceController.CurrentStance = EStance.Melee;
            _stanceController.StoredStance = EStance.None;
            _stanceController.StanceBlender.Target = 1f;
            _stanceController.MeleeIsToggleable = false;
            _stanceController.MeleeHitSomething = false;
        }

        public void OnADSToggled() 
        {
            if (AimStateInstance.IsAiming)
            {
                _stanceController.StoredStance = _stanceController.CurrentStance;
                _stanceController.CurrentStance = EStance.None;
            }
            else
            {
                _stanceController.CurrentStance = _stanceController.StoredStance;
            }
        }
    }
}

