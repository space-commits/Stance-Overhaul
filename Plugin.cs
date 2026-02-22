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
    [BepInPlugin("com.fontaine.stanceoverhaul", "Fontaine-StanceOverhaul", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {

        public static StanceController StanceControllerInstance;

        void Awake()
        {
            PluginConfig.InitConfigBindings(Config);

            SubscribeToEvents(); 

            EnablePatches();
        }

        void Update()
        {
            
        }

        void EnablePatches() 
        {
            new TacticalReloadPatch().Enable();
            new SprintPatch().Enable();
            new DisableAimOnReloadPatch().Enable();
            new ChangeScopePatch().Enable();
            new MountingAndCollisionPatch().Enable();
            new OnWeaponDrawPatch().Enable();
        }

        void SubscribeToEvents() 
        {
            RealismCommonLib.Events.PlayerEvents.OnPlayerInit += AddStanceComponentsToPlayer;
        }

        void AddStanceComponentsToPlayer(Player player)
        { 
            player.gameObject.AddComponent<StanceController>();
        }
    }
}



