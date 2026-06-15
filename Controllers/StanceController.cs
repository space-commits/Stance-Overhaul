using Comfort.Common;
using EFT;
using EFT.Animations;
using HarmonyLib;
using RealismCommonLib.Events;
using RealismCommonLib.ModifierHandlers;
using RealismCommonLib.Utils;
using StanceOverhaul.Controllers.PatchHooks;
using StanceOverhaul.Controllers.StateControllers;
using StanceOverhaul.Enums;
using StanceOverhaul.Handlers;
using StanceOverhaul.Handlers.Aiming;
using StanceOverhaul.Handlers.StanceInput;
using StanceOverhaul.Stances;
using StanceOverhaul.State;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static RealismCommonLib.Plugin;

namespace StanceOverhaul.Controllers
{
    public class StanceController : MonoBehaviour
    {
        public const float STANCE_WEIGHT_LIMIT_KG = 8f;
        public const float CHONKER_MODIFIER = 0.7f;
        public const float HIGH_READY_RADIATION_LIMIT = 80f;
        public const float HIGH_READY_TOXICITY_LIMIT = 80f;
        public const float BASE_AIM_SPEED_SKILL_FACTOR = 0.5f;
        public const float BASE_AIM_SPEED_PISTOL_FACTOR = 1.4f;
        public const float IDLE_ADS_MULTI = 1.5f;
        public const float ACTIVE_AIM_ADS_MULTI = 1.35f;
        public const float HIGH_ADS_MULTI = 1.25f;
        public const float LOW_ADS_MULTI = 1.25f;
        public const float SHORT_STOCK_ADS_MULTI = 0.9f;
        public const float LEFT_SHOULDER_ADS_MULTI = 0.85f;
        public const float PATROL_ADS_MULTI = 0.9f;
        public const float LEFT_SHOULDER_SWAY_MULTI = 1.3f;
        public const float HIGH_READY_RELOAD_SPEED_BUFF = 1.18f;
        public const float ACTIVE_AIM_RELOAD_SPEED_BUFF = 1.16f;
        public const float LOW_READY_RELOAD_SPEED_BUFF = 1.21f;
        public const float ACTIVE_AIM_RECHAMBER_SPEED_BUFF = 1.11f;
        public const float HIGH_READY_RECHAMBER_SPEED_BUFF = 1.15f;
        public const float HIGH_READY_CHECK_AMMO_SPEED_BUFF = 1.15f;

        private FloatMultiplierHandle _magReload;
        private FloatMultiplierHandle _quickMagReload;
        private FloatMultiplierHandle _checkAmmo;
        private FloatMultiplierHandle _checkChamber;
        private FloatMultiplierHandle _pumpBolt;
        private FloatMultiplierHandle _malfFix;
        private FloatMultiplierHandle _rechamber;
        private FloatMultiplierHandle _noMagReload;
        private FloatMultiplierHandle _currentMagRemove;
        private FloatMultiplierHandle _newMagReload;
        private FloatMultiplierHandle _internalMagReload;

        private EStanceType _targetStance = EStanceType.None;

        public Vector3 MountWeapPosition = Vector3.zero;
        public Vector3 CurrentVisualRecoil = Vector3.zero;
        public Vector3 TargetVisualRecoil = Vector3.zero;

        private float _pistolPosSpeed = 1f;

        private float _currentRifleXPos = 0f;
        private float _currentRifleYPos = 0f;
        private float _currentRifleZPos = 0f;
        private float _currentPistolXPos = 0f;
        private float _currentPistolYPos = 0f;
        private float _currentPistolZPos = 0f;

        private float _gunCameraAlignmentTargetX = 0f;
        private float _gunCameraAlignmentTargetY = 0f;
        private float _gunCameraAlignmentTargetZ = 0f;
        private float _gunXTarget = 0f;
        private float _gunYTarget = 0f;
        private float _gunZTarget = 0f;

        public Vector3 _riflePatrolPos = new Vector3(0.2f, 0.025f, 0.1f);
        public Vector3 _riflePatrolRot = new Vector3(0.05f, -0.05f, -0.5f);
        public Vector3 _pistolPatrolPos = new Vector3(0.05f, 0f, 0f);
        public Vector3 _pistolPatrolRot = new Vector3(0.1f, -0.1f, -0.1f);

        public Vector3 CoverWiggleDirection = Vector3.zero;
        public Vector3 BaseWeaponOffsetPosition = Vector3.zero;
        public Vector3 StanceCurrentPosition = Vector3.zero;
        private Vector3 _pistolLocalPosition = Vector3.zero;
        private Vector3 _rifleLocalPosition = Vector3.zero;

        //TODO move to collision controller
        public bool WasAimingBeforeCollision = false;
        public bool StopCameraMovement = false;
        public float CameraMovmentForCollisionSpeed = 0.01f;
        public bool IsColliding = false;
        public bool PistolIsColliding = false;

        private static FieldInfo _pwaAimField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_aimingSpeed");

        public float PwaAimSpeed
        {
            get
            {
                return (float)_pwaAimField.GetValue(PlayerStateInstance.PWA);
            }
            set
            {
                _pwaAimField.SetValue(PlayerStateInstance.PWA, value);
            }
        }

        public float PwaOriginalAimSpeed { get; set; }

        public bool IsBlindFiring
        {
            get
            {
                return PlayerStateInstance.PWA.BlindfireBlender.Value > 0f;
            }
        }

        public bool IsInThirdPerson
        {
            get
            {
                return PlayerStateInstance.Player.PointOfView == EPointOfView.ThirdPerson;
            }
        }

        /// <summary>
        /// Stores a stance to return to after doing active aim, melee or aiming
        /// </summary>
        public EStanceType StoredStances { get; set; }

        //TODO: move ResetProceduralState to sub to StanceState events
        public EStanceType TargetStances
        {
            get { return _targetStance; }
            set
            {
                if (value != _targetStance)
                {
                    _targetStance = value;
                    if (!AimStateInstance.IsAiming) PlayerStateInstance.ResetProceduralState();
                }
            }
        }

        //TODO: move to low ready stance class if force low ready will be a thing
        public bool CanDoHighReadyInjuredAnim
        {
            get
            {
                return CurrentStanceType == EStanceType.HighReady && HealthConditionForcesLowReady;
            }
        }

        public bool AdsIsBlocked
        {
            get
            {
                return !AimStateInstance.PlayerCanAim;
            }
        }

        //TODO: move to left shoulder class and implement it as setting PauseStance
        /*        public bool ShouldDoLeftShoulder
                {
                    get
                    {
                        return CurrentStance == EStance.LeftShoulder && !IsBlindFiring && !PauseLeftShoulder;
                    }
                }*/

        //TODO move to mounting/bracing controller
        public Vector3 MountPos { get; set; }
        public Vector3 MountDir { get; set; }

        //TODO move to collision controller
        public float BaseWeaponLength { get; set; }
        //TODO use this somewhere
        public float StanceModifiedWeaponLength { get; set; }

        //TODO: implement in a stance state controller
        public float StanceSwayFactor
        {
            get
            {
                return
                    CurrentStanceType == EStanceType.LeftShoulder ? LEFT_SHOULDER_SWAY_MULTI : 1f;
            }
        }

        //TODO redo these values
        public float ErgoStanceSpeed
        {
            get
            {
                return
                    WeaponStateInstance.BaseAimSpeed *
                    (1f + (SkillStateInstance.AimSkillADSBuff * BASE_AIM_SPEED_SKILL_FACTOR)) *
                    (WeaponStateInstance.TreatAsPistol ? BASE_AIM_SPEED_PISTOL_FACTOR : 1f);
            }
        }

        public bool TreatWeaponAsPistolStance
        {
            get
            {
                return WeaponStateInstance.TreatAsPistol;
            }
        }

        public bool HealthStateAffectsStance
        {
            get
            {
                return
                    HealthStateInstance.ArmsAreIncapacitated ||
                    HealthStateInstance.HasOverdosed ||
                    HealthStateInstance.IsPoisoned ||
                    HealthStateInstance.IsDehydrated ||
                    HealthStateInstance.IsExhausted;
            }
        }

        //TODO will no longer force low ready? will make default stance instead of ready?
        public bool HealthConditionForcesLowReady
        {
            get
            {
                return
                    HazardsStateInstance.TotalToxicity > HIGH_READY_TOXICITY_LIMIT ||
                    HazardsStateInstance.TotalRadiation > HIGH_READY_RADIATION_LIMIT ||
                    HealthStateAffectsStance;
            }
        }

        //TODO: use const for weight
        //replace with debuffs and animations
        /*       public bool ShouldForceLowReady
               {
                   get
                   {
                       return (HealthConditionForcesLowReady || (WeaponStateInstance.TotalWeaponWeight >= 10f && !IsMounting))
                           && !AimStateInstance.IsAiming
                           && !FiringStateInstance.IsFiringFromStance
                           && TargetStance != EStance.PistolCompressed
                           && TargetStance != EStance.PatrolStance
                           && TargetStance != EStance.ShortStock
                           && TargetStance != EStance.ActiveAiming
                           && MeleeIsToggleable
                           && !IsBracing;
                   }
               }*/

        public bool WeaponIsReady
        {
            get
            {
                return PlayerStateInstance.FirearmController != null;
            }
        }

        public bool ShouldAllowActiveOnReload
        {
            get
            {
                return PluginConfig.AllowActiveAimReload.Value && ReloadStateInstance.ReloadAnimationSupportsActiveAim;
            }
        }

        public float ChonkerFactorStanceRotationModifier
        {
            get
            {
                return IsChonker ? CHONKER_MODIFIER : 1f;
            }
        }

        //TODO: factor in strength skill
        public bool IsChonker
        {
            get
            {
                return WeaponStateInstance.TotalWeaponWeight >= STANCE_WEIGHT_LIMIT_KG;
            }
        }

        public Vector3 StanceRotation
        {
            get
            {
                return _stanceState.StanceRotation;
            }
        }

        public Vector3 StancePosition
        {
            get
            {
                return _stanceState.StancePosition;
            }
        }

        //TODO: this needs to move to FOV Fix and/or JSON file, or wherever weapon POS will be handled (common lib?)
        //Common lib can have functionality for it, but modules need to apply offsets themeselves
        //Common lib can then surface the starting weapon positions for PID
        public Dictionary<string, Vector3> GetWeaponOffsets()
        {
            return new Dictionary<string, Vector3>{
            { "5aafa857e5b5b00018480968", new Vector3(0f, 0f, -0.1f)}, //m1a
            { "5b0bbe4e5acfc40dc528a72d", new Vector3(0f, 0f, -0.035f)}, //sa58
            { "676176d362e0497044079f4c", new Vector3(0f, -0.0135f, 0.02f)}, //x17
            { "6183afd850224f204c1da514", new Vector3(0f, -0.0135f, 0.02f)}, //mk17
            { "6165ac306ef05c2ce828ef74", new Vector3(0f, -0.0135f, 0.02f)}, //mk17 fde
            { "6184055050224f204c1da540", new Vector3(0f, -0.0135f, 0.02f)}, //mk16
            { "618428466ef05c2ce828f218", new Vector3(0f, -0.0135f, 0.02f)}, //mk16 fde
            { "5ae08f0a5acfc408fb1398a1", new Vector3(0f, 0f, -0.005f)}, //mosin 
            { "5bfd297f0db834001a669119", new Vector3(0f, 0f, -0.005f)}, //mosin s
            { "54491c4f4bdc2db1078b4568", new Vector3(0f, 0f, -0.01f)}, //mp133
            { "56dee2bdd2720bc8328b4567", new Vector3(0f, 0f, -0.01f)}, //mp153
            { "606dae0ab0e443224b421bb7", new Vector3(0f, 0f, -0.01f)}, //mp155
            { "6259b864ebedf17603599e88", new Vector3(0f, 0f, -0.02f)}, //M3
            { "6783ae5bb52da6ed912e3d01", new Vector3(0f, 0f, -0.02f)}, //M3 mechanic
            };
        }

        private List<IControllerHelper> _stateControllers = new List<IControllerHelper>();
        private InputHookPipeline _inputHookPipeline;
        private StanceInputHandler _inputHandler;
        private StanceInputListener _inputListener;
        private StanceMovementHandler _movementState;
        private StanceAimHandler _aimState;
        private StanceState _stanceState;

        public Spring StancePositionSpring { get; private set; }
        public Spring StanceRotationSpring { get; private set; }

        private List<StanceBase> _stances = new List<StanceBase>();
        public PatrolStance PatrolStance { get; private set; }
        public LeftShoulder LeftShoulder { get; private set; }
        public LowReady LowReady { get; private set; }
        public HighReady HighReady { get; private set; }
        public ActiveAim ActiveAim { get; private set; }

        public bool AwakeRan { get; private set; } = false;

        public EStanceType CurrentStanceType => _stanceState.ActiveStanceType;

        void Awake()
        {
            AwakeRan = true;

            InitSprings();
            InitStateControllers();
            SubscribeToReloadEvents();
            SubscribeToInputEvents();
            AssignReloadHandlers();
            InitStances();
        }

        void Update()
        {
            float regularTime = Time.deltaTime;

            if (!CanDoUpdate()) return;

            RunUpdates(regularTime);

            //TODO: set these outside of update, and based on weapon stats
            StanceRotationSpring.ReturnSpeed = PluginConfig.test9.Value;
            StancePositionSpring.ReturnSpeed = PluginConfig.test9.Value;

            StanceRotationSpring.Damping = PluginConfig.test10.Value;
            StancePositionSpring.Damping = PluginConfig.test10.Value;
        }

        void OnDestroy()
        {
            UnassignReloadHandlers();
            RunStanceDispose();
            RunControllerOnDestroy();
            UnsubscribeFromReloadEvents();
            UnsubscribeFromInputEvents();
        }

        private void InitStances()
        {
            PatrolStance =
                InitStance(() => new PatrolStance());

            LeftShoulder =
                InitStance(() => new LeftShoulder());

            HighReady =
                InitStance(() => new HighReady());

            LowReady =
                InitStance(() => new LowReady());

            ActiveAim =
                InitStance(() => new ActiveAim());
        }

        private T InitStance<T>(Func<T> factory) where T : StanceBase
        {
            var instance = factory();
            _stances.Add(instance);
            return instance;
        }

        private void InitSprings()
        {
            StancePositionSpring = Cloner.ShallowClone(PlayerStateInstance.PWA.HandsContainer.HandsPosition);
            StanceRotationSpring = Cloner.ShallowClone(PlayerStateInstance.PWA.HandsContainer.HandsRotation);
        }

        private void InitStateControllers()
        {
            _stanceState =
                InitStateController(() => new StanceState());

            _inputHookPipeline =
                InitStateController(() => new InputHookPipeline());

            _movementState =
                InitStateController(() => new StanceMovementHandler());

            _aimState =
                InitStateController(() => new StanceAimHandler());

            _inputHandler =
                InitStateController(() => new StanceInputHandler(_stanceState));

            _inputListener =
                InitStateController(() => new StanceInputListener());

            RunControllerAwake();
        }

        private T InitStateController<T>(Func<T> factory) where T : IControllerHelper
        {
            var instance = factory();
            _stateControllers.Add(instance);
            return instance;
        }

        private void RunControllerAwake()
        {
            foreach (IControllerHelper controller in _stateControllers)
            {
                controller.RunOnAwake();
            }
        }

        private void RunUpdates(float deltaTime)
        {
            foreach (IControllerHelper controller in _stateControllers)
            {
                controller.RunOnUpdate(deltaTime);
            }
        }

        private void RunControllerOnDestroy()
        {
            foreach (IControllerHelper controller in _stateControllers)
            {
                controller.RunOnDestroy();
            }
        }

        private void RunStanceDispose()
        {
            foreach (StanceBase stance in _stances)
            {
                stance.Dispose();
            }
        }

        private void AssignReloadHandlers()
        {
            var reloadHandle = StatModifiers.MagReloadSpeed.Add(1.25f);

            _magReload = StatModifiers.MagReloadSpeed.Add(1f);
            _quickMagReload = StatModifiers.QuickMagReloadSpeed.Add(1f);
            _checkAmmo = StatModifiers.CheckAmmoSpeed.Add(1f);
            _checkChamber = StatModifiers.CheckChamberSpeed.Add(1f);
            _pumpBolt = StatModifiers.PumpBoltSpeed.Add(1f);
            _malfFix = StatModifiers.MalfFixSpeed.Add(1f);
            _rechamber = StatModifiers.RechamberSpeed.Add(1f);
            _noMagReload = StatModifiers.NoMagReloadSpeed.Add(1f);
            _currentMagRemove = StatModifiers.CurrentMagRemoveSpeed.Add(1f);
            _newMagReload = StatModifiers.NewMagReloadSpeed.Add(1f);
            _internalMagReload = StatModifiers.InternalReloadModifier.Add(1f);
        }

        private void UnassignReloadHandlers()
        {
            StatModifiers.MagReloadSpeed.Remove(_magReload);
            StatModifiers.QuickMagReloadSpeed.Remove(_quickMagReload);
            StatModifiers.CheckAmmoSpeed.Remove(_checkAmmo);
            StatModifiers.CheckChamberSpeed.Remove(_checkChamber);
            StatModifiers.PumpBoltSpeed.Remove(_pumpBolt);
            StatModifiers.MalfFixSpeed.Remove(_malfFix);
            StatModifiers.RechamberSpeed.Remove(_rechamber);
            StatModifiers.NoMagReloadSpeed.Remove(_noMagReload);
            StatModifiers.CurrentMagRemoveSpeed.Remove(_currentMagRemove);
            StatModifiers.NewMagReloadSpeed.Remove(_newMagReload);
            StatModifiers.InternalReloadModifier.Remove(_internalMagReload);
        }

        private void SubscribeToInputEvents()
        {
            PlayerEvents.OnWeaponDraw -= OnWeaponSwap;
            InputEvents.WeaponSwapInput += OnWeaponSwap;
            InputEvents.ToggleStepOutInput += OnToggleStepOut;
            InputEvents.ChangeStanceInput += OnChangeStance;
            InputEvents.ToggleBipodInput += OnToggleBipod;
            InputEvents.ToggleBreathingInput += OnToggleBreathing;
        }

        private void UnsubscribeFromInputEvents()
        {
            PlayerEvents.OnWeaponDraw -= OnWeaponSwap;
            InputEvents.WeaponSwapInput -= OnWeaponSwap;
            InputEvents.ToggleStepOutInput -= OnToggleStepOut;
            InputEvents.ChangeStanceInput -= OnChangeStance;
            InputEvents.ToggleBipodInput -= OnToggleBipod;
            InputEvents.ToggleBreathingInput -= OnToggleBreathing;
        }

        private void SubscribeToReloadEvents()
        {
            ReloadEvents.WeaponStateReset += OnWeaponStateReset;
            ReloadEvents.CheckAmmo += OnCheckAmmo;
            ReloadEvents.ChamberCheck += OnCheckChamber;
            ReloadEvents.MalfFix += OnMalfFix;
            ReloadEvents.Rechamber += OnRechamber;
            ReloadEvents.MagReload += OnMagReload;
            ReloadEvents.QuickMagReload += OnQuickMagReload;
            ReloadEvents.InternalMagReload += OnInternalMagReload;
        }

        private void UnsubscribeFromReloadEvents()
        {
            ReloadEvents.WeaponStateReset -= OnWeaponStateReset;
            ReloadEvents.CheckAmmo -= OnCheckAmmo;
            ReloadEvents.ChamberCheck -= OnCheckChamber;
            ReloadEvents.MalfFix -= OnMalfFix;
            ReloadEvents.Rechamber -= OnRechamber;
            ReloadEvents.MagReload -= OnMagReload;
            ReloadEvents.QuickMagReload -= OnQuickMagReload;
            ReloadEvents.InternalMagReload -= OnInternalMagReload;
        }

        //TODO: check if GameStateInstance.WeaponIsReady is essentially the same thing,
        //would make check in StanceStateUpdate redundant
        private bool CanDoUpdate()
        {
            Player player = PlayerStateInstance.Player;

            if (player != null &&
                PlayerStateInstance.FirearmController != null &&
                GameStateInstance.PlayerIsInRaidOrHideout &&
                PlayerStateInstance.WeaponIsReady)
            {
                return true;
            }
            return false;
        }

        //TODO: replace with event to trigger stances to cancel
        private bool IsUsingStationary()
        {
            return PlayerStateInstance.Player.MovementContext.CurrentState.Name != EPlayerState.Stationary;
        }

        public void ProceduralUpdate(float dt, int nFrames)
        {
            Plugin.StanceControllerInstance.StancePositionSpring.FixedUpdate(dt);
            Plugin.StanceControllerInstance.StanceRotationSpring.FixedUpdate(dt);


            /*            StancePositionSpring.AddAcceleration(StancePosition);
                        StanceRotationSpring.AddAcceleration(StanceRotation);*/


            // _stanceState.UpdateTransforms(dt);
        }

        //TODO: move reload bonuses to a reload controller class
        private void ApplyInternalReloadSpeedBonus()
        {
            float bonus = 1f;
            if (!ReloadStateInstance.IsAttemptingRevolverReload)
            {
                if (CurrentStanceType == EStanceType.LowReady == true && !WeaponStateInstance.IsShotgun) bonus = LOW_READY_RELOAD_SPEED_BUFF;
                else if (CurrentStanceType == EStanceType.HighReady == true && WeaponStateInstance.IsShotgun) bonus = HIGH_READY_RELOAD_SPEED_BUFF;
            }
            _internalMagReload.Multiplier = bonus;
        }

        private void ApplyMagReloadSpeedBonuses()
        {
            _magReload.Multiplier =
               CurrentStanceType == EStanceType.ActiveAiming && ShouldAllowActiveOnReload ? ACTIVE_AIM_RECHAMBER_SPEED_BUFF :
               CurrentStanceType == EStanceType.HighReady ? HIGH_READY_RELOAD_SPEED_BUFF :
               1f;
        }

        private void ApplyCheckAmmoSpeedBonus()
        {
            _checkAmmo.Multiplier = CurrentStanceType == EStanceType.HighReady ? HIGH_READY_CHECK_AMMO_SPEED_BUFF : 1f;
        }

        private void ApplyChamberSpeedBonus()
        {
            _rechamber.Multiplier =
                CurrentStanceType == EStanceType.ActiveAiming ? ACTIVE_AIM_RECHAMBER_SPEED_BUFF :
                CurrentStanceType == EStanceType.HighReady ? HIGH_READY_RECHAMBER_SPEED_BUFF :
                1f;
        }

        private void ApplyChamberCheckSpeedBonus()
        {
            _checkChamber.Multiplier =
                CurrentStanceType == EStanceType.ActiveAiming ? ACTIVE_AIM_RECHAMBER_SPEED_BUFF :
                CurrentStanceType == EStanceType.HighReady ? HIGH_READY_RECHAMBER_SPEED_BUFF :
                1f;
        }

        //TODO: what does pause mean, who is responsible to keep track of this?
        //Probably makes the most sense to allow individual stances handle this.
        private void CheckIfReloadPausesStance()
        {
            /*        //check might be unnecessary
                    if (!ReloadStateInstance.IsInReloadOpertation) return;

                    PauseShortStock = true;
                    PauseLeftShoulder = true;

                    if (ReloadStateInstance.IsAttemptingToReloadInternalMag)
                    {
                        PauseActiveAim = true;

                        bool isShotgun = WeaponStateInstance.IsShotgun;
                        PauseHighReady = !isShotgun;
                        PauseLowReady = isShotgun || WeaponStateInstance.TreatAsPistol;
                    }
                    else
                    {
                        PauseLowReady = true;
                        if (ShouldAllowActiveOnReload) PauseActiveAim = true;

                        //modify stance rotation/position
                        ModifyHighReady = true;
                    }*/
        }

        //TODO: replace all usages of DidWeaponSwap with events
        private void OnWeaponSwap()
        {
        }

        private void OnToggleStepOut()
        {
        }

        private void OnChangeStance()
        {
        }

        private void OnToggleBipod()
        {
        }

        //TODO: move IsAiming to common lib
        private void OnToggleBreathing()
        {
            if (AimStateInstance.IsAiming)
            {
                Player player = PlayerStateInstance.Player;
                if (player.Physical.HoldingBreath) return;

                //TODO: replace with animation bringing gun closer into shoulder
                //if (!IsChonker) DoWiggleEffects(player, player.ProceduralWeaponAnimation, WeaponStateInstance.WeaponInstance, new Vector3(0.25f, 0.25f, 0.5f), wiggleFactor: 0.5f);
            }
        }

        private void OnInternalMagReload()
        {
            CheckIfReloadPausesStance();
            ApplyInternalReloadSpeedBonus();
        }

        private void OnMagReload()
        {
            CheckIfReloadPausesStance();
            ApplyMagReloadSpeedBonuses();
        }

        private void OnQuickMagReload()
        {
            OnMagReload();
        }

        private void OnWeaponStateReset()
        {
        }

        private void OnRechamber()
        {
            /*PauseShortStock = true;
            PauseLeftShoulder = true;*/
            ApplyChamberSpeedBonus();
        }

        private void OnCheckChamber()
        {
            /* PauseLowReady = true;
             PauseHighReady = true;
             PauseShortStock = true;
             PauseLeftShoulder = true;*/
            ApplyChamberCheckSpeedBonus();
        }

        private void OnMalfFix()
        {
        }

        private void OnCheckAmmo()
        {
            /*            PauseLeftShoulder = true;
                        PauseLowReady = true;
                        PauseShortStock = true;
                        if (ShouldAllowActiveOnReload) PauseActiveAim = true;
                        ModifyHighReady = true;*/
            //_manipTimerTarget = 0f;
            ApplyCheckAmmoSpeedBonus();
        }

        private bool IsCantedAiming(ProceduralWeaponAnimation pwa, bool checkifAiming)
        {
            bool isCanted = Mathf.Abs(pwa.CurrentScope.Rotation) >= EFTHardSettings.Instance.SCOPE_ROTATION_THRESHOLD;
            bool isAimingOk = !checkifAiming || AimStateInstance.IsAiming;
            return isCanted && isAimingOk;
        }

        public bool IsIdle()
        {
            return _stanceState.ActiveStance == null;
        }

        //Should be replaced with some sort of event based system.
        //TODO: figure out why this is needed and if new IStance system can replace it, 
        //ideally move to each stance class responsiblity.
        public void StanceManipPauseTimer()
        {
            /*        _manipTime += Time.deltaTime;

                    if (_manipTime >= _manipTimerTarget)
                    {
                        PauseHighReady = false;
                        ModifyHighReady = false;
                        PauseLowReady = false;
                        PauseShortStock = false;
                        PausePistolStance = false;
                        PauseActiveAim = false;
                        PauseLeftShoulder = false;
                        ShouldUnpauseStances = false;

                        _manipTimerTarget = MANIP_TIMER;
                        _manipTime = 0f;
                    }*/
        }

        private void DoMeleeEffect()
        {
            Player player = Singleton<GameWorld>.Instance.MainPlayer;
            Player.FirearmController? fc = player.HandsController as Player.FirearmController;
            if (WeaponStateInstance.HasBayonet)
            {
                AudioControllerInstance.PlayKnifeAttackSound(2);
            }
            player.Physical.ConsumeAsMelee(2f + (WeaponStateInstance.ErgoFactor / 100f));
        }

        // TODO: replace values with consts and replace pwa reference with common lib only
        //this is used for changing weapon motion, not sure if should live here
        /*    private float GetStanceWeaponInertiaFactor(ProceduralWeaponAnimation pwa, bool forDisplacement = false)
            {
                if (forDisplacement)
                {
                    return
                        IsMounting ? 0.2f :
                        IsBracing ? 0.35f :
                        TargetStance == EStance.LeftShoulder ? 1.15f :
                        TargetStance == EStance.ShortStock ? 0.75f :
                        TargetStance == EStance.HighReady ? 0.91f :
                        TargetStance == EStance.LowReady ? 0.87f :
                        TargetStance == EStance.ActiveAiming ? 0.95f :
                        1f;
                }

                return
                    IsMounting ? 0.05f :
                    IsBracing ? 0.1f :
                    TargetStance == EStance.LeftShoulder && !pwa.IsAiming ? 1.15f :
                    TargetStance == EStance.LeftShoulder ? 0.87f :
                    pwa.IsAiming ? 0.75f :
                    WeaponStateInstance.TotalWeaponWeight > 1.6f && TargetStance == EStance.PistolCompressed ? 0.85f :
                    TargetStance == EStance.PistolCompressed ? 1.15f :
                    TargetStance == EStance.ShortStock ? 0.8f :
                    TargetStance == EStance.HighReady ? 0.85f :
                    TargetStance == EStance.LowReady ? 0.8f :
                    TargetStance == EStance.ActiveAiming ? 0.9f :
                    1f;
            }*/

        public float StanceSpeedMultiplier()
        {
            return TreatWeaponAsPistolStance ? StancePistolSpeedMultiplier() : StanceRifleSpeedMultiplier();
        }

        public float StancePistolSpeedMultiplier()
        {
#warning apply player weight factor
            float totalPlayerWeight = PlayerStateInstance.TotalWeightMinusWeapon;
            float playerWeightFactor = 1f + (totalPlayerWeight / 100f);

            float ergoMulti = Mathf.Clamp(ErgoStanceSpeed * Mathf.Pow(WeaponStateInstance.TotalWeaponHandlingModi, 0.5f), 0.65f, 1.45f);
            return Mathf.Clamp(ergoMulti * HealthStateInstance.StanceInjuryMulti * HealthStateInstance.AdrenalineStanceBonus * (Mathf.Max(PlayerStateInstance.RemainingArmStamFactor, 0.55f)), 0.5f, 1.45f);
        }
        public float StanceRifleSpeedMultiplier()
        {
#warning apply player weight factor
            float totalPlayerWeight = PlayerStateInstance.TotalWeightMinusWeapon;
            float playerWeightFactor = 1f + (totalPlayerWeight / 150f); //move to property + const, calculate once

            float lowerBaseLimit = IsChonker ? 0.45f : 0.55f; //move to property + const, calculate once
            float lowerSpeedLimit = IsChonker ? 0.3f : 0.4f; //move to property + const, calculate once
            float ergoMulti = Mathf.Clamp(1.15f * ErgoStanceSpeed * Mathf.Pow(WeaponStateInstance.TotalWeaponHandlingModi, 0.4f), lowerBaseLimit, 1.2f); //move to property + const, calculate once
            return Mathf.Clamp(ergoMulti * HealthStateInstance.StanceInjuryMulti * HealthStateInstance.AdrenalineStanceBonus * (Mathf.Max(PlayerStateInstance.RemainingArmStamFactor, 0.65f)), lowerSpeedLimit, 1.18f);
        }

        public void DoPistolStances(bool isThirdPerson, EFT.Animations.ProceduralWeaponAnimation pwa, float dt, Player player, Player.FirearmController fc, Vector3 camTarget)
        {
            /*   bool useThirdPersonStance = isThirdPerson;//  || Plugin.IsUsingFika
               float totalPlayerWeight = PlayerStateInstance.TotalWeightMinusWeapon;
               float playerWeightFactor = 1f + (totalPlayerWeight / 100f);
               float ergoMulti = Mathf.Clamp(ErgoStanceSpeed * Mathf.Pow(WeaponStateInstance.TotalWeaponHandlingModi, 0.5f), 0.65f, 1.45f);
               float stanceMulti = Mathf.Clamp(ergoMulti * HealthStateInstance.StanceInjuryMulti * HealthStateInstance.AdrenalineStanceBonus * (Mathf.Max(PlayerStateInstance.RemainingArmStamFactor, 0.55f)), 0.5f, 1.45f);

               //float balanceFactor = 1f + (WeaponStateInstance.Balance / 100f);
               // float rotationBalanceFactor = WeaponStateInstance.Balance <= -9f ? -balanceFactor : balanceFactor;
               //float wiggleBalanceFactor = Mathf.Abs(WeaponStateInstance.Balance) > 4f ? balanceFactor : Mathf.Abs(WeaponStateInstance.Balance) <= 4f ? 0.75f : Mathf.Abs(WeaponStateInstance.Balance) <= 3f ? 0.5f : 0.25f;
               float resetErgoMulti = (1f - stanceMulti) + 1f;

               float wiggleErgoMulti = Mathf.Clamp((ErgoStanceSpeed * 0.25f), 0.1f, 1f);
               WiggleReturnSpeed = (1f - (SkillStateInstance.AimSkillADSBuff * 0.5f)) * wiggleErgoMulti * HealthStateInstance.StanceInjuryMulti * playerWeightFactor * (Mathf.Max(PlayerStateInstance.RemainingArmStamFactor, 0.65f));

               float movementFactor = PlayerStateInstance.IsMoving ? 0.8f : 1f;

               Quaternion pistolRevertQuaternion = Quaternion.Euler(PluginConfig.PistolResetRotation.Value); // * rotationBalanceFactor
               Vector3 pistolPMCTargetPosition = useThirdPersonStance ? PluginConfig.PistolThirdPersonPosition.Value : PluginConfig.PistolOffset.Value;
               Vector3 pistolScavTargetPosition = useThirdPersonStance ? new Vector3(0.01f, 0.025f, -0.015f) : new Vector3(0.01f, 0.025f, -0.015f);
               Vector3 pistolTargetPosition = PlayerStateInstance.IsScav ? pistolScavTargetPosition : pistolPMCTargetPosition;
               Vector3 pistolPMCTargetRotation = useThirdPersonStance ? PluginConfig.PistolThirdPersonRotation.Value : PluginConfig.PistolRotation.Value;
               Vector3 pistolScavTargetRotation = useThirdPersonStance ? new Vector3(2f, -10f, 0f) : new Vector3(2f, -10f, 0f);
               Vector3 pistolTargetRotation = PlayerStateInstance.IsScav ? pistolScavTargetRotation : pistolPMCTargetRotation;
               Quaternion pistolTargetQuaternion = Quaternion.Euler(pistolTargetRotation);
               Quaternion pistolMiniTargetQuaternion = Quaternion.Euler(PluginConfig.PistolAdditionalRotation.Value);

               //I've no idea wtf is going on here but it sort of works
               HandleAltPistolPosition(player, fc, pwa, stanceMulti, dt, camTarget);

               if (TargetStance == EStance.PatrolStance) return;

               if (!pwa.IsAiming && !IsBlindFiring && !PistolIsColliding && !WeaponStateInstance.HasShoulderContact && PluginConfig.EnableAltPistol.Value) //!CancelPistolStance && !pwa.LeftStance
               {
                   if (TargetStance == EStance.PatrolStance || StoredStance == EStance.PatrolStance) _SkipPistolWiggle = true;
                   TargetStance = EStance.PistolCompressed;
                   StoredStance = EStance.None;
                   IsResettingPistol = false;
                   HasResetPistolPos = false;

                   StanceBlender.Speed = PluginConfig.PistolPosSpeedMulti.Value * stanceMulti;
                   StanceCurrentPosition = Vector3.Lerp(StanceCurrentPosition, pistolTargetPosition, PluginConfig.StanceTransitionSpeedMulti.Value * stanceMulti * dt);

                   if (StanceBlender.Value < 1f)
                   {
                       StanceRotationSpeed = 4f * stanceMulti * dt * PluginConfig.PistolAdditionalRotationSpeedMulti.Value * stanceMulti;
                       StanceTargetRotation = pistolMiniTargetQuaternion;
                   }
                   else
                   {
                       StanceRotationSpeed = 4f * stanceMulti * dt * PluginConfig.PistolRotationSpeedMulti.Value * stanceMulti * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value : 1f);
                       StanceTargetRotation = pistolTargetQuaternion;
                   }

                   if (StanceCurrentPosition == pistolTargetPosition && StanceBlender.Value >= 1f && !CanResetDamping)
                   {
                       _doDampingTimer = true;
                   }
                   else if (StanceCurrentPosition != pistolTargetPosition || StanceBlender.Value < 1)
                   {
                       CanResetDamping = false;
                   }

                   if (StanceBlender.Value < 0.95f || PausePistolStance)
                   {
                       //set DidStanceWiggle to false here previously
                   }
                   if ((StanceBlender.Value >= 1f && StanceCurrentPosition == pistolTargetPosition)) // && !DidStanceWiggle
                   {
   *//*                    if (!_SkipPistolWiggle && CurrentStance != EStance.LeftShoulder) DoWiggleEffects(player, pwa, fc.Weapon, new Vector3(-12.5f, 5f, 1f) * movementFactor);
                       DidStanceWiggle = true;
                       CancelPistolStance = false; //not sure why I set that here and how it relates to wiggling?
                       _SkipPistolWiggle = false;*//*
                   }

               }
               else if (StanceBlender.Value > 0f && !HasResetPistolPos && !PistolIsColliding)
               {
                   CanResetDamping = false;

                   IsResettingPistol = true;
                   StanceRotationSpeed = 4f * stanceMulti * dt * PluginConfig.PistolResetRotationSpeedMulti.Value * stanceMulti * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value : 1f);
                   StanceTargetRotation = pistolRevertQuaternion;
                   StanceBlender.Speed = PluginConfig.PistolPosResetSpeedMulti.Value * stanceMulti * (useThirdPersonStance ? PluginConfig.ThirdPersonPositionSpeed.Value : 1f);
               }
               else if (StanceBlender.Value == 0f && !HasResetPistolPos && !PistolIsColliding)
               {
                   if (!CanResetDamping)
                   {
                       _doDampingTimer = true;
                   }

                   if (TargetStance != EStance.LeftShoulder) DoWiggleEffects(player, pwa, fc.Weapon, new Vector3(-10f, 0f, -20f) * movementFactor); //new Vector3(10f, 1f, -30f) * wiggleBalanceFactor * rotationBalanceFactor  * wiggleBalanceFactor

                   IsResettingPistol = false;
                   TargetStance = EStance.None;
                   StanceTargetRotation = Quaternion.identity;
                   HasResetPistolPos = true;
               }*/
        }

        public void DoRifleStances(Player player, Player.FirearmController fc, bool isThirdPerson, EFT.Animations.ProceduralWeaponAnimation pwa, float dt, Vector3 camTarget)
        {
            float movementFactor = PlayerStateInstance.IsMoving ? 1.1f : 1f;
            bool useThirdPersonStance = isThirdPerson;
            float totalPlayerWeight = PlayerStateInstance.TotalWeightMinusWeapon;
            float playerWeightFactor = 1f + (totalPlayerWeight / 150f); //move to property + const, calculate once
            float lowerBaseLimit = IsChonker ? 0.45f : 0.55f; //move to property + const, calculate once
            float lowerSpeedLimit = IsChonker ? 0.3f : 0.4f; //move to property + const, calculate once
            float ergoMulti = Mathf.Clamp(1.15f * ErgoStanceSpeed * Mathf.Pow(WeaponStateInstance.TotalWeaponHandlingModi, 0.4f), lowerBaseLimit, 1.2f); //move to property + const, calculate once
            float stanceMulti = Mathf.Clamp(ergoMulti * HealthStateInstance.StanceInjuryMulti * HealthStateInstance.AdrenalineStanceBonus * (Mathf.Max(PlayerStateInstance.RemainingArmStamFactor, 0.65f)), lowerSpeedLimit, 1.18f); //move to property + const, calculate once
            float resetErgoMulti = (1f - stanceMulti) + 1f;

            bool pauseStance = PlayerStateInstance.IsInventoryOpen || IsBlindFiring || CurrentStanceType == EStanceType.LeftShoulder;

            float wiggleErgoMulti = Mathf.Clamp((ErgoStanceSpeed * 0.5f), 0.1f, 1f);
            float stocklessModifier = WeaponStateInstance.HasShoulderContact ? 1f : 0.5f;
            //WiggleReturnSpeed = (1f - (SkillStateInstance.AimSkillADSBuff * 0.5f)) * wiggleErgoMulti * HealthStateInstance.StanceInjuryMulti * stocklessModifier * playerWeightFactor * (Mathf.Max(PlayerStateInstance.RemainingArmStamFactor, 0.55f));

            /*          //for setting baseline position
                      if (!isThirdPerson)
                      {
                          HandleRiflePosition(player, fc, pwa, stanceMulti, movementFactor, dt, camTarget);
                      }

                      DoTacSprint(fc, player);

                      ////short-stock////
                      DoShortStock(player, fc, isThirdPerson, pwa, dt, useThirdPersonStance, stanceMulti, resetErgoMulti, pauseStance, movementFactor);

                      ////high ready////
                      DoHighReady(player, fc, isThirdPerson, pwa, dt, useThirdPersonStance, stanceMulti, resetErgoMulti, pauseStance, movementFactor);

                      ////low ready////
                      DoLowReady(player, fc, isThirdPerson, pwa, dt, useThirdPersonStance, stanceMulti, resetErgoMulti, pauseStance, movementFactor);

                      ////active aiming////
                      DoActiveAim(player, fc, isThirdPerson, pwa, dt, useThirdPersonStance, stanceMulti, resetErgoMulti, pauseStance, movementFactor);

                      ////Melee////
                      DoMeleeStance(player, fc, isThirdPerson, pwa, dt, useThirdPersonStance, stanceMulti, resetErgoMulti, pauseStance, movementFactor);
          */
        }

        public void DoShortStock(Player player, Player.FirearmController fc, bool isThirdPerson, EFT.Animations.ProceduralWeaponAnimation pwa, float dt, bool useThirdPersonStance, float stanceMulti, float resetErgoMulti, bool pauseStance, float movementFactor)
        {
            /*            float shortStockStanceMulti = Mathf.Clamp(stanceMulti, 0.65f, 1.5f);

                        Vector3 shortTargetRotation = useThirdPersonStance ?
                            PluginConfig.ShortStockThirdPersonRotation.Value :
                            PluginConfig.ShortStockRotation.Value * shortStockStanceMulti;
                        Quaternion shortStockTargetQuaternion = Quaternion.Euler(shortTargetRotation);
                        Quaternion shortStockMiniTargetQuaternion = Quaternion.Euler(PluginConfig.ShortStockAdditionalRotation.Value * resetErgoMulti);
                        Quaternion shortStockRevertQuaternion = Quaternion.Euler(PluginConfig.ShortStockResetRotation.Value * resetErgoMulti);
                        Vector3 shortStockTargetPosition = useThirdPersonStance ?
                            PluginConfig.ShortStockThirdPersonPosition.Value :
                            PluginConfig.ShortStockOffset.Value;

                        if (TargetStance == EStance.ShortStock && !pwa.IsAiming && !PauseShortStock && !IsBlindFiring && !pwa.LeftStance && !PlayerStateInstance.IsSprinting && !pauseStance)
                        {
                            float activeToShort = 1f;
                            float highToShort = 1f;
                            float lowToShort = 1f;
                            IsResettingShortStock = false;
                            HasResetShortStock = false;
                            HasResetMelee = true;

                            if (StanceCurrentPosition != shortStockTargetPosition)
                            {
                                if (!HasResetActiveAim)
                                {
                                    activeToShort = 0.55f;
                                }
                                if (!HasResetHighReady)
                                {
                                    highToShort = 0.78f;
                                }
                                if (!HasResetLowReady)
                                {
                                    lowToShort = 0.55f;
                                }
                            }
                            else
                            {
                                HasResetActiveAim = true;
                                HasResetHighReady = true;
                                HasResetLowReady = true;
                            }

                            if (StanceCurrentPosition == shortStockTargetPosition && StanceBlender.Value >= 1f && !CanResetDamping)
                            {
                                _doDampingTimer = true;
                            }
                            else if (StanceCurrentPosition != shortStockTargetPosition || StanceBlender.Value < 1)
                            {
                                CanResetDamping = false;
                            }

                            float transitionPositionFactor = activeToShort * highToShort * lowToShort;
                            float transitionRotationFactor = activeToShort * highToShort * lowToShort;

                            if (StanceBlender.Value < 1f)
                            {
                                StanceRotationSpeed = 4f * shortStockStanceMulti * dt * PluginConfig.ShortStockAdditionalRotationSpeedMulti.Value * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value : 1f) * transitionRotationFactor;
                                StanceTargetRotation = shortStockMiniTargetQuaternion;
                            }
                            else
                            {
                                StanceRotationSpeed = 4f * shortStockStanceMulti * dt * PluginConfig.ShortStockRotationMulti.Value * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value : 1f) * transitionRotationFactor;
                                StanceTargetRotation = shortStockTargetQuaternion;
                            }

                            StanceBlender.Speed = PluginConfig.ShortStockSpeedMulti.Value * shortStockStanceMulti * (useThirdPersonStance ? PluginConfig.ThirdPersonPositionSpeed.Value : 1f);
                            StanceCurrentPosition = Vector3.Lerp(StanceCurrentPosition, shortStockTargetPosition, PluginConfig.StanceTransitionSpeedMulti.Value * shortStockStanceMulti * transitionPositionFactor * dt);

                            if ((StanceBlender.Value >= 0.9f || StanceCurrentPosition == shortStockTargetPosition) && !useThirdPersonStance) //&& !DidStanceWiggle
                            {
                          *//*      DoWiggleEffects(player, pwa, fc.Weapon, new Vector3(5f, -2.5f, 30f) * movementFactor, true);
                                DidStanceWiggle = true;*//*
                            }
                        }
                        else if (StanceBlender.Value > 0f && !HasResetShortStock && TargetStance == EStance.None && !IsResettingActiveAim && !IsResettingHighReady && !IsResettingLowReady && !IsResettingMelee)
                        {
                            CanResetDamping = false;
                            IsResettingShortStock = true;
                            StanceRotationSpeed = 4f * shortStockStanceMulti * dt * PluginConfig.ShortStockResetRotationSpeedMulti.Value;
                            StanceTargetRotation = shortStockRevertQuaternion;
                            StanceBlender.Speed = PluginConfig.ShortStockResetSpeedMulti.Value * shortStockStanceMulti * (useThirdPersonStance ? PluginConfig.ThirdPersonPositionSpeed.Value : 1f);
                        }
                        else if (StanceBlender.Value == 0f && !HasResetShortStock)
                        {
                            if (!CanResetDamping)
                            {
                                _doDampingTimer = true;
                            }

                            if (!useThirdPersonStance) DoWiggleEffects(player, pwa, fc.Weapon, new Vector3(-4f, -2f, -30f) * movementFactor, true);
                            *//*DidStanceWiggle = false;*//*
                            StanceTargetRotation = Quaternion.identity;
                            IsResettingShortStock = false;
                            HasResetShortStock = true;
                        }*/
        }

        public void DoHighReady(Player player, Player.FirearmController fc, bool isThirdPerson, EFT.Animations.ProceduralWeaponAnimation pwa, float dt, bool useThirdPersonStance, float stanceMulti, float resetErgoMulti, bool pauseStance, float movementFactor)
        {
            /*           float highReadyStanceMulti = Mathf.Clamp(stanceMulti, 0.5f, 0.98f);
                       float highReadyXWiggleFactor = WeaponStateInstance.TotalErgo <= 49f ? -1f : 1f;
                       float highReadyZWiggleFactor = WeaponStateInstance.TotalErgo <= 40f ? 1f : 2f;

                       Vector3 highTargetRotation = useThirdPersonStance ?
                           PluginConfig.HighReadyThirdPersonRotation.Value :
                           new Vector3(
                               PluginConfig.HighReadyRotation.Value.x * stanceMulti,
                               PluginConfig.HighReadyRotation.Value.y * stanceMulti * (ModifyHighReady ? -1f : 1f),
                               PluginConfig.HighReadyRotation.Value.z * stanceMulti);

                       Vector3 highReadyTargetPosition = useThirdPersonStance ?
                           PluginConfig.HighReadyThirdPersonPosition.Value :
                           new Vector3(
                               PluginConfig.HighReadyOffset.Value.x,
                               PluginConfig.HighReadyOffset.Value.y * (ModifyHighReady ? 0.25f : 1f),
                               PluginConfig.HighReadyOffset.Value.z);

                       Quaternion highReadyTargetQuaternion = Quaternion.Euler(highTargetRotation);
                       Quaternion highReadyMiniTargetQuaternion = Quaternion.Euler(PluginConfig.HighReadyAdditionalRotation.Value * resetErgoMulti);
                       Quaternion highReadyRevertQuaternion = Quaternion.Euler(PluginConfig.HighReadyResetRotation.Value * resetErgoMulti);

                       if (TargetStance == EStance.HighReady && !pwa.IsAiming && !FiringStateInstance.IsFiringFromStance && !PauseHighReady && !pauseStance)
                       {
                           float shortToHighMulti = 1.0f;
                           float lowToHighMulti = 1.0f;
                           float activeToHighMulti = 1.0f;
                           IsResettingHighReady = false;
                           HasResetHighReady = false;
                           HasResetMelee = true;

                           if (StanceCurrentPosition != highReadyTargetPosition)
                           {
                               if (!HasResetShortStock)
                               {
                                   shortToHighMulti = 0.82f;
                               }
                               if (!HasResetActiveAim)
                               {
                                   activeToHighMulti = 1f;
                               }
                               if (!HasResetLowReady)
                               {
                                   lowToHighMulti = 1f;
                               }
                           }
                           else
                           {
                               HasResetActiveAim = true;
                               HasResetLowReady = true;
                               HasResetShortStock = true;
                           }

                           if (StanceCurrentPosition == highReadyTargetPosition && StanceBlender.Value == 1 && !CanResetDamping)
                           {
                               _doDampingTimer = true;
                           }
                           else if (StanceCurrentPosition != highReadyTargetPosition || StanceBlender.Value < 1)
                           {
                               CanResetDamping = false;
                           }

                           float transitionPositionFactor = shortToHighMulti * lowToHighMulti * activeToHighMulti;
                           float transitionRotationFactor = shortToHighMulti * lowToHighMulti * activeToHighMulti * (transitionPositionFactor != 1f ? 0.9f : 1f);

                           if (CanDoHighReadyInjuredAnim)
                           {
                               if (StanceBlender.Value < 0.3f)
                               {
           #warning replace this with a coroutined animation curve
                                   Vector3 lowTargetRotation = useThirdPersonStance ?
                                       PluginConfig.LowReadyThirdPersonRotation.Value :
                                       new Vector3(
                                           PluginConfig.LowReadyRotation.Value.x * resetErgoMulti,
                                           PluginConfig.LowReadyRotation.Value.y,
                                           PluginConfig.LowReadyRotation.Value.z);

                                   Quaternion lowReadyTargetQuaternion = Quaternion.Euler(lowTargetRotation);

                                   StanceRotationSpeed = 3f * highReadyStanceMulti * dt * PluginConfig.HighReadyRotationMulti.Value * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value * 0.7f : 1f) * (WeaponStateInstance.IsPistol ? 0.5f : 1f);
                                   StanceTargetRotation = lowReadyTargetQuaternion;
                               }
                               else
                               {
                                   StanceRotationSpeed = 3f * highReadyStanceMulti * dt * PluginConfig.HighReadyAdditionalRotationSpeedMulti.Value * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value * 0.2f : 1f) * (WeaponStateInstance.IsPistol ? 0.5f : 1f);
                                   StanceTargetRotation = highReadyMiniTargetQuaternion;
                               }
                           }
                           else
                           {
                               if (StanceBlender.Value < 0.3f)
                               {
                                   StanceRotationSpeed = 4f * highReadyStanceMulti * dt * PluginConfig.HighReadyAdditionalRotationSpeedMulti.Value * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value * 0.2f : 1f) * transitionRotationFactor * (WeaponStateInstance.IsPistol ? 0.5f : 1f);
                                   StanceTargetRotation = highReadyMiniTargetQuaternion;
                               }
                               else
                               {
                                   StanceRotationSpeed = 4f * highReadyStanceMulti * dt * PluginConfig.HighReadyRotationMulti.Value * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value * 0.7f : 1f) * transitionRotationFactor * (WeaponStateInstance.IsPistol ? 0.5f : 1f);
                                   StanceTargetRotation = highReadyTargetQuaternion;
                               }
                           }

                           StanceBlender.Speed = PluginConfig.HighReadySpeedMulti.Value * highReadyStanceMulti * (useThirdPersonStance ? PluginConfig.ThirdPersonPositionSpeed.Value : 1f);
                           StanceCurrentPosition = Vector3.Lerp(StanceCurrentPosition, highReadyTargetPosition, PluginConfig.StanceTransitionSpeedMulti.Value * highReadyStanceMulti * transitionPositionFactor * dt);

                           if ((StanceBlender.Value >= 1f || StanceCurrentPosition == highReadyTargetPosition) && !useThirdPersonStance) // && !DidStanceWiggle
                           {
                     *//*          if (!WeaponStateInstance.IsPistol) DoWiggleEffects(player, pwa, fc.Weapon, new Vector3(5f, 5f, 5f) * movementFactor, true);//new Vector3(11f, 5.5f, 50f)
                               DidStanceWiggle = true;*//*
                           }
                       }
                       else if (StanceBlender.Value > 0f && !HasResetHighReady && TargetStance == EStance.None && !IsResettingActiveAim && !IsResettingLowReady && !IsResettingShortStock && !IsResettingMelee)
                       {
                           CanResetDamping = false;
                           IsResettingHighReady = true;
                           StanceRotationSpeed = 4f * highReadyStanceMulti * dt * PluginConfig.HighReadyResetRotationMulti.Value * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value : 1f);
                           StanceTargetRotation = highReadyRevertQuaternion;
                           StanceBlender.Speed = PluginConfig.HighReadyResetSpeedMulti.Value * highReadyStanceMulti * (useThirdPersonStance ? PluginConfig.ThirdPersonPositionSpeed.Value : 1f);
                       }
                       else if (StanceBlender.Value <= 0f && !HasResetHighReady)
                       {
                           if (!CanResetDamping)
                           {
                               _doDampingTimer = true;
                           }

                           if (!useThirdPersonStance && !WeaponStateInstance.IsPistol) DoWiggleEffects(player, pwa, fc.Weapon, new Vector3(highReadyXWiggleFactor * 10f, highReadyXWiggleFactor * 1f, highReadyZWiggleFactor * -10f) * movementFactor, true); //(1.5f, 3.75f, -30)
                           //DidStanceWiggle = false;
                           StanceTargetRotation = Quaternion.identity;
                           IsResettingHighReady = false;
                           HasResetHighReady = true;
                       }*/
        }

        public void DoLowReady(Player player, Player.FirearmController fc, bool isThirdPerson, EFT.Animations.ProceduralWeaponAnimation pwa, float dt, bool useThirdPersonStance, float stanceMulti, float resetErgoMulti, bool pauseStance, float movementFactor)
        {
            /*         float lowReadyStanceMulti = Mathf.Clamp(stanceMulti, 0.5f, 0.98f);

                     Vector3 lowTargetRotation = useThirdPersonStance ?
                         PluginConfig.LowReadyThirdPersonRotation.Value :
                         new Vector3(
                             PluginConfig.LowReadyRotation.Value.x * resetErgoMulti,
                             PluginConfig.LowReadyRotation.Value.y,
                             PluginConfig.LowReadyRotation.Value.z);

                     Quaternion lowReadyTargetQuaternion = Quaternion.Euler(lowTargetRotation);
                     Quaternion lowReadyMiniTargetQuaternion = Quaternion.Euler(PluginConfig.LowReadyAdditionalRotation.Value * resetErgoMulti);
                     Quaternion lowReadyRevertQuaternion = Quaternion.Euler(PluginConfig.LowReadyResetRotation.Value * resetErgoMulti);

                     Vector3 lowReadyTargetPosition = useThirdPersonStance ?
                         PluginConfig.LowReadyThirdPersonPosition.Value :
                         PluginConfig.LowReadyOffset.Value;

                     if (TargetStance == EStance.LowReady && !pwa.IsAiming && !FiringStateInstance.IsFiringFromStance && !PauseLowReady && !pauseStance)
                     {
                         float highToLow = 1.0f;
                         float shortToLow = 1.0f;
                         float activeToLow = 1.0f;
                         IsResettingLowReady = false;
                         HasResetLowReady = false;
                         HasResetMelee = true;

                         if (StanceCurrentPosition != lowReadyTargetPosition)
                         {
                             if (!HasResetHighReady)
                             {
                                 highToLow = 0.95f;
                             }
                             if (!HasResetShortStock)
                             {
                                 shortToLow = 0.7f;
                             }
                             if (!HasResetActiveAim)
                             {
                                 activeToLow = 0.87f;
                             }
                         }
                         else
                         {
                             HasResetHighReady = true;
                             HasResetShortStock = true;
                             HasResetActiveAim = true;
                         }

                         if (StanceCurrentPosition == lowReadyTargetPosition && StanceBlender.Value >= 1f && !CanResetDamping)
                         {
                             _doDampingTimer = true;
                         }
                         else if (StanceCurrentPosition != lowReadyTargetPosition || StanceBlender.Value < 1)
                         {
                             CanResetDamping = false;
                         }

                         float transitionPositionFactor = highToLow * shortToLow * activeToLow;
                         float transitionRotationFactor = highToLow * shortToLow * activeToLow * (transitionPositionFactor != 1f ? 1.025f : 1f);

                         if (StanceBlender.Value < 1f)
                         {
                             StanceRotationSpeed = 4f * lowReadyStanceMulti * dt * PluginConfig.LowReadyAdditionalRotationSpeedMulti.Value * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value * 0.8f : 1f) * transitionRotationFactor;
                             StanceTargetRotation = lowReadyMiniTargetQuaternion;
                         }
                         else
                         {
                             StanceRotationSpeed = 4f * lowReadyStanceMulti * dt * PluginConfig.LowReadyRotationMulti.Value * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value * 0.8f : 1f) * transitionRotationFactor;
                             StanceTargetRotation = lowReadyTargetQuaternion;
                         }

                         StanceBlender.Speed = PluginConfig.LowReadySpeedMulti.Value * lowReadyStanceMulti * (useThirdPersonStance ? PluginConfig.ThirdPersonPositionSpeed.Value * 0.8f : 1f);
                         StanceCurrentPosition = Vector3.Lerp(StanceCurrentPosition, lowReadyTargetPosition, PluginConfig.StanceTransitionSpeedMulti.Value * lowReadyStanceMulti * transitionPositionFactor * dt);

                         if ((StanceBlender.Value >= 0.5f || StanceCurrentPosition == lowReadyTargetPosition) && !useThirdPersonStance) // && !DidStanceWiggle
                         {
                  *//*           DoWiggleEffects(player, pwa, fc.Weapon, new Vector3(7f, 7f, 0f) * movementFactor, true);
                             DidStanceWiggle = true;*//*
                         }
                         DidLowReadyResetStanceWiggle = false;
                     }
                     else if (StanceBlender.Value > 0f && !HasResetLowReady && TargetStance == EStance.None && !IsResettingActiveAim && !IsResettingHighReady && !IsResettingShortStock && !IsResettingMelee)
                     {
                         CanResetDamping = false;

                         IsResettingLowReady = true;
                         StanceRotationSpeed = 4f * lowReadyStanceMulti * dt * PluginConfig.LowReadyResetRotationMulti.Value * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value * 0.8f : 1f);
                         StanceTargetRotation = lowReadyRevertQuaternion;

                         StanceBlender.Speed = PluginConfig.LowReadyResetSpeedMulti.Value * lowReadyStanceMulti * (useThirdPersonStance ? PluginConfig.ThirdPersonPositionSpeed.Value * 0.8f : 1f);

                         if (!useThirdPersonStance && StanceBlender.Value <= 0.65f && !DidLowReadyResetStanceWiggle)
                         {
                             DoWiggleEffects(player, pwa, fc.Weapon, new Vector3(-10f, 4f, 10f) * movementFactor, true); //new Vector3(-4f, 2.5f, 10f)
                             DidLowReadyResetStanceWiggle = true;
                         }
                     }
                     else if (StanceBlender.Value == 0f && !HasResetLowReady)
                     {
                         if (!CanResetDamping)
                         {
                             _doDampingTimer = true;
                         }
                         StanceTargetRotation = Quaternion.identity;
                         IsResettingLowReady = false;
                         HasResetLowReady = true;
                     }*/
        }

        public void DoActiveAim(Player player, Player.FirearmController fc, bool isThirdPerson, EFT.Animations.ProceduralWeaponAnimation pwa, float dt, bool useThirdPersonStance, float stanceMulti, float resetErgoMulti, bool pauseStance, float movementFactor)
        {
            /*            Vector3 activeTargetRoation =
                            useThirdPersonStance ? PluginConfig.ActiveThirdPersonRotation.Value :
                            PluginConfig.ActiveAimRotation.Value;

                        Quaternion activeAimMiniTargetQuaternion =
                            Quaternion.Euler(PluginConfig.ActiveAimAdditionalRotation.Value * resetErgoMulti);

                        Quaternion activeAimRevertQuaternion =
                             IsCantedAiming(pwa, true) ? Quaternion.Euler(new Vector3(0f, 10f, -1f) * resetErgoMulti) :
                             Quaternion.Euler(PluginConfig.ActiveAimResetRotation.Value * resetErgoMulti);

                        Vector3 activeAimTargetPosition = useThirdPersonStance ?
                            PluginConfig.ActiveThirdPersonPosition.Value :
                            PluginConfig.ActiveAimOffset.Value;

                        Quaternion activeAimTargetQuaternion = Quaternion.Euler(activeTargetRoation);

                        if (TargetStance == EStance.ActiveAiming && !PauseActiveAim && !pauseStance)
                        {
                            float ergoFactor = WeaponStateInstance.TotalErgo <= 40f ? 0.75f : 1f;
                            float shortToActive = 1f;
                            float shortToActiveRotation = 1f;
                            float highToActive = 1f;
                            float lowToActive = 1f;
                            float highToActiveRotation = 1f;
                            float lowToActiveRotation = 1f;
                            IsResettingActiveAim = false;
                            HasResetActiveAim = false;
                            HasResetMelee = true;

                            if (StanceCurrentPosition != activeAimTargetPosition)
                            {
                                if (!HasResetShortStock)
                                {
                                    shortToActive = 0.45f;
                                    shortToActiveRotation = 0.9f;
                                }
                                if (!HasResetHighReady)
                                {
                                    highToActive = 1.15f;
                                    highToActiveRotation = 1.15f;
                                }
                                if (!HasResetLowReady)
                                {
                                    lowToActive = 1.29f;
                                    lowToActiveRotation = 1.37f;
                                }
                            }
                            else
                            {
                                HasResetShortStock = true;
                                HasResetHighReady = true;
                                HasResetLowReady = true;
                            }

                            if (StanceCurrentPosition == activeAimTargetPosition && StanceBlender.Value == 1 && !CanResetDamping)
                            {
                                _doDampingTimer = true;
                            }
                            else if (StanceCurrentPosition != activeAimTargetPosition || StanceBlender.Value < 1)
                            {
                                CanResetDamping = false;
                            }

                            float transitionPositionFactor = shortToActive * highToActive * lowToActive;
                            float transitionRotationFactor = shortToActiveRotation * highToActiveRotation * lowToActiveRotation; //(transitionPositionFactor != 1f ? 0.9f : 1f)

                            //additonal rotation makes ADS janky
                            *//*     if (StanceBlender.Value < 1f)
                                 {

                                     StanceTargetPosition = Vector3.Lerp(StanceTargetPosition, activeAimTargetPosition, PluginConfig.StanceTransitionSpeedMulti.Value * stanceMulti * transitionPositionFactor * dt);
                                     rotationSpeed = 4f * stanceMulti * dt * ergoFactor * PluginConfig.ActiveAimAdditionalRotationSpeedMulti.Value * ChonkerFactor * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value : 1f) * transitionRotationFactor;
                                     stanceRotation = activeAimMiniTargetQuaternion;
                                 }
                                 else
                                 {
                                     StanceTargetPosition = Vector3.Lerp(StanceTargetPosition, activeAimTargetPosition, PluginConfig.StanceTransitionSpeedMulti.Value * stanceMulti * transitionPositionFactor * dt);
                                     rotationSpeed = 4f * stanceMulti * dt * ergoFactor * PluginConfig.ActiveAimRotationMulti.Value * ChonkerFactor * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value : 1f) * transitionRotationFactor;
                                     stanceRotation = activeAimTargetQuaternion;
                                 }*//*

                            StanceCurrentPosition = Vector3.Lerp(StanceCurrentPosition, activeAimTargetPosition, PluginConfig.StanceTransitionSpeedMulti.Value * stanceMulti * transitionPositionFactor * dt);
                            StanceRotationSpeed = 4f * stanceMulti * dt * ergoFactor * PluginConfig.ActiveAimRotationSpeedMulti.Value * ChonkerFactorStanceRotationModifier * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value : 1f) * transitionRotationFactor;
                            StanceTargetRotation = activeAimTargetQuaternion;

                            StanceBlender.Speed = PluginConfig.ActiveAimPosSpeedMulti.Value * stanceMulti * ergoFactor * ChonkerFactorStanceRotationModifier * (useThirdPersonStance ? PluginConfig.ThirdPersonPositionSpeed.Value : 1f);

                    *//*        if ((StanceBlender.Value >= 1f || StanceTargetPosition == activeAimTargetPosition) && !DidStanceWiggle && !useThirdPersonStance)
                            {
                                DoWiggleEffects(player, pwa, fc.Weapon, new Vector3(-10f, -10f, 0f), true, 3f);
                                DidStanceWiggle = true;
                            }*//*
                        }
                        else if (StanceBlender.Value > 0f && !HasResetActiveAim && TargetStance == EStance.None && !IsResettingLowReady && !IsResettingHighReady && !IsResettingShortStock && !IsResettingMelee)
                        {
                            CanResetDamping = false;

                            IsResettingActiveAim = true;
                            StanceRotationSpeed = stanceMulti * dt * PluginConfig.ActiveAimResetRotationSpeedMulti.Value * ChonkerFactorStanceRotationModifier * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value : 1f);
                            StanceTargetRotation = activeAimRevertQuaternion;
                            StanceBlender.Speed = PluginConfig.ActiveAimResetSpeedMulti.Value * stanceMulti * ChonkerFactorStanceRotationModifier * (useThirdPersonStance ? PluginConfig.ThirdPersonPositionSpeed.Value : 1f);
                        }
                        else if (StanceBlender.Value == 0f && !HasResetActiveAim)
                        {
                            if (!CanResetDamping)
                            {
                                _doDampingTimer = true;
                            }

                   *//*         if (!useThirdPersonStance) DoWiggleEffects(player, pwa, fc.Weapon, new Vector3(-5f, 1.5f, 0f) * movementFactor, true, 3f);
                            DidStanceWiggle = false;*//*

                            StanceTargetRotation = Quaternion.identity;

                            IsResettingActiveAim = false;
                            HasResetActiveAim = true;
                        }*/
        }

        public void DoMeleeStance(Player player, Player.FirearmController fc, bool isThirdPerson, EFT.Animations.ProceduralWeaponAnimation pwa, float dt, bool useThirdPersonStance, float stanceMulti, float resetErgoMulti, bool pauseStance, float movementFactor)
        {
            /*           if (WeaponStateInstance.HasBayonet)
                       {
                           DoMeleeStanceBayonet(player, fc, isThirdPerson, pwa, dt, useThirdPersonStance, stanceMulti, resetErgoMulti, pauseStance, movementFactor);
                           return;
                       }

                       bool isDoingMelee = TargetStance == EStance.Melee && !pwa.IsAiming && !pauseStance;

                       Quaternion meleeInitialQuaternion = Quaternion.Euler(new Vector3(2.5f * resetErgoMulti, -15f * resetErgoMulti, -1f));
                       Quaternion meleeFinalQuaternion = Quaternion.Euler(new Vector3(-1.5f * resetErgoMulti, -7.5f * resetErgoMulti, -0.5f));
                       Vector3 meleeInitialPos = new Vector3(0f, 0.06f, 0f);
                       Vector3 meleeFinalPos = new Vector3(0f, -0.0275f, 0f);

                       ////Melee////
                       if (isDoingMelee && !PlayerStateInstance.IsSprinting)
                       {
                           IsResettingMelee = false;
                           HasResetMelee = false;
                           HasResetActiveAim = true;
                           HasResetHighReady = true;
                           HasResetLowReady = true;
                           HasResetShortStock = true;

                           if (StanceCurrentPosition == meleeFinalPos && StanceBlender.Value >= 1f && !CanResetDamping)
                           {
                               _doDampingTimer = true;
                           }
                           else if (StanceCurrentPosition != meleeFinalPos || StanceBlender.Value < 1)
                           {
                               CanResetDamping = false;
                           }

                           StanceRotationSpeed = 10f * Mathf.Clamp(stanceMulti, 0.8f, 1f) * dt * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value : 1f);

                           float initialPosDistance = Vector3.Distance(StanceCurrentPosition, meleeInitialPos);
                           float finalPosDistance = Vector3.Distance(StanceCurrentPosition, meleeFinalPos);

                           if (initialPosDistance > 0.001f && !DidHalfMeleeAnim)
                           {
                               StanceTargetRotation = meleeInitialQuaternion;
                               StanceCurrentPosition = Vector3.Lerp(StanceCurrentPosition, meleeInitialPos, PluginConfig.StanceTransitionSpeedMulti.Value * Mathf.Clamp(stanceMulti, 0.75f, 1f) * dt * 1.5f * ChonkerFactorStanceRotationModifier);
                           }
                           else
                           {
                               DidHalfMeleeAnim = true;
                               StanceTargetRotation = meleeFinalQuaternion;
                               StanceCurrentPosition = Vector3.Lerp(StanceCurrentPosition, meleeFinalPos, PluginConfig.StanceTransitionSpeedMulti.Value * Mathf.Clamp(stanceMulti, 0.75f, 1f) * dt * 2f * ChonkerFactorStanceRotationModifier);
                           }

                           //TODO: GATE THIS, OTHERWISE IT'LL RUN MULTIPLE TIMES
                           if (StanceBlender.Value >= 1f && finalPosDistance <= 0.001f) // && !DidStanceWiggle
                           {
                               DoMeleeEffect();
           *//*                    DoWiggleEffects(player, pwa, fc.Weapon, new Vector3(-20f, -10f, -90f) * movementFactor, true, 1f, useGearSound: true);
                               DidStanceWiggle = true;*//*
                           }

                           if (StanceBlender.Value >= 0.9f && DidHalfMeleeAnim)
                           {
                               CanDoMeleeDetection = true;
                           }

                           if (StanceBlender.Value >= 1f && finalPosDistance <= 0.001f)
                           {
                               TargetStance = StoredStance;
                               StanceBlender.Target = 0f;
                           }
                       }
                       else if (StanceBlender.Value > 0f && !HasResetMelee) //&& !IsLowReady && !IsActiveAiming && !IsHighReady && !IsShortStock && !isResettingActiveAim && !isResettingHighReady && !isResettingLowReady && !isResettingShortStock
                       {
                           CanDoMeleeDetection = false;
                           CanResetDamping = false;
                           IsResettingMelee = true;
                           StanceRotationSpeed = 10f * stanceMulti * dt;
                           StanceTargetRotation = Quaternion.identity;
                           StanceBlender.Speed = 15f * stanceMulti * (useThirdPersonStance ? PluginConfig.ThirdPersonPositionSpeed.Value : 1f);
                       }
                       else if (StanceBlender.Value == 0f && !HasResetMelee)
                       {
                           _doMeleeReset = true;
                           if (!CanResetDamping)
                           {
                               _doDampingTimer = true;
                           }
                           StanceTargetRotation = Quaternion.identity;
                           IsResettingMelee = false;
                           HasResetMelee = true;
                           DidHalfMeleeAnim = false;
                       }*/
        }

        public void DoMeleeStanceBayonet(Player player, Player.FirearmController fc, bool isThirdPerson, EFT.Animations.ProceduralWeaponAnimation pwa, float dt, bool useThirdPersonStance, float stanceMulti, float resetErgoMulti, bool pauseStance, float movementFactor)
        {
            /*           bool isDoingMelee = TargetStance == EStance.Melee && !pwa.IsAiming && !pauseStance;
                       _isHoldingBackMelee = Input.GetKey(PluginConfig.MeleeKeybind.Value.MainKey) && !MeleeHitSomething && isDoingMelee;

                       Quaternion meleeInitialQuaternion = Quaternion.Euler(new Vector3(2.5f * resetErgoMulti, -15f * resetErgoMulti, -1f));
                       Quaternion meleeFinalQuaternion = Quaternion.Euler(new Vector3(-1.5f * resetErgoMulti, -7.5f * resetErgoMulti, -0.5f));
                       Vector3 meleeInitialPos = new Vector3(0f, 0.06f, 0f);
                       Vector3 meleeFinalPos = new Vector3(0f, -0.0275f, 0f);

                       if (isDoingMelee)
                       {
                           IsResettingMelee = false;
                           HasResetMelee = false;
                           HasResetActiveAim = true;
                           HasResetHighReady = true;
                           HasResetLowReady = true;
                           HasResetShortStock = true;

                           if (StanceCurrentPosition == meleeFinalPos && StanceBlender.Value >= 1f && !CanResetDamping)
                           {
                               _doDampingTimer = true;
                           }
                           else if (StanceCurrentPosition != meleeFinalPos || StanceBlender.Value < 1)
                           {
                               CanResetDamping = false;
                           }

                           StanceRotationSpeed = 10f * Mathf.Clamp(stanceMulti, 0.8f, 1f) * dt * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value : 1f);

                           float initialPosDistance = Vector3.Distance(StanceCurrentPosition, meleeInitialPos);
                           float finalPosDistance = Vector3.Distance(StanceCurrentPosition, meleeFinalPos);

                           if ((initialPosDistance > 0.001f && !DidHalfMeleeAnim))
                           {
                               StanceTargetRotation = meleeInitialQuaternion;
                               StanceCurrentPosition = Vector3.Lerp(StanceCurrentPosition, meleeInitialPos, PluginConfig.StanceTransitionSpeedMulti.Value * Mathf.Clamp(stanceMulti, 0.75f, 1f) * dt * 1.5f * ChonkerFactorStanceRotationModifier);
                           }
                           else
                           {
                               DidHalfMeleeAnim = true;
                               if (!_isHoldingBackMelee)
                               {
                                   StanceTargetRotation = meleeFinalQuaternion;
                                   StanceCurrentPosition = Vector3.Lerp(StanceCurrentPosition, meleeFinalPos, PluginConfig.StanceTransitionSpeedMulti.Value * Mathf.Clamp(stanceMulti, 0.75f, 1f) * dt * 2f * ChonkerFactorStanceRotationModifier);
                               }
                           }

                           StanceBlender.Speed = 50f * (useThirdPersonStance ? PluginConfig.ThirdPersonPositionSpeed.Value : 1f);

                           //TODO: GATE THIS, OTHERWISE IT'LL RUN MULTIPLE TIMES
                           if (StanceBlender.Value >= 0.9f && !MeleeHitSomething && !_isHoldingBackMelee) // && finalPosDistance <= 0.001f && !DidStanceWiggle
                           {
                               DoMeleeEffect();
                 *//*              DoWiggleEffects(player, pwa, fc.Weapon, new Vector3(-20f, -10f, -90f) * movementFactor, true, 1f, useGearSound: true);
                               DidStanceWiggle = true;*//*
                           }

                           if (StanceBlender.Value >= 0.9f && DidHalfMeleeAnim)
                           {
                               CanDoMeleeDetection = true;
                           }

                           if (StanceBlender.Value >= 1f && finalPosDistance <= 0.001f)
                           {
                               TargetStance = StoredStance;
                               StanceBlender.Target = 0f;
                           }
                       }
                       else if (StanceBlender.Value > 0f && !HasResetMelee) 
                       {
                           CanDoMeleeDetection = false;
                           CanResetDamping = false;
                           IsResettingMelee = true;
                           StanceRotationSpeed = 10f * stanceMulti * dt;
                           StanceTargetRotation = Quaternion.identity;
                           StanceBlender.Speed = 15f * stanceMulti * (useThirdPersonStance ? PluginConfig.ThirdPersonPositionSpeed.Value : 1f);
                       }
                       else if (StanceBlender.Value == 0f && !HasResetMelee)
                       {
                           _doMeleeReset = true;
                           if (!CanResetDamping)
                           {
                               _doDampingTimer = true;
                           }
                           StanceTargetRotation = Quaternion.identity;
                           IsResettingMelee = false;
                           HasResetMelee = true;
                           DidHalfMeleeAnim = false;
                       }*/
        }
    }
}

