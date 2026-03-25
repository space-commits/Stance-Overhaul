using RealismCommonLib.ModifierHandlers;
using System;
using System.Collections.Generic;
using System.Text;
using RealismCommonLib.StateControllers;
using static RealismCommonLib.Plugin;

namespace StanceOverhaul.Controllers.StateControllers
{
    public class StanceAimState : IStateController
    {
        private BoolGateHandle _canAim;

        public bool AimingInterrupted { get; private set; }

        public void RunOnAwake()
        {
            _canAim = BoolHandlers.CanAim.Add(true);
        }

        public void RunOnDestroy()
        {
            BoolHandlers.CanAim.Remove(_canAim);
        }

        public void RunOnUpdate(float deltaTime)
        {
            CheckForAimBlockers();
        }

        private void CheckForAimBlockers()
        {
            _canAim.Allowed = true;

            bool hasActiveGoggles = GearStateInstance.NVGIsActive || GearStateInstance.ThermalIsActive;

            bool nvgBlocksAds =
                PluginConfig.EnableNVGAimBlock.Value
                && GearStateInstance.NVGIsActive
                && WeaponStateInstance.HasOptic;

            bool thermalBlocksAds =
                PluginConfig.EnableThermalAimBlock.Value
                && GearStateInstance.ThermalIsActive;

            bool faceshieldBlocksADS =
                PluginConfig.EnableThermalAimBlock.Value
                && GearStateInstance.FaceShieldIsActive
                && WeaponStateInstance.HasShoulderContact;

            if (nvgBlocksAds || faceshieldBlocksADS || thermalBlocksAds)
            {
                _canAim.Allowed = false;
            }
        }

        public void InterruptAim()
        {
            if (PlayerStateInstance.FirearmController.IsAiming && !AimingInterrupted)
            {
                PlayerStateInstance.FirearmController.ToggleAim();
                AimingInterrupted = true;
            }
        }
        public void UnInterruptAim()
        {
            if (!PlayerStateInstance.FirearmController.IsAiming && AimingInterrupted)
            {
                PlayerStateInstance.FirearmController.ToggleAim();
                AimingInterrupted = false;
            }
        }
    }
}
