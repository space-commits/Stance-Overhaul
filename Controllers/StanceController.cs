using Comfort.Common;
using EFT;
using EFT.Animations;
using EFT.Animations.NewRecoil;
using EFT.InventoryLogic;
using RealismCommonLib.Utils;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EFT.Player;
using static RealismCommonLib.Plugin;
using System.Reflection;
using RealismCommonLib.Events;

namespace StanceOverhaul
{
    public enum EBracingDirection 
    {
        Top,
        Left, 
        Right,
        None
    }

    public enum EStance
    {
        None,
        LowReady,
        HighReady,
        ShortStock,
        ActiveAiming,
        PatrolStance,
        Melee,
        PistolCompressed
    }

    public class StanceController: MonoBehaviour
    {
        public const float STANCE_WEIGHT_LIMIT_KG = 8f;
        public const float CHONKER_MODIFIER = 0.7f;
        public const float TAC_SPRINT_WEIGHT_LIMIT = 5.1f;
        public const float TAC_SPRINT_WEIGHT_BULLPUP = 5.75f;
        public const int TAC_SPRINT_LENGTH_LIMIT = 6;
        public const float TAC_SPRINT_ERGO_LIMIT = 35f;
        public const float TAC_SPRINT_RADIATION_LIMIT = 50f;
        public const float TAC_SPRINT_TOXICITY_LIMIT = 50f;
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

        private FieldInfo _aimSpeedField;
        private FieldInfo _compensatoryField;
        private FieldInfo _displacementStrField;
        private FieldInfo _scopeRotationField;
        private FieldInfo _weapTempRotationField;
        private FieldInfo _weapTempPositionField;
        private FieldInfo _isAimingField;
        private FieldInfo _vCameraTargetField;

        public Quaternion CurrentRotation = Quaternion.identity;
        public Quaternion StanceRotation = Quaternion.identity;
        public Vector3 MountWeapPosition = Vector3.zero;
        public Vector3 CurrentVisualRecoil = Vector3.zero;
        public Vector3 TargetVisualRecoil = Vector3.zero;

        public bool HasResetActiveAim = true;
        public bool HasResetLowReady = true;
        public bool HasResetHighReady = true;
        public bool HasResetShortStock = true;
        public bool HasResetPistolPos = true;
        public bool HasResetMelee = true;

        public bool IsResettingActiveAim = false;
        public bool IsResettingLowReady = false;
        public bool IsResettingHighReady = false;
        public bool IsResettingShortStock = false;
        public bool IsResettingPistol = false;
        public bool IsResettingMelee = false;
        public bool DidHalfMeleeAnim = false;

        public float StanceRotationSpeed = 1f;

        public bool HaveSetAiming = false;
        public bool HaveSetActiveAim = false;

        public bool _isLeftStanceResetState = false;
        private float _leftStanceTime = 0f;
        private Vector3 _leftStanceRotaiton;

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

        private Vector3 _leftStancePistolRotaitonTarget = new Vector3(0f, -10f, 0f);
        private Vector3 _leftStancePistolPositionTarget = new Vector3(0f, -0.02f, 0f);
        private Vector3 _leftStanceRifleRotaitonTarget = new Vector3(0f, -10f, 0f);
        private Vector3 _leftStanceRiflePositionTarget = new Vector3(0f, 0f, 0f);
        private Vector3 _leftStancePosition = Vector3.zero;
        private Vector3 _leftStanceVelocity = Vector3.zero;
        private float _leftStanceProgress = 0f;
        private float _leftStanceTargetX;

        private AnimationCurve _leftRotationXCurve = new AnimationCurve(
            new Keyframe(0, 0f),
            new Keyframe(0.25f, -2f),
            new Keyframe(0.5f, -5f),
            new Keyframe(0.75f, -1.5f),
            new Keyframe(1, 0f)
        );

        private AnimationCurve _leffPosZCurve = new AnimationCurve(
            new Keyframe(0, 0f),
            new Keyframe(0.15f, 0.1f),
            new Keyframe(0.3f, 0.075f),
            new Keyframe(0.5f, 0.1f),
            new Keyframe(0.65f, 0.05f),
            new Keyframe(0.7f, 0.025f),
            new Keyframe(0.9f, -0.045f),
            new Keyframe(1, 0f)
        );

        private AnimationCurve _leffPosZCurveReturn = new AnimationCurve(
            new Keyframe(0, 0f),
            new Keyframe(0.15f, -0.05f),
            new Keyframe(0.3f, 0.025f),
            new Keyframe(0.5f, 0.05f),
            new Keyframe(0.65f, 0.075f),
            new Keyframe(0.7f, 0.05f),
            new Keyframe(0.9f, 0.1f),
            new Keyframe(1, 0f)
            );

        public Vector3 CoverWiggleDirection = Vector3.zero;
        public Vector3 BaseWeaponOffsetPosition = Vector3.zero;
        public Vector3 StanceTargetPosition = Vector3.zero;
        private Vector3 _pistolLocalPosition = Vector3.zero;
        private Vector3 _rifleLocalPosition = Vector3.zero;

        private const float _clickDelay = 0.2f;
        private float _doubleClickTime = 0f;
        private bool _clickTriggered = true;
        public int StanceIndex = 0;

        public bool MeleeIsToggleable = true;
        public bool CanDoMeleeDetection = false;
        public bool MeleeHitSomething = false;
        private float _meleeTimer = 0.0f;
        private bool _isHoldingBackMelee = false;

        private float _manipTime = 0.0f;
        public float ManipTimer = 0.25f;

        private float _dampingTimer = 0.0f;
        private bool _didAimWiggle = false;
        private bool _doDampingTimer = false;
        private bool _canResetDamping = true;

        public bool WasAimingBeforeCollision = false;
        public bool StopCameraMovement = false;
        public float CameraMovmentForCollisionSpeed = 0.01f;
        public bool IsColliding = false;

        public float HighReadyBlackedArmTime = 0.0f;
        public bool CanDoHighReadyInjuredAnim = false;

        public bool CancelPistolStance = false;
        public bool PistolIsColliding = false;
        public bool CancelHighReady = false;
        public bool ModifyHighReady = false;
        public bool CancelLowReady = false;
        public bool CancelShortStock = false;
        public bool CancelActiveAim = false;
        public bool ShouldResetStanceCancels = false;
        private bool _doMeleeReset = false;

        private EStance _lastRecordedStanceStamina = EStance.None; //used for stamina drate rate updates
        private EStance _previousStance = EStance.None;
        private EStance _currentStance = EStance.None;
        private EStance _storedStance = EStance.None;
        public bool FinishedUnPatrolStancing = false;
        private bool _SkipPistolWiggle = false;
        public bool WasActiveAim = false;

        private bool _isLeftShoulder = false;
        public bool CancelLeftShoulder = false;
        public bool HaveResetLeftShoulder = false;
        public bool IsDoingTacSprint = false;

        public bool IsInForcedLowReady = false;
        public bool IsAiming = false;
        public bool DidWeaponSwap = false;
        public bool IsBlindFiring = false;
        public bool IsInThirdPerson = false;
        public bool ToggledLight = false;
        public bool DidStanceWiggle = false;
        public bool DidLowReadyResetStanceWiggle = false;
        public float WiggleReturnSpeed = 1f;

        //arm stamina
        private bool _regenStam = false;
        private bool _drainStamStam = false;
        private bool _neutralStam = false;
        private bool _wasBracingStam = false;
        private bool _wasMountingStam = false;
        private bool _wasAimingStam = false;
        public bool HaveResetStamDrain = false;
        public bool CanResetAimDrain = false;

        //extra rotaitons
        private Vector3 _posePosOffest = Vector3.zero;
        private Vector3 _poseRotOffest = Vector3.zero;
        private Vector3 _patrolPos = Vector3.zero;
        private Vector3 _patrolRot = Vector3.zero;

        //patrol
        private Vector3 _riflePatrolPos = new Vector3(0.2f, 0.025f, 0.1f);
        private Vector3 _riflePatrolRot = new Vector3(0.05f, -0.05f, -0.5f);
        private Vector3 _pistolPatrolPos = new Vector3(0.05f, 0f, 0f);
        private Vector3 _pistolPatrolRot = new Vector3(0.1f, -0.1f, -0.1f);

        //tac sprint
        private float _tacSprintTime = 0.0f;
        private bool _canDoTacSprintTimer = false;

        //mounting
        private Quaternion _makeQuaternionDelta(Quaternion from, Quaternion to) => to * Quaternion.Inverse(from); //yeah I don't know what this is either
        private float _mountAimSmoothed = 0f;
        public float _cumulativeMountPitch = 0f;
        public float _cumulativeMountYaw = 0f;
        static Vector2 _lastMountYawPitch;
        public EBracingDirection BracingDirection = EBracingDirection.None;
        public bool IsBracing = false;
        public bool _isRealismMounting = false;
        public float BracingSwayBonus = 1f;
        public float BracingRecoilBonus = 1f;

        //this sucks, don't use in update
        //move bot stances to a component on the bot gameobject later
        public WildSpawnType[] _botsToUseTacticalStances = { WildSpawnType.bossKolontay, WildSpawnType.pmcBEAR, WildSpawnType.pmcUSEC, WildSpawnType.exUsec, WildSpawnType.pmcBot, WildSpawnType.bossKnight, WildSpawnType.followerBigPipe, WildSpawnType.followerBirdEye, WildSpawnType.bossGluhar, WildSpawnType.followerGluharAssault, WildSpawnType.followerGluharScout, WildSpawnType.followerGluharSecurity, WildSpawnType.followerGluharSnipe };

        public Player.BetterValueBlender StanceBlender = new Player.BetterValueBlender
        {
            Speed = 5f,
            Target = 0f
        };

        public Vector3 MountPos { get; set; }
        public Vector3 MountDir { get; set; }

        public float BaseWeaponLength { get; set; }
        public float StanceModifiedWeaponLength { get; set; }

        public float StanceSwayFactor
        {
            get
            {
                return
                    IsLeftShoulder ? LEFT_SHOULDER_SWAY_MULTI : 1f;
            }
        }

        public float StanceADSSpeedMulti
        {
            get
            {
                return
                    IsIdle() && !IsLeftShoulder ? IDLE_ADS_MULTI :
                    WasActiveAim || CurrentStance == EStance.ActiveAiming ? ACTIVE_AIM_ADS_MULTI :
                    CurrentStance == EStance.HighReady || CurrentStance == EStance.HighReady ? HIGH_ADS_MULTI :
                    StoredStance == EStance.LowReady || CurrentStance == EStance.LowReady ? LOW_ADS_MULTI :
                    StoredStance == EStance.ShortStock || CurrentStance == EStance.ShortStock ? SHORT_STOCK_ADS_MULTI :
                    StoredStance == EStance.PatrolStance || CurrentStance == EStance.PatrolStance ? PATROL_ADS_MULTI :
                    IsLeftShoulder ? LEFT_SHOULDER_ADS_MULTI : 1f;
            }
        }

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

        public bool AllStancesReset
        {
            get
            {
                return HasResetActiveAim && HasResetLowReady && HasResetHighReady && HasResetShortStock && HasResetPistolPos && HaveResetLeftShoulder; //&& HasResetMelee
            }
        }

        public bool ShouldBlockAllStances
        {
            get
            {
                return (IsMounting && WeaponStateInstance.BipodIsDeployed) || !MeleeIsToggleable;
            }
        }

        public bool IsReadyForBayonetCharge
        {
            get
            {
                return (_isHoldingBackMelee);
            }
        }

        public bool TreatWeaponAsPistolStance
        {
            get
            {
                return WeaponStateInstance.TreatAsPistol;
            }
        }

        public bool FaceShieldBlocksADS
        {
            get
            {
                return PluginConfig.EnableFSPatch.Value && GearStateInstance.FaceShieldCouldBlockADS;
            }
        }

        public bool NVGdBlocksADS
        {
            get
            {

                return PluginConfig.EnableNVGPatch.Value && GearStateInstance.NVGsCouldBlockADS;
            }
        }

        public bool AdsIsBlocked
        {
            get
            {
                return NVGdBlocksADS || FaceShieldBlocksADS;
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

        public bool HealthConditionPreventsTacSprint
        {
            get
            {
                return
                    HazardsStateInstance.TotalToxicity > TAC_SPRINT_TOXICITY_LIMIT ||
                    HazardsStateInstance.TotalRadiation > TAC_SPRINT_RADIATION_LIMIT ||
                    HazardsStateInstance.IsCoughingInGas ||
                    HealthStateAffectsStance;
            }
        }

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

        //weight limit should be factored by strength skill
        public bool CanDoTacSprint
        {
            get
            {
                return PluginConfig.EnableTacSprint.Value && PlayerStateInstance.IsSprinting && CurrentStance != EStance.ActiveAiming
                && (CurrentStance == EStance.HighReady || StoredStance == EStance.HighReady) &&
                WeaponStateInstance.TotalWeaponWeight <= (WeaponStateInstance.IsBullpup ? TAC_SPRINT_WEIGHT_BULLPUP : TAC_SPRINT_WEIGHT_LIMIT)
                && WeaponStateInstance.WeaponLength <= TAC_SPRINT_LENGTH_LIMIT && !PlayerStateInstance.IsScav
                && !HealthConditionPreventsTacSprint && WeaponStateInstance.TotalErgo > TAC_SPRINT_ERGO_LIMIT;
            }
        }

        public bool ShouldForceLowReady
        {
            get
            {
                return (HealthConditionForcesLowReady || (WeaponStateInstance.TotalWeaponWeight >= 10f && !IsMounting))
                    && !IsAiming && ! FiringStateInstance.IsFiringFromStance && CurrentStance != EStance.PistolCompressed
                    && CurrentStance != EStance.PatrolStance && CurrentStance != EStance.ShortStock
                    && CurrentStance != EStance.ActiveAiming && MeleeIsToggleable && !IsBracing;
            }
        }

        public float HighReadyManipBuff
        {
            get
            {
                return CurrentStance == EStance.HighReady ? 1.18f : 1f;
            }
        }
        public float ActiveAimManipBuff
        {
            get
            {
                return CurrentStance == EStance.ActiveAiming && PluginConfig.ActiveAimReload.Value ? 1.15f : 1f;
            }
        }
        public float LowReadyManipBuff
        {
            get
            {
                return CurrentStance == EStance.LowReady ? 1.21f : 1f;
            }
        }

        public EStance StoredStance
        {
            get { return _storedStance; }
            set { _storedStance = value; }
        }

        public EStance CurrentStance
        {
            get { return _currentStance; }
            set
            {
                if (value != _currentStance)
                {
                    _currentStance = value;
                    if (!IsAiming) PlayerStateInstance.ResetProceduralState();
                }
            }
        }

        public bool IsLeftStanceResetState 
        {
            get { return _isLeftStanceResetState; }
            private set  { _isLeftStanceResetState = value; }
        }

        public bool IsLeftShoulder
        {
            get { return _isLeftShoulder; }
            set
            {
                if (value != _isLeftShoulder)
                {
                    _isLeftShoulder = value;
                    PlayerStateInstance.ResetProceduralState(); 
                }
            }
        }

        public bool IsDoingLeftShoulderNotBlocked
        {
            get 
            {
                return IsLeftShoulder && !IsBlindFiring && !CancelLeftShoulder; 
            }
        }

        public bool IsMounting
        {
            get
            {
                return _isRealismMounting;
            }
            set
            {
                if (value != _isRealismMounting)
                {
                    Player player = PlayerStateInstance.Player;
                    FirearmController fc = player.HandsController as FirearmController;
                    if (fc == null)
                    {
                        value = false;
                        return;
                    }
                    _isRealismMounting = value;
                    if (player.ProceduralWeaponAnimation != null) player.ProceduralWeaponAnimation.method_23();
                    float accuracy = fc.Item.GetTotalCenterOfImpact(false); //forces accuracy to update
                    AccessTools.Field(typeof(Player.FirearmController), "float_3").SetValue(fc, accuracy); //update weapon accuracy
                    player.ProceduralWeaponAnimation.UpdateTacticalReload(); //gives better chamber animations
                    //this causes camera to detatch from weapon, breaks pretty badly
                    //it's needed to enable animation change (player grip changes), maybe there is a check for this anim state that caused the issue that can be disabled
                    //player.MovementContext.PlayerAnimator.SetProneBipodMount(player.MovementContext.IsInPronePose && WeaponStateInstance.BipodIsDeployed && value);
                    fc.FirearmsAnimator.SetMounted(value);
                    //player.ProceduralWeaponAnimation.SetMountingData(value, BracingDirection != EBracingDirection.Top);
                }
            }
        }

        void Awake()
        {
            AssignFieldRefs();
            SubscribeToEvents();
        }

        void Update()
        {
            if (!DoUpdate()) return;
            StanceStateUpdate();
            ProceduralAnimationsUpdate();
        }

        void onDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            ReloadEvents.WeaponStateReset += OnWeaponStateReset;
            ReloadEvents.CheckAmmo += OnCheckAmmo;
            ReloadEvents.ChamberCheck += OnCheckChamber;
            ReloadEvents.Rechamber += OnRechamber;
            ReloadEvents.MagReload += OnMagReload;
            ReloadEvents.QuickMagReload += OnMagReload;
        }

        private void UnsubscribeFromEvents()
        {
            ReloadEvents.WeaponStateReset -= OnWeaponStateReset;
            ReloadEvents.CheckAmmo -= OnCheckAmmo;
            ReloadEvents.ChamberCheck -= OnCheckChamber;
            ReloadEvents.Rechamber -= OnRechamber;
            ReloadEvents.MagReload -= OnMagReload;
            ReloadEvents.QuickMagReload -= OnMagReload;
        }

        private void AssignFieldRefs() 
        {
            _aimSpeedField = AccessTools.Field(typeof(EFT.Animations.ProceduralWeaponAnimation), "_aimingSpeed");
            _compensatoryField = AccessTools.Field(typeof(EFT.Animations.ProceduralWeaponAnimation), "_compensatoryScale");
            _displacementStrField = AccessTools.Field(typeof(EFT.Animations.ProceduralWeaponAnimation), "_displacementStr");
            _scopeRotationField = AccessTools.Field(typeof(EFT.Animations.ProceduralWeaponAnimation), "_targetScopeRotation");
            _weapTempPositionField = AccessTools.Field(typeof(EFT.Animations.ProceduralWeaponAnimation), "_temporaryPosition");
            _weapTempRotationField = AccessTools.Field(typeof(EFT.Animations.ProceduralWeaponAnimation), "_temporaryRotation");
            _isAimingField = AccessTools.Field(typeof(EFT.Animations.ProceduralWeaponAnimation), "_isAiming");
            _vCameraTargetField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_vCameraTarget");
        }

        public bool DoUpdate() 
        {
            Player player = PlayerStateInstance.Player;
            if (player != null && player.IsYourPlayer && PlayerStateInstance.FirearmController != null)
            {
                return true;
            }
            return false;
        }

        public void ProceduralAnimationsUpdate()
        {
            Player player = PlayerStateInstance.Player;
            if (player != null && player.IsYourPlayer && player.MovementContext.CurrentState.Name != EPlayerState.Stationary)
            {
                FirearmController fc = PlayerStateInstance.FirearmController;
                ProceduralWeaponAnimation pwa = player.ProceduralWeaponAnimation;

                IsInThirdPerson = false;

                float aimSpeed = (float)_aimSpeedField.GetValue(pwa);
                float compensatoryScale = (float)_compensatoryField.GetValue(pwa);
                float displacementStr = (float)_displacementStrField.GetValue(pwa);
                Quaternion scopeRotation = (Quaternion)_scopeRotationField.GetValue(pwa);
                Vector3 weapTempPosition = (Vector3)_weapTempPositionField.GetValue(pwa);
                Quaternion weapTempRotation = (Quaternion)_weapTempRotationField.GetValue(pwa);
                bool isAiming = (bool)_isAimingField.GetValue(pwa);
                Vector3 _vCameraTarget = (Vector3)_vCameraTargetField.GetValue(pwa);

                Vector3 handsRotation = pwa.HandsContainer.HandsRotation.Get();
                Vector3 sway = pwa.HandsContainer.SwaySpring.Value;
                handsRotation += displacementStr * (isAiming ? pwa.AimingDisplacementStr : 1f) * new Vector3(sway.x, 0f, sway.z);
                handsRotation += sway;
                Vector3 rotationCenter = pwa._shouldMoveWeaponCloser ? pwa.HandsContainer.RotationCenterWoStock : pwa.HandsContainer.RotationCenter;
                Vector3 weapRootPivot = pwa.HandsContainer.WeaponRootAnim.TransformPoint(rotationCenter);

                bool isInStance =
                    CurrentStance == EStance.HighReady ||
                    CurrentStance == EStance.LowReady ||
                    CurrentStance == EStance.ShortStock ||
                    CurrentStance == EStance.ActiveAiming ||
                    CurrentStance == EStance.Melee ||
                    IsLeftShoulder;
                bool isInShootableStance =
                    CurrentStance == EStance.ShortStock ||
                    CurrentStance == EStance.ActiveAiming ||
                    TreatWeaponAsPistolStance ||
                    CurrentStance == EStance.Melee;
                bool cancelBecauseShooting = PluginConfig.RememberStanceFiring.Value && !isAiming && FiringStateInstance.IsFiringFromStance && !isInShootableStance;
                bool doStanceRotation = (isInStance || !AllStancesReset || CurrentStance == EStance.PistolCompressed) && !cancelBecauseShooting;
                bool allowActiveAimReload = PluginConfig.ActiveAimReload.Value && ReloadStateInstance.ReloadAnimationSupportsActiveAim;
                bool cancelStance =
                    (CancelActiveAim && CurrentStance == EStance.ActiveAiming && !allowActiveAimReload) ||
                    (CancelHighReady && CurrentStance == EStance.HighReady) ||
                    (CancelLowReady && CurrentStance == EStance.LowReady) ||
                    (CancelShortStock && CurrentStance == EStance.ShortStock); // || (CancelPistolStance && PistolIsCompressed)

                float rotationTime = 
                    doStanceRotation ? StanceRotationSpeed * PluginConfig.StanceRotationSpeedMulti.Value :
                    pwa.IsAiming ? 7f * aimSpeed * Time.deltaTime : 
                    8f * Time.deltaTime; //__instance.IsAiming ? 8f * aimSpeed * dt

                CurrentRotation = Quaternion.Slerp(
                    CurrentRotation, 
                    pwa.IsAiming && AllStancesReset ? Quaternion.identity : doStanceRotation ? StanceRotation : Quaternion.identity,
                    rotationTime); 

                pwa.HandsContainer.WeaponRootAnim.SetPositionAndRotation(weapTempPosition, weapTempRotation * CurrentRotation);

                if (TreatWeaponAsPistolStance)//&& CurrentStance != EStance.PatrolStance
                {
                    if (CurrentStance == EStance.PistolCompressed && !IsAiming && !IsResettingPistol && !IsBlindFiring) //&& !__instance.LeftStance
                    {
                        StanceBlender.Target = 1f;
                    }
                    else
                    {
                        StanceBlender.Target = 0f;
                    }

                    if ((CurrentStance != EStance.PistolCompressed && !IsAiming && !IsResettingPistol) || IsBlindFiring) // || __instance.LeftStance
                    {
                        StanceTargetPosition = Vector3.Lerp(StanceTargetPosition, Vector3.zero, 5f * Time.deltaTime);
                    }

                    HasResetActiveAim = true;
                    HasResetHighReady = true;
                    HasResetLowReady = true;
                    HasResetShortStock = true;
                    HasResetMelee = true;
                    DoPistolStances(false, pwa, Time.deltaTime, player, fc, _vCameraTarget);
                }
                else if (!TreatWeaponAsPistolStance || WeaponStateInstance.HasShoulderContact)
                {
                    if ((!isInStance && AllStancesReset) || (cancelBecauseShooting && !isInShootableStance) || IsAiming || cancelStance || IsBlindFiring || IsLeftShoulder)
                    {
                        StanceBlender.Target = 0f;
                    }
                    else if (isInStance)
                    {
                        StanceBlender.Target = 1f;
                    }

                    if (((!isInStance && AllStancesReset) && !cancelBecauseShooting && !IsAiming) || IsBlindFiring || IsLeftShoulder)
                    {
                        StanceTargetPosition = Vector3.Lerp(StanceTargetPosition, Vector3.zero, 5f * Time.deltaTime);
                    }

                    HasResetPistolPos = true;
                    DoRifleStances(player, fc, false, pwa, Time.deltaTime, _vCameraTarget);
                }

                if (PluginConfig.EnableExtraProcEffects.Value) DoExtraPosAndRot(pwa, player);
                DoPatrolStance(pwa, player);
            }
        }

        public void ReloadUpdate()
        {
            if (ReloadStateInstance.IsInReloadOpertation)
            {
                if (CurrentStance == EStance.PatrolStance)
                {
                    CurrentStance = EStance.None;
                }

                ModifyHighReady = true;
                CancelShortStock = true;
                CancelActiveAim = true;
                CancelLeftShoulder = true;

                if (ReloadStateInstance.IsAttemptingToReloadInternalMag)
                {
                    bool isShotgun = WeaponStateInstance.IsShotgun;
                    CancelHighReady = !isShotgun ? true : false;
                    CancelLowReady = isShotgun || WeaponStateInstance.TreatAsPistol ? true : false;
                }
            }
        }

        public void OnMagReload() 
        {
            CancelLowReady = true;
            CancelLeftShoulder = true;
        }

        public void OnWeaponStateReset()
        {
            ShouldResetStanceCancels = true;
        }

        public void OnRechamber()
        {
            CancelShortStock = true;
            CancelLeftShoulder = true;
        }

        public void OnCheckChamber() 
        {
            CancelLowReady = true;
            CancelHighReady = true;
            CancelShortStock = true;
            CancelLeftShoulder = true;
        }

        public void OnCheckAmmo()
        {
            CancelLeftShoulder = true;
            CancelLowReady = true;
            CancelShortStock = true;
            CancelActiveAim = true;
            ModifyHighReady = true;
            ManipTimer = 0f;
        }

        public bool IsCantedAiming(ProceduralWeaponAnimation pwa, bool checkifAiming)
        {
            bool isCanted = Mathf.Abs(pwa.CurrentScope.Rotation) >= EFTHardSettings.Instance.SCOPE_ROTATION_THRESHOLD;
            bool isAimingOk = !checkifAiming || IsAiming;
            return isCanted && isAimingOk;
        }

        public bool AimingInterrupted { get; set; }

        public void InterruptAim(FirearmController fc)
        {
            if (fc.IsAiming && !AimingInterrupted)
            {
                fc.ToggleAim();
                AimingInterrupted = true;
            }
        }

        public void UnInterruptAim(FirearmController fc) 
        {
            if (!fc.IsAiming && AimingInterrupted)
            {
                fc.ToggleAim();
                AimingInterrupted = false;
            }
        }

        public float ChonkerFactorStanceRotationModifier 
        {
            get 
            {
                return IsChonker ? CHONKER_MODIFIER : 1f;
            }
        }

        public bool IsChonker
        {
            get
            {
                return WeaponStateInstance.TotalWeaponWeight >= STANCE_WEIGHT_LIMIT_KG;
            }
        }

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

        private float GetRestoreRate()
        {
            float baseRestoreRate = 0f;
            if (IsMounting && WeaponStateInstance.BipodIsDeployed)
            {
                baseRestoreRate = 5f;
            }
            if (CurrentStance == EStance.PatrolStance || IsMounting)
            {
                baseRestoreRate = 4f;
            }
            else if (CurrentStance == EStance.LowReady || CurrentStance == EStance.PistolCompressed || IsBracing)
            {
                baseRestoreRate = 2.4f;
            }
            else if (CurrentStance == EStance.HighReady)
            {
                baseRestoreRate = 1.85f;
            }
            else if (CurrentStance == EStance.ShortStock)
            {
                baseRestoreRate = 1.3f;
            }
            else if (IsIdle() && !PluginConfig.EnableIdleStamDrain.Value)
            {
                baseRestoreRate = 1f;
            }
            else
            {
                baseRestoreRate = 1f;
            }
            float formfactor = WeaponStateInstance.IsBullpup ? 1.05f : 1f;
            return (1f - ((WeaponStateInstance.ErgoFactor * formfactor) / 100f)) * baseRestoreRate * HealthStateInstance.HealthStamRegenFactor;
        }

        private float GetDrainRate(Player player)
        {
            float baseDrainRate = 0f;
            if (player.Physical.HoldingBreath)
            {
                baseDrainRate = IsMounting && WeaponStateInstance.BipodIsDeployed ? 0.025f : IsMounting ? 0.05f : IsBracing ? 0.1f : 0.5f;
            }
            else if (IsAiming)
            {
                baseDrainRate = 0.15f;
            }
            else if (IsDoingTacSprint)
            {
                baseDrainRate = 0.15f;
            }
            else if (CurrentStance == EStance.ActiveAiming)
            {
                baseDrainRate = 0.075f;
            }
            else
            {
                baseDrainRate = 0.1f;
            }
            float formfactor = WeaponStateInstance.IsBullpup ? 0.4f : 1f;
            return WeaponStateInstance.ErgoFactor * formfactor * baseDrainRate * ((1f - HealthStateInstance.HealthStamRegenFactor) + 1f) * (1f - (SkillStateInstance.StrengthSkillAimBuff)) * PluginConfig.IdleStamDrainModi.Value;
        }

        //this method makes baby Jesus cry
        public void SetStanceStamina(Player player)
        {
            bool isInRegenableStance = CurrentStance == EStance.HighReady || CurrentStance == EStance.LowReady || CurrentStance == EStance.PatrolStance || CurrentStance == EStance.ShortStock || (IsIdle() && !PluginConfig.EnableIdleStamDrain.Value);
            bool isInRegenableState = (!player.Physical.HoldingBreath && (IsMounting || IsBracing)) || player.IsInPronePose || CurrentStance == EStance.PistolCompressed || PlayerStateInstance.IsUsingStationaryWeapon;
            bool doRegen = ((isInRegenableStance && !IsAiming && !FiringStateInstance.IsFiringFromStance) || isInRegenableState) && !PlayerStateInstance.IsSprinting;
            bool shouldDoIdleDrain = IsIdle() && PluginConfig.EnableIdleStamDrain.Value;
            bool shouldInterruptRegen = isInRegenableStance && (IsAiming || FiringStateInstance.IsFiringFromStance);
            bool doNeutral = PlayerStateInstance.IsSprinting || player.IsInventoryOpened || (CurrentStance == EStance.ActiveAiming && player.Pose == EPlayerPose.Duck);
            bool doDrain = ((shouldInterruptRegen || !isInRegenableStance || shouldDoIdleDrain) && !isInRegenableState && !doNeutral) || (IsDoingTacSprint && PluginConfig.EnableIdleStamDrain.Value);
            EStance stance = CurrentStance;

            if (HaveResetStamDrain || DidWeaponSwap || IsAiming != _wasAimingStam || _regenStam != doRegen || _drainStamStam != doDrain || _neutralStam != doNeutral || _lastRecordedStanceStamina != CurrentStance || IsMounting != _wasMountingStam || IsBracing != _wasBracingStam)
            {
                if (doDrain)
                {
                    player.Physical.Aim(1f);
                }
                else if (doRegen)
                {
                    player.Physical.Aim(0f);
                }
                else if (doNeutral)
                {
                    player.Physical.Aim(1f);
                }
                HaveResetStamDrain = false;
            }

            //drain
            if (doDrain)
            {
                player.Physical.HandsStamina.Multiplier = GetDrainRate(player);
            }
            //regen
            else if (doRegen)
            {
                player.Physical.HandsStamina.Multiplier = GetRestoreRate();
            }
            //no drain or regen
            else if (doNeutral)
            {
                player.Physical.HandsStamina.Multiplier = 0f;
            }

            _regenStam = doRegen;
            _drainStamStam = doDrain;
            _neutralStam = doNeutral;
            _wasBracingStam = IsBracing;
            _wasMountingStam = IsMounting;
            _wasAimingStam = IsAiming;
            _lastRecordedStanceStamina = CurrentStance;
        }

        public void ResetStanceStamina() 
        {
            _regenStam = false;
            _drainStamStam = false;
            _neutralStam = false;
            _wasBracingStam = false;
            _wasMountingStam = false;
            _wasAimingStam = false;
            _lastRecordedStanceStamina = EStance.None;
        }

        public void UnarmedStanceStamina(Player player)
        {
            player.Physical.Aim(0f);
            player.Physical.HandsStamina.Multiplier = 1f;
            ResetStanceStamina();
        }

        public bool IsIdle()
        {
            return CurrentStance == EStance.None && StoredStance == EStance.None && HasResetActiveAim && HasResetHighReady && HasResetLowReady && HasResetShortStock && HasResetPistolPos && HasResetMelee ? true : false;
        }

        public void CancelAllStances()
        {
            StanceBlender.Target = 0f;
            CurrentStance = EStance.None;
            StoredStance = EStance.None;
            DidStanceWiggle = false;
            WasActiveAim = false;
            IsLeftShoulder = false;
        }

        //Should be replaced with coroutine or time gate
        //Timer value should use constant
        private void StanceManipCancelTimer()
        {
            _manipTime += Time.deltaTime;

            if (_manipTime >= ManipTimer)
            {
                CancelHighReady = false;
                ModifyHighReady = false;
                CancelLowReady = false;
                CancelShortStock = false;
                CancelPistolStance = false;
                CancelActiveAim = false;
                ShouldResetStanceCancels = false;
                CancelLeftShoulder = false;
                ManipTimer = 0.25f;
                _manipTime = 0f;
            }
        }

        //Replace with time gate or coroutine, ideally won't be needed after refactor
        private void StanceDampingTimer()
        {
            _dampingTimer += Time.deltaTime;

            if (_dampingTimer >= 0.01f) //0.05f
            {
                _canResetDamping = true;
                _doDampingTimer = false;
                _dampingTimer = 0f;
            }
        }

        //Replace with time gate or coroutine
        private void MeleeCooldownTimer()
        {
            _meleeTimer += Time.deltaTime;

            if (_meleeTimer >= 0.25f)
            {
                _doMeleeReset = false;
                MeleeIsToggleable = true;
                _meleeTimer = 0f;
            }
        }

        private void DoMeleeEffect()
        {
            Player player = Singleton<GameWorld>.Instance.MainPlayer;
            Player.FirearmController fc = player.HandsController as Player.FirearmController;
            if (WeaponStateInstance.HasBayonet)
            {
                AudioControllerInstance.PlayKnifeAttackSound(2);


            }
            player.Physical.ConsumeAsMelee(2f + (WeaponStateInstance.ErgoFactor / 100f));
        }

        private void ToggleStance(EStance targetStance, bool setPrevious = false, bool setPrevisousAsCurrent = false)
        {
            _previousStance = _currentStance;
            if (IsLeftShoulder) IsLeftShoulder = false;
            if (setPrevious) StoredStance = CurrentStance;
            if (CurrentStance == targetStance) CurrentStance = EStance.None;
            else CurrentStance = targetStance;
            if (setPrevisousAsCurrent) StoredStance = CurrentStance;
        }

        private void ToggleHighReady()
        {
            StanceBlender.Target = StanceBlender.Target == 0f ? 1f : 0f;
            ToggleStance(EStance.HighReady, false, true);
            WasActiveAim = false;
            DidStanceWiggle = false;

            if (CurrentStance == EStance.HighReady && HealthConditionForcesLowReady)
            {
                CanDoHighReadyInjuredAnim = true;
            }
        }

        private void ToggleLowReady()
        {
            StanceBlender.Target = StanceBlender.Target == 0f ? 1f : 0f;
            ToggleStance(EStance.LowReady, false, true);
            WasActiveAim = false;
            DidStanceWiggle = false;
        }

        private void HandleScrollInput(float scrollIncrement)
        {
            if (scrollIncrement == -1)
            {
                if (CurrentStance == EStance.HighReady)
                {
                    ToggleHighReady();
                }
                else if (CurrentStance != EStance.LowReady && HasResetHighReady)
                {
                    ToggleLowReady();
                }
            }
            if (scrollIncrement == 1 && CurrentStance != EStance.HighReady)
            {
                if (CurrentStance == EStance.LowReady && !HealthConditionForcesLowReady)
                {
                    ToggleLowReady();
                }
                else if (CurrentStance != EStance.HighReady && HasResetLowReady)
                {
                    ToggleHighReady();
                }
            }
        }

        public void ToggleLeftShoulder()
        {
            AudioControllerInstance.PlayADSSound(5f * PluginConfig.StanceSfxModifier.Value, false);
            IsLeftShoulder = !IsLeftShoulder;
            if (!TreatWeaponAsPistolStance)
            {
                CurrentStance = EStance.None;
                StoredStance = EStance.None;
                WasActiveAim = false;
                HaveSetActiveAim = false;
                DidStanceWiggle = false;
                StanceBlender.Target = 0f;
            }
        }

        public void StanceStateUpdate()
        {
            if (GameStateInstance.WeaponIsReady && PlayerStateInstance.IsUsingStationaryWeapon)
            {
                if (_doDampingTimer)
                {
                    StanceDampingTimer();
                }

                if (_doMeleeReset)
                {
                    MeleeCooldownTimer();
                }

                //patrol
                if (!ShouldBlockAllStances && Input.GetKeyDown(PluginConfig.PatrolKeybind.Value.MainKey) && PluginConfig.PatrolKeybind.Value.Modifiers.All(Input.GetKey))
                {
                    AudioControllerInstance.PlayADSSound(5f * PluginConfig.StanceSfxModifier.Value, false);
                    ToggleStance(EStance.PatrolStance);
                    StoredStance = EStance.None;
                    StanceBlender.Target = 0f;
                    DidStanceWiggle = false;
                }

                if (!PlayerStateInstance.IsSprinting && !PlayerStateInstance.IsInventoryOpen && !TreatWeaponAsPistolStance)
                {
                    //cycle stances
                    if (!ShouldBlockAllStances && Input.GetKeyUp(PluginConfig.CycleStancesKeybind.Value.MainKey))
                    {
                        if (Time.time <= _doubleClickTime)
                        {
                            _clickTriggered = true;
                            StanceBlender.Target = 0f;
                            StanceIndex = 0;
                            CancelAllStances();
                            DidStanceWiggle = false;
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
                            IsLeftShoulder = false;
                            StanceBlender.Target = 1f;
                            _clickTriggered = true;
                            StanceIndex++;
                            StanceIndex = StanceIndex > 3 ? 1 : StanceIndex;
                            CurrentStance = (EStance)StanceIndex;
                            StoredStance = CurrentStance;
                            DidStanceWiggle = false;
                            if (CurrentStance == EStance.HighReady && HealthConditionForcesLowReady)
                            {
                                CanDoHighReadyInjuredAnim = true;
                            }
                        }
                    }

                    //active aim
                    if (!PluginConfig.ToggleActiveAim.Value)
                    {
                        if ((!IsAiming && !ShouldBlockAllStances && Input.GetKey(PluginConfig.ActiveAimKeybind.Value.MainKey) && PluginConfig.ActiveAimKeybind.Value.Modifiers.All(Input.GetKey)) || (Input.GetKey(KeyCode.Mouse1) && AdsIsBlocked))
                        {
                            if (!HaveSetActiveAim)
                            {
                                DidStanceWiggle = false;
                            }
                            IsLeftShoulder = false;
                            StanceBlender.Target = 1f;
                            CurrentStance = EStance.ActiveAiming;
                            WasActiveAim = true;
                            HaveSetActiveAim = true;
                        }
                        else if (HaveSetActiveAim)
                        {
                            StanceBlender.Target = 0f;
                            CurrentStance = StoredStance;
                            WasActiveAim = false;
                            HaveSetActiveAim = false;
                            DidStanceWiggle = false;
                        }
                    }
                    else
                    {
                        if ((!IsAiming && !ShouldBlockAllStances && Input.GetKeyDown(PluginConfig.ActiveAimKeybind.Value.MainKey) && PluginConfig.ActiveAimKeybind.Value.Modifiers.All(Input.GetKey)) || (Input.GetKeyDown(KeyCode.Mouse1) && AdsIsBlocked))
                        {
                            StanceBlender.Target = StanceBlender.Target == 0f ? 1f : 0f;
                            ToggleStance(EStance.ActiveAiming);
                            WasActiveAim = CurrentStance == EStance.ActiveAiming ? true : false;
                            DidStanceWiggle = false;
                            if (CurrentStance != EStance.ActiveAiming)
                            {
                                CurrentStance = StoredStance;
                            }
                        }
                    }

                    if (!ShouldBlockAllStances && PluginConfig.UseMouseWheelStance.Value && !IsAiming)
                    {
                        if ((Input.GetKey(PluginConfig.StanceWheelComboKeyBind.Value.MainKey) && PluginConfig.UseMouseWheelPlusKey.Value) || (!PluginConfig.UseMouseWheelPlusKey.Value && !Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.R) && !Input.GetKey(KeyCode.C)))
                        {
                            float scrollDelta = Input.mouseScrollDelta.y;
                            if (scrollDelta != 0f)
                            {
                                HandleScrollInput(scrollDelta);
                            }
                        }
                    }

                    //Melee
                    if (!IsAiming && MeleeIsToggleable && Input.GetKeyDown(PluginConfig.MeleeKeybind.Value.MainKey) && PluginConfig.MeleeKeybind.Value.Modifiers.All(Input.GetKey))
                    {
                        IsMounting = false;
                        IsLeftShoulder = false;
                        CurrentStance = EStance.Melee;
                        StoredStance = EStance.None;
                        WasActiveAim = false;
                        DidStanceWiggle = false;
                        StanceBlender.Target = 1f;
                        MeleeIsToggleable = false;
                        MeleeHitSomething = false;
                    }

                    //short-stock
                    if (!ShouldBlockAllStances && Input.GetKeyDown(PluginConfig.ShortStockKeybind.Value.MainKey) && PluginConfig.ShortStockKeybind.Value.Modifiers.All(Input.GetKey))
                    {
                        StanceBlender.Target = StanceBlender.Target == 0f ? 1f : 0f;
                        ToggleStance(EStance.ShortStock, false, true);
                        WasActiveAim = false;
                        DidStanceWiggle = false;
                    }

                    //high ready
                    if (!ShouldBlockAllStances && !IsInForcedLowReady && Input.GetKeyDown(PluginConfig.HighReadyKeybind.Value.MainKey) && PluginConfig.HighReadyKeybind.Value.Modifiers.All(Input.GetKey))
                    {
                        ToggleHighReady();
                    }

                    //low ready
                    if (!ShouldBlockAllStances && !IsInForcedLowReady && Input.GetKeyDown(PluginConfig.LowReadyKeybind.Value.MainKey) && PluginConfig.LowReadyKeybind.Value.Modifiers.All(Input.GetKey))
                    {
                        ToggleLowReady();
                    }

                    //cancel if aiming
                    if (IsAiming)
                    {
                        if (CurrentStance == EStance.ActiveAiming || WasActiveAim)
                        {
                            StoredStance = EStance.None;
                        }
                        CurrentStance = EStance.None;
                        HaveSetAiming = true;
                    }
                    else if (HaveSetAiming)
                    {
                        CurrentStance = WasActiveAim ? EStance.ActiveAiming : StoredStance;
                        HaveSetAiming = false;
                    }
                }


                if (FiringStateInstance.IsFiring) //stance specific firing check is too slow
                {
                    bool rememberStance = PluginConfig.RememberStanceFiring.Value && IsAiming;
                    bool isActiveAim = CurrentStance == EStance.ActiveAiming && !IsAiming;
                    bool keepStance = rememberStance || (isActiveAim || CurrentStance == EStance.ShortStock || CurrentStance == EStance.PistolCompressed);

                    if (!keepStance)
                    {
                        CurrentStance = EStance.None;
                        StoredStance = EStance.None;
                        StanceBlender.Target = 0f;
                    }
                }

                if (CanDoHighReadyInjuredAnim)
                {
                    HighReadyBlackedArmTime += Time.deltaTime;
                    if (HighReadyBlackedArmTime >= 0.35f)
                    {
                        CanDoHighReadyInjuredAnim = false;
                        CurrentStance = EStance.LowReady;
                        StoredStance = EStance.LowReady;
                        HighReadyBlackedArmTime = 0f;
                    }
                }

                if (ShouldForceLowReady)
                {
                    StanceBlender.Target = 1f;
                    CurrentStance = EStance.LowReady;
                    StoredStance = EStance.LowReady;
                    WasActiveAim = false;
                    IsLeftShoulder = false;
                    IsInForcedLowReady = true;
                }
                else IsInForcedLowReady = false;
            }

            if (ShouldResetStanceCancels)
            {
                StanceManipCancelTimer();
            }

            if (DidWeaponSwap || (!PluginConfig.RememberStanceItem.Value && !GameStateInstance.WeaponIsReady) || !GameStateInstance.PlayerIsReady)
            {
                IsLeftShoulder = false;
                IsMounting = false;
                CurrentStance = EStance.None;
                StoredStance = EStance.None;
                StanceBlender.Target = 0f;
                StanceIndex = 0;
                WasActiveAim = false;
                DidWeaponSwap = false;
                AimingInterrupted = false;
                ResetStanceStamina();
            }
        }

        private void DoTacSprint(Player.FirearmController fc, Player player)
        {
            if (CanDoTacSprint)
            {
                IsDoingTacSprint = true;
                player.BodyAnimatorCommon.SetFloat(PlayerAnimator.WEAPON_SIZE_MODIFIER_PARAM_HASH, 2f);
                _tacSprintTime = 0f;
                _canDoTacSprintTimer = true;
            }
            else if (PluginConfig.EnableTacSprint.Value && _canDoTacSprintTimer)
            {
                _tacSprintTime += Time.deltaTime;
                if (_tacSprintTime >= 0.5f)
                {
                    player.BodyAnimatorCommon.SetFloat(PlayerAnimator.WEAPON_SIZE_MODIFIER_PARAM_HASH, WeaponStateInstance.WeaponLength);
                    _tacSprintTime = 0f;
                    _canDoTacSprintTimer = false;
                }
                IsDoingTacSprint = false;
            }
            else
            {
                IsDoingTacSprint = false;
            }
        }

        private float GetStanceWeaponInertiaFactor(ProceduralWeaponAnimation pwa, bool forDisplacement = false)
        {
            if (forDisplacement)
            {
                return
                    IsMounting ? 0.2f :
                    IsBracing ? 0.35f :
                    IsLeftShoulder ? 1.15f :
                    CurrentStance == EStance.ShortStock ? 0.75f :
                    CurrentStance == EStance.HighReady ? 0.91f :
                    CurrentStance == EStance.LowReady ? 0.87f :
                    CurrentStance == EStance.ActiveAiming ? 0.95f :
                    1f;
            }

            return
                IsMounting ? 0.05f :
                IsBracing ? 0.1f :
                IsLeftShoulder && !pwa.IsAiming ? 1.15f :
                IsLeftShoulder ? 0.87f :
                pwa.IsAiming ? 0.75f :
                WeaponStateInstance.TotalWeaponWeight > 1.6f && CurrentStance == EStance.PistolCompressed ? 0.85f :
                CurrentStance == EStance.PistolCompressed ? 1.15f :
                CurrentStance == EStance.ShortStock ? 0.8f :
                CurrentStance == EStance.HighReady ? 0.85f :
                CurrentStance == EStance.LowReady ? 0.8f :
                CurrentStance == EStance.ActiveAiming ? 0.9f :
                1f;
        }

        //TODO: replace using recoil processes for wiggle effect, with bespoke procedural motion
        //Intended to be toggled on PwaWeaponParamsPatch
        private void DoADSWiggle(ProceduralWeaponAnimation pwa, Player player, FirearmController fc, float factor)
        {
            if (IsIdle() && !IsChonker && !WeaponStateInstance.IsPistol)
            {
                _canResetDamping = false;
                float mountingFactor = IsMounting ? 0.1f : IsBracing ? 0.25f : 1f;
                float headGearFactor = GearStateInstance.FaceShieldIsActive || GearStateInstance.NVGIsActive || GearStateInstance.HasGasMask ? 3f : 1f;
                float baseLine = Mathf.Clamp(3.5f * factor * headGearFactor * mountingFactor * WeaponStateInstance.TotalAimStabilityModi, 0.1f, 17f);
                float rndX = UnityEngine.Random.Range(baseLine * 0.9f, baseLine);
                float rndY = UnityEngine.Random.Range(baseLine * 0.9f, baseLine);
                Vector3 wiggleDir = new Vector3(-rndX, -rndY, 0f);

                if (pwa.IsAiming && !_didAimWiggle)
                {
                    if (!FiringStateInstance.IsFiringFromStance && !IsLeftShoulder) DoWiggleEffects(player, pwa, fc.Weapon, wiggleDir, wiggleFactor: factor, isADS: true);
                    _didAimWiggle = true;
                }
                else if (!pwa.IsAiming && _didAimWiggle)
                {
                    _didAimWiggle = false;
                }
                _doDampingTimer = true;
            }
        }

        //TODO: replace using recoil processes for wiggle effect, with bespoke procedural motion
        public void DoWiggleEffects(Player player, ProceduralWeaponAnimation pwa, Weapon weapon, Vector3 wiggleDirection, bool playSound = false, float volume = 4f, float wiggleFactor = 1f, bool isADS = false, bool useGearSound = false)
        {
            if (playSound)
            {
                AudioControllerInstance.PlayADSSound(volume * PluginConfig.StanceSfxModifier.Value, useGearSound);
            }

            NewRecoilShotEffect newRecoil = pwa.Shootingg.CurrentRecoilEffect as NewRecoilShotEffect;
            if (isADS)
            {
                newRecoil.HandRotationRecoil.ReturnTrajectoryDumping = 0.3f * wiggleFactor;
                pwa.Shootingg.CurrentRecoilEffect.HandRotationRecoilEffect.Damping = 0.3f * wiggleFactor;
            }
            player.ProceduralWeaponAnimation.Shootingg.CurrentRecoilEffect.RecoilProcessValues[3].IntensityMultiplicator = 0;
            player.ProceduralWeaponAnimation.Shootingg.CurrentRecoilEffect.RecoilProcessValues[4].IntensityMultiplicator = 0;
            float count = pwa.Shootingg.CurrentRecoilEffect.RecoilProcessValues.Length;
            for (int i = 0; i < count; i++)
            {
                pwa.Shootingg.CurrentRecoilEffect.RecoilProcessValues[i].Process(wiggleDirection);
            }
            player.ProceduralWeaponAnimation.Shootingg.CurrentRecoilEffect.RecoilProcessValues[3].IntensityMultiplicator = 0;
            player.ProceduralWeaponAnimation.Shootingg.CurrentRecoilEffect.RecoilProcessValues[4].IntensityMultiplicator = 0;
        }

        private void MoveGunToCameraPID(ProceduralWeaponAnimation pwa, float dt, float stanceMulti, ref float gunAxesTarget, ref float gunCameraAlignmentTarget, float camTargetAxes, float speedModifer, float tolerance = 0.001f, bool ignoreLeftShoulder = false)
        {
            if (!IsAiming)
            {
                gunCameraAlignmentTarget = camTargetAxes;
            }

            if (IsColliding || PistolIsColliding || !pwa.OverlappingAllowsBlindfire || StopCameraMovement || (IsDoingLeftShoulderNotBlocked && !ignoreLeftShoulder)) return;

            bool skipPIDForRifle = FiringStateInstance.IsFiringMovement && !PluginConfig.EnableAltRifleRecoil.Value && !TreatWeaponAsPistolStance;
            bool skipPIDForPistol = FiringStateInstance.IsFiringMovement && TreatWeaponAsPistolStance;
            if (IsAiming && !skipPIDForRifle && !skipPIDForPistol)
            {
                float speed = speedModifer * stanceMulti;

                // Calculate difference
                float error = gunCameraAlignmentTarget - camTargetAxes;

                if (Mathf.Abs(error) > tolerance)
                {
                    // Convert error into a vertical offset
                    // (positive error = move weapon upward, negative = downward)
                    float adjustment = error * speed * dt;

                    gunAxesTarget += adjustment;
                }
            }
        }

        private Vector3 GetRifleStancePIDModifier()
        {
            if (StoredStance == EStance.HighReady)
                return new Vector3(0.6f, 0.35f, 1f);
            if (StoredStance == EStance.LowReady)
                return new Vector3(0.8f, 0.7f, 1f);
            if (StoredStance == EStance.ShortStock)
                return new Vector3(0.5f, 0.3f, 1f);
            if (StoredStance == EStance.ActiveAiming || WasActiveAim)
                return new Vector3(1.5f, 0.75f, 1f);

            return Vector3.one;
        }

        public bool ShouldDoFaceGearCantedOffset() 
        {
            return (GearStateInstance.HasGasMask || (GearStateInstance.FaceShieldIsActive && GearStateInstance.GearBlocksMouth));
        }

        //non-stance related rotational and postion changes for immersion
        public void DoExtraPosAndRot(ProceduralWeaponAnimation pwa, Player player)
        {
            //position
            float stockOffset = !WeaponStateInstance.IsPistol && !WeaponStateInstance.HasShoulderContact ? -0.04f : 0f;
            float stockPosOffset = WeaponStateInstance.StockPosition * 0.01f;
            float posOffsetMulti = WeaponStateInstance.HasShoulderContact ? -0.04f : 0.04f;
            float posePosOffset = (1f - player.MovementContext.PoseLevel) * posOffsetMulti;

            float targetPosXOffset = pwa.IsAiming ? 0f : 0f;
            float targetPosYOffset = pwa.IsAiming ? 0f : 0f;
            float targetPosZOffset = pwa.IsAiming ? 0f : Mathf.Clamp(posePosOffset + stockOffset + stockPosOffset, -0.05f, 0.05f);
            Vector3 targetPos = new Vector3(targetPosXOffset, targetPosYOffset, targetPosZOffset);

            _posePosOffest = Vector3.Lerp(_posePosOffest, targetPos, 5f * Time.deltaTime);
            pwa.HandsContainer.WeaponRoot.localPosition += _posePosOffest;

            //rotation
            bool isMountedWithBipod = WeaponStateInstance.BipodIsDeployed && IsMounting;
            bool doCantedSightOffset = IsCantedAiming(pwa, true);
            bool doMaskOffset = 
                !doCantedSightOffset && 
                !isMountedWithBipod &&
                ShouldDoFaceGearCantedOffset() &&
                !WeaponStateInstance.WeaponCanFSADS && 
                pwa.IsAiming && 
                WeaponStateInstance.HasShoulderContact && 
                WeaponStateInstance.TreatAsPistol;
            bool doLongMagOffset = WeaponStateInstance.HasLongMag && player.IsInPronePose && !isMountedWithBipod;
            float cantedOffsetBase = -0.41f;
            float magOffset = doCantedSightOffset ? 0f : doLongMagOffset && !pwa.IsAiming ? -0.35f : doLongMagOffset && pwa.IsAiming ? -0.12f : 0f;
            float ergoOffset = WeaponStateInstance.ErgoFactor * -0.001f;
            float poseRotOffset = (1f - player.MovementContext.PoseLevel) * -0.03f;
            poseRotOffset += player.IsInPronePose ? -0.03f : 0f;
            float maskFactor = doMaskOffset ? -0.025f + ergoOffset : 0f;
            float baseRotOffset = pwa.IsAiming || IsMounting || IsBracing ? 0f : poseRotOffset + ergoOffset;
            float cantedSightOffset = doCantedSightOffset ? cantedOffsetBase : 0f;

            float rotX = 0f;
            float rotY = Mathf.Clamp(baseRotOffset + maskFactor + magOffset, -0.5f, 0f) + cantedSightOffset;
            float rotZ = 0f;
            Vector3 targetRot = new Vector3(rotX, rotY, rotZ);

            _poseRotOffest = Vector3.Lerp(_poseRotOffest, targetRot, 5f * Time.deltaTime); //speeds should be affected by stance multi? or player crouch speed?

            Quaternion newRot = Quaternion.identity;
            newRot.x = _poseRotOffest.x;
            newRot.y = _poseRotOffest.y;
            newRot.z = _poseRotOffest.z;
            pwa.HandsContainer.WeaponRoot.localRotation *= newRot;
        }

        private void CheckLeftShoulder(Player player, Player.FirearmController fc, ProceduralWeaponAnimation pwa, float stanceMulti, float dt, Vector3 posTarget, Vector3 rotTarget, float rotSpeed, float curveModifier = 1f)
        {
            float baseSpeed = Mathf.Clamp((1f - stanceMulti) + 1f, 0.05f, 1.5f);
            float speed = IsAiming ? baseSpeed * 0.22f : baseSpeed * 0.22f;

            //position

            var xTarget = posTarget.x + PluginConfig.LeftShoulderOffset.Value;
            var position = IsDoingLeftShoulderNotBlocked
                ? new Vector3(xTarget, posTarget.y, posTarget.z + (_leffPosZCurve.Evaluate(_leftStanceProgress) * curveModifier))
                : new Vector3(0f, 0f, _leffPosZCurveReturn.Evaluate(_leftStanceProgress) * curveModifier);

            if (IsDoingLeftShoulderNotBlocked)
            {
                _leftStanceTargetX = xTarget;
                _leftStanceTime = 0f;
                _isLeftStanceResetState = false;
            }
            else
            {        
                _leftStanceTime += dt;
                if (_leftStanceTime <= 0.5f)
                {
                    _isLeftStanceResetState = true;
                }
                else
                {
                    _isLeftStanceResetState = false;
                }
            }

            _leftStancePosition = Vector3.SmoothDamp(_leftStancePosition, position, ref _leftStanceVelocity, speed, 0.55f, dt);

            _leftStanceProgress = Mathf.InverseLerp(0f, _leftStanceTargetX, _leftStancePosition.x);

            if (MathUtils.AreFloatsEqual(_leftStanceProgress, 0f) && !IsLeftShoulder) HaveResetLeftShoulder = true;
            else HaveResetLeftShoulder = false;

            //moving towards 1, and is left shoulder
            bool isTransitionignLeft = IsLeftShoulder && MathUtils.IsLessThan(_leftStanceProgress, 0.99f);
            bool isTransitioningRight = (_isLeftStanceResetState || !IsLeftShoulder) && MathUtils.IsGreaterThan(_leftStanceProgress, 0.01f);

            if (IsAiming && (isTransitionignLeft || isTransitioningRight))
            {
                InterruptAim(fc);
            } 
            if (!isTransitionignLeft && !isTransitioningRight)
            {
                UnInterruptAim(fc);
            }

            //rotation
            var rotation = IsDoingLeftShoulderNotBlocked && !IsAiming ? rotTarget : Vector3.zero;
            rotation.x += _leftRotationXCurve.Evaluate(_leftStanceProgress);

            _leftStanceRotaiton = Vector3.Lerp(_leftStanceRotaiton, rotation, rotSpeed * dt);
            Quaternion newRot = Quaternion.Euler(_leftStanceRotaiton);

            pwa.HandsContainer.WeaponRoot.localRotation *= newRot;
        }

        //I've no idea wtf is going on here but it sort of works
        private void HandleAltPistolPosition(Player player, Player.FirearmController fc, ProceduralWeaponAnimation pwa, float stanceMulti, float dt, Vector3 camTarget)
        {
            //left stance speed
            float leftResetSpeedModi = _isLeftStanceResetState ? 0.2f : 1f;

            //speed
            float fpsFactor = Mathf.Pow(RealismCommonLib.Plugin.FPSFactor, 0.25f);
            float speedFactorTarget = IsAiming ? PluginConfig.PistolPosResetSpeedMulti.Value * stanceMulti : PluginConfig.PistolPosSpeedMulti.Value * stanceMulti;
            float pidSpeed = fpsFactor * leftResetSpeedModi * PluginConfig.PistolPosResetSpeedMulti.Value;
            _pistolPosSpeed = Mathf.Lerp(_pistolPosSpeed, speedFactorTarget, dt * 10f);

            if (!IsAiming)
            {
                _gunXTarget = !IsBlindFiring ? 0.038f : 0f;
                _gunYTarget = -0.0385f;
                _gunZTarget =  0f;
            }

            CheckLeftShoulder(player, fc, pwa, _pistolPosSpeed, dt, _leftStancePistolPositionTarget, _leftStancePistolRotaitonTarget, stanceMulti * 2.5f, 0.05f);

            if (RealismCommonLib.Plugin.FOVFixEnabled) 
            {
                MoveGunToCameraPID(pwa, dt, stanceMulti, ref _gunXTarget, ref _gunCameraAlignmentTargetX, camTarget.x, 0.15f * pidSpeed, 0.0001f);
                MoveGunToCameraPID(pwa, dt, stanceMulti, ref _gunYTarget, ref _gunCameraAlignmentTargetY, camTarget.y, 0.3f * pidSpeed, ignoreLeftShoulder: true);
                MoveGunToCameraPID(pwa, dt, stanceMulti, ref _gunZTarget, ref _gunCameraAlignmentTargetZ, camTarget.z, 0.4f * pidSpeed, ignoreLeftShoulder: true);
            }

            _currentPistolXPos = Mathf.Lerp(_currentPistolXPos, _gunXTarget, dt * _pistolPosSpeed);
            _currentPistolYPos = Mathf.Lerp(_currentPistolYPos, _gunYTarget, dt * _pistolPosSpeed);
            _currentPistolZPos = Mathf.Lerp(_currentPistolZPos, _gunZTarget, dt * _pistolPosSpeed); 

            _pistolLocalPosition.x = _currentPistolXPos + _leftStancePosition.x;
            _pistolLocalPosition.y = _currentPistolYPos + _leftStancePosition.y;
            _pistolLocalPosition.z = _currentPistolZPos + _leftStancePosition.z;

            pwa.HandsContainer.WeaponRoot.localPosition = _pistolLocalPosition;

        }

        private void HandleRiflePosition(Player player, Player.FirearmController fc, ProceduralWeaponAnimation pwa, float stanceMulti, float movementFactor, float dt, Vector3 camTarget)
        {
            //left stance speeds
            float leftResetPidModi = _isLeftStanceResetState ? 0f : 1f;

            //speeds
            float fpsFactor = Mathf.Pow(RealismCommonLib.Plugin.FPSFactor, 0.25f);   
            float posSpeed = IsAiming ? 30f * WeaponStateInstance.TotalFinalAimSpeed : 6f * WeaponStateInstance.TotalFinalAimSpeed;
            float pidSpeed = 30f * fpsFactor * leftResetPidModi;
            Vector3 stanceModifer = GetRifleStancePIDModifier();

            bool isCantedAiming = IsCantedAiming(pwa, false);
            bool adjustSpeedForCant = isCantedAiming && WasActiveAim;

            if (!IsAiming) 
            {
                _gunXTarget = BaseWeaponOffsetPosition.x + PluginConfig.WeapOffset.Value.x;
                _gunYTarget = BaseWeaponOffsetPosition.y + PluginConfig.WeapOffset.Value.y;
                _gunZTarget = BaseWeaponOffsetPosition.z + PluginConfig.WeapOffset.Value.z;
            }

            CheckLeftShoulder(player, fc, pwa, stanceMulti, dt, _leftStanceRiflePositionTarget, _leftStanceRifleRotaitonTarget, stanceMulti * 4.5f);

            if (PluginConfig.EnableAltRifle.Value && RealismCommonLib.Plugin.FOVFixEnabled)
            {
                MoveGunToCameraPID(pwa, dt, WeaponStateInstance.TotalFinalAimSpeed, ref _gunXTarget, ref _gunCameraAlignmentTargetX, camTarget.x, 0.3f * pidSpeed * stanceModifer.x, 0.0001f);
                MoveGunToCameraPID(pwa, dt, WeaponStateInstance.TotalFinalAimSpeed, ref _gunYTarget, ref _gunCameraAlignmentTargetY, camTarget.y, 0.3f * pidSpeed * stanceModifer.y, 0.0001f, true);
                MoveGunToCameraPID(pwa, dt, WeaponStateInstance.TotalFinalAimSpeed, ref _gunZTarget, ref _gunCameraAlignmentTargetZ, camTarget.z, 0.3f * pidSpeed * stanceModifer.z, 0.0001f, true);
            }
          
            _currentRifleXPos = Mathf.Lerp(_currentRifleXPos, _gunXTarget, dt * posSpeed);
            _currentRifleYPos = Mathf.Lerp(_currentRifleYPos, _gunYTarget, dt * posSpeed); //if trying to fix stance ADS, animspeed might be fucking with things
            _currentRifleZPos = Mathf.Lerp(_currentRifleZPos, _gunZTarget, dt * posSpeed);

            _rifleLocalPosition.x = _currentRifleXPos + _leftStancePosition.x;
            _rifleLocalPosition.y = _currentRifleYPos + _leftStancePosition.y;
            _rifleLocalPosition.z = _currentRifleZPos + _leftStancePosition.z;

            pwa.HandsContainer.WeaponRoot.localPosition = _rifleLocalPosition;
        }

        public void DoPistolStances(bool isThirdPerson, EFT.Animations.ProceduralWeaponAnimation pwa, float dt, Player player, Player.FirearmController fc, Vector3 camTarget)
        {
            bool useThirdPersonStance = isThirdPerson;//  || Plugin.IsUsingFika
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

            if (CurrentStance == EStance.PatrolStance) return;

            if (!pwa.IsAiming && !IsBlindFiring && !PistolIsColliding && !WeaponStateInstance.HasShoulderContact && PluginConfig.EnableAltPistol.Value) //!CancelPistolStance && !pwa.LeftStance
            {
                if (CurrentStance == EStance.PatrolStance || _previousStance == EStance.PatrolStance) _SkipPistolWiggle = true;
                CurrentStance = EStance.PistolCompressed;
                StoredStance = EStance.None;
                IsResettingPistol = false;
                HasResetPistolPos = false;

                StanceBlender.Speed = PluginConfig.PistolPosSpeedMulti.Value * stanceMulti;
                StanceTargetPosition = Vector3.Lerp(StanceTargetPosition, pistolTargetPosition, PluginConfig.StanceTransitionSpeedMulti.Value * stanceMulti * dt);

                if (StanceBlender.Value < 1f)
                {
                    StanceRotationSpeed = 4f * stanceMulti * dt * PluginConfig.PistolAdditionalRotationSpeedMulti.Value * stanceMulti;
                    StanceRotation = pistolMiniTargetQuaternion;
                }
                else
                {
                    StanceRotationSpeed = 4f * stanceMulti * dt * PluginConfig.PistolRotationSpeedMulti.Value * stanceMulti * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value : 1f);
                    StanceRotation = pistolTargetQuaternion;
                }

                if (StanceTargetPosition == pistolTargetPosition && StanceBlender.Value >= 1f && !_canResetDamping)
                {
                    _doDampingTimer = true;
                }
                else if (StanceTargetPosition != pistolTargetPosition || StanceBlender.Value < 1)
                {
                    _canResetDamping = false;
                }

                if (StanceBlender.Value < 0.95f || CancelPistolStance)
                {
                    DidStanceWiggle = false;
                }
                if ((StanceBlender.Value >= 1f && StanceTargetPosition == pistolTargetPosition) && !DidStanceWiggle)
                {
                    if (!_SkipPistolWiggle && !IsLeftShoulder) DoWiggleEffects(player, pwa, fc.Weapon, new Vector3(-12.5f, 5f, 1f) * movementFactor);
                    DidStanceWiggle = true;
                    CancelPistolStance = false;
                    _SkipPistolWiggle = false;
                }

            }
            else if (StanceBlender.Value > 0f && !HasResetPistolPos && !PistolIsColliding)
            {
                _canResetDamping = false;

                IsResettingPistol = true;
                StanceRotationSpeed = 4f * stanceMulti * dt * PluginConfig.PistolResetRotationSpeedMulti.Value * stanceMulti * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value : 1f);
                StanceRotation = pistolRevertQuaternion;
                StanceBlender.Speed = PluginConfig.PistolPosResetSpeedMulti.Value * stanceMulti * (useThirdPersonStance ? PluginConfig.ThirdPersonPositionSpeed.Value : 1f);
            }
            else if (StanceBlender.Value == 0f && !HasResetPistolPos && !PistolIsColliding)
            {
                if (!_canResetDamping)
                {
                    _doDampingTimer = true;
                }

                if (!IsLeftShoulder) DoWiggleEffects(player, pwa, fc.Weapon, new Vector3(-10f, 0f, -20f) * movementFactor); //new Vector3(10f, 1f, -30f) * wiggleBalanceFactor * rotationBalanceFactor  * wiggleBalanceFactor

                IsResettingPistol = false;
                CurrentStance = EStance.None;
                StanceRotation = Quaternion.identity;
                HasResetPistolPos = true;
            }
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

            bool pauseStance = PlayerStateInstance.IsInventoryOpen || IsBlindFiring || IsLeftShoulder;
             
            float wiggleErgoMulti = Mathf.Clamp((ErgoStanceSpeed * 0.5f), 0.1f, 1f);
            float stocklessModifier = WeaponStateInstance.HasShoulderContact ? 1f : 0.5f;
            WiggleReturnSpeed = (1f - (SkillStateInstance.AimSkillADSBuff * 0.5f)) * wiggleErgoMulti * HealthStateInstance.StanceInjuryMulti * stocklessModifier * playerWeightFactor * (Mathf.Max(PlayerStateInstance.RemainingArmStamFactor, 0.55f));
          
            //for setting baseline position
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

        }

        public void DoShortStock(Player player, Player.FirearmController fc, bool isThirdPerson, EFT.Animations.ProceduralWeaponAnimation pwa, float dt, bool useThirdPersonStance, float stanceMulti, float resetErgoMulti, bool pauseStance, float movementFactor)
        {
            float shortStockStanceMulti = Mathf.Clamp(stanceMulti, 0.65f, 1.5f);

            Vector3 shortTargetRotation = useThirdPersonStance ?
                PluginConfig.ShortStockThirdPersonRotation.Value :
                PluginConfig.ShortStockRotation.Value * shortStockStanceMulti;
            Quaternion shortStockTargetQuaternion = Quaternion.Euler(shortTargetRotation);
            Quaternion shortStockMiniTargetQuaternion = Quaternion.Euler(PluginConfig.ShortStockAdditionalRotation.Value * resetErgoMulti);
            Quaternion shortStockRevertQuaternion = Quaternion.Euler(PluginConfig.ShortStockResetRotation.Value * resetErgoMulti);
            Vector3 shortStockTargetPosition = useThirdPersonStance ?
                PluginConfig.ShortStockThirdPersonPosition.Value :
                PluginConfig.ShortStockOffset.Value;

            if (CurrentStance == EStance.ShortStock && !pwa.IsAiming && !CancelShortStock && !IsBlindFiring && !pwa.LeftStance && !PlayerStateInstance.IsSprinting && !pauseStance)
            {
                float activeToShort = 1f;
                float highToShort = 1f;
                float lowToShort = 1f;
                IsResettingShortStock = false;
                HasResetShortStock = false;
                HasResetMelee = true;

                if (StanceTargetPosition != shortStockTargetPosition)
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

                if (StanceTargetPosition == shortStockTargetPosition && StanceBlender.Value >= 1f && !_canResetDamping)
                {
                    _doDampingTimer = true;
                }
                else if (StanceTargetPosition != shortStockTargetPosition || StanceBlender.Value < 1)
                {
                    _canResetDamping = false;
                }

                float transitionPositionFactor = activeToShort * highToShort * lowToShort;
                float transitionRotationFactor = activeToShort * highToShort * lowToShort;

                if (StanceBlender.Value < 1f)
                {
                    StanceRotationSpeed = 4f * shortStockStanceMulti * dt * PluginConfig.ShortStockAdditionalRotationSpeedMulti.Value * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value : 1f) * transitionRotationFactor;
                    StanceRotation = shortStockMiniTargetQuaternion;
                }
                else
                {
                    StanceRotationSpeed = 4f * shortStockStanceMulti * dt * PluginConfig.ShortStockRotationMulti.Value * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value : 1f) * transitionRotationFactor;
                    StanceRotation = shortStockTargetQuaternion;
                }

                StanceBlender.Speed = PluginConfig.ShortStockSpeedMulti.Value * shortStockStanceMulti * (useThirdPersonStance ? PluginConfig.ThirdPersonPositionSpeed.Value : 1f);
                StanceTargetPosition = Vector3.Lerp(StanceTargetPosition, shortStockTargetPosition, PluginConfig.StanceTransitionSpeedMulti.Value * shortStockStanceMulti * transitionPositionFactor * dt);

                if ((StanceBlender.Value >= 0.9f || StanceTargetPosition == shortStockTargetPosition) && !DidStanceWiggle && !useThirdPersonStance)
                {
                    DoWiggleEffects(player, pwa, fc.Weapon, new Vector3(5f, -2.5f, 30f) * movementFactor, true);
                    DidStanceWiggle = true;
                }
            }
            else if (StanceBlender.Value > 0f && !HasResetShortStock && CurrentStance != EStance.LowReady && CurrentStance != EStance.ActiveAiming && CurrentStance != EStance.HighReady && !IsResettingActiveAim && !IsResettingHighReady && !IsResettingLowReady && !IsResettingMelee)
            {
                _canResetDamping = false;
                IsResettingShortStock = true;
                StanceRotationSpeed = 4f * shortStockStanceMulti * dt * PluginConfig.ShortStockResetRotationSpeedMulti.Value;
                StanceRotation = shortStockRevertQuaternion;
                StanceBlender.Speed = PluginConfig.ShortStockResetSpeedMulti.Value * shortStockStanceMulti * (useThirdPersonStance ? PluginConfig.ThirdPersonPositionSpeed.Value : 1f);
            }
            else if (StanceBlender.Value == 0f && !HasResetShortStock)
            {
                if (!_canResetDamping)
                {
                    _doDampingTimer = true;
                }

                if (!useThirdPersonStance) DoWiggleEffects(player, pwa, fc.Weapon, new Vector3(-4f, -2f, -30f) * movementFactor, true);
                DidStanceWiggle = false;
                StanceRotation = Quaternion.identity;
                IsResettingShortStock = false;
                HasResetShortStock = true;
            }
        }

        public void DoHighReady(Player player, Player.FirearmController fc, bool isThirdPerson, EFT.Animations.ProceduralWeaponAnimation pwa, float dt, bool useThirdPersonStance, float stanceMulti, float resetErgoMulti, bool pauseStance, float movementFactor)
        {
            float highReadyStanceMulti = Mathf.Clamp(stanceMulti, 0.5f, 0.98f);
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

            if (CurrentStance == EStance.HighReady && !pwa.IsAiming && !FiringStateInstance.IsFiringFromStance && !CancelHighReady && !pauseStance)
            {
                float shortToHighMulti = 1.0f;
                float lowToHighMulti = 1.0f;
                float activeToHighMulti = 1.0f;
                IsResettingHighReady = false;
                HasResetHighReady = false;
                HasResetMelee = true;

                if (StanceTargetPosition != highReadyTargetPosition)
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

                if (StanceTargetPosition == highReadyTargetPosition && StanceBlender.Value == 1 && !_canResetDamping)
                {
                    _doDampingTimer = true;
                }
                else if (StanceTargetPosition != highReadyTargetPosition || StanceBlender.Value < 1)
                {
                    _canResetDamping = false;
                }

                float transitionPositionFactor = shortToHighMulti * lowToHighMulti * activeToHighMulti;
                float transitionRotationFactor = shortToHighMulti * lowToHighMulti * activeToHighMulti * (transitionPositionFactor != 1f ? 0.9f : 1f);

                if (CanDoHighReadyInjuredAnim)
                {
                    if (StanceBlender.Value < 0.3f)
                    {
#warning replace this with bespoke rotation target and speed
                        Vector3 lowTargetRotation = useThirdPersonStance ?
                            PluginConfig.LowReadyThirdPersonRotation.Value :
                            new Vector3(
                                PluginConfig.LowReadyRotation.Value.x * resetErgoMulti,
                                PluginConfig.LowReadyRotation.Value.y,
                                PluginConfig.LowReadyRotation.Value.z);

                        Quaternion lowReadyTargetQuaternion = Quaternion.Euler(lowTargetRotation);

                        StanceRotationSpeed = 3f * highReadyStanceMulti * dt * PluginConfig.HighReadyRotationMulti.Value * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value * 0.7f : 1f) * (WeaponStateInstance.IsPistol ? 0.5f : 1f);
                        StanceRotation = lowReadyTargetQuaternion;
                    }
                    else
                    {
                        StanceRotationSpeed = 3f * highReadyStanceMulti * dt * PluginConfig.HighReadyAdditionalRotationSpeedMulti.Value * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value * 0.2f : 1f) * (WeaponStateInstance.IsPistol ? 0.5f : 1f);
                        StanceRotation = highReadyMiniTargetQuaternion;
                    }
                }
                else
                {
                    if (StanceBlender.Value < 0.3f)
                    {
                        StanceRotationSpeed = 4f * highReadyStanceMulti * dt * PluginConfig.HighReadyAdditionalRotationSpeedMulti.Value * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value * 0.2f : 1f) * transitionRotationFactor * (WeaponStateInstance.IsPistol ? 0.5f : 1f);
                        StanceRotation = highReadyMiniTargetQuaternion;
                    }
                    else
                    {
                        StanceRotationSpeed = 4f * highReadyStanceMulti * dt * PluginConfig.HighReadyRotationMulti.Value * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value * 0.7f : 1f) * transitionRotationFactor * (WeaponStateInstance.IsPistol ? 0.5f : 1f);
                        StanceRotation = highReadyTargetQuaternion;
                    }
                }

                StanceBlender.Speed = PluginConfig.HighReadySpeedMulti.Value * highReadyStanceMulti * (useThirdPersonStance ? PluginConfig.ThirdPersonPositionSpeed.Value : 1f);
                StanceTargetPosition = Vector3.Lerp(StanceTargetPosition, highReadyTargetPosition, PluginConfig.StanceTransitionSpeedMulti.Value * highReadyStanceMulti * transitionPositionFactor * dt);

                if ((StanceBlender.Value >= 1f || StanceTargetPosition == highReadyTargetPosition) && !DidStanceWiggle && !useThirdPersonStance)
                {
                    if (!WeaponStateInstance.IsPistol) DoWiggleEffects(player, pwa, fc.Weapon, new Vector3(5f, 5f, 5f) * movementFactor, true);//new Vector3(11f, 5.5f, 50f)
                    DidStanceWiggle = true;
                }
            }
            else if (StanceBlender.Value > 0f && !HasResetHighReady && CurrentStance != EStance.LowReady && CurrentStance != EStance.ActiveAiming && CurrentStance != EStance.ShortStock && !IsResettingActiveAim && !IsResettingLowReady && !IsResettingShortStock && !IsResettingMelee)
            {
                _canResetDamping = false;
                IsResettingHighReady = true;
                StanceRotationSpeed = 4f * highReadyStanceMulti * dt * PluginConfig.HighReadyResetRotationMulti.Value * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value : 1f);
                StanceRotation = highReadyRevertQuaternion;
                StanceBlender.Speed = PluginConfig.HighReadyResetSpeedMulti.Value * highReadyStanceMulti * (useThirdPersonStance ? PluginConfig.ThirdPersonPositionSpeed.Value : 1f);
            }
            else if (StanceBlender.Value <= 0f && !HasResetHighReady)
            {
                if (!_canResetDamping)
                {
                    _doDampingTimer = true;
                }

                if (!useThirdPersonStance && !WeaponStateInstance.IsPistol) DoWiggleEffects(player, pwa, fc.Weapon, new Vector3(highReadyXWiggleFactor * 10f, highReadyXWiggleFactor * 1f, highReadyZWiggleFactor * -10f) * movementFactor, true); //(1.5f, 3.75f, -30)
                DidStanceWiggle = false;
                StanceRotation = Quaternion.identity;
                IsResettingHighReady = false;
                HasResetHighReady = true;
            }
        }

        public void DoLowReady(Player player, Player.FirearmController fc, bool isThirdPerson, EFT.Animations.ProceduralWeaponAnimation pwa, float dt, bool useThirdPersonStance, float stanceMulti, float resetErgoMulti, bool pauseStance, float movementFactor)
        {
            float lowReadyStanceMulti = Mathf.Clamp(stanceMulti, 0.5f, 0.98f);

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

            if (CurrentStance == EStance.LowReady && !pwa.IsAiming && !FiringStateInstance.IsFiringFromStance && !CancelLowReady && !pauseStance)
            {
                float highToLow = 1.0f;
                float shortToLow = 1.0f;
                float activeToLow = 1.0f;
                IsResettingLowReady = false;
                HasResetLowReady = false;
                HasResetMelee = true;

                if (StanceTargetPosition != lowReadyTargetPosition)
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

                if (StanceTargetPosition == lowReadyTargetPosition && StanceBlender.Value >= 1f && !_canResetDamping)
                {
                    _doDampingTimer = true;
                }
                else if (StanceTargetPosition != lowReadyTargetPosition || StanceBlender.Value < 1)
                {
                    _canResetDamping = false;
                }

                float transitionPositionFactor = highToLow * shortToLow * activeToLow;
                float transitionRotationFactor = highToLow * shortToLow * activeToLow * (transitionPositionFactor != 1f ? 1.025f : 1f);

                if (StanceBlender.Value < 1f)
                {
                    StanceRotationSpeed = 4f * lowReadyStanceMulti * dt * PluginConfig.LowReadyAdditionalRotationSpeedMulti.Value * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value * 0.8f : 1f) * transitionRotationFactor;
                    StanceRotation = lowReadyMiniTargetQuaternion;
                }
                else
                {
                    StanceRotationSpeed = 4f * lowReadyStanceMulti * dt * PluginConfig.LowReadyRotationMulti.Value * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value * 0.8f : 1f) * transitionRotationFactor;
                    StanceRotation = lowReadyTargetQuaternion;
                }

                StanceBlender.Speed = PluginConfig.LowReadySpeedMulti.Value * lowReadyStanceMulti * (useThirdPersonStance ? PluginConfig.ThirdPersonPositionSpeed.Value * 0.8f : 1f);
                StanceTargetPosition = Vector3.Lerp(StanceTargetPosition, lowReadyTargetPosition, PluginConfig.StanceTransitionSpeedMulti.Value * lowReadyStanceMulti * transitionPositionFactor * dt);

                if ((StanceBlender.Value >= 0.5f || StanceTargetPosition == lowReadyTargetPosition) && !DidStanceWiggle && !useThirdPersonStance)
                {
                    DoWiggleEffects(player, pwa, fc.Weapon, new Vector3(7f, 7f, 0f) * movementFactor, true);
                    DidStanceWiggle = true;
                }
                DidLowReadyResetStanceWiggle = false;
            }
            else if (StanceBlender.Value > 0f && !HasResetLowReady && CurrentStance != EStance.ActiveAiming && CurrentStance != EStance.HighReady && CurrentStance != EStance.ShortStock && !IsResettingActiveAim && !IsResettingHighReady && !IsResettingShortStock && !IsResettingMelee)
            {
                _canResetDamping = false;

                IsResettingLowReady = true;
                StanceRotationSpeed = 4f * lowReadyStanceMulti * dt * PluginConfig.LowReadyResetRotationMulti.Value * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value * 0.8f : 1f);
                StanceRotation = lowReadyRevertQuaternion;

                StanceBlender.Speed = PluginConfig.LowReadyResetSpeedMulti.Value * lowReadyStanceMulti * (useThirdPersonStance ? PluginConfig.ThirdPersonPositionSpeed.Value * 0.8f : 1f);

                if (!useThirdPersonStance && StanceBlender.Value <= 0.65f && !DidLowReadyResetStanceWiggle)
                {
                    DoWiggleEffects(player, pwa, fc.Weapon, new Vector3(-10f, 4f, 10f) * movementFactor, true); //new Vector3(-4f, 2.5f, 10f)
                    DidLowReadyResetStanceWiggle = true;
                }
            }
            else if (StanceBlender.Value == 0f && !HasResetLowReady)
            {
                if (!_canResetDamping)
                {
                    _doDampingTimer = true;
                }
                StanceRotation = Quaternion.identity;
                IsResettingLowReady = false;
                HasResetLowReady = true;
            }
        }

        public void DoActiveAim(Player player, Player.FirearmController fc, bool isThirdPerson, EFT.Animations.ProceduralWeaponAnimation pwa, float dt, bool useThirdPersonStance, float stanceMulti, float resetErgoMulti, bool pauseStance, float movementFactor)
        {
            Vector3 activeTargetRoation =
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

            if (CurrentStance == EStance.ActiveAiming && !CancelActiveAim && !pauseStance)
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

                if (StanceTargetPosition != activeAimTargetPosition)
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

                if (StanceTargetPosition == activeAimTargetPosition && StanceBlender.Value == 1 && !_canResetDamping)
                {
                    _doDampingTimer = true;
                }
                else if (StanceTargetPosition != activeAimTargetPosition || StanceBlender.Value < 1)
                {
                    _canResetDamping = false;
                }

                float transitionPositionFactor = shortToActive * highToActive * lowToActive;
                float transitionRotationFactor = shortToActiveRotation * highToActiveRotation * lowToActiveRotation; //(transitionPositionFactor != 1f ? 0.9f : 1f)

                //additonal rotation makes ADS janky
                /*     if (StanceBlender.Value < 1f)
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
                     }*/

                StanceTargetPosition = Vector3.Lerp(StanceTargetPosition, activeAimTargetPosition, PluginConfig.StanceTransitionSpeedMulti.Value * stanceMulti * transitionPositionFactor * dt);
                StanceRotationSpeed = 4f * stanceMulti * dt * ergoFactor * PluginConfig.ActiveAimRotationSpeedMulti.Value * ChonkerFactorStanceRotationModifier * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value : 1f) * transitionRotationFactor;
                StanceRotation = activeAimTargetQuaternion;

                StanceBlender.Speed = PluginConfig.ActiveAimPosSpeedMulti.Value * stanceMulti * ergoFactor * ChonkerFactorStanceRotationModifier * (useThirdPersonStance ? PluginConfig.ThirdPersonPositionSpeed.Value : 1f);

                if ((StanceBlender.Value >= 1f || StanceTargetPosition == activeAimTargetPosition) && !DidStanceWiggle && !useThirdPersonStance)
                {
                    DoWiggleEffects(player, pwa, fc.Weapon, new Vector3(-10f, -10f, 0f), true, 3f);
                    DidStanceWiggle = true;
                }
            }
            else if (StanceBlender.Value > 0f && !HasResetActiveAim && CurrentStance != EStance.LowReady && CurrentStance != EStance.HighReady && CurrentStance != EStance.ShortStock && !IsResettingLowReady && !IsResettingHighReady && !IsResettingShortStock && !IsResettingMelee)
            {
                _canResetDamping = false;

                IsResettingActiveAim = true;
                StanceRotationSpeed = stanceMulti * dt * PluginConfig.ActiveAimResetRotationSpeedMulti.Value * ChonkerFactorStanceRotationModifier * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value : 1f);
                StanceRotation = activeAimRevertQuaternion;
                StanceBlender.Speed = PluginConfig.ActiveAimResetSpeedMulti.Value * stanceMulti * ChonkerFactorStanceRotationModifier * (useThirdPersonStance ? PluginConfig.ThirdPersonPositionSpeed.Value : 1f);
            }
            else if (StanceBlender.Value == 0f && !HasResetActiveAim)
            {
                if (!_canResetDamping)
                {
                    _doDampingTimer = true;
                }

                if (!useThirdPersonStance) DoWiggleEffects(player, pwa, fc.Weapon, new Vector3(-5f, 1.5f, 0f) * movementFactor, true, 3f);
                DidStanceWiggle = false;

                StanceRotation = Quaternion.identity;

                IsResettingActiveAim = false;
                HasResetActiveAim = true;
            }
        }

        public void DoMeleeStance(Player player, Player.FirearmController fc, bool isThirdPerson, EFT.Animations.ProceduralWeaponAnimation pwa, float dt, bool useThirdPersonStance, float stanceMulti, float resetErgoMulti, bool pauseStance, float movementFactor)
        {
            if (WeaponStateInstance.HasBayonet)
            {
                DoMeleeStanceBayonet(player, fc, isThirdPerson, pwa, dt, useThirdPersonStance, stanceMulti, resetErgoMulti, pauseStance, movementFactor);
                return;
            }

            bool isDoingMelee = CurrentStance == EStance.Melee && !pwa.IsAiming && !pauseStance;

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

                if (StanceTargetPosition == meleeFinalPos && StanceBlender.Value >= 1f && !_canResetDamping)
                {
                    _doDampingTimer = true;
                }
                else if (StanceTargetPosition != meleeFinalPos || StanceBlender.Value < 1)
                {
                    _canResetDamping = false;
                }

                StanceRotationSpeed = 10f * Mathf.Clamp(stanceMulti, 0.8f, 1f) * dt * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value : 1f);

                float initialPosDistance = Vector3.Distance(StanceTargetPosition, meleeInitialPos);
                float finalPosDistance = Vector3.Distance(StanceTargetPosition, meleeFinalPos);

                if (initialPosDistance > 0.001f && !DidHalfMeleeAnim)
                {
                    StanceRotation = meleeInitialQuaternion;
                    StanceTargetPosition = Vector3.Lerp(StanceTargetPosition, meleeInitialPos, PluginConfig.StanceTransitionSpeedMulti.Value * Mathf.Clamp(stanceMulti, 0.75f, 1f) * dt * 1.5f * ChonkerFactorStanceRotationModifier);
                }
                else
                {
                    DidHalfMeleeAnim = true;
                    StanceRotation = meleeFinalQuaternion;
                    StanceTargetPosition = Vector3.Lerp(StanceTargetPosition, meleeFinalPos, PluginConfig.StanceTransitionSpeedMulti.Value * Mathf.Clamp(stanceMulti, 0.75f, 1f) * dt * 2f * ChonkerFactorStanceRotationModifier);
                }
                if (StanceBlender.Value >= 1f && finalPosDistance <= 0.001f && !DidStanceWiggle)
                {
                    DoMeleeEffect();
                    DoWiggleEffects(player, pwa, fc.Weapon, new Vector3(-20f, -10f, -90f) * movementFactor, true, 1f, useGearSound: true);
                    DidStanceWiggle = true;
                }

                if (StanceBlender.Value >= 0.9f && DidHalfMeleeAnim)
                {
                    CanDoMeleeDetection = true;
                }

                if (StanceBlender.Value >= 1f && finalPosDistance <= 0.001f)
                {
                    CurrentStance = StoredStance;
                    StanceBlender.Target = 0f;
                }
            }
            else if (StanceBlender.Value > 0f && !HasResetMelee) //&& !IsLowReady && !IsActiveAiming && !IsHighReady && !IsShortStock && !isResettingActiveAim && !isResettingHighReady && !isResettingLowReady && !isResettingShortStock
            {
                CanDoMeleeDetection = false;
                _canResetDamping = false;
                IsResettingMelee = true;
                StanceRotationSpeed = 10f * stanceMulti * dt;
                StanceRotation = Quaternion.identity;
                StanceBlender.Speed = 15f * stanceMulti * (useThirdPersonStance ? PluginConfig.ThirdPersonPositionSpeed.Value : 1f);
            }
            else if (StanceBlender.Value == 0f && !HasResetMelee)
            {
                _doMeleeReset = true;
                if (!_canResetDamping)
                {
                    _doDampingTimer = true;
                }
                StanceRotation = Quaternion.identity;
                IsResettingMelee = false;
                HasResetMelee = true;
                DidHalfMeleeAnim = false;
            }
        }

        public void DoMeleeStanceBayonet(Player player, Player.FirearmController fc, bool isThirdPerson, EFT.Animations.ProceduralWeaponAnimation pwa, float dt, bool useThirdPersonStance, float stanceMulti, float resetErgoMulti, bool pauseStance, float movementFactor)
        {
            bool isDoingMelee = CurrentStance == EStance.Melee && !pwa.IsAiming && !pauseStance;
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

                if (StanceTargetPosition == meleeFinalPos && StanceBlender.Value >= 1f && !_canResetDamping)
                {
                    _doDampingTimer = true;
                }
                else if (StanceTargetPosition != meleeFinalPos || StanceBlender.Value < 1)
                {
                    _canResetDamping = false;
                }

                StanceRotationSpeed = 10f * Mathf.Clamp(stanceMulti, 0.8f, 1f) * dt * (useThirdPersonStance ? PluginConfig.ThirdPersonRotationSpeed.Value : 1f);

                float initialPosDistance = Vector3.Distance(StanceTargetPosition, meleeInitialPos);
                float finalPosDistance = Vector3.Distance(StanceTargetPosition, meleeFinalPos);

                if ((initialPosDistance > 0.001f && !DidHalfMeleeAnim))
                {
                    StanceRotation = meleeInitialQuaternion;
                    StanceTargetPosition = Vector3.Lerp(StanceTargetPosition, meleeInitialPos, PluginConfig.StanceTransitionSpeedMulti.Value * Mathf.Clamp(stanceMulti, 0.75f, 1f) * dt * 1.5f * ChonkerFactorStanceRotationModifier);
                }
                else
                {
                    DidHalfMeleeAnim = true;
                    if (!_isHoldingBackMelee)
                    {
                        StanceRotation = meleeFinalQuaternion;
                        StanceTargetPosition = Vector3.Lerp(StanceTargetPosition, meleeFinalPos, PluginConfig.StanceTransitionSpeedMulti.Value * Mathf.Clamp(stanceMulti, 0.75f, 1f) * dt * 2f * ChonkerFactorStanceRotationModifier);
                    }
                }

                StanceBlender.Speed = 50f * (useThirdPersonStance ? PluginConfig.ThirdPersonPositionSpeed.Value : 1f);

                if (StanceBlender.Value >= 0.9f && !DidStanceWiggle && !MeleeHitSomething && !_isHoldingBackMelee) // && finalPosDistance <= 0.001f
                {
                    DoMeleeEffect();
                    DoWiggleEffects(player, pwa, fc.Weapon, new Vector3(-20f, -10f, -90f) * movementFactor, true, 1f, useGearSound: true);
                    DidStanceWiggle = true;
                }

                if (StanceBlender.Value >= 0.9f && DidHalfMeleeAnim)
                {
                    CanDoMeleeDetection = true;
                }

                if (StanceBlender.Value >= 1f && finalPosDistance <= 0.001f)
                {
                    CurrentStance = StoredStance;
                    StanceBlender.Target = 0f;
                }
            }
            else if (StanceBlender.Value > 0f && !HasResetMelee) 
            {
                CanDoMeleeDetection = false;
                _canResetDamping = false;
                IsResettingMelee = true;
                StanceRotationSpeed = 10f * stanceMulti * dt;
                StanceRotation = Quaternion.identity;
                StanceBlender.Speed = 15f * stanceMulti * (useThirdPersonStance ? PluginConfig.ThirdPersonPositionSpeed.Value : 1f);
            }
            else if (StanceBlender.Value == 0f && !HasResetMelee)
            {
                _doMeleeReset = true;
                if (!_canResetDamping)
                {
                    _doDampingTimer = true;
                }
                StanceRotation = Quaternion.identity;
                IsResettingMelee = false;
                HasResetMelee = true;
                DidHalfMeleeAnim = false;
            }
        }

        public void DoPatrolStance(ProceduralWeaponAnimation pwa, Player player)
        {
            Vector3 patrolPos = CurrentStance != EStance.PatrolStance ? Vector3.zero : WeaponStateInstance.TreatAsPistol ? _pistolPatrolPos : _riflePatrolPos;
            _patrolPos = Vector3.Lerp(_patrolPos, patrolPos, 5.5f * Time.deltaTime);
            pwa.HandsContainer.WeaponRoot.localPosition += _patrolPos;

            Vector3 patrolRot = CurrentStance != EStance.PatrolStance ? Vector3.zero : WeaponStateInstance.TreatAsPistol ? _pistolPatrolRot : _riflePatrolRot;
            _patrolRot = Vector3.Lerp(_patrolRot, patrolRot, 5.5f * Time.deltaTime);

            Quaternion newRot = Quaternion.identity;
            newRot.x = _patrolRot.x;
            newRot.y = _patrolRot.y;
            newRot.z = _patrolRot.z;
            pwa.HandsContainer.WeaponRoot.localRotation *= newRot;

            if (Vector3.Distance(_patrolPos, Vector3.zero) <= 0.05f) FinishedUnPatrolStancing = true;
            else FinishedUnPatrolStancing = false;
        }

        ///
        //thanks and credit to lualeet's deadzone mod for this code, 0 jank compared to Realism's previous mounting system
        ///
        static void SetRotationWrapped(ref float yaw, ref float pitch)
        {
            // I prefer using (-180; 180) euler angle range over (0; 360)
            // However, wrapping the angles is easier with (0; 360), so temporarily cast it
            if (yaw < 0) yaw += 360;
            if (pitch < 0) pitch += 360;

            pitch %= 360;
            yaw %= 360;

            // Now cast it back
            if (yaw > 180) yaw -= 360;
            if (pitch > 180) pitch -= 360;
        }

        public void SetRotationClamped(ref float yaw, ref float pitch, float maxAngle)
        {
            Vector2 clampedVector
                = Vector2.ClampMagnitude(
                    new Vector2(yaw, pitch),
                    maxAngle
                );

            yaw = clampedVector.x;
            pitch = clampedVector.y;
        }

        public void UpdateAimSmoothed(ProceduralWeaponAnimation pwa, float deltaTime)
        {
            _mountAimSmoothed = Mathf.Lerp(_mountAimSmoothed, pwa.IsAiming ? 1f : 0f, deltaTime * 6f);
        }

        public void UpdateMountRotation(Vector2 currentYawPitch, float clamp)
        {
            Quaternion lastRotation = Quaternion.Euler(_lastMountYawPitch.x, _lastMountYawPitch.y, 0);
            Quaternion currentRotation = Quaternion.Euler(currentYawPitch.x, currentYawPitch.y, 0);

            _lastMountYawPitch = currentYawPitch;
            lastRotation = Quaternion.SlerpUnclamped(currentRotation, lastRotation, 0.115f);

            Vector3 delta = _makeQuaternionDelta(lastRotation, currentRotation).eulerAngles;

            _cumulativeMountYaw += delta.x;
            _cumulativeMountPitch += delta.y;

            SetRotationWrapped(ref _cumulativeMountYaw, ref _cumulativeMountPitch);
            SetRotationClamped(ref _cumulativeMountYaw, ref _cumulativeMountPitch, clamp);
        }

        public void ApplyPivotPoint(ProceduralWeaponAnimation pwa, Player player, float pivotPoint, float aimPivot)
        {
            float aimMultiplier = 1f - ((1f - aimPivot) * _mountAimSmoothed);

            Transform weaponRootAnim = pwa.HandsContainer.WeaponRootAnim;

            if (weaponRootAnim == null) return;

            weaponRootAnim.LocalRotateAround(Vector3.up * -pivotPoint, new Vector3( _cumulativeMountPitch * aimMultiplier, 0, _cumulativeMountYaw * aimMultiplier));

            // Not doing this messes up pivot for all offsets after this
            weaponRootAnim.LocalRotateAround(
                Vector3.up * pivotPoint,
                Vector3.zero
            );
        }

        public void MountingPivotUpdate(Player player, ProceduralWeaponAnimation pwa, float clamp, float deltaTime, float pivotPoint = 0.75f, float aimPivot = 0.25f)
        {
            Vector2 currentYawPitch = new(player.MovementContext.Yaw, player.MovementContext.Pitch);

            UpdateMountRotation(currentYawPitch, clamp);
            UpdateAimSmoothed(pwa, deltaTime);
            ApplyPivotPoint(pwa, player, pivotPoint, aimPivot);
        }

        static readonly System.Diagnostics.Stopwatch aimWatch = new();

        public float GetDeltaTime()
        {
            float deltaTime = aimWatch.Elapsed.Milliseconds / 1000f;
            aimWatch.Reset();
            aimWatch.Start();
            return deltaTime;
        }

        public void ToggleMounting(Player player, ProceduralWeaponAnimation pwa, Player.FirearmController fc)
        {
           /* if (player.IsInPronePose && WeaponStateInstance.BipodIsDeployed)
            {
                IsMounting = true;
            }*/
            if (IsMounting && PlayerStateInstance.IsMoving)
            {
                IsMounting = false;
            }
        }
    }
}

