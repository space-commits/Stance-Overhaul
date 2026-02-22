using EFT;
using EFT.Animations;
using EFT.InputSystem;
using RealismCommonLib.PatchPipeline;
using StanceOverhaul.Enums;
using System;
using UnityEngine;
using static EFT.Player;
using static RealismCommonLib.Plugin;

namespace StanceOverhaul.Controllers.PatchHooks
{
    internal class InputHookPipeline: IStateController
    {
        private StanceController _stanceController;

        private IDisposable _inputVeto;
        private IDisposable _inputOverride;
        private IDisposable _inputOverrideHandler;

        public InputHookPipeline(StanceController stanceController)
        {
            _stanceController = stanceController;
        }

        public void RunOnAwake()
        {
            RegisterWithPipelines();
        }

        public void RunOnDestroy()
        {
            DeRegisterWithPipelines();
        }

        private void RegisterWithPipelines()
        {
            _inputVeto = Pipelines.InputVetoPipeline.Register(ShouldVetoInput);
            _inputOverride = Pipelines.InputOverridePipeline.Register(ShouldOverrideInput);
            _inputOverrideHandler = Pipelines.InputOverrideHandler.Register(HandleOverrideLogic);
        }

        private void DeRegisterWithPipeline(IDisposable pipeline)
        {
            pipeline?.Dispose();
            pipeline = null;
        }

        private void DeRegisterWithPipelines()
        {
            DeRegisterWithPipeline(_inputVeto);
            DeRegisterWithPipeline(_inputOverride);
            DeRegisterWithPipeline(_inputOverrideHandler);
        }

        private bool ShouldVetoInput(InputContext ctx)
        {
            switch (ctx.Command)
            {
                case ECommand.ToggleShooting:
                    return ShouldVetoFiring();
                case ECommand.ScrollNext:
                case ECommand.ScrollPrevious:
                    return ShouldVetoScrollInput();
                default:
                    return false;
            }

        }

        private bool ShouldOverrideInput(InputContext ctx)
        {
            switch (ctx.Command)
            {
                case ECommand.LeftStanceToggle:
                    return true;
                case ECommand.ToggleBipods:
                    return PluginConfig.OverrideMounting.Value;
                default:
                    return false;
            }
        }

        private void HandleOverrideLogic(InputContext ctx)
        {
            switch (ctx.Command)
            {
                case ECommand.LeftStanceToggle:
                    LeftStanceOverride();
                    break;
                case ECommand.ToggleBipods:
                    MountingOverride();
                    break;

            }
        }

        private bool ShouldVetoScrollInput()
        {
            return
                Input.GetKey(PluginConfig.StanceWheelComboKeyBind.Value.MainKey) &&
                PluginConfig.UseMouseWheelPlusKey.Value;
        }

        private bool ShouldVetoFiring()
        {
            bool isInStanceThatCanBlockFiring =
                _stanceController.TargetStance != EStance.None &&
                _stanceController.TargetStance != EStance.ActiveAiming &&
                _stanceController.TargetStance != EStance.ShortStock &&
                _stanceController.TargetStance != EStance.PistolCompressed;

            bool shouldVeto =
                PluginConfig.BlockFiring.Value &&
                !_stanceController.ShouldForceLowReady &&
                isInStanceThatCanBlockFiring;

            if (shouldVeto)
            {
                _stanceController.TargetStance = EStance.None;
                _stanceController.StoredStance = EStance.None;
                _stanceController.StanceBlender.Target = 0f;
            }

            return shouldVeto;
        }

        private void MountingOverride()
        {
            Player player = PlayerStateInstance.Player;

            if (_stanceController.IsBracing && !_stanceController.IsColliding)
            {
                if (WeaponStateInstance.BipodIsDeployed && _stanceController.BracingDirection != EBracingDirection.Top) return;

                _stanceController.IsMounting = !_stanceController.IsMounting;
                if (_stanceController.IsMounting) _stanceController.CancelAllStances();

                Vector3 wiggleDirection = _stanceController.IsMounting ? _stanceController.CoverWiggleDirection : _stanceController.CoverWiggleDirection * -1f;
                _stanceController.DoWiggleEffects(player, player.ProceduralWeaponAnimation, WeaponStateInstance.Weapon, wiggleDirection, true, 1f, useGearSound: true);
            }
            if (!_stanceController.IsBracing && _stanceController.IsMounting) _stanceController.IsMounting = false;

            if (_stanceController.IsMounting && WeaponStateInstance.BipodIsDeployed)
            {
                ChangeScopeModeOnMount(player.ProceduralWeaponAnimation, PlayerStateInstance.FirearmController);

                //attempts to enable prone mounted animation
                /*
                    MountPointData mountData = new MountPointData(StanceController.MountPos, StanceController.MountDir, EMountSideDirection.Forward);
                    Quaternion targetBodyRotation = Quaternion.AngleAxis(player.MovementContext.Yaw, Vector3.up);
                    player.MovementContext.PlayerMountingPointData.SetData(mountData, player.MovementContext.TransformPosition, player.MovementContext.PoseLevel, player.MovementContext.Yaw, PluginConfig.test10.Value, targetBodyRotation, new Vector2(0f, 0f), new Vector2(-3, 6), new Vector2(-10, 10));
                    player.MovementContext.EnterMountedState();
                    player.MovementContext.PlayerAnimator.SetProneBipodMount(true);*/

                /*       AccessTools.Field(typeof(MovementContext), "_inMountedState").SetValue(player.MovementContext, true);
                         player.MovementContext.PlayerAnimator.SetProneBipodMount(true);
                         fc.FirearmsAnimator.SetMounted(true);
                         player.ProceduralWeaponAnimation.SetMountingData(true, true);*/
            }
        }

        private void LeftStanceOverride()
        {
            if (!_stanceController.IsInForcedLowReady && !_stanceController.ShouldBlockAllStances) _stanceController.ToggleLeftShoulder();
        }

        //TODO: replace instance paramaters and use state instance
        private void ChangeScopeModeOnMount(ProceduralWeaponAnimation pwa, FirearmController fc)
        {
            int aimIndex = pwa.AimIndex;
            if (Mathf.Abs(pwa.ScopeAimTransforms[aimIndex].Rotation) >= EFTHardSettings.Instance.SCOPE_ROTATION_THRESHOLD)
            {
                for (int i = 0; i < pwa.ScopeAimTransforms.Count; i++)
                {
                    if (Mathf.Abs(pwa.ScopeAimTransforms[i].Rotation) < EFTHardSettings.Instance.SCOPE_ROTATION_THRESHOLD)
                    {
                        fc.ChangeAimingMode(i);
                        break;
                    }
                }
            }
        }

        public void RunOnUpdate()
        {
        }
    }
}
