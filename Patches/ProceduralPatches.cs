using Comfort.Common;
using EFT;
using EFT.Animations;
using EFT.Ballistics;
using EFT.InventoryLogic;
using HarmonyLib;
using RealismCommonLib.Utils;
using SPT.Reflection.Patching;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using StanceOverhaul.Enums;
using static EFT.Player;
using static RealismCommonLib.Plugin;
using static RealismCommonLib.PluginRegistry;
using CollisionLayerClass = LayerMasksDataAbstractClass;
using ReloadClass = EFT.Player.FirearmController.GClass2037;

namespace StanceOverhaul.Patches
{
    public class ApplyComplexRotationPatch : ModulePatch
    {
        private static FieldInfo _aimSpeedField;
        private static FieldInfo _compensatoryField;
        private static FieldInfo _displacementStrField;
        private static FieldInfo _scopeRotationField;
        private static FieldInfo _weapTempRotationField;
        private static FieldInfo _weapTempPositionField;
        private static FieldInfo _isAimingField;
        private static FieldInfo _firearmControllerField;
        private static FieldInfo _playerField;

        static Vector3 _patrolPos = Vector3.zero;
        static Vector3 _patrolRot = Vector3.zero;

        protected override MethodBase GetTargetMethod()
        {
            _aimSpeedField = AccessTools.Field(typeof(EFT.Animations.ProceduralWeaponAnimation), "_aimingSpeed");
            _compensatoryField = AccessTools.Field(typeof(EFT.Animations.ProceduralWeaponAnimation), "_compensatoryScale");
            _displacementStrField = AccessTools.Field(typeof(EFT.Animations.ProceduralWeaponAnimation), "_displacementStr");
            _scopeRotationField = AccessTools.Field(typeof(EFT.Animations.ProceduralWeaponAnimation), "_targetScopeRotation");
            _weapTempPositionField = AccessTools.Field(typeof(EFT.Animations.ProceduralWeaponAnimation), "_temporaryPosition");
            _weapTempRotationField = AccessTools.Field(typeof(EFT.Animations.ProceduralWeaponAnimation), "_temporaryRotation");
            _isAimingField = AccessTools.Field(typeof(EFT.Animations.ProceduralWeaponAnimation), "_isAiming");
            _firearmControllerField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_firearmController");
            _playerField = AccessTools.Field(typeof(FirearmController), "_player");

            return typeof(EFT.Animations.ProceduralWeaponAnimation).GetMethod("ApplyComplexRotation", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void Postfix(EFT.Animations.ProceduralWeaponAnimation __instance, float dt, Vector3 ____vCameraTarget)
        {
            FirearmController firearmController = (FirearmController)_firearmControllerField.GetValue(__instance);
            if (firearmController == null)
            {
                return;
            }
            Player player = (Player)_playerField.GetValue(firearmController);
            if (player != null && player.IsYourPlayer && player.MovementContext.CurrentState.Name != EPlayerState.Stationary)
            {

                float aimSpeed = (float)_aimSpeedField.GetValue(__instance);
                float compensatoryScale = (float)_compensatoryField.GetValue(__instance);
                float displacementStr = (float)_displacementStrField.GetValue(__instance);
                Quaternion scopeRotation = (Quaternion)_scopeRotationField.GetValue(__instance);
                Vector3 weapTempPosition = (Vector3)_weapTempPositionField.GetValue(__instance);
                Quaternion weapTempRotation = (Quaternion)_weapTempRotationField.GetValue(__instance);
                bool isAiming = (bool)_isAimingField.GetValue(__instance);


                //this should be base position + stance position. PID would slot in more easily this way, as I'd modify the base position to align with camera.
/*                __instance.HandsContainer.WeaponRoot.localPosition = Plugin.StanceControllerInstance.StancePositionSpring.Get();
                __instance.HandsContainer.WeaponRoot.localRotation *= Quaternion.Euler(Plugin.StanceControllerInstance.StanceRotationSpring.Get());

*/

                /*       var stanceRot = Quaternion.Euler(Plugin.StanceControllerInstance.StanceRotationSpring.Get());*/

                /*                __instance.HandsContainer.WeaponRootAnim.SetPositionAndRotation(weapTempPosition + Plugin.StanceControllerInstance.StancePositionSpring.Get(), weapTempRotation * stanceRot);
                */


                /*        var _riflelPos = new Vector3(PluginConfig.test1.Value, PluginConfig.test2.Value, PluginConfig.test3.Value);
                        var _rifleRot = new Vector3(PluginConfig.test4.Value, PluginConfig.test5.Value, PluginConfig.test6.Value);

                        bool isPatrol = Plugin.StanceControllerInstance.CurrentStanceType == EStanceType.LeftShoulder;
                        Vector3 patrolPos = !isPatrol ? Vector3.zero : _riflelPos;
                        _patrolPos = Vector3.Lerp(_patrolPos, patrolPos, 2f * Time.deltaTime);
                        __instance.HandsContainer.WeaponRoot.localPosition = _patrolPos;

                        Vector3 patrolRot = !isPatrol ? Vector3.zero : _rifleRot;
                        _patrolRot = Vector3.Lerp(_patrolRot, patrolRot, 2f * Time.deltaTime);

                        Quaternion newRot = Quaternion.identity;
                        newRot.x = _patrolRot.x;
                        newRot.y = _patrolRot.y;
                        newRot.z = _patrolRot.z;
                        __instance.HandsContainer.WeaponRoot.localRotation *= newRot;*/

            }
        }
    }

    public class CalibratePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(ProceduralWeaponAnimation), nameof(ProceduralWeaponAnimation.Calibrate));
        }

        [PatchPrefix]
        private static bool PatchPrefix(ProceduralWeaponAnimation __instance)
        {
            return false;
        }
    }

    public class CalibrateLocalPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(ProceduralWeaponAnimation), nameof(ProceduralWeaponAnimation.CalculateLocalSightTarget));
        }

        [PatchPrefix]
        private static bool PatchPrefix(ProceduralWeaponAnimation __instance)
        {
            return false;
        }
    }

    public class SpringUpdatePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Spring), nameof(Spring.FixedUpdate));
        }

        [PatchPrefix]
        private static void PatchPrefix(Spring __instance, float dt, int nFixedFrames)
        {
            if (Plugin.StanceControllerInstance == null || !Plugin.StanceControllerInstance.AwakeRan) return;

            if (__instance == PlayerStateInstance.PWA.HandsContainer.HandsPosition)
                Plugin.StanceControllerInstance.StancePositionSpring.FixedUpdate(dt, nFixedFrames);

            if (__instance == PlayerStateInstance.PWA.HandsContainer.HandsRotation)
                Plugin.StanceControllerInstance.StanceRotationSpring.FixedUpdate(dt, nFixedFrames);

        }
    }

    public class SpringResetPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Spring), nameof(Spring.Reset));
        }

        [PatchPostfix]
        private static void PatchPostfix(Spring __instance)
        {
            if (Plugin.StanceControllerInstance == null || !Plugin.StanceControllerInstance.AwakeRan) return;

            if (__instance == PlayerStateInstance.PWA.HandsContainer.HandsPosition)
                Plugin.StanceControllerInstance.StancePositionSpring.Reset();

            if (__instance == PlayerStateInstance.PWA.HandsContainer.HandsRotation)
                Plugin.StanceControllerInstance.StanceRotationSpring.Reset();

        }
    }

    public class SpringGetPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Spring), nameof(Spring.Get));
        }

        [PatchPostfix]
        private static void Postfix(Spring __instance, ref Vector3 __result)
        {
            if (Plugin.StanceControllerInstance == null || !Plugin.StanceControllerInstance.AwakeRan) return;

            if (__instance == PlayerStateInstance.PWA.HandsContainer.HandsPosition)
            {
                __result += Plugin.StanceControllerInstance.StancePositionSpring.Get();
            }

            if (__instance == PlayerStateInstance.PWA.HandsContainer.HandsRotation)
            {
                __result += Plugin.StanceControllerInstance.StanceRotationSpring.Get();
            }
        }
    }

    public class SpringGetRelativePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Spring), nameof(Spring.GetRelative));
        }

        [PatchPostfix]
        private static void Postfix(Spring __instance, ref Vector3 __result)
        {
            if (Plugin.StanceControllerInstance == null || !Plugin.StanceControllerInstance.AwakeRan) return;

            if (__instance == PlayerStateInstance.PWA.HandsContainer.HandsPosition)
            {
                __result += Plugin.StanceControllerInstance.StancePositionSpring.GetRelative();
            }

            if (__instance == PlayerStateInstance.PWA.HandsContainer.HandsRotation)
            {
                __result += Plugin.StanceControllerInstance.StanceRotationSpring.GetRelative();
            }
        }
    }

    public class ProcessEffectorsPatch : ModulePatch
    {
        private static FieldInfo _playerField;
        private static FieldInfo _fcField;

        protected override MethodBase GetTargetMethod()
        {
            _playerField = AccessTools.Field(typeof(FirearmController), "_player");
            _fcField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_firearmController");
            return AccessTools.Method(typeof(ProceduralWeaponAnimation), nameof(ProceduralWeaponAnimation.ProcessEffectors));
        }

        [PatchPrefix]
        private static void PatchPrefix(ProceduralWeaponAnimation __instance, float deltaTime)
        {
            FirearmController firearmController = (FirearmController)_fcField.GetValue(__instance);
            if (firearmController == null) return;
            Player player = (Player)_playerField.GetValue(firearmController);
            if (player != null && player.IsYourPlayer)
            {
                Plugin.StanceControllerInstance.ProceduralUpdate(deltaTime, 0);
            }
        }
    }

    public class ZeroAdjustmentsPatch : ModulePatch
    {
        private static FieldInfo _blindfireStrengthField;
        private static FieldInfo _blindfireRotationField;
        private static PropertyInfo _overlappingBlindfireField;
        private static FieldInfo _blindfirePositionField;
        private static FieldInfo _firearmControllerField;
        private static FieldInfo _playerField;
        private static FieldInfo _fcField;

        private static Vector3 _targetPosition = Vector3.zero;

        protected override MethodBase GetTargetMethod()
        {
            _blindfireStrengthField = AccessTools.Field(typeof(EFT.Animations.ProceduralWeaponAnimation), "_blindfireStrength");
            _blindfireRotationField = AccessTools.Field(typeof(EFT.Animations.ProceduralWeaponAnimation), "_blindFireRotation");
            _overlappingBlindfireField = AccessTools.Property(typeof(EFT.Animations.ProceduralWeaponAnimation), "Single_3");
            _blindfirePositionField = AccessTools.Field(typeof(EFT.Animations.ProceduralWeaponAnimation), "_blindFirePosition");
            _firearmControllerField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_firearmController");
            _playerField = AccessTools.Field(typeof(FirearmController), "_player");
            _fcField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_firearmController");

            return AccessTools.Method(typeof(ProceduralWeaponAnimation), nameof(ProceduralWeaponAnimation.ZeroAdjustments));
        }

        /*     [PatchPrefix]
             private static bool PatchPrefix(ProceduralWeaponAnimation __instance, ref float ____blindfireStrength, ref Vector3 ____blindFirePosition, Vector3 ____blindFireRotation)
             {
                 FirearmController firearmController = (FirearmController)_fcField.GetValue(__instance);
                 if (firearmController == null) return false;
                 Player player = (Player)_playerField.GetValue(firearmController);
                 if (player != null && player.IsYourPlayer)
                 {

                     __instance.PositionZeroSum = Plugin.StanceControllerInstance.StancePositionSpring.Get();
                     __instance.RotationZeroSum = Plugin.StanceControllerInstance.StanceRotationSpring.Get();

                     __instance.PositionZeroSum.y += (__instance._shouldMoveWeaponCloser ? 0.05f : 0f);
                     __instance.RotationZeroSum.y += __instance.SmoothedTilt * __instance.PossibleTilt;

                     float value = __instance.BlindfireBlender.Value;
                     if (Mathf.Abs(value) > 0f)
                     {
                         ____blindfireStrength = ((Mathf.Abs(__instance.Pitch) < 45f) ? 1f : ((90f - Mathf.Abs(__instance.Pitch)) / 45f));
                         __instance.BlindFireEndPosition = ((value > 0f) ? __instance.BlindFireOffset : __instance.SideFireOffset);
                         __instance.BlindFireEndPosition *= ____blindfireStrength;
                     }
                     else
                     {
                         ____blindFirePosition = Vector3.zero;
                         ____blindFireRotation = Vector3.zero;
                     }
                     __instance.HandsContainer.HandsPosition.Zero = __instance.PositionZeroSum + ____blindfireStrength * __instance.Single_3 * ____blindFirePosition;
                     __instance.HandsContainer.HandsRotation.Zero = __instance.RotationZeroSum;

                     Plugin.StanceControllerInstance.StancePositionSpring.Zero = Plugin.StanceControllerInstance.StancePosition + ____blindfireStrength * __instance.Single_3 * ____blindFirePosition;
                     Plugin.StanceControllerInstance.StanceRotationSpring.Zero = Plugin.StanceControllerInstance.StanceRotation;

                     return false;
                 }

                 return true;
             }*/


        /*   [PatchPrefix]
           private static bool PatchPrefix(EFT.Animations.ProceduralWeaponAnimation __instance)
           {
               FirearmController firearmController = (FirearmController)_firearmControllerField.GetValue(__instance);
               if (firearmController == null)
               {
                   return true;
               }
               Player player = (Player)_playerField.GetValue(firearmController);
               if (player != null && player.IsYourPlayer) // player.MovementContext.CurrentState.Name != EPlayerState.Stationary && player.IsYourPlayer
               {
                   float collidingModifier = (float)_overlappingBlindfireField.GetValue(__instance);
                   Vector3 blindfirePosition = (Vector3)_blindfirePositionField.GetValue(__instance);

                   __instance.PositionZeroSum.y = (__instance._shouldMoveWeaponCloser ? 0.05f : 0f);
                   __instance.RotationZeroSum.y = __instance.SmoothedTilt * __instance.PossibleTilt;
                   float stanceBlendValue = PluginConfig.test17.Value;
                   float blindFireBlendValue = __instance.BlindfireBlender.Value;
                   if (Mathf.Abs(blindFireBlendValue) > 0f)
                   {
                       float strength = ((Mathf.Abs(__instance.Pitch) < 45f) ? 1f : ((90f - Mathf.Abs(__instance.Pitch)) / 45f));
                       _blindfireStrengthField.SetValue(__instance, strength);
                       __instance.BlindFireEndPosition = ((blindFireBlendValue > 0f) ? __instance.BlindFireOffset : __instance.SideFireOffset);
                       __instance.BlindFireEndPosition *= strength;
                   }
                   else
                   {
                       _blindfirePositionField.SetValue(__instance, Vector3.zero);
                       _blindfireRotationField.SetValue(__instance, Vector3.zero);
                   }

                   if (Mathf.Abs(stanceBlendValue) > 0f)
                   {
                       float strength = ((Mathf.Abs(__instance.Pitch) < 45f) ? 1f : ((90f - Mathf.Abs(__instance.Pitch)) / 45f));
                       _blindfireStrengthField.SetValue(__instance, strength);
                       _targetPosition = Plugin.StanceControllerInstance.StancePositionSpring.Get() * stanceBlendValue;
                       __instance.HandsContainer.HandsPosition.Zero = __instance.PositionZeroSum + (float)_blindfireStrengthField.GetValue(__instance) * _targetPosition;
                       __instance.HandsContainer.HandsRotation.Zero = __instance.RotationZeroSum;
                       return false;
                   }
                   else
                   {
                       _targetPosition = Vector3.zero;
                   }

                   __instance.HandsContainer.HandsPosition.Zero = __instance.PositionZeroSum + _targetPosition + (Vector3)_blindfirePositionField.GetValue(__instance) * (float)_blindfireStrengthField.GetValue(__instance) * collidingModifier;
                   __instance.HandsContainer.HandsRotation.Zero = __instance.RotationZeroSum;
                   return false;
               }
               return true;
           }*/

        [PatchPostfix]
        private static void PatchPostfix(ProceduralWeaponAnimation __instance)
        {
            FirearmController firearmController = (FirearmController)_fcField.GetValue(__instance);
            if (firearmController == null) return;
            Player player = (Player)_playerField.GetValue(firearmController);
            if (player != null && player.IsYourPlayer)
            {
                Plugin.StanceControllerInstance.StancePositionSpring.Zero = Plugin.StanceControllerInstance.StancePosition;
                Plugin.StanceControllerInstance.StanceRotationSpring.Zero = Plugin.StanceControllerInstance.StanceRotation;

            }

        }
    }

    public class TacticalReloadMethodPatch : ModulePatch
    {
        private static FieldInfo _playerField;
        private static FieldInfo _fcField;

        protected override MethodBase GetTargetMethod()
        {
            _playerField = AccessTools.Field(typeof(FirearmController), "_player");
            _fcField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_firearmController");
            return AccessTools.Method(typeof(ProceduralWeaponAnimation), nameof(ProceduralWeaponAnimation.UpdateTacticalReload));
        }

        [PatchPrefix]
        private static bool PatchPrefix(ProceduralWeaponAnimation __instance)
        {
            FirearmController firearmController = (FirearmController)_fcField.GetValue(__instance);
            if (firearmController == null) return true;
            Player player = (Player)_playerField.GetValue(firearmController);
            if (player != null && player.IsYourPlayer)
            {
                return false;
            }
            return true;
        }
    }

    public class ApplyPositionPatch : ModulePatch
    {
        private static FieldInfo _playerField;
        private static FieldInfo _fcField;

        protected override MethodBase GetTargetMethod()
        {
            _playerField = AccessTools.Field(typeof(FirearmController), "_player");
            _fcField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_firearmController");
            return AccessTools.Method(typeof(ProceduralWeaponAnimation), nameof(ProceduralWeaponAnimation.ApplyPosition));
        }

        [PatchPrefix]
        private static bool PatchPrefix(ProceduralWeaponAnimation __instance)
        {
            FirearmController firearmController = (FirearmController)_fcField.GetValue(__instance);
            if (firearmController == null) return true;
            Player player = (Player)_playerField.GetValue(firearmController);
            if (player != null && player.IsYourPlayer)
            {
                __instance.HandsContainer.WeaponRootAnim.localPosition =
                    __instance.HandsContainer.HandsPosition.Get() +
                     Plugin.StanceControllerInstance.StancePositionSpring.Get() +
                    __instance.Shootingg.CurrentRecoilEffect.GetHandPositionRecoil() +
                    __instance.Shootingg.CurrentRecoilEffect.WeaponRecoilEffect.GetPositionRecoil();
              
                if (__instance.IsGrenadeLauncher)
                {
                    __instance.UpdateWeaponBoneByLauncherWeaponBone();
                }

                return false;
            }
            return true;
        }
    }

    public class BlenderPatch : ModulePatch
    {
        private static FieldInfo _playerField;
        private static FieldInfo _fcField;

        protected override MethodBase GetTargetMethod()
        {
            _playerField = AccessTools.Field(typeof(FirearmController), "_player");
            _fcField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_firearmController");
            return AccessTools.Method(typeof(ProceduralWeaponAnimation), nameof(ProceduralWeaponAnimation.BlendAnimatorPose));
        }

        [PatchPrefix]
        private static bool PatchPrefix(ProceduralWeaponAnimation __instance, float dt)
        {
            FirearmController firearmController = (FirearmController)_fcField.GetValue(__instance);
            if (firearmController == null) return true;
            Player player = (Player)_playerField.GetValue(firearmController);
            if (player != null && player.IsYourPlayer)
            {
                for (int i = __instance.ActiveBlends.Count - 1; i >= 0; i--)
                {
                    ValueTuple<AnimatorPose, float, bool> valueTuple = __instance.ActiveBlends[i];
                    AnimatorPose item = valueTuple.Item1;
                    float num = valueTuple.Item2;
                    bool item2 = valueTuple.Item3;
                    dt *= __instance.WeaponFlipSpeed;
                    num += (item2 ? dt : (-dt));
                    if (num < 0f)
                    {
                        __instance.ActiveBlends.RemoveAt(i);
                        __instance.method_17();
                    }
                    else
                    {
                        float num2 = item.Blend.Evaluate(num);
                        __instance.HandsContainer.HandsPosition.Zero += item.Position * num2;
                        __instance.HandsContainer.HandsRotation.Zero += item.Rotation * num2;
                        Plugin.StanceControllerInstance.StancePositionSpring.Zero += item.Position * num2;
                        Plugin.StanceControllerInstance.StanceRotationSpring.Zero += item.Rotation * num2;
                        __instance.ActiveBlends[i] = new ValueTuple<AnimatorPose, float, bool>(item, Mathf.Min(num, item.Blend.GetDuration()), item2);
                    }
                }

                return false;
            }

            return true;
        }
    }

    public class ApplyAimingAlignmentPatch : ModulePatch
    {
        private static FieldInfo _playerField;
        private static FieldInfo _fcField;

        protected override MethodBase GetTargetMethod()
        {
            _playerField = AccessTools.Field(typeof(FirearmController), "_player");
            _fcField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_firearmController");
            return AccessTools.Method(typeof(ProceduralWeaponAnimation), nameof(ProceduralWeaponAnimation.ApplyAimingAlignment));
        }

        [PatchPrefix]
        private static bool PatchPrefix(ProceduralWeaponAnimation __instance)
        {
            FirearmController firearmController = (FirearmController)_fcField.GetValue(__instance);
            if (firearmController == null) return false;
            Player player = (Player)_playerField.GetValue(firearmController);
            if (player != null && player.IsYourPlayer)
            {
                return false;
            }
            return true;
        }
    }

    public class FOVPatch : ModulePatch
    {
        private static FieldInfo _playerField;
        private static FieldInfo _fcField;

        protected override MethodBase GetTargetMethod()
        {
            _playerField = AccessTools.Field(typeof(FirearmController), "_player");
            _fcField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_firearmController");
            return typeof(ProceduralWeaponAnimation).GetMethod("set_IsAiming", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPrefix]
        private static bool PatchPrefix(ProceduralWeaponAnimation __instance, ref int fov, ref bool ____isAiming, GInterface200 ____firearmAnimationData)
        {
            FirearmController firearmController = (FirearmController)_fcField.GetValue(__instance);
            if (firearmController == null) return false;
            Player player = (Player)_playerField.GetValue(firearmController);
            if (player != null && player.IsYourPlayer)
            {

                if (__instance.PointOfView == EPointOfView.FirstPerson && ____firearmAnimationData != null)
                {
                    if (__instance.AimIndex < __instance.ScopeAimTransforms.Count && !____firearmAnimationData.MouseLookControl)
                    {
                        float x = __instance.Single_2;
             /*           if (__instance.IsAiming)
                        {
                            x = (__instance.CurrentScope.IsOptic ? 35f : (__instance.Single_2 - 15f));
                        }*/
                        CameraClass.Instance.SetFov(x, 1f, true);
                    }
                    __instance.method_1();
                    return false;
                }

                return false;
            }
            return true;
        }
    }

    public class IsAimingPatch : ModulePatch
    {
        private static FieldInfo _playerField;
        private static FieldInfo _fcField;

        protected override MethodBase GetTargetMethod()
        {
            _playerField = AccessTools.Field(typeof(FirearmController), "_player");
            _fcField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_firearmController");
            return typeof(ProceduralWeaponAnimation).GetMethod("set_IsAiming", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPrefix]
        private static bool PatchPrefix(ProceduralWeaponAnimation __instance, ref bool value, ref bool ____isAiming)
        {
            FirearmController firearmController = (FirearmController)_fcField.GetValue(__instance);
            if (firearmController == null) return false;
            Player player = (Player)_playerField.GetValue(firearmController);
            if (player != null && player.IsYourPlayer)
            {
                ____isAiming = value;
     /*           __instance.CameraSmoothBlender.ChangeValue(this.CameraSmoothSteady, 0f);
                if (__instance._aimSwayStrength > 0f)
                {
                    __instance._aimSwayBlender.Value = 1f;
                    __instance._aimSwayDirection = this._aimSwayStrength * this.IntensityByPoseLevel * new Vector3(Random.Range(this.AimSwayMin.x, this.AimSwayMax.x), Random.Range(this.AimSwayMin.y, this.AimSwayMax.y), Random.Range(this.AimSwayMin.z, this.AimSwayMax.z));
                }
                if (this._isAiming && this._buffInfo.StiffDraw)
                {
                    this.Breath.StiffUntill = Time.time + 3f;
                }
                this.method_23(false);
                if (this._isAiming)
                {
                    this.method_13();
                    this.method_14();
                    this.method_2();
                    return;
                }*/
                return false;
            }
            return true;
        }
    }

    public class ScopeRotationPatch : ModulePatch
    {
        private static FieldInfo _playerField;
        private static FieldInfo _fcField;

        protected override MethodBase GetTargetMethod()
        {
            _playerField = AccessTools.Field(typeof(FirearmController), "_player");
            _fcField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_firearmController");
            return AccessTools.Method(typeof(ProceduralWeaponAnimation), nameof(ProceduralWeaponAnimation.method_13));
        }

        [PatchPrefix]
        private static bool PatchPrefix(ProceduralWeaponAnimation __instance)
        {
            FirearmController firearmController = (FirearmController)_fcField.GetValue(__instance);
            if (firearmController == null) return false;
            Player player = (Player)_playerField.GetValue(firearmController);
            if (player != null && player.IsYourPlayer)
            {
                __instance.ResetScopeRotation();
                return false;
            }
            return true;
        }
    }

    

    public class ShiftWeaponRootPatch : ModulePatch
    {

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(PlayerBones), nameof(PlayerBones.ShiftWeaponRoot));
        }

        [PatchPrefix]
        private static bool PatchPrefix()
        {
            return false;
        }
    }

    public class HeadPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(PlayerBones), nameof(PlayerBones.RotateHead));
        }

        [PatchPrefix]
        private static bool PatchPrefix()
        {
            return false;
        }
    }

    public class IsAimingPatch1 : ModulePatch
    {
        private static FieldInfo _playerField;
        private static FieldInfo _fcField;

        protected override MethodBase GetTargetMethod()
        {
            _playerField = AccessTools.Field(typeof(FirearmController), "_player");
            _fcField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_firearmController");
            return AccessTools.Method(typeof(ProceduralWeaponAnimation), nameof(ProceduralWeaponAnimation.method_13));
        }

        [PatchPrefix]
        private static bool PatchPrefix(ProceduralWeaponAnimation __instance)
        {
            FirearmController firearmController = (FirearmController)_fcField.GetValue(__instance);
            if (firearmController == null) return false;
            Player player = (Player)_playerField.GetValue(firearmController);
            if (player != null && player.IsYourPlayer)
            {
                return false;
            }
            return true;
        }
    }

    public class IsAimingPatch4 : ModulePatch
    {
        private static FieldInfo _playerField;
        private static FieldInfo _fcField;

        protected override MethodBase GetTargetMethod()
        {
            _playerField = AccessTools.Field(typeof(FirearmController), "_player");
            _fcField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_firearmController");
            return AccessTools.Method(typeof(ProceduralWeaponAnimation), nameof(ProceduralWeaponAnimation.method_17));
        }

        [PatchPrefix]
        private static bool PatchPrefix(ProceduralWeaponAnimation __instance)
        {
            FirearmController firearmController = (FirearmController)_fcField.GetValue(__instance);
            if (firearmController == null) return false;
            Player player = (Player)_playerField.GetValue(firearmController);
            if (player != null && player.IsYourPlayer)
            {
                return false;
            }
            return true;
        }
    }

    public class IsAimingPatch5 : ModulePatch
    {
        private static FieldInfo _playerField;
        private static FieldInfo _fcField;

        protected override MethodBase GetTargetMethod()
        {
            _playerField = AccessTools.Field(typeof(FirearmController), "_player");
            _fcField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_firearmController");
            return AccessTools.Method(typeof(ProceduralWeaponAnimation), nameof(ProceduralWeaponAnimation.UpdateAimWeight));
        }

        [PatchPrefix]
        private static bool PatchPrefix(ProceduralWeaponAnimation __instance, float deltaTime, float ____aimingWeight, float ____currentScopeWeight)
        {
            FirearmController firearmController = (FirearmController)_fcField.GetValue(__instance);
            if (firearmController == null) return false;
            Player player = (Player)_playerField.GetValue(firearmController);
            if (player != null && player.IsYourPlayer)
            {
                float num = __instance.CameraSmoothOut;
                int num2 = 0;
                ____aimingWeight = Mathf.Lerp(____aimingWeight, (float)num2, num * deltaTime);
                ____currentScopeWeight = Mathf.Lerp(____currentScopeWeight, 1f, num * deltaTime);
                return false;
            }
            return true;
        }
    }

    public class IsAimingPatch2 : ModulePatch
    {
        private static FieldInfo _playerField;
        private static FieldInfo _fcField;

        protected override MethodBase GetTargetMethod()
        {
            _playerField = AccessTools.Field(typeof(FirearmController), "_player");
            _fcField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_firearmController");
            return AccessTools.Method(typeof(ProceduralWeaponAnimation), nameof(ProceduralWeaponAnimation.method_14));
        }

        [PatchPrefix]
        private static bool PatchPrefix(ProceduralWeaponAnimation __instance)
        {
            FirearmController firearmController = (FirearmController)_fcField.GetValue(__instance);
            if (firearmController == null) return false;
            Player player = (Player)_playerField.GetValue(firearmController);
            if (player != null && player.IsYourPlayer)
            {
                return false;
            }
            return true;
        }
    }

    public class IsAimingPatch3 : ModulePatch
    {
        private static FieldInfo _playerField;
        private static FieldInfo _fcField;

        protected override MethodBase GetTargetMethod()
        {
            _playerField = AccessTools.Field(typeof(FirearmController), "_player");
            _fcField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_firearmController");
            return AccessTools.Method(typeof(ProceduralWeaponAnimation), nameof(ProceduralWeaponAnimation.method_12));
        }

        [PatchPrefix]
        private static bool PatchPrefix(ProceduralWeaponAnimation __instance)
        {
            FirearmController firearmController = (FirearmController)_fcField.GetValue(__instance);
            if (firearmController == null) return false;
            Player player = (Player)_playerField.GetValue(firearmController);
            if (player != null && player.IsYourPlayer)
            {
                return false;
            }
            return true;
        }
    }

    public class UpdateAimWeightPatch : ModulePatch
    {
        private static FieldInfo _playerField;
        private static FieldInfo _fcField;

        protected override MethodBase GetTargetMethod()
        {
            _playerField = AccessTools.Field(typeof(FirearmController), "_player");
            _fcField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_firearmController");
            return AccessTools.Method(typeof(ProceduralWeaponAnimation), nameof(ProceduralWeaponAnimation.UpdateAimWeight));
        }

        [PatchPrefix]
        private static bool PatchPrefix(ProceduralWeaponAnimation __instance, float deltaTime, ref float ____aimingWeight, ref float ____currentScopeWeight)
        {
            FirearmController firearmController = (FirearmController)_fcField.GetValue(__instance);
            if (firearmController == null) return false;
            Player player = (Player)_playerField.GetValue(firearmController);
            if (player != null && player.IsYourPlayer)
            {
                float num = __instance.CameraSmoothOut;
                int num2 = 0;
                ____aimingWeight = Mathf.Lerp(____aimingWeight, (float)num2, num * deltaTime);
                ____currentScopeWeight = Mathf.Lerp(____currentScopeWeight, 1f, num * deltaTime);
                return false;
            }
            return true;
        }
    }

    public class CamRecoilPatch : ModulePatch
    {
        private static FieldInfo _cameraRecoilField;
        private static FieldInfo _cameraRecoilRotateField;
        private static FieldInfo _prevCameraTargetField;
        private static FieldInfo _headRotationField;
        private static FieldInfo _playerField;
        private static FieldInfo _fcField;
        private static float _camRecoilLerpTempSpeed = 1f;

        protected override MethodBase GetTargetMethod()
        {
            _cameraRecoilField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_cameraRecoilLerpTempSpeed");
            _cameraRecoilRotateField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_currentRecoilCameraRotate");
            _prevCameraTargetField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_previousCameraTargetRotation");
            _headRotationField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_headRotationVec");
            _playerField = AccessTools.Field(typeof(FirearmController), "_player");
            _fcField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_firearmController");
            return typeof(ProceduralWeaponAnimation).GetMethod("method_19", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPrefix]
        private static bool PatchPrefix(ProceduralWeaponAnimation __instance, float deltaTime)
        {
            FirearmController firearmController = (FirearmController)_fcField.GetValue(__instance);
            if (firearmController == null) return false;
            Player player = (Player)_playerField.GetValue(firearmController);
            if (player != null && player.IsYourPlayer && player.MovementContext.CurrentState.Name != EPlayerState.Stationary)
            {
                float _cameraRecoilLerpTempSpeed = (float)_cameraRecoilField.GetValue(__instance);
                Quaternion _currentRecoilCameraRotate = (Quaternion)_cameraRecoilRotateField.GetValue(__instance);
                Quaternion _previousCameraTargetRotation = (Quaternion)_prevCameraTargetField.GetValue(__instance);

                __instance.HandsContainer.CameraRotation.ReturnSpeed = 0.2f;
                __instance.HandsContainer.CameraRotation.Damping = 0.55f;

                Vector3 _headRotationVec = (Vector3)_headRotationField.GetValue(__instance);
                if (_headRotationVec != Vector3.zero)
                {
                    return false;
                }
                Vector3 current = __instance.Shootingg.CurrentRecoilEffect.HandRotationRecoilEffect.Current;
                bool autoFireOn;
                if (((autoFireOn = (__instance.Shootingg.CurrentRecoilEffect.HandRotationRecoilEffect as NewRotationRecoilProcess).AutoFireOn) & __instance.IsAiming) && current != Vector3.zero)
                {
                    if (!__instance.Shootingg.CurrentRecoilEffect.HandRotationRecoilEffect.StableOn)
                    {
                        Quaternion baseLocalRotation = __instance.HandsContainer.CameraTransform.localRotation;
                        baseLocalRotation.y = 0f;
                        /*               Quaternion rhs = Quaternion.Euler(current);*/
                        Quaternion newLocalRotation = baseLocalRotation; // * rhs //bsg uses this mmodifer
                        _camRecoilLerpTempSpeed = Mathf.Clamp(_camRecoilLerpTempSpeed + __instance.CameraToWeaponAngleStep * deltaTime, __instance.CameraToWeaponAngleSpeedRange.x, __instance.CameraToWeaponAngleSpeedRange.y) * 2f;
                        Quaternion newRecoilRotation = Quaternion.Lerp(_currentRecoilCameraRotate, newLocalRotation, _camRecoilLerpTempSpeed);
                        __instance.HandsContainer.CameraTransform.localRotation = newRecoilRotation;
                        _prevCameraTargetField.SetValue(__instance, newLocalRotation);
                        _cameraRecoilRotateField.SetValue(__instance, newRecoilRotation);
                        return false;
                    }
                    Quaternion previousCameraTargetRotation = _previousCameraTargetRotation;
                    previousCameraTargetRotation.z = __instance.HandsContainer.CameraTransform.localRotation.z;
                    _camRecoilLerpTempSpeed = Mathf.Clamp(_camRecoilLerpTempSpeed + __instance.CameraToWeaponAngleStep * deltaTime, __instance.CameraToWeaponAngleSpeedRange.x, __instance.CameraToWeaponAngleSpeedRange.y) * 2f;
                    Quaternion newCameraRotation = Quaternion.Lerp(_currentRecoilCameraRotate, previousCameraTargetRotation, _camRecoilLerpTempSpeed);
                    __instance.HandsContainer.CameraTransform.localRotation = newCameraRotation;
                    _cameraRecoilRotateField.SetValue(__instance, newCameraRotation);
                    return false;
                }
                else
                {
                    /*     if (!autoFireOn & __instance.IsAiming)
                         {
                             Quaternion localCameraRotation = __instance.HandsContainer.CameraTransform.localRotation;
                             localCameraRotation.y = 0f;
                             *//*              Quaternion rhs = Quaternion.Euler(current);*//*
                             Quaternion cameraLocalRotaitonModified = localCameraRotation; // * rhs //bsg uses this modifer
                             _camRecoilLerpTempSpeed = Mathf.Clamp(_camRecoilLerpTempSpeed - __instance.CameraToWeaponAngleStep * deltaTime, __instance.CameraToWeaponAngleSpeedRange.x, __instance.CameraToWeaponAngleSpeedRange.y);
                             Quaternion newCameraRotaiton = Quaternion.Lerp(_currentRecoilCameraRotate, cameraLocalRotaitonModified, _camRecoilLerpTempSpeed * 2f); //bsg does not multi by 2, can't remember why I do this :')
                             __instance.HandsContainer.CameraTransform.localRotation = newCameraRotaiton;
                             _prevCameraTargetField.SetValue(__instance, cameraLocalRotaitonModified);
                             _cameraRecoilRotateField.SetValue(__instance, newCameraRotaiton);
                             return false;
                         }*/
                    Quaternion localRotation3 = __instance.HandsContainer.CameraTransform.localRotation;
                    var tempSpeed = Mathf.Clamp(_cameraRecoilLerpTempSpeed - __instance.CameraToWeaponAngleStep * deltaTime, __instance.CameraToWeaponAngleSpeedRange.x, __instance.CameraToWeaponAngleSpeedRange.y);
                    _cameraRecoilField.SetValue(__instance, tempSpeed);
                     Quaternion quaternion8 = Quaternion.Lerp(_currentRecoilCameraRotate, localRotation3, __instance.CameraToWeaponAngleSpeedRange.y);
                    __instance.HandsContainer.CameraTransform.localRotation = quaternion8;
                    _prevCameraTargetField.SetValue(__instance, localRotation3);
                    _cameraRecoilRotateField.SetValue(__instance, quaternion8);
                    return false;
                }
            }
            return false;
        }
    }

    public class IntensityByAimingPatch : ModulePatch
    {
        private static FieldInfo _playerField;
        private static FieldInfo _fcField;

        protected override MethodBase GetTargetMethod()
        {
            _playerField = AccessTools.Field(typeof(FirearmController), "_player");
            _fcField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_firearmController");
            return typeof(ProceduralWeaponAnimation).GetMethod("get_IntensityByAiming", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPrefix]
        private static bool PatchPrefix(ProceduralWeaponAnimation __instance, ref float __result)
        {
            FirearmController firearmController = (FirearmController)_fcField.GetValue(__instance);
            if (firearmController == null) return false;
            Player player = (Player)_playerField.GetValue(firearmController);
            if (player != null && player.IsYourPlayer)
            {
                __result = 1f;
            }
            return true;
        }
    }

    //TODO: move to common lib to allow other modules to patch this
    public class TacticalReloadPatch : ModulePatch
    {
        private static FieldInfo _playerField;
        private static FieldInfo _fcField;

        protected override MethodBase GetTargetMethod()
        {
            _playerField = AccessTools.Field(typeof(FirearmController), "_player");
            _fcField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_firearmController");
            return typeof(ProceduralWeaponAnimation).GetMethod("get_TacticalReload", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPrefix]
        private static bool PatchPrefix(ProceduralWeaponAnimation __instance, ref bool __result)
        {
            FirearmController firearmController = (FirearmController)_fcField.GetValue(__instance);
            if (firearmController == null) return false;
            Player player = (Player)_playerField.GetValue(firearmController);
            if (player != null && player.IsYourPlayer)
            {
             /*   if (Plugin.StanceControllerInstance.IsMounting && WeaponStateInstance.BipodIsDeployed && Plugin.StanceControllerInstance.BracingDirection == EBracingDirection.Top)
                {
                    __result = true;
                    return false;
                }*/
            }
            return true;
        }
    }

    //TODO: move to common lib to allow other modules to patch this
    //Needed to block sprint animation when doing bayonet charge
    class SprintPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(PlayerAnimator).GetMethod("EnableSprint", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPrefix]
        private static bool Prefix(PlayerAnimator __instance, bool enabled)
        {
     /*       if (enabled && Plugin.StanceControllerInstance.CanDoMeleeDetection && WeaponStateInstance.HasBayonet && Plugin.StanceControllerInstance.IsReadyForBayonetCharge)
            {
                return false;
            }*/
            return true;
        }
    }

    //TODO: move to common lib to allow other modules to patch this
    //Allow reoading while aiming if mounting

    public class DisableAimOnReloadPatch : ModulePatch
    {
        private static FieldInfo _playerField;
        protected override MethodBase GetTargetMethod()
        {
            _playerField = AccessTools.Field(typeof(ReloadClass), "Player_0");
            return typeof(ReloadClass).GetMethod("DisableAimingOnReload");
        }

        [PatchPrefix]
        private static bool PatchPreFix(ReloadClass __instance)
        {
            Player player = (Player)_playerField.GetValue(__instance);
 /*           if (player.IsYourPlayer && Plugin.StanceControllerInstance.IsMounting)
            {
                return false;
            }*/
            return true;
        }
    }

    //TODO: condier simplification or moving to common lib
    //force scope swtich and block when using canted sight while mounting
    public class ChangeScopePatch : ModulePatch
    {
        private static FieldInfo _playerField;
        private static FieldInfo _weaponManagerClassField;
        protected override MethodBase GetTargetMethod()
        {
            _playerField = AccessTools.Field(typeof(FirearmController), "_player");
            _weaponManagerClassField = AccessTools.Field(typeof(FirearmController), "weaponManagerClass");
            return typeof(Player.FirearmController).GetMethod("ChangeAimingMode", new Type[] { });
        }

        [PatchPrefix]
        private static bool PatchPreFix(FirearmController __instance)
        {
            Player player = (Player)_playerField.GetValue(__instance);
   /*         if (player.IsYourPlayer && PluginConfig.OverrideMounting.Value)
            {
                if (WeaponStateInstance.BipodIsDeployed && Plugin.StanceControllerInstance.IsMounting)
                {
                    WeaponManagerClass weaponManagerClass = (WeaponManagerClass)_weaponManagerClassField.GetValue(__instance);
                    var scopeIndex = __instance.Item.AimIndex.Value + 1;
                    if (scopeIndex >= weaponManagerClass.ProceduralWeaponAnimation.ScopeAimTransforms.Count)
                    {
                        scopeIndex = 0;
                    }
                    while (scopeIndex != __instance.Item.AimIndex.Value && Mathf.Abs(weaponManagerClass.ProceduralWeaponAnimation.ScopeAimTransforms[scopeIndex].Rotation) >= EFTHardSettings.Instance.SCOPE_ROTATION_THRESHOLD)
                    {
                        scopeIndex++;
                        if (scopeIndex >= weaponManagerClass.ProceduralWeaponAnimation.ScopeAimTransforms.Count)
                        {
                            scopeIndex = 0;
                        }
                    }
                    if (scopeIndex == __instance.Item.AimIndex.Value || Mathf.Abs(weaponManagerClass.ProceduralWeaponAnimation.ScopeAimTransforms[scopeIndex].Rotation) >= EFTHardSettings.Instance.SCOPE_ROTATION_THRESHOLD)
                    {
                        return false;
                    }
                    __instance.Item.AimIndex.Value = scopeIndex;
                    __instance.UpdateSensitivity();
                    player.RaiseSightChangedEvent(player.ProceduralWeaponAnimation.CurrentAimingMod);
                    return false;
                }
            }*/
            return true;
        }
    }

    //TODO: move logic to own class
    //Entry point for moutning and collision overrides 
    public class MountingAndCollisionPatch : ModulePatch
    {
        private static FieldInfo _playerField;
        private static FieldInfo _fcField;
        private static FieldInfo _blendField;
        private static FieldInfo _smoothInField;
        private static FieldInfo _smoothOutField;
        private static float _mountClamp = 0f;
        private static float _collidingModifier = 1f;
        private static float _finalStateEndTimer = 0f;
        private static float _finalStateDelayTimer = 0f;
        private static bool _delayFinalState = false;
        private static float _adsResetTimer = 0f;
        private static float _collisionOverrideTimer = 0f;
        private static float _collisionTimer = 0f;
        private static float _collisionResetTimer = 0f;
        private static float _previousOverlapValue = 0f;
        private static float _currentOverlapValue = 0f;
        private static float _smoothedOverlapValue = 0f;
        private static float _lastDistance = 0f;
        private static bool _isColliding = false;
        private static bool _wasInFinalState = false;
        private static float _stanceFactor = 0f;
        private static float _stanceInverseFactor = 0f;

        private static Vector3 _initialPos = new Vector3(0.01f, -0.075f, -0.13f);
        private static Vector3 _initialRot = new Vector3(-0.025f, 0.005f, 0.005f);
        private static Vector3 _finalPos = Vector3.zero;
        private static Vector3 _finalRot = Vector3.zero;

        private static Vector3 _collisionPos = Vector3.zero;
        private static Vector3 _collisionRot = Vector3.zero;

        protected override MethodBase GetTargetMethod()
        {
            _playerField = AccessTools.Field(typeof(FirearmController), "_player");
            _fcField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_firearmController");
            _blendField = AccessTools.Field(typeof(TurnAwayEffector), "_blendSpeed");
            _smoothInField = AccessTools.Field(typeof(TurnAwayEffector), "_inSmoothTime");
            _smoothOutField = AccessTools.Field(typeof(TurnAwayEffector), "_outSmoothTime");
            return typeof(ProceduralWeaponAnimation).GetMethod("AvoidObstacles", BindingFlags.Instance | BindingFlags.Public);
        }

/*        private static void DoMounting(Player player, ProceduralWeaponAnimation pwa)
        {
            if (Plugin.StanceControllerInstance.IsMounting)
            {
                _mountClamp = Mathf.Lerp(_mountClamp, 2.5f, 0.1f);
            }
            else
            {
                _mountClamp = Mathf.Lerp(_mountClamp, 0f, 0.1f);
            }
            float pivotPoint = WeaponStateInstance.BipodIsDeployed ? 1.5f : 0.75f;
            float aimPivot = WeaponStateInstance.BipodIsDeployed ? 0.15f : 0.25f;
            Plugin.StanceControllerInstance.MountingPivotUpdate(player, pwa, _mountClamp, Plugin.StanceControllerInstance.GetDeltaTime(), pivotPoint, aimPivot);
        }

        private static void ModifyBSGCollisions(ProceduralWeaponAnimation pwa, FirearmController fc)
        {
            _currentOverlapValue = fc.OverlapValue;
            float _smoothingFactor = 0.1f; //0.1f
            _smoothedOverlapValue = _smoothedOverlapValue + _smoothingFactor * (_currentOverlapValue - _smoothedOverlapValue);
            //_smoothedOverlapValue = !__instance.OverlappingAllowsBlindfire ? Mathf.Max(_smoothedOverlapValue, _previousOverlapValue) : Mathf.Min(_smoothedOverlapValue, _previousOverlapValue); //this was for when I was trying my own rotaiton + position

            bool isIncreasing = _smoothedOverlapValue > _previousOverlapValue;
            bool isDecreasing = _smoothedOverlapValue < _previousOverlapValue;
            bool isStable = MathUtils.AreFloatsEqual(_smoothedOverlapValue, _previousOverlapValue, 0.0001f);
            float normalSpeed = 0.1f * Time.deltaTime; //0.1f
            float delaySpeed = 0.2f * Time.deltaTime; //0.2f
            float slowDown = 0.15f; //0.05
            float resetTime = 2.5f; //2
            float delayTime = 0.15f; //0.1

            if (isStable)
            {
                _collisionTimer = 0;
                _collisionResetTimer = 0f;
                _collidingModifier = Mathf.MoveTowards(_collidingModifier, 1f, normalSpeed);

            }
            else if (isIncreasing)
            {
                _collisionTimer += Time.deltaTime;
                if (_collisionTimer <= delayTime)
                {
                    _collidingModifier = Mathf.MoveTowards(_collidingModifier, slowDown, delaySpeed);
                }
                else
                {
                    _collidingModifier = Mathf.MoveTowards(_collidingModifier, 1f, normalSpeed);
                }

                _collisionResetTimer = 0f;
            }
            else if (isDecreasing)
            {
                _collisionTimer = 0;
                _collisionResetTimer += Time.deltaTime;
                if (_collisionResetTimer <= resetTime)
                {
                    _collidingModifier = Mathf.MoveTowards(_collidingModifier, slowDown, delaySpeed);
                }
                else
                {
                    _collidingModifier = Mathf.MoveTowards(_collidingModifier, 1f, normalSpeed);
                }
            }
            _previousOverlapValue = _smoothedOverlapValue;

            _blendField.SetValue(pwa.TurnAway, 4.5f * _collidingModifier);
            _smoothInField.SetValue(pwa.TurnAway, 14f * _collidingModifier);
            _smoothOutField.SetValue(pwa.TurnAway, 8f * _collidingModifier);
        }

        private static void SetStanceSpeedModi(bool isPistol)
        {
            if (isPistol)
            {
                _stanceFactor = 1f;
                _stanceInverseFactor = 1f;
                Plugin.StanceControllerInstance.CameraMovmentForCollisionSpeed = 0.06f;
            }
            else if (Plugin.StanceControllerInstance.TargetStance == EStance.ShortStock || Plugin.StanceControllerInstance.StoredStance == EStance.ShortStock)
            {
                _stanceFactor = 1.15f;
                _stanceInverseFactor = 0.85f;
                Plugin.StanceControllerInstance.CameraMovmentForCollisionSpeed = 0.1f;
            }
            else if (Plugin.StanceControllerInstance.TargetStance == EStance.HighReady || Plugin.StanceControllerInstance.StoredStance == EStance.HighReady)
            {
                _stanceFactor = 1.1f;
                _stanceInverseFactor = 0.89f;
                Plugin.StanceControllerInstance.CameraMovmentForCollisionSpeed = 0.2f;
            }
            else if (Plugin.StanceControllerInstance.TargetStance == EStance.LowReady || Plugin.StanceControllerInstance.StoredStance == EStance.LowReady)
            {
                _stanceFactor = 1.07f;
                _stanceInverseFactor = 0.92f;
                Plugin.StanceControllerInstance.CameraMovmentForCollisionSpeed = 0.16f;
            }
            else if (Plugin.StanceControllerInstance.TargetStance == EStance.ActiveAiming || Plugin.StanceControllerInstance.StoredStance == EStance.ActiveAiming)
            {
                _stanceFactor = 1.03f;
                _stanceInverseFactor = 0.95f;
                Plugin.StanceControllerInstance.CameraMovmentForCollisionSpeed = 0.08f;
            }
            else if (Plugin.StanceControllerInstance.TargetStance == EStance.PatrolStance)
            {
                _stanceFactor = 1f;
                _stanceInverseFactor = 1f;
                Plugin.StanceControllerInstance.CameraMovmentForCollisionSpeed = 0.1f;
            }
            else
            {
                _stanceFactor = 1f;
                _stanceInverseFactor = 1f;
                Plugin.StanceControllerInstance.CameraMovmentForCollisionSpeed = 0.07f;
            }
        }*/

 /*       private static void AssignFinalTransforms(bool isPistol, float length)
        {
            if (isPistol)
            {
                _finalPos = new Vector3(0.15f, -0.6f, 0.1f);
                _finalRot = new Vector3(-0.9f, -0.01f, -0.01f);
            }
            else if (Plugin.StanceControllerInstance.TargetStance == EStance.ShortStock || Plugin.StanceControllerInstance.StoredStance == EStance.ShortStock)
            {
                _finalPos = new Vector3(0f, 0f, -0.5f);
                _finalRot = new Vector3(0.01f, 0.1f, -0.05f);

            }
            else if (Plugin.StanceControllerInstance.TargetStance == EStance.HighReady || Plugin.StanceControllerInstance.StoredStance == EStance.HighReady)
            {
                _finalPos = new Vector3(0.08f, -0.34f, -0.4f);
                _finalRot = new Vector3(-0.25f, -0.05f, -0.025f);
            }
            else if (Plugin.StanceControllerInstance.TargetStance == EStance.LowReady || Plugin.StanceControllerInstance.StoredStance == EStance.LowReady)
            {
                _finalPos = new Vector3(0f, 0f, -0.15f);
                _finalRot = new Vector3(0.15f, -0.4f, 0f);
            }
            else if (Plugin.StanceControllerInstance.TargetStance == EStance.ActiveAiming || Plugin.StanceControllerInstance.StoredStance == EStance.ActiveAiming)
            {
                *//*                _finalPos = new Vector3(0.35f, 0.0f, 0.2f);
                                _finalRot = new Vector3(0f, 0f, -0.9f);*//*
                _finalPos = new Vector3(0.05f, -0.2f, 0.1f);
                _finalRot = new Vector3(-0.5f, -0.5f, -0.5f);
            }
            else if (Plugin.StanceControllerInstance.TargetStance == EStance.PatrolStance)
            {
                _finalPos = Vector3.zero;
                _finalRot = Vector3.zero;
            }
            else
            {
                _finalPos = new Vector3(0f, 0.05f, -0.15f);
                _finalRot = new Vector3(0.2f, -0.1f, -0.1f);
            }
        }*/

/*        private static void CollisionOverride(ProceduralWeaponAnimation pwa, FirearmController fc)
        {
            _blendField.SetValue(pwa.TurnAway, 0f);
            _smoothInField.SetValue(pwa.TurnAway, 0f);
            _smoothOutField.SetValue(pwa.TurnAway, 0f);

            Vector3 rayStart = pwa.HandsContainer.WeaponRoot.position;
            Vector3 forward = -pwa.HandsContainer.WeaponRoot.transform.up;
            bool treatAsPistol = WeaponStateInstance.TreatAsPistol;
            float weaponLn = Plugin.StanceControllerInstance.StanceModifiedWeaponLength;
            float weaponLengthFactor = weaponLn * (treatAsPistol ? 1.05f : 1.25f); //stance should be a factor here too

            _isColliding = false;
            RaycastHit raycastHit;
            if (!Plugin.StanceControllerInstance.IsMounting && EFTPhysicsClass.Raycast(new Ray(rayStart, forward), out raycastHit, weaponLengthFactor, LayerMaskClass.HighPolyWithTerrainMask))
            {
                _lastDistance = raycastHit.distance;
                _isColliding = true;
            }

            SetStanceSpeedModi(treatAsPistol);
            bool isStocklessRifle = !WeaponStateInstance.IsPistol && !WeaponStateInstance.HasShoulderContact;
            float stanceBonus = (isStocklessRifle ? 1.15f : 1f) * _stanceFactor;
            float stanceInverseBonus = (isStocklessRifle ? 0.85f : 1f) * _stanceInverseFactor;
            float collisionTimerSpeed = 0.015f * WeaponStateInstance.ErgoFactor * stanceInverseBonus;
            float adsTimerSpeed = 0.03f * WeaponStateInstance.ErgoFactor * _stanceInverseFactor;
            float finalStateTimerSpeed = 0.5f * _stanceFactor;

            if (_isColliding)
            {
                _collisionOverrideTimer = 0;
                Plugin.StanceControllerInstance.IsColliding = true;
            }
            else
            {
                _collisionOverrideTimer += Time.deltaTime;
                if (_collisionOverrideTimer >= collisionTimerSpeed)
                {
                    Plugin.StanceControllerInstance.IsColliding = false;
                }
                else
                {
                    Plugin.StanceControllerInstance.IsColliding = true;
                }
            }

            Plugin.StanceControllerInstance.StopCameraMovement = false;
            if (_isColliding) //&& _lastDistance <= 1.5f
            {
                _adsResetTimer = 0;
                Plugin.StanceControllerInstance.StopCameraMovement = true;
            }
            else
            {
                _adsResetTimer += Time.deltaTime;
                if (_adsResetTimer >= adsTimerSpeed) //this delay needs to factor in the weapon's ADS speed. 0.25 feels good for SKS with supp, 0.5 at least for full length mosin
                {
                    Plugin.StanceControllerInstance.StopCameraMovement = false;
                }
                else Plugin.StanceControllerInstance.StopCameraMovement = true;
            }

            //weapon length ranges around 0.5-1.4, need to modify collision reaction based on the length of the weapon, particularly the threshold.
            //shorter guns need to react less, maybe have a different Pow value for inverseDistance derived from length
            //length = 1.4, threshold = 0.65
            //try using relative distance instead ? distance / length of raycast

            AssignFinalTransforms(treatAsPistol, weaponLn);
            float speed = 0.15f * WeaponStateInstance.TotalErgo * stanceBonus;
            float baseThrehold = treatAsPistol ? 0.45f : 0.5f;
            float threshold = baseThrehold * Mathf.Pow(weaponLn, 1.15f);
            bool doInitialState = _lastDistance >= threshold || _delayFinalState;

            float distanceFactor = Mathf.InverseLerp(weaponLn, 0, _lastDistance);
            float smoothedInverseDistance = Mathf.Pow(distanceFactor, 0.45f);

            float intitalPosY = (weaponLn * -0.4f) * (1 - (_lastDistance / (weaponLengthFactor)));
            float intitalPosZ = (weaponLn * -0.65f) * (1 - (_lastDistance / (weaponLengthFactor))); // treatAsPistol ? -0.145f : treatAsPistol && pwa.IsAiming ? -0.16f : -0.13f;
            Vector3 initialPos = new Vector3(0.025f, 0f, 0f) * smoothedInverseDistance;
            initialPos.y = intitalPosY;
            initialPos.z = intitalPosZ;
            Vector3 lastPos = _finalPos * smoothedInverseDistance;

            Vector3 initialRot = _initialRot * smoothedInverseDistance;
            Vector3 lastRot = _finalRot * smoothedInverseDistance; //
            Vector3 targetRot = !Plugin.StanceControllerInstance.IsColliding ? Vector3.zero : doInitialState ? initialRot : lastRot;

            bool isInFinalState;
            Vector3 targetPos = Vector3.zero;

            if (!Plugin.StanceControllerInstance.IsColliding) isInFinalState = false;
            else if (doInitialState) isInFinalState = false;
            else
            {
                isInFinalState = true;
                _wasInFinalState = true;
            }

            bool reset = !Plugin.StanceControllerInstance.IsColliding && !_wasInFinalState;
            bool initial = doInitialState && !_wasInFinalState;
            targetPos = reset ? Vector3.zero : initial ? initialPos : lastPos;
            targetRot = reset ? Vector3.zero : initial ? initialRot : lastRot;

            _collisionPos = Vector3.Lerp(_collisionPos, targetPos, speed * Time.deltaTime);
            pwa.HandsContainer.WeaponRoot.localPosition += _collisionPos;

            _collisionRot = Vector3.Lerp(_collisionRot, targetRot, speed * Time.deltaTime);
            Quaternion newRot = Quaternion.identity;
            newRot.x = _collisionRot.x;
            newRot.y = _collisionRot.y;
            newRot.z = _collisionRot.z;

            pwa.HandsContainer.WeaponRoot.localRotation *= newRot;

            if (_isColliding)
            {
                _finalStateDelayTimer += Time.deltaTime;
                if (_finalStateDelayTimer >= 0.05f)
                {
                    _delayFinalState = false;
                }
                else _delayFinalState = true;
            }
            else
            {
                _finalStateDelayTimer = 0f;
                _delayFinalState = true;
            }

            if (_wasInFinalState && !isInFinalState)
            {
                _finalStateEndTimer += Time.deltaTime;
                if (_finalStateEndTimer >= finalStateTimerSpeed)
                {
                    _wasInFinalState = false;
                }
            }
            else
            {
                _finalStateEndTimer = 0f;
            }
        }
*/
        [PatchPostfix]
        private static void PatchPostfix(ProceduralWeaponAnimation __instance)
        {
            FirearmController firearmController = (FirearmController)_fcField.GetValue(__instance);
            if (firearmController == null) return;
            Player player = (Player)_playerField.GetValue(firearmController);
            if (player != null && player.IsYourPlayer && player.MovementContext.CurrentState.Name != EPlayerState.Stationary)
            {
      /*          if (PluginConfig.OverrideCollision.Value && FOVFixEnabled) CollisionOverride(__instance, firearmController);
                else if (PluginConfig.OverrideCollision.Value) ModifyBSGCollisions(__instance, firearmController);
                DoMounting(player, __instance);*/
            }
        }
    }

    //Block firemode change animation when in high ready and sprinting
    //This still allows mode to change, and fire selector animtates. This should be blocked too.
    public class SetFireModePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(FirearmsAnimator).GetMethod("SetFireMode", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPrefix]
        private static bool Prefix(FirearmsAnimator __instance, Weapon.EFireMode fireMode, bool skipAnimation = false)
        {
            __instance.ResetLeftHand();
            skipAnimation = Plugin.StanceControllerInstance.CurrentStanceType == EStanceType.HighReady && PlayerStateInstance.IsSprinting ? true : skipAnimation;
            WeaponAnimationSpeedControllerClass.SetFireMode(__instance.Animator, (float)fireMode);
            if (!skipAnimation)
            {
                WeaponAnimationSpeedControllerClass.TriggerFiremodeSwitch(__instance.Animator);
            }
            return false;
        }
    }

    //Entry point to disable stances when starting operating stationary weapon
    //This could be moved to common lib, and check ran in update for using stationary wepaon
    //TODO: replace with event that stancecontroller listens to, or inputhandler
    public class OperateStationaryWeaponPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("OperateStationaryWeapon", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(Player __instance)
        {
        /*    if (__instance.IsYourPlayer)
            {
                Plugin.StanceControllerInstance.CancelAllStances();
                Plugin.StanceControllerInstance.StanceCurrentPosition = Vector3.zero;

            }*/
        }
    }

    //Used as entry point for melee hit and mounting
    //Should move logic to own classes
    //Could move some of this to gneric player update in monobehaviour
    public class CollisionPatch : ModulePatch
    {
        private static FieldInfo _playerField;
        private static FieldInfo _hitIgnoreField;

        private static int _timer = 0;
        private static MaterialType[] _allowedMats = { MaterialType.Helmet, MaterialType.BodyArmor, MaterialType.Body, MaterialType.Glass, MaterialType.GlassShattered, MaterialType.GlassVisor };

        private static Vector3 _startLeftDir = new Vector3(0.143f, 0f, 0f);
        private static Vector3 _startRightDir = new Vector3(-0.143f, 0f, 0f);
        private static Vector3 _startDownDir = new Vector3(0f, 0f, -0.19f);

        private static Vector3 _wiggleLeftDir = new Vector3(2.5f, 7.5f, -5) * 0.5f;
        private static Vector3 _wiggleRightDir = new Vector3(2.5f, -7.5f, -5f) * 0.5f;
        private static Vector3 _wiggleDownDir = new Vector3(7.5f, 2.5f, -5f) * 0.5f;

        private static int PlayerMask = LayerMask.NameToLayer("Player");

        protected override MethodBase GetTargetMethod()
        {
            _playerField = AccessTools.Field(typeof(EFT.Player.FirearmController), "_player");
            _hitIgnoreField = AccessTools.Field(typeof(EFT.Player.FirearmController), "func_2");

            return typeof(Player.FirearmController).GetMethod("method_11", BindingFlags.Instance | BindingFlags.Public);
        }

 /*       private static void SetMountingStatus(EBracingDirection coverDir)
        {
            if (!Plugin.StanceControllerInstance.IsMounting)
            {
                Plugin.StanceControllerInstance.BracingDirection = coverDir;
            }
            Plugin.StanceControllerInstance.IsBracing = true;
        }

        private static Vector3 GetWiggleDir(EBracingDirection coverDir)
        {
            {
                switch (coverDir)
                {
                    case EBracingDirection.Right:
                        return _wiggleRightDir;
                    case EBracingDirection.Left:
                        return _wiggleLeftDir;
                    case EBracingDirection.Top:
                        return _wiggleDownDir;
                    default: return Vector3.zero;
                }
            }
        }

        private static bool IsBracingProne(Player player)
        {
            if (player.IsInPronePose)
            {
                SetMountingStatus(EBracingDirection.Top);
                return true;
            }
            return false;
        }*/

   /*     private static bool CheckForCoverCollision(EBracingDirection coverDir, Vector3 start, Vector3 direction, Vector3 spherePos, float radius)
        {
            RaycastHit raycastHit;
            if (Physics.Linecast(start, direction, out raycastHit, EFTHardSettings.Instance.WEAPON_OCCLUSION_LAYERS))
            {
                if (raycastHit.collider.gameObject.layer != PlayerMask)
                {
                    SetMountingStatus(coverDir);
                    Plugin.StanceControllerInstance.CoverWiggleDirection = GetWiggleDir(coverDir);
                    return true;
                }

            }

            Collider[] hitColliders = Physics.OverlapSphere(spherePos, radius, EFTHardSettings.Instance.WEAPON_OCCLUSION_LAYERS);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.layer != PlayerMask)
                {
                    SetMountingStatus(coverDir);
                    Plugin.StanceControllerInstance.CoverWiggleDirection = GetWiggleDir(coverDir);
                    return true;
                }
            }

            return false;
        }*/

        // TODO: move to a difference class and possible integrate with  BSG's mounting system
/*        private static void DetectBracing(FirearmController fc, Player player, float ln)
        {
            _timer += 1;
            if (_timer >= 60)
            {
                _timer = 0;
                Transform weapTransform = player.ProceduralWeaponAnimation.HandsContainer.WeaponRootAnim;
                Vector3 linecastDirection = weapTransform.TransformDirection(Vector3.up);

                Vector3 downDir = WeaponStateInstance.BipodIsDeployed ? new Vector3(_startDownDir.x, _startDownDir.y, _startDownDir.z + -0.21f) : _startDownDir;

                Vector3 startDown = weapTransform.position + weapTransform.TransformDirection(downDir);
                Vector3 startLeft = weapTransform.position + weapTransform.TransformDirection(_startLeftDir);
                Vector3 startRight = weapTransform.position + weapTransform.TransformDirection(_startRightDir);

                Vector3 sphereDown = weapTransform.position + weapTransform.TransformDirection(new Vector3(0f, -0.45f, -0.1f));
                Vector3 sphereLeft = weapTransform.position + weapTransform.TransformDirection(new Vector3(0.05f, -0.5f, -0.065f));
                Vector3 sphereRight = weapTransform.position + weapTransform.TransformDirection(new Vector3(-0.05f, -0.5f, -0.065f));

                Vector3 forwardDirection = startDown - linecastDirection * ln;
                Vector3 leftDirection = startLeft - linecastDirection * ln;
                Vector3 rightDirection = startRight - linecastDirection * ln;

                if (PluginConfig.OverrideMounting.Value)
                {
                    if (IsBracingProne(player) ||
                    CheckForCoverCollision(EBracingDirection.Top, startDown, forwardDirection, sphereDown, 0.045f) ||
                    CheckForCoverCollision(EBracingDirection.Left, startLeft, leftDirection, sphereLeft, 0.09f) ||
                    CheckForCoverCollision(EBracingDirection.Right, startRight, rightDirection, sphereRight, 0.09f))
                    {
                        return;
                    }
                }
                Plugin.StanceControllerInstance.IsBracing = false;
            }

            if (Plugin.StanceControllerInstance.IsBracing || Plugin.StanceControllerInstance.IsMounting)
            {
                float mountOrientationBonus = Plugin.StanceControllerInstance.BracingDirection == EBracingDirection.Top ? 0.75f : 1f;
                float mountingRecoilLimit = Plugin.StanceControllerInstance.TreatWeaponAsPistolStance ? 0.25f : 0.75f;
                float recoilBonus =
                    Plugin.StanceControllerInstance.IsMounting && fc.Weapon.IsBeltMachineGun && WeaponStateInstance.BipodIsDeployed ? 0.4f :
                    Plugin.StanceControllerInstance.IsMounting && fc.Weapon.IsBeltMachineGun ? 0.75f :
                    Plugin.StanceControllerInstance.IsMounting && WeaponStateInstance.BipodIsDeployed ? 0.45f :
                    Plugin.StanceControllerInstance.IsMounting ? 0.85f :
                    0.95f;
                float swayBonus = Plugin.StanceControllerInstance.IsMounting && WeaponStateInstance.BipodIsDeployed ? 0.05f : Plugin.StanceControllerInstance.IsMounting ? 0.35f : 0.65f;
                Plugin.StanceControllerInstance.BracingRecoilBonus = Mathf.Lerp(Plugin.StanceControllerInstance.BracingRecoilBonus, recoilBonus * mountOrientationBonus, 0.04f);
                Plugin.StanceControllerInstance.BracingSwayBonus = Mathf.Lerp(Plugin.StanceControllerInstance.BracingSwayBonus, swayBonus * mountOrientationBonus, 0.04f);
            }
            else
            {
                Plugin.StanceControllerInstance.BracingSwayBonus = Mathf.Lerp(Plugin.StanceControllerInstance.BracingSwayBonus, 1f, 0.05f);
                Plugin.StanceControllerInstance.BracingRecoilBonus = Mathf.Lerp(Plugin.StanceControllerInstance.BracingRecoilBonus, 1f, 0.05f);
            }
        }
*/
        //move to a mellee stance class as much as possible
/*        private static void DoMelee(FirearmController fc, Player player, float ln)
        {
            if (Plugin.StanceControllerInstance.TargetStance == EStance.Melee && Plugin.StanceControllerInstance.CanDoMeleeDetection && !Plugin.StanceControllerInstance.MeleeHitSomething)
            {
                Transform weapTransform = player.ProceduralWeaponAnimation.HandsContainer.WeaponRootAnim;
                Vector3 linecastDirection = weapTransform.TransformDirection(Vector3.up);
                Vector3 startMeleeDir = new Vector3(0, -0.5f, -0.025f);
                Vector3 meleeStart = weapTransform.position + weapTransform.TransformDirection(startMeleeDir);
                Vector3 meleeDir = meleeStart - linecastDirection * (ln - (WeaponStateInstance.HasBayonet ? 0.1f : 0.25f));

                BallisticCollider hitBalls = null;
                RaycastHit raycastHit;
                if (Physics.Linecast(meleeStart, meleeDir, out raycastHit, CollisionLayerClass.HitMask))
                {
                    Collider col = raycastHit.collider;
                    BaseBallistic baseballComp = col.GetComponent<BaseBallistic>();
                    if (baseballComp != null)
                    {
                        hitBalls = baseballComp.Get(raycastHit.point);
                    }
                    float weaponWeight = fc.Weapon.TotalWeight;
                    float damage = 19f + WeaponStateInstance.BaseMeleeDamage * (1f + player.Skills.StrengthBuffMeleePowerInc) * (1f + (weaponWeight / 10f));
                    damage *= player.Physical.HandsStamina.Exhausted ? Singleton<BackendConfigSettingsClass>.Instance.Stamina.ExhaustedMeleeDamageMultiplier : 1f;
                    damage *= player.Speed;
                    float pen = 15f + WeaponStateInstance.BaseMeleePen * (1f + (weaponWeight / 10f));
                    bool shouldSkipHit = false;

                    if (hitBalls as BodyPartCollider != null)
                    {
                        player.ExecuteSkill(new Action(() => player.Skills.FistfightAction.Complete(1f)));
                    }

                    if (hitBalls.TypeOfMaterial == MaterialType.Glass || hitBalls.TypeOfMaterial == MaterialType.GlassShattered)
                    {
                        int rndNum = UnityEngine.Random.Range(1, 11);
                        if (rndNum > (4f + WeaponStateInstance.BaseMeleeDamage))
                        {
                            shouldSkipHit = true;
                        }
                    }

                    bool isAllowableHit = _allowedMats.Contains(hitBalls.TypeOfMaterial) && !shouldSkipHit;
                    if ((WeaponStateInstance.HasBayonet || isAllowableHit) && raycastHit.collider.gameObject.layer != PlayerMask)
                    {
                        Vector3 position = fc.CurrentFireport.position;
                        Vector3 vector = fc.WeaponDirection;
                        Vector3 shotPosition = position;
                        fc.AdjustShotVectors(ref shotPosition, ref vector);
                        Vector3 shotDirection = vector;
                        DamageInfoStruct damageInfo = new DamageInfoStruct
                        {
                            SourceId = fc.Weapon.Id,
                            DamageType = EDamageType.Melee,
                            Damage = damage,
                            PenetrationPower = pen,
                            ArmorDamage = 10f + (damage / 10f),
                            Direction = shotDirection.normalized,
                            HitCollider = col,
                            HitPoint = raycastHit.point,
                            Player = Singleton<GameWorld>.Instance.GetAlivePlayerBridgeByProfileID(player.ProfileId),
                            HittedBallisticCollider = hitBalls,
                            HitNormal = raycastHit.normal,
                            Weapon = fc.Item as Item,
                            IsForwardHit = true,
                            StaminaBurnRate = 5f
                        };
                        ShotInfoClass result = Singleton<GameWorld>.Instance.HackShot(damageInfo);
                    }
                    float vol = WeaponStateInstance.HasBayonet ? 10f : 12f;
                    Singleton<BetterAudio>.Instance.PlayDropItem(baseballComp.SurfaceSound, JsonType.EItemDropSoundType.Rifle, raycastHit.point, vol);
                    *//*                  Plugin.StanceControllerInstance.DoWiggleEffects(player, player.ProceduralWeaponAnimation, fc, new Vector3(-10f, 10f, 0f), true, 1.5f);
                    *//*
                    player.Physical.ConsumeAsMelee(0.2f * (1f + (weaponWeight * 0.1f)));

                    Plugin.StanceControllerInstance.CanDoMeleeDetection = false;
                    Plugin.StanceControllerInstance.MeleeHitSomething = true;
                    return;
                }
            }
        }*/


        [PatchPrefix]
        private static void PatchPrefix(Player.FirearmController __instance, Vector3 origin, float ln, Vector3? weaponUp = null)
        {
            Player player = (Player)_playerField.GetValue(__instance);
            if (player.IsYourPlayer)
            {
                //DoMelee(__instance, player, ln);
                //DetectBracing(__instance, player, ln);
            }
        }
    }

    //TODO: If using BSG's mounting, skip check
    //Override to prevent BSG collision check when mounting or doing own collision detectio
    public class WeaponOverlapViewPatch : ModulePatch
    {
        private static FieldInfo _playerField;

        protected override MethodBase GetTargetMethod()
        {
            _playerField = AccessTools.Field(typeof(EFT.Player.FirearmController), "_player");

            return typeof(Player.FirearmController).GetMethod("WeaponOverlapView", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPrefix]
        private static bool PatchPrefix(Player.FirearmController __instance)
        {
            Player player = (Player)_playerField.GetValue(__instance);
            /*            if (player.IsYourPlayer && (Plugin.StanceControllerInstance.CurrentStance == EStance.Mounting || Plugin.StanceControllerInstance.IsColliding))
                        {
                            return false;
                        }*/

     /*     too jank to use TurnAway, but I could possible try to extract how they rotate the arms
            if (Plugin.StanceControllerInstance.CurrentStanceType == EStance.PatrolStance) 
            {
                Vector3 vector = player.ProceduralWeaponAnimation.HandsContainer.HandsPosition.Get();
                player.ProceduralWeaponAnimation.OverlappingAllowsBlindfire = false;
                player.ProceduralWeaponAnimation.TurnAway.OverlapsWithPlayer = true;
                player.ProceduralWeaponAnimation.TurnAway.OriginZShift = vector.y;
                player.ProceduralWeaponAnimation.TurnAway.OverlapDepth = PluginConfig.test18.Value;

                _blendField.SetValue(player.ProceduralWeaponAnimation.TurnAway, PluginConfig.test15.Value);
                _smoothInField.SetValue(player.ProceduralWeaponAnimation.TurnAway, PluginConfig.test16.Value);
                _smoothOutField.SetValue(player.ProceduralWeaponAnimation.TurnAway, PluginConfig.test17.Value);

                return false;
            }*/



            return true;
        }
    }

    //Entry point to get original and new weapon length, should move to common lib
    public class WeaponLengthPatch : ModulePatch
    {
        private static FieldInfo playerField;
        private static FieldInfo weapLn;

        protected override MethodBase GetTargetMethod()
        {
            playerField = AccessTools.Field(typeof(EFT.Player.FirearmController), "_player");
            weapLn = AccessTools.Field(typeof(EFT.Player.FirearmController), "WeaponLn");
            return typeof(Player.FirearmController).GetMethod("method_10", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(Player.FirearmController __instance)
        {
            Player player = (Player)playerField.GetValue(__instance);
            float length = (float)weapLn.GetValue(__instance);
            if (player.IsYourPlayer)
            {
                Plugin.StanceControllerInstance.BaseWeaponLength = length;
                Plugin.StanceControllerInstance.StanceModifiedWeaponLength = length < 0.92f ? length * 0.95f : length * 1.05f; //length >= 0.92f ? length * 1.12f : length
            }
        }
    }

    //Modify weapon length used for collision based on stance
    public class WeaponOverlappingPatch : ModulePatch
    {
        private static FieldInfo playerField;
        private static FieldInfo weaponLnField;

        protected override MethodBase GetTargetMethod()
        {
            playerField = AccessTools.Field(typeof(EFT.Player.FirearmController), "_player");
            weaponLnField = AccessTools.Field(typeof(EFT.Player.FirearmController), "WeaponLn");
            return typeof(Player.FirearmController).GetMethod("WeaponOverlapping", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPrefix]
        private static void Prefix(Player.FirearmController __instance)
        {
            Player player = (Player)playerField.GetValue(__instance);
            if (player.IsYourPlayer)
            {
                if (Plugin.StanceControllerInstance.CurrentStanceType == EStanceType.PatrolStance)
                {
                    weaponLnField.SetValue(__instance, Plugin.StanceControllerInstance.StanceModifiedWeaponLength * 0.75f);
                    return;
                }

                if (WeaponStateInstance.TreatAsPistol)
                {
                    weaponLnField.SetValue(__instance, Plugin.StanceControllerInstance.StanceModifiedWeaponLength * 0.85f);
                }
                else
                {
                    if (FikaIsPresent) //collisions acts funky with stances from another client's perspective
                    {
                        weaponLnField.SetValue(__instance, Plugin.StanceControllerInstance.StanceModifiedWeaponLength * 0.8f);
                        return;
                    }
                    if (Plugin.StanceControllerInstance.CurrentStanceType == EStanceType.ShortStock)
                    {
                        weaponLnField.SetValue(__instance, Plugin.StanceControllerInstance.StanceModifiedWeaponLength * 0.9f);
                        return;
                    }
                    if (Plugin.StanceControllerInstance.CurrentStanceType == EStanceType.HighReady)
                    {
                        weaponLnField.SetValue(__instance, Plugin.StanceControllerInstance.StanceModifiedWeaponLength * 0.95f);
                        return;
                    }
                    if (Plugin.StanceControllerInstance.CurrentStanceType == EStanceType.LowReady)
                    {
                        weaponLnField.SetValue(__instance, Plugin.StanceControllerInstance.StanceModifiedWeaponLength * 0.98f);
                        return;
                    }
                }
                weaponLnField.SetValue(__instance, Plugin.StanceControllerInstance.StanceModifiedWeaponLength);
            }
        }
    }


    //Prevent BSG from moving weapon closer based on having a stock
    //should reproduce this behaviour myself, different weapon hold and position if not stock
    public class ShouldMoveWeapCloserPatch : ModulePatch
    {
        private static FieldInfo _playerField;
        private static FieldInfo _fcField;

        protected override MethodBase GetTargetMethod()
        {
            _playerField = AccessTools.Field(typeof(EFT.Player.FirearmController), "_player");
            _fcField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_firearmController");
            return typeof(EFT.Animations.ProceduralWeaponAnimation).GetMethod("CheckShouldMoveWeaponCloser", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(ProceduralWeaponAnimation __instance, ref bool ____shouldMoveWeaponCloser)
        {
            FirearmController firearmController = (FirearmController)_fcField.GetValue(__instance);
            if (firearmController == null) return;
            Player player = (Player)_playerField.GetValue(firearmController);
            if (player != null && player.MovementContext.CurrentState.Name != EPlayerState.Stationary && player.IsYourPlayer)
            {
                ____shouldMoveWeaponCloser = false;
            }
        }
    }

    //TODO: possibly move to common lib to allow multiple modules to modify it
    //Get initial weapon position when initializing PWA
    public class InitTransformsPatch : ModulePatch
    {
        private static FieldInfo playerField;
        private static FieldInfo fcField;

        protected override MethodBase GetTargetMethod()
        {
            playerField = AccessTools.Field(typeof(EFT.Player.FirearmController), "_player");
            fcField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_firearmController");
            return typeof(EFT.Animations.ProceduralWeaponAnimation).GetMethod("InitTransforms", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(EFT.Animations.ProceduralWeaponAnimation __instance)
        {
            FirearmController firearmController = (FirearmController)fcField.GetValue(__instance);
            if (firearmController == null) return;
            Player player = (Player)playerField.GetValue(firearmController);
            if (player != null && player.MovementContext.CurrentState.Name != EPlayerState.Stationary && player.IsYourPlayer)
            {
                Vector3 baseOffset = Plugin.StanceControllerInstance.GetWeaponOffsets().TryGetValue(firearmController.Weapon.TemplateId, out Vector3 offset) ? offset : Vector3.zero;
                Plugin.StanceControllerInstance.BaseWeaponOffsetPosition = __instance.HandsContainer.WeaponRoot.localPosition + baseOffset;

                //if (!Plugin.FOVFixPresent) __instance.HandsContainer.CameraOffset = new Vector3(0.04f, 0.04f, 0.025f);
            }
        }
    }

    //TODO: move to common lib
    //entry point for pose changes
    //move to common lib + use event
    public class ChangePosePatch : ModulePatch
    {
        private static FieldInfo movementContextField;
        private static FieldInfo playerField;

        protected override MethodBase GetTargetMethod()
        {
            movementContextField = AccessTools.Field(typeof(MovementState), "MovementContext");
            playerField = AccessTools.Field(typeof(MovementContext), "_player");
            return typeof(MovementState).GetMethod("ChangePose", BindingFlags.Instance | BindingFlags.Public);
        }


        [PatchPrefix]
        private static void Prefix(MovementState __instance)
        {
            MovementContext movementContext = (MovementContext)movementContextField.GetValue(__instance);
            Player player = (Player)playerField.GetValue(movementContext);

            if (player.IsYourPlayer)
            {   //TODO replace with event, and have mount stance class sub to it
                //Plugin.StanceControllerInstance.IsMounting = false;
            }
        }
    }

    //TODO: move to common lib and make event driven
    //used to determine threshold for cancelling mounting
    public class SetTiltPatch : ModulePatch
    {
        private static FieldInfo movementContextField;
        private static FieldInfo playerField;
        public static float tiltBeforeMount = 0f;

        protected override MethodBase GetTargetMethod()
        {
            movementContextField = AccessTools.Field(typeof(MovementState), "MovementContext");
            playerField = AccessTools.Field(typeof(MovementContext), "_player");
            return typeof(MovementState).GetMethod("SetTilt", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPrefix]
        private static void Prefix(MovementState __instance, float tilt)
        {
            MovementContext movementContext = (MovementContext)movementContextField.GetValue(__instance);
            Player player = (Player)playerField.GetValue(movementContext);

            if (player.IsYourPlayer)
            {
                float tiltTolerance = WeaponStateInstance.BipodIsDeployed ? 0.5f : 2.5f;
                if (!Plugin.StanceControllerInstance)
                {
                    tiltBeforeMount = tilt;
                }
                else if (Math.Abs(tiltBeforeMount - tilt) > tiltTolerance)
                {
                    //TODO replace with event, and have mount stance class sub to it
                    //Plugin.StanceControllerInstance.IsMounting = false;
                    tiltBeforeMount = 0f;
                }
            }
        }
    }

    //TODO move 3rd person logic to controller, and own class. It's likely not needed at all, since I am now doing it all in component update. If so, just adjust stances if third person.
    //used for applying player and bot third person proc animations
    //bot portion should move to component added to bots
    //player third person portion should be standardized and integrated with first persn stance controller
/*    public class ApplySimpleRotationPatch : ModulePatch
    {
        private static FieldInfo aimSpeedField;
        private static FieldInfo blindFireStrength;
        private static FieldInfo scopeRotationField;
        private static FieldInfo weapRotationField;
        private static FieldInfo isAimingField;
        private static FieldInfo weaponPositionField;
        private static FieldInfo currentRotationField;
        private static FieldInfo firearmControllerField;
        private static FieldInfo playerField;

        private static Vector3 mountWeapPosition = Vector3.zero;

        private static Vector3 lowReadyTargetRotation = new Vector3(18.0f, 5.0f, -1.0f);
        private static Quaternion lowReadyTargetQuaternion = Quaternion.Euler(lowReadyTargetRotation);
        private static Vector3 lowReadyTargetPostion = new Vector3(0.06f, 0.04f, 0.0f);
        private static Vector3 highReadyTargetRotation = new Vector3(-15.0f, 3.0f, 3.0f);
        private static Quaternion highReadyTargetQuaternion = Quaternion.Euler(highReadyTargetRotation);
        private static Vector3 highReadyTargetPostion = new Vector3(0.05f, 0.1f, -0.12f);
        private static Vector3 activeAimTargetRotation = new Vector3(0.0f, -40.0f, 0.0f);
        private static Quaternion activeAimTargetQuaternion = Quaternion.Euler(activeAimTargetRotation);
        private static Vector3 activeAimTargetPostion = new Vector3(0.0f, 0.0f, 0.0f);
        private static Vector3 shortStockTargetRotation = new Vector3(0.0f, -28.0f, 0.0f);
        private static Quaternion shortStockTargetQuaternion = Quaternion.Euler(shortStockTargetRotation);
        private static Vector3 shortStockTargetPostion = new Vector3(0.05f, 0.18f, -0.2f);
        private static Vector3 tacPistolTargetRotation = new Vector3(0.0f, -20.0f, 0.0f);
        private static Quaternion tacPistolTargetQuaternion = Quaternion.Euler(tacPistolTargetRotation);
        private static Vector3 tacPistolTargetPosition = new Vector3(-0.1f, 0.1f, -0.05f);
        private static Vector3 normalPistolTargetRotation = new Vector3(0f, -5.0f, 0.0f);
        private static Quaternion normalPistolTargetQuaternion = Quaternion.Euler(normalPistolTargetRotation);
        private static Vector3 normalPistolTargetPosition = new Vector3(-0.05f, 0.0f, 0.0f);

        protected override MethodBase GetTargetMethod()
        {
            aimSpeedField = AccessTools.Field(typeof(EFT.Animations.ProceduralWeaponAnimation), "_aimingSpeed");
            blindFireStrength = AccessTools.Field(typeof(EFT.Animations.ProceduralWeaponAnimation), "_blindfireStrength");
            weaponPositionField = AccessTools.Field(typeof(EFT.Animations.ProceduralWeaponAnimation), "_temporaryPosition");
            scopeRotationField = AccessTools.Field(typeof(EFT.Animations.ProceduralWeaponAnimation), "_targetScopeRotation");
            weapRotationField = AccessTools.Field(typeof(EFT.Animations.ProceduralWeaponAnimation), "_temporaryRotation");
            isAimingField = AccessTools.Field(typeof(EFT.Animations.ProceduralWeaponAnimation), "_isAiming");
            currentRotationField = AccessTools.Field(typeof(EFT.Animations.ProceduralWeaponAnimation), "_cameraIdenity");
            firearmControllerField = AccessTools.Field(typeof(ProceduralWeaponAnimation), "_firearmController");
            playerField = AccessTools.Field(typeof(FirearmController), "_player");

            return typeof(EFT.Animations.ProceduralWeaponAnimation).GetMethod("ApplySimpleRotation", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void Postfix(EFT.Animations.ProceduralWeaponAnimation __instance, float dt)
        {
            FirearmController firearmController = (FirearmController)firearmControllerField.GetValue(__instance);
            if (firearmController == null)
            {
                return;
            }
            Player player = (Player)playerField.GetValue(firearmController);
            if (player != null && player.MovementContext.CurrentState.Name != EPlayerState.Stationary)
            {
                float pitch = (float)blindFireStrength.GetValue(__instance);
                Quaternion scopeRotation = (Quaternion)scopeRotationField.GetValue(__instance);
                Vector3 weaponPosition = (Vector3)weaponPositionField.GetValue(__instance);
                Quaternion weapRotation = (Quaternion)weapRotationField.GetValue(__instance);

                if (player.IsYourPlayer)
                {
                    Plugin.StanceControllerInstance.IsInThirdPerson = true;

                    float aimSpeed = (float)aimSpeedField.GetValue(__instance);
                    bool isAiming = (bool)isAimingField.GetValue(__instance);

                    bool isInStance =
                        Plugin.StanceControllerInstance.TargetStance == EStance.HighReady ||
                        Plugin.StanceControllerInstance.TargetStance == EStance.LowReady ||
                        Plugin.StanceControllerInstance.TargetStance == EStance.ShortStock ||
                        Plugin.StanceControllerInstance.TargetStance == EStance.ActiveAiming ||
                        Plugin.StanceControllerInstance.TargetStance == EStance.Melee;
                    bool isInShootableStance =
                        Plugin.StanceControllerInstance.TargetStance == EStance.ShortStock ||
                        Plugin.StanceControllerInstance.TargetStance == EStance.ActiveAiming ||
                        Plugin.StanceControllerInstance.TreatWeaponAsPistolStance ||
                        Plugin.StanceControllerInstance.TargetStance == EStance.Melee;
                    bool cancelBecauseShooting = !(PluginConfig.RememberStanceFiring.Value && isAiming) && FiringStateInstance.IsFiringFromStance && !isInShootableStance;
                    bool doStanceRotation = (isInStance || !Plugin.StanceControllerInstance.AllStancesReset || Plugin.StanceControllerInstance.TargetStance == EStance.PistolCompressed) && !cancelBecauseShooting;
                    bool cancelStance =
                        (Plugin.StanceControllerInstance.PauseActiveAim && Plugin.StanceControllerInstance.TargetStance == EStance.ActiveAiming) ||
                        (Plugin.StanceControllerInstance.PauseHighReady && Plugin.StanceControllerInstance.TargetStance == EStance.HighReady) ||
                        (Plugin.StanceControllerInstance.PauseLowReady && Plugin.StanceControllerInstance.TargetStance == EStance.LowReady) ||
                        (Plugin.StanceControllerInstance.PauseShortStock && Plugin.StanceControllerInstance.TargetStance == EStance.ShortStock); //|| (Plugin.StanceControllerInstance.CancelPistolStance && Plugin.StanceControllerInstance.PistolIsCompressed)

                    Plugin.StanceControllerInstance.CurrentStanceRotation = Quaternion.Slerp(Plugin.StanceControllerInstance.CurrentStanceRotation, __instance.IsAiming && Plugin.StanceControllerInstance.AllStancesReset ? scopeRotation : doStanceRotation ? Plugin.StanceControllerInstance.StanceTargetRotation : Quaternion.identity, doStanceRotation ? Plugin.StanceControllerInstance.StanceRotationSpeed * PluginConfig.StanceRotationSpeedMulti.Value : __instance.IsAiming ? 8f * aimSpeed * dt : 8f * dt);

                    __instance.HandsContainer.WeaponRootAnim.SetPositionAndRotation(weaponPosition, weapRotation * Plugin.StanceControllerInstance.CurrentStanceRotation);

                    if (Plugin.StanceControllerInstance.TreatWeaponAsPistolStance && PluginConfig.EnableAltPistol.Value) // && Plugin.StanceControllerInstance.CurrentStance != EStance.PatrolStance
                    {
                        if (Plugin.StanceControllerInstance.TargetStance == EStance.PistolCompressed && !AimStateInstance.IsAiming && !Plugin.StanceControllerInstance.IsResettingPistol && !Plugin.StanceControllerInstance.IsBlindFiring)
                        {
                            Plugin.StanceControllerInstance.StanceBlender.Target = 1f;
                        }
                        else
                        {
                            Plugin.StanceControllerInstance.StanceBlender.Target = 0f;
                        }

                        if ((Plugin.StanceControllerInstance.TargetStance != EStance.PistolCompressed && !AimStateInstance.IsAiming && !Plugin.StanceControllerInstance.IsResettingPistol) || (Plugin.StanceControllerInstance.IsBlindFiring))
                        {
                            Plugin.StanceControllerInstance.StanceCurrentPosition = Vector3.Lerp(Plugin.StanceControllerInstance.StanceCurrentPosition, Vector3.zero, 5f * dt);
                        }

                        Plugin.StanceControllerInstance.HasResetActiveAim = true;
                        Plugin.StanceControllerInstance.HasResetHighReady = true;
                        Plugin.StanceControllerInstance.HasResetLowReady = true;
                        Plugin.StanceControllerInstance.HasResetShortStock = true;
                        Plugin.StanceControllerInstance.DoPistolStances(true, __instance, dt, player, firearmController, Vector3.zero);
                    }
                    else if (!Plugin.StanceControllerInstance.TreatWeaponAsPistolStance || WeaponStateInstance.HasShoulderContact)
                    {
                        if ((!isInStance && Plugin.StanceControllerInstance.AllStancesReset) || (cancelBecauseShooting && !isInShootableStance) || AimStateInstance.IsAiming || cancelStance || Plugin.StanceControllerInstance.IsBlindFiring)
                        {
                            Plugin.StanceControllerInstance.StanceBlender.Target = 0f;
                        }
                        else if (isInStance)
                        {
                            Plugin.StanceControllerInstance.StanceBlender.Target = 1f;
                        }

                        if (((!isInStance && Plugin.StanceControllerInstance.AllStancesReset) && !cancelBecauseShooting && !AimStateInstance.IsAiming) || (Plugin.StanceControllerInstance.IsBlindFiring))
                        {
                            Plugin.StanceControllerInstance.StanceCurrentPosition = Vector3.Lerp(Plugin.StanceControllerInstance.StanceCurrentPosition, Vector3.zero, 5f * dt);
                        }

                        Plugin.StanceControllerInstance.HasResetPistolPos = true;
                        Plugin.StanceControllerInstance.DoRifleStances(player, firearmController, true, __instance, dt, Vector3.zero);
                    }
                }
       *//*         else if (player.IsAI && !player.AIData.UseZombieSimpleAnimator)
                {
                    Quaternion targetRotation = Quaternion.identity;
                    Quaternion currentRotation = (Quaternion)currentRotationField.GetValue(__instance);
                    aimSpeedField.SetValue(__instance, 1f);

                    FaceShieldComponent fsComponent = player.FaceShieldObserver.Component;
                    NightVisionComponent nvgComponent = player.NightVisionObserver.Component;
                    bool nvgIsOn = nvgComponent != null && (nvgComponent.Togglable == null || nvgComponent.Togglable.On);
                    bool fsIsON = fsComponent != null && (fsComponent.Togglable == null || fsComponent.Togglable.On);

                    float lastDistance = player.AIData.BotOwner.AimingManager.CurrentAiming.LastDist2Target;
                    Vector3 distanceVect = player.AIData.BotOwner.AimingManager.CurrentAiming.RealTargetPoint - player.AIData.BotOwner.MyHead.position;
                    float realDistance = distanceVect.magnitude;

                    bool isTacBot = Plugin.StanceControllerInstance._botsToUseTacticalStances.IndexOf(player.AIData.BotOwner.Profile.Info.Settings.Role) != -1;
                    bool isPeace = player.AIData.BotOwner.Memory.IsPeace;
                    bool notShooting = !player.AIData.BotOwner.ShootData.Shooting && Time.time - player.AIData.BotOwner.ShootData.LastTriggerPressd > 15f;
                    bool isInStance = false;
                    float stanceSpeed = 1f;

                    if (player.MovementContext.BlindFire == 0 && player.MovementContext.StationaryWeapon == null)
                    {
                        if (isPeace && !player.IsSprintEnabled && !__instance.IsAiming && !firearmController.IsInReloadOperation() && !firearmController.IsInventoryOpen() && !firearmController.IsInInteractionStrictCheck() && !firearmController.IsInSpawnOperation() && !firearmController.IsHandsProcessing()) // && player.AIData.BotOwner.WeaponManager.IsWeaponReady &&  player.AIData.BotOwner.WeaponManager.InIdleState()
                        {
                            isInStance = true;
                            player.MovementContext.SetPatrol(true);
                        }
                        else
                        {
                            player.MovementContext.SetPatrol(false);
                            if (firearmController.Weapon.WeapClass != "pistol")
                            {
                                ////low ready//// 
                                if (!isTacBot && !firearmController.IsInReloadOperation() && !player.IsSprintEnabled && !__instance.IsAiming && notShooting && (lastDistance >= 25f || lastDistance == 0f))    // (Time.time - player.AIData.BotOwner.Memory.LastEnemyTimeSeen) > 1f
                                {
                                    isInStance = true;
                                    stanceSpeed = 12f * dt;
                                    targetRotation = lowReadyTargetQuaternion;
                                    __instance.HandsContainer.HandsPosition.Zero = __instance.PositionZeroSum + pitch * lowReadyTargetPostion;
                                }

                                ////high ready////
                                if (isTacBot && !firearmController.IsInReloadOperation() && !__instance.IsAiming && notShooting && (lastDistance >= 25f || lastDistance == 0f))
                                {
                                    isInStance = true;
                                    player.BodyAnimatorCommon.SetFloat(PlayerAnimator.WEAPON_SIZE_MODIFIER_PARAM_HASH, 2);
                                    stanceSpeed = 10.8f * dt;
                                    targetRotation = highReadyTargetQuaternion;
                                    __instance.HandsContainer.HandsPosition.Zero = __instance.PositionZeroSum + pitch * highReadyTargetPostion;
                                }
                                else
                                {
                                    player.BodyAnimatorCommon.SetFloat(PlayerAnimator.WEAPON_SIZE_MODIFIER_PARAM_HASH, (float)firearmController.Item.CalculateCellSize().X);
                                }

                                ///active aim//// 
                                if (isTacBot && (((nvgIsOn || fsIsON) && !player.IsSprintEnabled && !firearmController.IsInReloadOperation() && lastDistance < 25f && lastDistance > 2f && lastDistance != 0f) || (__instance.IsAiming && (nvgIsOn && __instance.CurrentScope.IsOptic || fsIsON))))
                                {
                                    isInStance = true;
                                    stanceSpeed = 6f * dt;
                                    targetRotation = activeAimTargetQuaternion;
                                    __instance.HandsContainer.HandsPosition.Zero = __instance.PositionZeroSum + pitch * activeAimTargetPostion;
                                }

                                ///short stock//// 
                                if (isTacBot && !player.IsSprintEnabled && !firearmController.IsInReloadOperation() && lastDistance <= 2f && lastDistance != 0f)
                                {
                                    isInStance = true;
                                    stanceSpeed = 12f * dt;
                                    targetRotation = shortStockTargetQuaternion;
                                    __instance.HandsContainer.HandsPosition.Zero = __instance.PositionZeroSum + pitch * shortStockTargetPostion;
                                }
                            }
                            else
                            {
                                if (!isTacBot && !player.IsSprintEnabled && !__instance.IsAiming && notShooting)
                                {
                                    isInStance = true;
                                    stanceSpeed = 6f * dt;
                                    targetRotation = normalPistolTargetQuaternion;
                                    __instance.HandsContainer.HandsPosition.Zero = __instance.PositionZeroSum + pitch * normalPistolTargetPosition;
                                }

                                if (isTacBot && !player.IsSprintEnabled && !__instance.IsAiming && notShooting)
                                {
                                    isInStance = true;
                                    stanceSpeed = 6f * dt;
                                    targetRotation = tacPistolTargetQuaternion;
                                    __instance.HandsContainer.HandsPosition.Zero = __instance.PositionZeroSum + pitch * tacPistolTargetPosition;
                                }
                            }
                        }
                    }

                    currentRotation = Quaternion.Slerp(currentRotation, __instance.IsAiming && !isInStance ? scopeRotation : isInStance ? targetRotation : Quaternion.identity, isInStance ? stanceSpeed : 8f * dt);
                    __instance.HandsContainer.WeaponRootAnim.SetPositionAndRotation(weaponPosition, weapRotation * currentRotation);
                    currentRotationField.SetValue(__instance, currentRotation);
                }*//*
            }
        }
    }*/
}


