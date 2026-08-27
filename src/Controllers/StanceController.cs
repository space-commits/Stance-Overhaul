using Comfort.Common;
using EFT;
using EFT.Animations;
using HarmonyLib;
using RealismCommonLib.Events;
using RealismCommonLib.Utils;
using StanceOverhaul.Controllers.PatchHooks;
using StanceOverhaul.SubSystem;
using StanceOverhaul.Enums;
using StanceOverhaul.SubSystem.Aiming;
using StanceOverhaul.SubSystem.StanceInput;
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
        public const float LEFT_SHOULDER_SWAY_MULTI = 1.3f;
  

        private EStanceType _targetStance = EStanceType.None;

        /// <summary>
        /// The base offset for weapon root position + weapon-specific base offsets if present. Used as the baseline for weapon offsets.
        /// </summary>
        public Vector3 BaseWeaponOffsetPosition = Vector3.zero;

        /// <summary>
        /// Sum total of all offsets not related to stances, including BaseWeaponOffsetPosition
        /// </summary>
        public Vector3 CurrentOffsetPosition = Vector3.zero;

        /// <summary>
        /// Sum total of all offsets not related to stances
        /// </summary>
        public Vector3 CurrentOffsetRotation = Vector3.zero;

        //TODO move to collision controller
        public bool WasAimingBeforeCollision = false;
        public bool StopCameraMovement = false;
        public float CameraMovmentForCollisionSpeed = 0.01f;
        public bool IsColliding = false;
        public bool PistolIsColliding = false;

        private static FieldInfo _pwaAimField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_aimingSpeed");

        //TODO: move to aim controller
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
        //TODO: move to stance health controller
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

        //TODO: move to a stance health controller
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
        //TODO: move to a stance health controller
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
        public Vector3? GetWeaponOffset(string weaponId)
        {
            if (_baseWeaponOffsets.TryGetValue(weaponId, out var offset))
            {
                return offset;
            }
            return null;
        }

        private Dictionary<string, Vector3> _baseWeaponOffsets = new Dictionary<string, Vector3>
        {
            {"5aafa857e5b5b00018480968", new Vector3(0f, 0f, -0.1f)}, //m1a
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

        private List<ISubSystem> _stateControllers = new List<ISubSystem>();
        private InputHookPipeline _inputHookPipeline;
        private StanceInputHandler _inputHandler;
        private StanceInputListener _inputListener;
        private StanceStaminaSystem _staminaSystem;
        private StanceMovementSystem _movementSystem;
        private TacSprintSystem _tacSprintSystem;
        private StatsSystem _statsSystem;
        private StanceAimSystem _aimSystem;
        private StanceState _stanceState;
        private StanceAudioSystem _stanceAudioSystem;
        private WeaponOffsetSystem _weaponOffsetSystem;
        private StanceReloadSpeedSystem _reloadSpeedSystem;

        public Spring StancePositionSpring { get; private set; }
        public Spring StanceRotationSpring { get; private set; }

        private List<StanceBase> _stances = new List<StanceBase>();
        public PatrolStance PatrolStance { get; private set; }
        public LeftShoulder LeftShoulder { get; private set; }
        public LowReady LowReady { get; private set; }
        public HighReady HighReady { get; private set; }
        public ActiveAim ActiveAim { get; private set; }
        public ShortStock ShortStock { get; private set; }
        public PistolCompress PistolCompress { get; private set; }
        public bool AwakeRan { get; private set; } = false;

        public EStanceReloadType CurrentReloadType => _reloadSpeedSystem.CurrentReloadType;
        public EStanceType CurrentStanceType => _stanceState.ActiveStanceType;
        public IStance? CurrentStance => _stanceState.ActiveStance;
        public StatsSystem StatsHandlerInstance => _statsSystem;
        public bool IsDoingTacSprint => _tacSprintSystem.IsDoingTacSprint;

        public float StanceHipfireBonus => _stanceState.ActiveStance?.HipfireBonus ?? 1f;

        void Awake()
        {
            Plugin.StanceControllerInstance = this;

            InitSprings();
            InitStateControllers();
            SubscribeToInputEvents();
            InitStances();

            AwakeRan = true;
        }

        void Update()
        {
            float regularTime = Time.deltaTime;

            if (!CanDoUpdate()) return;

            RunUpdates(regularTime);
        }

        void OnDestroy()
        {
            RunStanceDispose();
            RunControllerOnDestroy();
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

            ShortStock =
                InitStance(() => new ShortStock());

            PistolCompress =
                InitStance(() => new PistolCompress());

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

            _statsSystem =
               InitStateController(() => new StatsSystem());

            _staminaSystem =
                InitStateController(() => new StanceStaminaSystem());

            _movementSystem =
                InitStateController(() => new StanceMovementSystem());

            _tacSprintSystem =
                InitStateController(() => new TacSprintSystem());

            _aimSystem =
                InitStateController(() => new StanceAimSystem());

            _inputHandler =
                InitStateController(() => new StanceInputHandler(_stanceState));

            _inputListener =
                InitStateController(() => new StanceInputListener());

            _stanceAudioSystem =
                InitStateController(() => new StanceAudioSystem());

            _weaponOffsetSystem =
                InitStateController(() => new WeaponOffsetSystem());

            _reloadSpeedSystem =
                InitStateController(() => new StanceReloadSpeedSystem());

            RunControllerAwake();
        }

        private T InitStateController<T>(Func<T> factory) where T : SubSystem.ISubSystem
        {
            var instance = factory();
            _stateControllers.Add(instance);
            return instance;
        }

        private void RunControllerAwake()
        {
            foreach (SubSystem.ISubSystem controller in _stateControllers)
            {
                controller.RunOnAwake();
            }
        }

        private void RunUpdates(float deltaTime)
        {
            foreach (SubSystem.ISubSystem controller in _stateControllers)
            {
                controller.RunOnUpdate(deltaTime);
            }
        }

        private void RunControllerOnDestroy()
        {
            foreach (SubSystem.ISubSystem controller in _stateControllers)
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

        private void SubscribeToInputEvents()
        {
            PlayerEvents.OnWeaponEquipped -= OnWeaponSwap;
            InputEvents.WeaponSwapInput += OnWeaponSwap;
            InputEvents.ToggleStepOutInput += OnToggleStepOut;
            InputEvents.ChangeStanceInput += OnChangeStance;
            InputEvents.ToggleBipodInput += OnToggleBipod;
            InputEvents.ToggleBreathingInput += OnToggleBreathing;
        }

        private void UnsubscribeFromInputEvents()
        {
            PlayerEvents.OnWeaponEquipped -= OnWeaponSwap;
            InputEvents.WeaponSwapInput -= OnWeaponSwap;
            InputEvents.ToggleStepOutInput -= OnToggleStepOut;
            InputEvents.ChangeStanceInput -= OnChangeStance;
            InputEvents.ToggleBipodInput -= OnToggleBipod;
            InputEvents.ToggleBreathingInput -= OnToggleBreathing;
        }

        //TODO: check if GameStateInstance.WeaponIsReady is essentially the same thing,
        //would make check in StanceStateUpdate redundant
        private bool CanDoUpdate()
        {
            Player player = PlayerStateInstance.Player;
            return player != null && GameStateInstance.PlayerIsInRaidOrHideout;
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

        //TODO: move to wiggle effects class
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

        private bool IsUsingCantedSight(ProceduralWeaponAnimation pwa, bool checkifAiming)
        {
            bool isCanted = Mathf.Abs(pwa.CurrentScope.Rotation) >= EFTHardSettings.Instance.SCOPE_ROTATION_THRESHOLD;
            bool isAiming = !checkifAiming || AimStateInstance.IsAiming;
            return isCanted && isAiming;
        }
    }
}

