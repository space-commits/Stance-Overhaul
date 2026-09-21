using BepInEx;
using StanceOverhaul.Controllers;
using StanceOverhaul.Patches;
using EFT;
using Unity;
using UnityEngine;
using static RealismCommonLib.Plugin;

namespace StanceOverhaul
{
    [BepInDependency(RealismCommonLib.Plugin.MOD_GUID)]
    [BepInPlugin(MOD_GUID, MOD_NAME, MOD_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static StanceController StanceControllerInstance;

        public const string MOD_GUID = "com.fontaine.stanceoverhaul";
        public const string MOD_NAME = "Fontaine-StanceOverhaul";
        public const string MOD_VERSION = "0.0.1";


        public static GameObject LeftHandMarkerGO;
        public static GripPose LeftHandMarkerGrip;

        void Awake()
        {
            PluginConfig.InitConfigBindings(Config);

            SubscribeToEvents();

            EnablePatches();
        }

        void EnablePatches()
        {
            //new TacticalReloadPatch().Enable();
            /*            new SprintPatch().Enable();
                        new DisableAimOnReloadPatch().Enable();*/
            /*            new ChangeScopePatch().Enable();*/
            /*        new MountingAndCollisionPatch().Enable();
                    new CollisionPatch().Enable();*/
            new SetFireModePatch().Enable();
            new OperateStationaryWeaponPatch().Enable();
            /*            new WeaponOverlapViewPatch().Enable();
                        new WeaponOverlappingPatch().Enable();*/
            /*            new ShouldMoveWeapCloserPatch().Enable();*/
            /*       new WeaponLengthPatch().Enable();*/
            /*      new InitTransformsPatch().Enable();*/
            /*       new ChangePosePatch().Enable();
                   new SetTiltPatch().Enable();*/

            //new ProcessEffectorsPatch().Enable();


            new ZeroAdjustmentsPatch().Enable();
            new SpringResetPatch().Enable();

            new SpringGetPatch().Enable();
            //new SpringGetRelativePatch().Enable();

            new UpdateWeaponVariablesPatch().Enable();

            new UpdateHipInaccuracyPatch().Enable();

            new ShouldMoveWeapCloserPatch().Enable();

            new ComplexRotationsPatch().Enable();

            new InitTransformsPatch().Enable();

            new HandIKPatch().Enable();


            /*            new ShiftWeaponRootPatch().Enable();
                        new HeadPatch().Enable();*/

            //new ApplyComplexRotationPatch().Enable();


            //new CameraPatch().Enable();
            /*
                        new CalibrateLocalPatch().Enable();
                        new CalibratePatch().Enable();
            */

            //new BlenderPatch().Enable();


            /*            new ScopeRotationPatch().Enable();



                        new ApplyAimingAlignmentPatch().Enable();*/


            //new ApplyPositionPatch().Enable();
            new SpringUpdatePatch().Enable();
            //new TacticalReloadMethodPatch().Enable();
            /*           new FOVPatch().Enable();
                       new IsAimingPatch().Enable();
                       new IsAimingPatch1().Enable();
                       new IsAimingPatch2().Enable();
                       new IsAimingPatch3().Enable();
                       new IsAimingPatch4().Enable();
                       new IsAimingPatch5().Enable();
                       new CamRecoilPatch().Enable();
                       new IntensityByAimingPatch().Enable();
                       new UpdateAimWeightPatch().Enable();*/
        }

        void SubscribeToEvents()
        {
            RealismCommonLib.Events.PlayerEvents.OnPlayerInitRef += AddStanceComponentsToPlayer;
        }

        void AddStanceComponentsToPlayer(Player player)
        {
            player.gameObject.AddComponent<StanceController>();

            LeftHandMarkerGO = new GameObject("ProceduralLeftHandTarget");
            LeftHandMarkerGO.transform.SetParent(player.gameObject.transform, false); //

            LeftHandMarkerGrip = LeftHandMarkerGO.AddComponent<GripPose>();
            LeftHandMarkerGrip.Hand = GripPose.EHand.Left;
            LeftHandMarkerGrip.GripType = GripPose.EGripType.Common;
            LeftHandMarkerGrip.DontCache = true;
            LeftHandMarkerGrip.transform.localPosition = Vector3.zero;
            LeftHandMarkerGrip.transform.localRotation =  Quaternion.identity;
        }
    }
}



