using EFT.Animations;
using EFT;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using StanceOverhaul.Events;
using static StanceOverhaul.Plugin;
using static RealismCommonLib.Plugin;


namespace StanceOverhaul.SubSystem
{
    public class WeaponOffsetSystem : ISubSystem
    {
    
        public Vector3 ConfigWeaponOffset
        {
            get
            {
                return WeaponStateInstance.TreatAsPistol ?
                   new Vector3(PluginConfig.PistolOffsetX.Value, PluginConfig.PistolOffsetY.Value, PluginConfig.PistolOffsetZ.Value) :
                   new Vector3(PluginConfig.RifleOffsetX.Value, PluginConfig.RifleOffsetY.Value, PluginConfig.RifleOffsetZ.Value);
            }
        }

        public void RunOnAwake()
        {
        }

        public void RunOnDestroy()
        {
        }

        public void RunOnUpdate(float deltaTime)
        {
            ApplyOffset(deltaTime);
        }

        public void ApplyOffset(float deltaTime)
        {
            StanceControllerInstance.CurrentOffsetPosition = StanceControllerInstance.BaseWeaponOffsetPosition + ConfigWeaponOffset;
        }

    }
}

