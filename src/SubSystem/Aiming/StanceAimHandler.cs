using RealismCommonLib.ModifierHandlers;
using System;
using System.Collections.Generic;
using System.Text;
using RealismCommonLib.StateControllers;
using static RealismCommonLib.Plugin;
using StanceOverhaul.SubSystem;

namespace StanceOverhaul.SubSystem.Aiming
{
    public class StanceAimHandler : ISubSystem
    {
        private BoolGateHandle _canAim;
        private bool _blockerInterruptedAim;

        public void RunOnAwake()
        {
            RealismCommonLib.Events.PlayerEvents.AimStateChanged += CheckForAimBlockers;
            RealismCommonLib.Events.PlayerEvents.ToggleHeadDevice += CheckForAimBlockers;
            WeaponStateInstance.OnWeaponStateChanged += CheckForAimBlockers;
            RealismCommonLib.Events.PlayerEvents.OnWeaponEquipped += CheckForAimBlockers;
            _canAim = BoolHandlers.CanAim.Add(true);
        }

        public void RunOnDestroy()
        {
            RealismCommonLib.Events.PlayerEvents.AimStateChanged -= CheckForAimBlockers;
            RealismCommonLib.Events.PlayerEvents.ToggleHeadDevice -= CheckForAimBlockers;
            WeaponStateInstance.OnWeaponStateChanged -= CheckForAimBlockers;
            RealismCommonLib.Events.PlayerEvents.OnWeaponEquipped -= CheckForAimBlockers;
            BoolHandlers.CanAim.Remove(_canAim);
        }

        public void RunOnUpdate(float deltaTime)
        {
            //CheckForAimBlockers();
        }

        //TODO: make event-based on aim
        private void CheckForAimBlockers()
        {
            bool nvgBlocksAds =
                PluginConfig.EnableNVGAimBlock.Value
                && GearStateInstance.NVGIsActive
                && WeaponStateInstance.HasOptic;

            bool thermalBlocksAds =
                PluginConfig.EnableThermalAimBlock.Value
                && GearStateInstance.ThermalIsActive;

            bool faceshieldBlocksADS =
                PluginConfig.EnableFSAimBlock.Value
                && GearStateInstance.FaceShieldIsActive
                && WeaponStateInstance.HasShoulderContact;

            bool blocked = nvgBlocksAds || faceshieldBlocksADS || thermalBlocksAds;
            _canAim.Allowed = !blocked;

            //ModLogger.LogWarning($"CheckForAimBlockers: NVG={nvgBlocksAds}, Thermal={thermalBlocksAds}, FaceShield={faceshieldBlocksADS}, Blocked={blocked}");

            if (blocked)
            {
                // Force-exit ADS once if a blocker became active while already aiming
                if (!_blockerInterruptedAim && AimStateInstance.IsAiming)
                {
                    PlayerStateInstance?.FirearmController?.ToggleAim();
                    _blockerInterruptedAim = true;
                }
            }
            else
            {
                _blockerInterruptedAim = false;
            }
        }
    }
}
