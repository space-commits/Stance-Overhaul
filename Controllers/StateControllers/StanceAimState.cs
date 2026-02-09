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
        private StanceController _stanceController;

        private BoolGateHandle _canAim;

        public StanceAimState(StanceController stanceController)
        {
            _stanceController = stanceController;
        }

        public void RunOnAwake()
        {
            _canAim = BoolHandlers.CanAim.Add(true);
        }

        public void RunOnDestroy()
        {
            BoolHandlers.CanAim.Remove(_canAim);
        }

        public void RunOnUpdate()
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
    }
}
