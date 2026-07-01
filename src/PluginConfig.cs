using BepInEx.Configuration;
using System;
using UnityEngine;
using static MineDirectional;


namespace StanceOverhaul
{
    internal static class PluginConfig
    {
        public static ConfigEntry<float> test1 { get; set; }
        public static ConfigEntry<float> test2 { get; set; }
        public static ConfigEntry<float> test3 { get; set; }
        public static ConfigEntry<float> test4 { get; set; }
        public static ConfigEntry<float> test5 { get; set; }
        public static ConfigEntry<float> test6 { get; set; }
        public static ConfigEntry<float> test7 { get; set; }
        public static ConfigEntry<float> test8 { get; set; }
        public static ConfigEntry<float> test9 { get; set; }
        public static ConfigEntry<float> test10 { get; set; }
        public static ConfigEntry<float> test11 { get; set; }
        public static ConfigEntry<float> test12 { get; set; }
        public static ConfigEntry<float> test13 { get; set; }
        public static ConfigEntry<float> test14 { get; set; }
        public static ConfigEntry<float> test15 { get; set; }
        public static ConfigEntry<float> test16 { get; set; }
        public static ConfigEntry<float> test17 { get; set; }
        public static ConfigEntry<float> test18 { get; set; }
        public static ConfigEntry<float> test19 { get; set; }
        public static ConfigEntry<float> test20 { get; set; }

        // Movement Speed Bonuses and Stamina Rates
        public static ConfigEntry<float> ActiveAimWalkSpeedBonus { get; set; }
        public static ConfigEntry<float> ActiveAimSprintAccelBonus { get; set; }
        public static ConfigEntry<float> ActiveAimStaminaRate { get; set; }
        public static ConfigEntry<float> HighReadyWalkSpeedBonus { get; set; }
        public static ConfigEntry<float> HighReadySprintAccelBonus { get; set; }
        public static ConfigEntry<float> HighReadyStaminaRate { get; set; }
        public static ConfigEntry<float> LowReadyWalkSpeedBonus { get; set; }
        public static ConfigEntry<float> LowReadySprintAccelBonus { get; set; }
        public static ConfigEntry<float> LowReadyStaminaRate { get; set; }
        public static ConfigEntry<float> ShortStockWalkSpeedBonus { get; set; }
        public static ConfigEntry<float> ShortStockSprintAccelBonus { get; set; }
        public static ConfigEntry<float> ShortStockStaminaRate { get; set; }
        public static ConfigEntry<float> PatrolWalkSpeedBonus { get; set; }
        public static ConfigEntry<float> PatrolSprintAccelBonus { get; set; }
        public static ConfigEntry<float> PatrolStaminaRate { get; set; }
        public static ConfigEntry<float> LeftShoulderWalkSpeedBonus { get; set; }
        public static ConfigEntry<float> LeftShoulderSprintAccelBonus { get; set; }
        public static ConfigEntry<float> LeftShoulderStaminaRate { get; set; }

        // Active Aim Transitions
        public static ConfigEntry<float> ActiveAimBlendThresholdLowReady { get; set; }
        public static ConfigEntry<float> ActiveAimBlendThresholdHighReady { get; set; }
        public static ConfigEntry<float> ActiveAimBlendThresholdLeftShoulder { get; set; }
        public static ConfigEntry<float> ActiveAimBlendThresholdPatrol { get; set; }
        public static ConfigEntry<float> ActiveAimBlendThresholdShortStock { get; set; }
        public static ConfigEntry<float> ActiveAimTransitionFromLowReady { get; set; }
        public static ConfigEntry<float> ActiveAimTransitionFromHighReady { get; set; }
        public static ConfigEntry<float> ActiveAimTransitionFromLeftShoulder { get; set; }
        public static ConfigEntry<float> ActiveAimTransitionFromPatrol { get; set; }
        public static ConfigEntry<float> ActiveAimTransitionFromShortStock { get; set; }
        public static ConfigEntry<float> ActiveAimTransitionToLowReady { get; set; }
        public static ConfigEntry<float> ActiveAimTransitionToHighReady { get; set; }
        public static ConfigEntry<float> ActiveAimTransitionToLeftShoulder { get; set; }
        public static ConfigEntry<float> ActiveAimTransitionToPatrol { get; set; }
        public static ConfigEntry<float> ActiveAimTransitionToShortStock { get; set; }
        public static ConfigEntry<float> ActiveAimTransitionFromIdle { get; set; }

        // High Ready Transitions
        public static ConfigEntry<float> HighReadyBlendThresholdActiveAim { get; set; }
        public static ConfigEntry<float> HighReadyBlendThresholdLowReady { get; set; }
        public static ConfigEntry<float> HighReadyBlendThresholdLeftShoulder { get; set; }
        public static ConfigEntry<float> HighReadyBlendThresholdPatrol { get; set; }
        public static ConfigEntry<float> HighReadyBlendThresholdShortStock { get; set; }
        public static ConfigEntry<float> HighReadyTransitionFromActiveAim { get; set; }
        public static ConfigEntry<float> HighReadyTransitionFromLowReady { get; set; }
        public static ConfigEntry<float> HighReadyTransitionFromLeftShoulder { get; set; }
        public static ConfigEntry<float> HighReadyTransitionFromPatrol { get; set; }
        public static ConfigEntry<float> HighReadyTransitionFromShortStock { get; set; }
        public static ConfigEntry<float> HighReadyTransitionToActiveAim { get; set; }
        public static ConfigEntry<float> HighReadyTransitionToLowReady { get; set; }
        public static ConfigEntry<float> HighReadyTransitionToLeftShoulder { get; set; }
        public static ConfigEntry<float> HighReadyTransitionToPatrol { get; set; }
        public static ConfigEntry<float> HighReadyTransitionToShortStock { get; set; }
        public static ConfigEntry<float> HighReadyTransitionFromIdle { get; set; }

        // Low Ready Transitions
        public static ConfigEntry<float> LowReadyBlendThresholdActiveAim { get; set; }
        public static ConfigEntry<float> LowReadyBlendThresholdHighReady { get; set; }
        public static ConfigEntry<float> LowReadyBlendThresholdLeftShoulder { get; set; }
        public static ConfigEntry<float> LowReadyBlendThresholdPatrol { get; set; }
        public static ConfigEntry<float> LowReadyBlendThresholdShortStock { get; set; }
        public static ConfigEntry<float> LowReadyTransitionFromActiveAim { get; set; }
        public static ConfigEntry<float> LowReadyTransitionFromHighReady { get; set; }
        public static ConfigEntry<float> LowReadyTransitionFromLeftShoulder { get; set; }
        public static ConfigEntry<float> LowReadyTransitionFromPatrol { get; set; }
        public static ConfigEntry<float> LowReadyTransitionFromShortStock { get; set; }
        public static ConfigEntry<float> LowReadyTransitionToActiveAim { get; set; }
        public static ConfigEntry<float> LowReadyTransitionToHighReady { get; set; }
        public static ConfigEntry<float> LowReadyTransitionToLeftShoulder { get; set; }
        public static ConfigEntry<float> LowReadyTransitionToPatrol { get; set; }
        public static ConfigEntry<float> LowReadyTransitionToShortStock { get; set; }
        public static ConfigEntry<float> LowReadyTransitionFromIdle { get; set; }

        // Short-Stock Transitions
        public static ConfigEntry<float> ShortStockBlendThresholdActiveAim { get; set; }
        public static ConfigEntry<float> ShortStockBlendThresholdHighReady { get; set; }
        public static ConfigEntry<float> ShortStockBlendThresholdLowReady { get; set; }
        public static ConfigEntry<float> ShortStockBlendThresholdPatrol { get; set; }
        public static ConfigEntry<float> ShortStockBlendThresholdLeftShoulder { get; set; }
        public static ConfigEntry<float> ShortStockTransitionFromActiveAim { get; set; }
        public static ConfigEntry<float> ShortStockTransitionFromHighReady { get; set; }
        public static ConfigEntry<float> ShortStockTransitionFromLowReady { get; set; }
        public static ConfigEntry<float> ShortStockTransitionFromPatrol { get; set; }
        public static ConfigEntry<float> ShortStockTransitionFromLeftShoulder { get; set; }
        public static ConfigEntry<float> ShortStockTransitionToActiveAim { get; set; }
        public static ConfigEntry<float> ShortStockTransitionToHighReady { get; set; }
        public static ConfigEntry<float> ShortStockTransitionToLowReady { get; set; }
        public static ConfigEntry<float> ShortStockTransitionToPatrol { get; set; }
        public static ConfigEntry<float> ShortStockTransitionToLeftShoulder { get; set; }
        public static ConfigEntry<float> ShortStockTransitionFromIdle { get; set; }

        // Patrol Stance Transitions
        public static ConfigEntry<float> PatrolBlendThresholdActiveAim { get; set; }
        public static ConfigEntry<float> PatrolBlendThresholdHighReady { get; set; }
        public static ConfigEntry<float> PatrolBlendThresholdLowReady { get; set; }
        public static ConfigEntry<float> PatrolBlendThresholdLeftShoulder { get; set; }
        public static ConfigEntry<float> PatrolBlendThresholdShortStock { get; set; }
        public static ConfigEntry<float> PatrolTransitionFromActiveAim { get; set; }
        public static ConfigEntry<float> PatrolTransitionFromHighReady { get; set; }
        public static ConfigEntry<float> PatrolTransitionFromLowReady { get; set; }
        public static ConfigEntry<float> PatrolTransitionFromLeftShoulder { get; set; }
        public static ConfigEntry<float> PatrolTransitionFromShortStock { get; set; }
        public static ConfigEntry<float> PatrolTransitionToActiveAim { get; set; }
        public static ConfigEntry<float> PatrolTransitionToHighReady { get; set; }
        public static ConfigEntry<float> PatrolTransitionToLowReady { get; set; }
        public static ConfigEntry<float> PatrolTransitionToLeftShoulder { get; set; }
        public static ConfigEntry<float> PatrolTransitionToShortStock { get; set; }
        public static ConfigEntry<float> PatrolTransitionFromIdle { get; set; }

        // Left Shoulder Transitions
        public static ConfigEntry<float> LeftShoulderBlendThresholdActiveAim { get; set; }
        public static ConfigEntry<float> LeftShoulderBlendThresholdHighReady { get; set; }
        public static ConfigEntry<float> LeftShoulderBlendThresholdLowReady { get; set; }
        public static ConfigEntry<float> LeftShoulderBlendThresholdPatrol { get; set; }
        public static ConfigEntry<float>? LeftShoulderBlendThresholdShortStock { get; set; }
        public static ConfigEntry<float>? LeftShoulderTransitionFromActiveAim { get; set; }
        public static ConfigEntry<float>? LeftShoulderTransitionFromHighReady { get; set; }
        public static ConfigEntry<float>? LeftShoulderTransitionFromLowReady { get; set; }
        public static ConfigEntry<float>? LeftShoulderTransitionFromPatrol { get; set; }
        public static ConfigEntry<float>? LeftShoulderTransitionFromShortStock { get; set; }
        public static ConfigEntry<float>? LeftShoulderTransitionToActiveAim { get; set; }
        public static ConfigEntry<float>? LeftShoulderTransitionToHighReady { get; set; }
        public static ConfigEntry<float>? LeftShoulderTransitionToLowReady { get; set; }
        public static ConfigEntry<float>? LeftShoulderTransitionToPatrol { get; set; }
        public static ConfigEntry<float>? LeftShoulderTransitionToShortStock { get; set; }
        public static ConfigEntry<float> LeftShoulderTransitionFromIdle { get; set; }

        public static ConfigEntry<KeyboardShortcut> ActiveAimKeybind { get; set; }
        public static ConfigEntry<KeyboardShortcut> LowReadyKeybind { get; set; }
        public static ConfigEntry<KeyboardShortcut> HighReadyKeybind { get; set; }
        public static ConfigEntry<KeyboardShortcut> ShortStockKeybind { get; set; }
        public static ConfigEntry<KeyboardShortcut> PatrolKeybind { get; set; }
        public static ConfigEntry<KeyboardShortcut> MeleeKeybind { get; set; } = null!;
        public static ConfigEntry<KeyboardShortcut> StanceWheelComboKeyBind { get; set; } = null!;
        public static ConfigEntry<float> StanceSfxModifier { get; set; } = null!;
        public static ConfigEntry<bool> EnableAnimationFixes { get; set; } = null!;
        public static ConfigEntry<bool> OverrideCollision { get; set; } = null!;
        public static ConfigEntry<bool> OverrideMounting { get; set; } = null!;
        public static ConfigEntry<bool> UseMouseWheelStance { get; set; } = null!;
        public static ConfigEntry<bool> UseMouseWheelPlusKey { get; set; } = null!;

        public static ConfigEntry<bool> EnableFSAimBlock { get; set; } = null!;
        public static ConfigEntry<bool> EnableNVGAimBlock { get; set; } = null!;
        public static ConfigEntry<bool> EnableThermalAimBlock { get; set; }  = null!;

        public static ConfigEntry<bool> EnableMountUI { get; set; }
        public static ConfigEntry<bool> ToggleActiveAim { get; set; }
        public static ConfigEntry<bool> AllowActiveAimReload { get; set; }
        public static ConfigEntry<bool> EnableAltPistol { get; set; }
        public static ConfigEntry<bool> EnableAltRifle { get; set; }
        public static ConfigEntry<bool> EnableAltRifleRecoil { get; set; }
        public static ConfigEntry<bool> EnableIdleStamDrain { get; set; }
        public static ConfigEntry<float> IdleStamDrainModi { get; set; }
        public static ConfigEntry<bool> EnableStanceStamChanges { get; set; }
        public static ConfigEntry<bool> EnableTacSprint { get; set; }
        public static ConfigEntry<bool> BlockFiring { get; set; }
        public static ConfigEntry<bool> RememberStanceFiring { get; set; }
        public static ConfigEntry<bool> RememberStanceItem { get; set; }
        public static ConfigEntry<bool> EnableExtraProcEffects { get; set; }        
        public static ConfigEntry<float> StanceBlendSpeed { get; set; }
        public static ConfigEntry<float> GlobalStanceSpeed { get; set; }
        public static ConfigEntry<Vector3> WeapOffset { get; set; }

        public static void InitConfigBindings(ConfigFile config)
        {
            string dev = "0. Dev.";
            string weapAimAndPos = "1. Weapon Stances And Position.";
            string stanceBinds = "2. Weapon Stances Keybinds.";
            string activeAim = "3. Active Aim.";
            string highReady = "4. High Ready.";
            string lowReady = "5. Low Ready.";
            string shortStock = "6. Short-Stocking.";
            string patrol = "7. Patrol Stance.";
            string leftShoulder = "8. Left Shoulder.";

            test1 = config.Bind<float>(dev, "test 1", 1f, new ConfigDescription("", new AcceptableValueRange<float>(-5000f, 5000f), new ConfigurationManagerAttributes { Order = 170, IsAdvanced = true, Browsable = true }));
            test2 = config.Bind<float>(dev, "test 2", 1f, new ConfigDescription("", new AcceptableValueRange<float>(-5000f, 5000f), new ConfigurationManagerAttributes { Order = 160, IsAdvanced = true, Browsable = true }));
            test3 = config.Bind<float>(dev, "test 3", 1f, new ConfigDescription("", new AcceptableValueRange<float>(-5000f, 5000f), new ConfigurationManagerAttributes { Order = 150, IsAdvanced = true, Browsable = true }));
            test4 = config.Bind<float>(dev, "test 4", 1f, new ConfigDescription("", new AcceptableValueRange<float>(-5000f, 5000f), new ConfigurationManagerAttributes { Order = 140, IsAdvanced = true, Browsable = true }));
            test5 = config.Bind<float>(dev, "test 5", 1f, new ConfigDescription("", new AcceptableValueRange<float>(-5000f, 5000f), new ConfigurationManagerAttributes { Order = 130, IsAdvanced = true, Browsable = true }));
            test6 = config.Bind<float>(dev, "test 6", 1f, new ConfigDescription("", new AcceptableValueRange<float>(-5000f, 5000f), new ConfigurationManagerAttributes { Order = 120, IsAdvanced = true, Browsable = true }));
            test7 = config.Bind<float>(dev, "test 7", 1f, new ConfigDescription("", new AcceptableValueRange<float>(-5000f, 5000f), new ConfigurationManagerAttributes { Order = 110, IsAdvanced = true, Browsable = true }));
            test8 = config.Bind<float>(dev, "test 8", 1f, new ConfigDescription("", new AcceptableValueRange<float>(-5000f, 5000f), new ConfigurationManagerAttributes { Order = 100, IsAdvanced = true, Browsable = true }));
            test9 = config.Bind<float>(dev, "test 9", 1f, new ConfigDescription("", new AcceptableValueRange<float>(-5000f, 5000f), new ConfigurationManagerAttributes { Order = 90, IsAdvanced = true, Browsable = true }));
            test10 = config.Bind<float>(dev, "test 10", 1f, new ConfigDescription("", new AcceptableValueRange<float>(-5000f, 5000f), new ConfigurationManagerAttributes { Order = 80, IsAdvanced = true, Browsable = true }));
            test11 = config.Bind<float>(dev, "test 11", 1f, new ConfigDescription("", new AcceptableValueRange<float>(-5000f, 5000f), new ConfigurationManagerAttributes { Order = 70, IsAdvanced = true, Browsable = true }));
            test12 = config.Bind<float>(dev, "test 12", 1f, new ConfigDescription("", new AcceptableValueRange<float>(-5000f, 5000f), new ConfigurationManagerAttributes { Order = 60, IsAdvanced = true, Browsable = true }));
            test13 = config.Bind<float>(dev, "test 13", 1f, new ConfigDescription("", new AcceptableValueRange<float>(-5000f, 5000f), new ConfigurationManagerAttributes { Order = 50, IsAdvanced = true, Browsable = true }));
            test14 = config.Bind<float>(dev, "test 14", 1f, new ConfigDescription("", new AcceptableValueRange<float>(-5000f, 5000f), new ConfigurationManagerAttributes { Order = 40, IsAdvanced = true, Browsable = true }));
            test15 = config.Bind<float>(dev, "test 15", 1f, new ConfigDescription("", new AcceptableValueRange<float>(-5000f, 5000f), new ConfigurationManagerAttributes { Order = 30, IsAdvanced = true, Browsable = true }));
            test16 = config.Bind<float>(dev, "test 16", 1f, new ConfigDescription("", new AcceptableValueRange<float>(-5000f, 5000f), new ConfigurationManagerAttributes { Order = 20, IsAdvanced = true, Browsable = true }));
            test17 = config.Bind<float>(dev, "test 17", 1f, new ConfigDescription("", new AcceptableValueRange<float>(-5000f, 5000f), new ConfigurationManagerAttributes { Order = 10, IsAdvanced = true, Browsable = true }));
            test18 = config.Bind<float>(dev, "test 18", 1f, new ConfigDescription("", new AcceptableValueRange<float>(-5000f, 5000f), new ConfigurationManagerAttributes { Order = 0, IsAdvanced = true, Browsable = true }));
            test19 = config.Bind<float>(dev, "test 19", 1f, new ConfigDescription("", new AcceptableValueRange<float>(-5000f, 5000f), new ConfigurationManagerAttributes { Order = -10, IsAdvanced = true, Browsable = true }));
            test20 = config.Bind<float>(dev, "test 20", 1f, new ConfigDescription("", new AcceptableValueRange<float>(-5000f, 5000f), new ConfigurationManagerAttributes { Order = -10, IsAdvanced = true, Browsable = true }));

            EnableAnimationFixes = config.Bind<bool>(weapAimAndPos, "De-Jank EFT Animations",true, new ConfigDescription("Attempts To Make EFT Certain Animations Less Janky, Like Inventory And Door Animations.", null, new ConfigurationManagerAttributes { Order = 430, Browsable =true }));
            OverrideCollision = config.Bind<bool>(weapAimAndPos, "Override Collision",true, new ConfigDescription("If FOV Fix is installed, will override BSG's collision system completely to make it work well with stances and alt weapon positions. If not installed, will modify BSG collision system.", null, new ConfigurationManagerAttributes { Order = 410, Browsable =true }));
            OverrideMounting = config.Bind<bool>(weapAimAndPos, "Use Realism Mounting System",true, new ConfigDescription("Overrides BSG's Mounting System WIth Realism's (That Was Implemented First). Recoil, Stance and Sway Mechanics Are All Built Around Realism's Mounting And Won't Function Correctly With BSG's.", null, new ConfigurationManagerAttributes { Order = 300, Browsable =true }));
            EnableExtraProcEffects = config.Bind<bool>(weapAimAndPos, "Enable Extra Weapon Position/Rotation Effects",true, new ConfigDescription("Weapon Has A Slight Cant To It based On Ergo. ADS With Gasmask/Faceshield Is Canted. Weapon Cant Increases When Crouching, And Moves Closer To You. Other Sublte Effects.", null, new ConfigurationManagerAttributes { Order = 280, Browsable =true }));
            RememberStanceItem = config.Bind<bool>(weapAimAndPos, "Remember Stance After Using Item",true, new ConfigDescription("Remember Stance After Actions (Using Items)", null, new ConfigurationManagerAttributes { Order = 260, Browsable =true }));
            RememberStanceFiring = config.Bind<bool>(weapAimAndPos, "Remember Stance After Firing",true, new ConfigDescription("Remember Stance After Firing If The Player Was Aiming.", null, new ConfigurationManagerAttributes { Order = 260, Browsable =true }));
            BlockFiring = config.Bind<bool>(weapAimAndPos, "Block Shooting While In Stance", false, new ConfigDescription("Blocks Firing While In A Stance, Will Cancel Stance If Attempting To Fire.", null, new ConfigurationManagerAttributes { Order = 250, Browsable =true }));
            EnableTacSprint = config.Bind<bool>(weapAimAndPos, "Enable High Ready Sprint Animation",true, new ConfigDescription("Enables Usage Of High Ready Sprint Animation When Sprinting From High Ready Position.", null, new ConfigurationManagerAttributes { Order = 230, Browsable =true }));
            EnableAltPistol = config.Bind<bool>(weapAimAndPos, "Enable Alternative Pistol Position And ADS",true, new ConfigDescription("Pistol Will Be Held Centered And In A Compressed Stance. ADS is animated. If FOV Fix is used, the gun will move to the camera for smoother ADS.", null, new ConfigurationManagerAttributes { Order = 229, Browsable =true }));
            EnableAltRifle = config.Bind<bool>(weapAimAndPos, "Enable Alternative Rifle Position And ADS",true, new ConfigDescription("Rifle position will be more centered. If FOV Fix is used, the gun will move to the camera for smoother ADS.", null, new ConfigurationManagerAttributes { Order = 229, Browsable =true }));
            EnableAltRifleRecoil = config.Bind<bool>(weapAimAndPos, "Enable Alternative Rifle Recoil Override",true, new ConfigDescription("When using alt rifle, let it override recoil. This results in different recoil feel but smoother transition from firing to non-firing ADS state.", null, new ConfigurationManagerAttributes { Order = 229, Browsable =true }));
            EnableIdleStamDrain = config.Bind<bool>(weapAimAndPos, "Enable Idle Arm Stamina Drain",true, new ConfigDescription("Arm Stamina Will Drain When Not In A Stance (High And Low Ready, Short-Stocking).", null, new ConfigurationManagerAttributes { Order = 210, Browsable =true }));
            IdleStamDrainModi = config.Bind<float>(weapAimAndPos, "Idle Stam Drain Modifer", 0.1f, new ConfigDescription("", new AcceptableValueRange<float>(0.0f, 5f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 200, Browsable =true }));
            EnableStanceStamChanges = config.Bind<bool>(weapAimAndPos, "Enable Stance Stamina And Movement Effects",true, new ConfigDescription("Enabled Stances And Mounting To Affect Stamina And Movement Speed. Stamina Drain May Not Work Correctly If Disabled. High + Low Ready, Short-Stocking And Pistol Idle Will Regenerate Stamina Faster And Optionally Idle With Rifles Drains Stamina. High Ready Has Faster Sprint Speed And Sprint Accel, Low Ready Has Faster Sprint Accel. Arm Stamina Won't Drain Regular Stamina If It Reaches 0.", null, new ConfigurationManagerAttributes { Order = 183, Browsable =true }));
            AllowActiveAimReload = config.Bind<bool>(weapAimAndPos, "Allow Reload From Active Aim", false, new ConfigDescription("Allows Reload From Magazine While In Active Aim With Speed Bonus.", null, new ConfigurationManagerAttributes { Order = 190, Browsable =true }));
            EnableMountUI = config.Bind<bool>(weapAimAndPos, "Enable Mounting UI",true, new ConfigDescription("If Enabled, An Icon On Screen Will Indicate If Player Is Bracing, Mounting And What Side Of Cover They Are On.", null, new ConfigurationManagerAttributes { Order = 179, Browsable =true }));
            WeapOffset = config.Bind<Vector3>(weapAimAndPos, "Rifle Position Offset", new Vector3(-0.04f, -0.015f, 0f), new ConfigDescription("Config option 'alt rife' is required. Adjusts The Starting Position Of Rifle On Screen.", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 152, IsAdvanced = true, Browsable =true }));
            StanceSfxModifier = config.Bind<float>(weapAimAndPos, "Stance Sfx Volume Modifier", 2f, new ConfigDescription("Gear rattle volume modifer when doing stance related things", new AcceptableValueRange<float>(0.1f, 20f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 153, Browsable =true }));
            StanceBlendSpeed = config.Bind<float>(weapAimAndPos, "Stance Blend Speed", 18f, new ConfigDescription("Speed of stance blending transitions", new AcceptableValueRange<float>(0.1f, 100f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 151, IsAdvanced = true, Browsable = true }));
            GlobalStanceSpeed = config.Bind<float>(weapAimAndPos, "Global Stance Speed", 1f, new ConfigDescription("Global multiplier for all stance speeds", new AcceptableValueRange<float>(0.1f, 10f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 150, IsAdvanced = true, Browsable = true }));

            EnableFSAimBlock = config.Bind<bool>(weapAimAndPos, "Enable Faceshield Aim Block", true, new ConfigDescription("Faceshields Block ADS Unless The Specfic Stock/Weapon/Faceshield Allows It.", null, new ConfigurationManagerAttributes { Order = 4, Browsable = true }));
            EnableNVGAimBlock = config.Bind<bool>(weapAimAndPos, "Enable NVG Aim Block", true, new ConfigDescription("Magnified Optics Block ADS When Using NVGs.", null, new ConfigurationManagerAttributes { Order = 5, Browsable = true }));
            EnableThermalAimBlock = config.Bind<bool>(weapAimAndPos, "Enable Thermal Aim Block", true, new ConfigDescription("Can't Aim With Sights When Using Thermal Goggles.", null, new ConfigurationManagerAttributes { Order = 5, Browsable = true }));

            MeleeKeybind = config.Bind(stanceBinds, "Melee Keybind", new KeyboardShortcut(KeyCode.None), new ConfigDescription("Strike With Muzzle Or Bayonet Of Equipped Weapon.", null, new ConfigurationManagerAttributes { Order = 150, Browsable =true }));
            UseMouseWheelStance = config.Bind<bool>(stanceBinds, "Enable Mouse Wheel Stance Switching",true, new ConfigDescription("Switches Between High And Low Ready Via Mouse Wheel.", null, new ConfigurationManagerAttributes { Order = 160, Browsable =true }));
            UseMouseWheelPlusKey = config.Bind<bool>(stanceBinds, "Require Key + Mouse Wheel",true, new ConfigDescription("Require Keybind + Mouse Wheel To Change Stance.", null, new ConfigurationManagerAttributes { Order = 170, Browsable =true }));
            StanceWheelComboKeyBind = config.Bind(stanceBinds, "Keybind To Use With Mouse Wheel", new KeyboardShortcut(KeyCode.LeftAlt), new ConfigDescription("Key Used In Combination With Mouse Wheel If Enabled ", null, new ConfigurationManagerAttributes { Order = 180, Browsable =true }));

            ActiveAimKeybind = config.Bind(activeAim, "Active Aim Keybind", new KeyboardShortcut(KeyCode.Mouse4), new ConfigDescription("Cants The Weapon Sideways, Improving Hipfire Accuracy.", null, new ConfigurationManagerAttributes { Order = 110, Browsable = true }));
            ToggleActiveAim = config.Bind<bool>(activeAim, "Use Toggle For Active Aim", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 109, Browsable = true }));
            ActiveAimWalkSpeedBonus = config.Bind<float>(activeAim, "Active Aim Walk Speed Modifier", 0.9f, new ConfigDescription("Multiplier applied to walk speed while in Active Aim.", new AcceptableValueRange<float>(0.1f, 5f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 108, Browsable = true }));
            ActiveAimSprintAccelBonus = config.Bind<float>(activeAim, "Active Aim Sprint Accel Modifier", 1.1f, new ConfigDescription("Multiplier applied to sprint acceleration while in Active Aim.", new AcceptableValueRange<float>(0.1f, 5f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 107, Browsable = true }));
            ActiveAimStaminaRate = config.Bind<float>(activeAim, "Active Aim Stamina Rate", 0.075f, new ConfigDescription("Rate at which arm stamina drains while in Active Aim.", new AcceptableValueRange<float>(0f, 20f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 106, Browsable = true }));
            ActiveAimTransitionFromIdle = config.Bind<float>(activeAim, "Active Aim Transition From: Idle", 3f, new ConfigDescription("Speed of entering Active Aim from idle (no previous stance).", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 105, Browsable = true }));
            ActiveAimBlendThresholdLowReady = config.Bind<float>(activeAim, "Active Aim Blend Threshold: Low Ready", 0.15f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 99, IsAdvanced = true, Browsable = true }));
            ActiveAimBlendThresholdHighReady = config.Bind<float>(activeAim, "Active Aim Blend Threshold: High Ready", 0f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 98, IsAdvanced = true, Browsable = true }));
            ActiveAimBlendThresholdLeftShoulder = config.Bind<float>(activeAim, "Active Aim Blend Threshold: Left Shoulder", 0f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 97, IsAdvanced = true, Browsable = true }));
            ActiveAimBlendThresholdPatrol = config.Bind<float>(activeAim, "Active Aim Blend Threshold: Patrol", 1f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 96, IsAdvanced = true, Browsable = true }));
            ActiveAimBlendThresholdShortStock = config.Bind<float>(activeAim, "Active Aim Blend Threshold: Short-Stock", 0f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 95, IsAdvanced = true, Browsable = true }));
            ActiveAimTransitionFromLowReady = config.Bind<float>(activeAim, "Active Aim Transition From: Low Ready", 3.85f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 94, IsAdvanced = true, Browsable = true }));
            ActiveAimTransitionFromHighReady = config.Bind<float>(activeAim, "Active Aim Transition From: High Ready", 3f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 93, IsAdvanced = true, Browsable = true }));
            ActiveAimTransitionFromLeftShoulder = config.Bind<float>(activeAim, "Active Aim Transition From: Left Shoulder", 3f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 92, IsAdvanced = true, Browsable = true }));
            ActiveAimTransitionFromPatrol = config.Bind<float>(activeAim, "Active Aim Transition From: Patrol", 1f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 91, IsAdvanced = true, Browsable = true }));
            ActiveAimTransitionFromShortStock = config.Bind<float>(activeAim, "Active Aim Transition From: Short-Stock", 2.25f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 90, IsAdvanced = true, Browsable = true }));
            ActiveAimTransitionToLowReady = config.Bind<float>(activeAim, "Active Aim Transition To Speed: Low Ready", 0.75f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 89, IsAdvanced = true, Browsable = true }));
            ActiveAimTransitionToHighReady = config.Bind<float>(activeAim, "Active Aim Transition To Speed: High Ready", 1f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 88, IsAdvanced = true, Browsable = true }));
            ActiveAimTransitionToLeftShoulder = config.Bind<float>(activeAim, "Active Aim Transition To Speed: Left Shoulder", 0.1f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 87, IsAdvanced = true, Browsable = true }));
            ActiveAimTransitionToPatrol = config.Bind<float>(activeAim, "Active Aim Transition To Speed: Patrol", 1f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 86, IsAdvanced = true, Browsable = true }));
            ActiveAimTransitionToShortStock = config.Bind<float>(activeAim, "Active Aim Transition To Speed: Short-Stock", 0.2f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 85, IsAdvanced = true, Browsable = true }));

            HighReadyKeybind = config.Bind(highReady, "High Ready Keybind", new KeyboardShortcut(KeyCode.UpArrow), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 75, Browsable = true }));
            HighReadyWalkSpeedBonus = config.Bind<float>(highReady, "High Ready Walk Speed Modifier", 1.035f, new ConfigDescription("Multiplier applied to walk speed while in High Ready.", new AcceptableValueRange<float>(0.1f, 5f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 74, Browsable = true }));
            HighReadySprintAccelBonus = config.Bind<float>(highReady, "High Ready Sprint Accel Modifier", 1.2f, new ConfigDescription("Multiplier applied to sprint acceleration while in High Ready.", new AcceptableValueRange<float>(0.1f, 5f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 73, Browsable = true }));
            HighReadyStaminaRate = config.Bind<float>(highReady, "High Ready Stamina Rate", 1.85f, new ConfigDescription("Rate at which arm stamina regenerates while in High Ready.", new AcceptableValueRange<float>(0f, 20f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 72, Browsable = true }));
            HighReadyTransitionFromIdle = config.Bind<float>(highReady, "High Ready Transition From: Idle", 3f, new ConfigDescription("Speed of entering High Ready from idle (no previous stance).", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 71, Browsable = true }));
            HighReadyBlendThresholdActiveAim = config.Bind<float>(highReady, "High Ready Blend Threshold: Active Aim", 0.2f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 64, IsAdvanced = true, Browsable = true }));
            HighReadyBlendThresholdLowReady = config.Bind<float>(highReady, "High Ready Blend Threshold: Low Ready", 0.05f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 63, IsAdvanced = true, Browsable = true }));
            HighReadyBlendThresholdLeftShoulder = config.Bind<float>(highReady, "High Ready Blend Threshold: Left Shoulder", 0.2f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 62, IsAdvanced = true, Browsable = true }));
            HighReadyBlendThresholdPatrol = config.Bind<float>(highReady, "High Ready Blend Threshold: Patrol", 0f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 61, IsAdvanced = true, Browsable = true }));
            HighReadyBlendThresholdShortStock = config.Bind<float>(highReady, "High Ready Blend Threshold: Short-Stock", 0f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 60, IsAdvanced = true, Browsable = true }));
            HighReadyTransitionFromActiveAim = config.Bind<float>(highReady, "High Ready Transition From: Active Aim", 2.5f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 59, IsAdvanced = true, Browsable = true }));
            HighReadyTransitionFromLowReady = config.Bind<float>(highReady, "High Ready Transition From: Low Ready", 3f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 58, IsAdvanced = true, Browsable = true }));
            HighReadyTransitionFromLeftShoulder = config.Bind<float>(highReady, "High Ready Transition From: Left Shoulder", 1.85f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 57, IsAdvanced = true, Browsable = true }));
            HighReadyTransitionFromPatrol = config.Bind<float>(highReady, "High Ready Transition From: Patrol", 1.25f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 56, IsAdvanced = true, Browsable = true }));
            HighReadyTransitionFromShortStock = config.Bind<float>(highReady, "High Ready Transition From: Short-Stock", 0.75f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 55, IsAdvanced = true, Browsable = true }));
            HighReadyTransitionToActiveAim = config.Bind<float>(highReady, "High Ready Transition To Speed: Active Aim", 1f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 54, IsAdvanced = true, Browsable = true }));
            HighReadyTransitionToLowReady = config.Bind<float>(highReady, "High Ready Transition To Speed: Low Ready", 0.6f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 53, IsAdvanced = true, Browsable = true }));
            HighReadyTransitionToLeftShoulder = config.Bind<float>(highReady, "High Ready Transition To Speed: Left Shoulder", 1.8f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 52, IsAdvanced = true, Browsable = true }));
            HighReadyTransitionToPatrol = config.Bind<float>(highReady, "High Ready Transition To Speed: Patrol", 1f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 51, IsAdvanced = true, Browsable = true }));
            HighReadyTransitionToShortStock = config.Bind<float>(highReady, "High Ready Transition To Speed: Short-Stock", 0.8f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 50, IsAdvanced = true, Browsable = true }));

            LowReadyKeybind = config.Bind(lowReady, "Low Ready Keybind", new KeyboardShortcut(KeyCode.DownArrow), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 40, Browsable = true }));
            LowReadyWalkSpeedBonus = config.Bind<float>(lowReady, "Low Ready Walk Speed Modifier", 1.055f, new ConfigDescription("Multiplier applied to walk speed while in Low Ready.", new AcceptableValueRange<float>(0.1f, 5f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 39, Browsable = true }));
            LowReadySprintAccelBonus = config.Bind<float>(lowReady, "Low Ready Sprint Accel Modifier", 1.25f, new ConfigDescription("Multiplier applied to sprint acceleration while in Low Ready.", new AcceptableValueRange<float>(0.1f, 5f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 38, Browsable = true }));
            LowReadyStaminaRate = config.Bind<float>(lowReady, "Low Ready Stamina Rate", 2.4f, new ConfigDescription("Rate at which arm stamina regenerates while in Low Ready.", new AcceptableValueRange<float>(0f, 20f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 37, Browsable = true }));
            LowReadyTransitionFromIdle = config.Bind<float>(lowReady, "Low Ready Transition From: Idle", 3f, new ConfigDescription("Speed of entering Low Ready from idle (no previous stance).", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 36, Browsable = true }));
            LowReadyBlendThresholdActiveAim = config.Bind<float>(lowReady, "Low Ready Blend Threshold: Active Aim", 0f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 34, IsAdvanced = true, Browsable = true }));
            LowReadyBlendThresholdHighReady = config.Bind<float>(lowReady, "Low Ready Blend Threshold: High Ready", 0.25f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 33, IsAdvanced = true, Browsable = true }));
            LowReadyBlendThresholdLeftShoulder = config.Bind<float>(lowReady, "Low Ready Blend Threshold: Left Shoulder", 0.5f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 32, IsAdvanced = true, Browsable = true }));
            LowReadyBlendThresholdPatrol = config.Bind<float>(lowReady, "Low Ready Blend Threshold: Patrol", 0f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 31, IsAdvanced = true, Browsable = true }));
            LowReadyBlendThresholdShortStock = config.Bind<float>(lowReady, "Low Ready Blend Threshold: Short-Stock", 0f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 30, IsAdvanced = true, Browsable = true }));
            LowReadyTransitionFromActiveAim = config.Bind<float>(lowReady, "Low Ready Transition From: Active Aim", 3f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 29, IsAdvanced = true, Browsable = true }));
            LowReadyTransitionFromHighReady = config.Bind<float>(lowReady, "Low Ready Transition From: High Ready", 3f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 28, IsAdvanced = true, Browsable = true }));
            LowReadyTransitionFromLeftShoulder = config.Bind<float>(lowReady, "Low Ready Transition From: Left Shoulder", 3f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 27, IsAdvanced = true, Browsable = true }));
            LowReadyTransitionFromPatrol = config.Bind<float>(lowReady, "Low Ready Transition From: Patrol", 2f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 26, IsAdvanced = true, Browsable = true }));
            LowReadyTransitionFromShortStock = config.Bind<float>(lowReady, "Low Ready Transition From: Short-Stock", 2f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 25, IsAdvanced = true, Browsable = true }));
            LowReadyTransitionToActiveAim = config.Bind<float>(lowReady, "Low Ready Transition To Speed: Active Aim", 2f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 24, IsAdvanced = true, Browsable = true }));
            LowReadyTransitionToHighReady = config.Bind<float>(lowReady, "Low Ready Transition To Speed: High Ready", 1.25f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 23, IsAdvanced = true, Browsable = true }));
            LowReadyTransitionToLeftShoulder = config.Bind<float>(lowReady, "Low Ready Transition To Speed: Left Shoulder", 1.8f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 22, IsAdvanced = true, Browsable = true }));
            LowReadyTransitionToPatrol = config.Bind<float>(lowReady, "Low Ready Transition To Speed: Patrol", 0.15f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 21, IsAdvanced = true, Browsable = true }));
            LowReadyTransitionToShortStock = config.Bind<float>(lowReady, "Low Ready Transition To Speed: Short-Stock", 1f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 20, IsAdvanced = true, Browsable = true }));

            ShortStockKeybind = config.Bind(shortStock, "Short-Stock Keybind", new KeyboardShortcut(KeyCode.J), new ConfigDescription("Tucks The Weapon's Stock Under Player's Arm, Shortening The Overall Length Of The Wweapon To Prevent Muzzle Being Pushed Away From Target.", null, new ConfigurationManagerAttributes { Order = 10, Browsable = true }));
            ShortStockWalkSpeedBonus = config.Bind<float>(shortStock, "Short-Stock Walk Speed Modifier", 0.9f, new ConfigDescription("Multiplier applied to walk speed while Short-Stocking.", new AcceptableValueRange<float>(0.1f, 5f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 9, Browsable = true }));
            ShortStockSprintAccelBonus = config.Bind<float>(shortStock, "Short-Stock Sprint Accel Modifier", 0.9f, new ConfigDescription("Multiplier applied to sprint acceleration while Short-Stocking.", new AcceptableValueRange<float>(0.1f, 5f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 8, Browsable = true }));
            ShortStockStaminaRate = config.Bind<float>(shortStock, "Short-Stock Stamina Rate", 1.3f, new ConfigDescription("Rate at which arm stamina regenerates while Short-Stocking.", new AcceptableValueRange<float>(0f, 20f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 7, Browsable = true }));
            ShortStockTransitionFromIdle = config.Bind<float>(shortStock, "Short-Stock Transition From: Idle", 2.25f, new ConfigDescription("Speed of entering Short-Stock from idle (no previous stance).", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 6, Browsable = true }));
            ShortStockBlendThresholdActiveAim = config.Bind<float>(shortStock, "Short-Stock Blend Threshold: Active Aim", 0f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 2, IsAdvanced = true, Browsable = true }));
            ShortStockBlendThresholdHighReady = config.Bind<float>(shortStock, "Short-Stock Blend Threshold: High Ready", 0f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 1, IsAdvanced = true, Browsable = true }));
            ShortStockBlendThresholdLowReady = config.Bind<float>(shortStock, "Short-Stock Blend Threshold: Low Ready", 0f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 0, IsAdvanced = true, Browsable = true }));
            ShortStockBlendThresholdPatrol = config.Bind<float>(shortStock, "Short-Stock Blend Threshold: Patrol", 0f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = -1, IsAdvanced = true, Browsable = true }));
            ShortStockBlendThresholdLeftShoulder = config.Bind<float>(shortStock, "Short-Stock Blend Threshold: Left Shoulder", 0f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = -2, IsAdvanced = true, Browsable = true }));
            ShortStockTransitionFromActiveAim = config.Bind<float>(shortStock, "Short-Stock Transition From: Active Aim", 2.25f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = -3, IsAdvanced = true, Browsable = true }));
            ShortStockTransitionFromHighReady = config.Bind<float>(shortStock, "Short-Stock Transition From: High Ready", 0.7f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = -4, IsAdvanced = true, Browsable = true }));
            ShortStockTransitionFromLowReady = config.Bind<float>(shortStock, "Short-Stock Transition From: Low Ready", 1f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = -5, IsAdvanced = true, Browsable = true }));
            ShortStockTransitionFromPatrol = config.Bind<float>(shortStock, "Short-Stock Transition From: Patrol", 0.5f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = -6, IsAdvanced = true, Browsable = true }));
            ShortStockTransitionFromLeftShoulder = config.Bind<float>(shortStock, "Short-Stock Transition From: Left Shoulder", 1.55f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = -7, IsAdvanced = true, Browsable = true }));
            ShortStockTransitionToActiveAim = config.Bind<float>(shortStock, "Short-Stock Transition To Speed: Active Aim", 0.1f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = -8, IsAdvanced = true, Browsable = true }));
            ShortStockTransitionToHighReady = config.Bind<float>(shortStock, "Short-Stock Transition To Speed: High Ready", 1f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = -9, IsAdvanced = true, Browsable = true }));
            ShortStockTransitionToLowReady = config.Bind<float>(shortStock, "Short-Stock Transition To Speed: Low Ready", 0.8f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = -10, IsAdvanced = true, Browsable = true }));
            ShortStockTransitionToPatrol = config.Bind<float>(shortStock, "Short-Stock Transition To Speed: Patrol", 1f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = -11, IsAdvanced = true, Browsable = true }));
            ShortStockTransitionToLeftShoulder = config.Bind<float>(shortStock, "Short-Stock Transition To Speed: Left Shoulder", 0.5f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = -12, IsAdvanced = true, Browsable = true }));

            PatrolKeybind = config.Bind(patrol, "Patrol/Neutral Stance Keybind", new KeyboardShortcut(KeyCode.H), new ConfigDescription("Puts The Weapon In A Neutral Position, Improving Arm Stam Regen And Walk Speed. For Maximum Larping.", null, new ConfigurationManagerAttributes { Order = 55, Browsable = true }));
            PatrolWalkSpeedBonus = config.Bind<float>(patrol, "Patrol Walk Speed Modifier", 1.1f, new ConfigDescription("Multiplier applied to walk speed while in Patrol stance.", new AcceptableValueRange<float>(0.1f, 5f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 54, Browsable = true }));
            PatrolSprintAccelBonus = config.Bind<float>(patrol, "Patrol Sprint Accel Modifier", 1.45f, new ConfigDescription("Multiplier applied to sprint acceleration while in Patrol stance.", new AcceptableValueRange<float>(0.1f, 5f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 53, Browsable = true }));
            PatrolStaminaRate = config.Bind<float>(patrol, "Patrol Stamina Rate", 4.0f, new ConfigDescription("Rate at which arm stamina regenerates while in Patrol stance.", new AcceptableValueRange<float>(0f, 20f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 52, Browsable = true }));
            PatrolTransitionFromIdle = config.Bind<float>(patrol, "Patrol Transition From: Idle", 1.65f, new ConfigDescription("Speed of entering Patrol stance from idle (no previous stance).", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 51, Browsable = true }));
            PatrolBlendThresholdActiveAim = config.Bind<float>(patrol, "Patrol Blend Threshold: Active Aim", 0f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 45, IsAdvanced = true, Browsable = true }));
            PatrolBlendThresholdHighReady = config.Bind<float>(patrol, "Patrol Blend Threshold: High Ready", 0f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 44, IsAdvanced = true, Browsable = true }));
            PatrolBlendThresholdLowReady = config.Bind<float>(patrol, "Patrol Blend Threshold: Low Ready", 0f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 43, IsAdvanced = true, Browsable = true }));
            PatrolBlendThresholdLeftShoulder = config.Bind<float>(patrol, "Patrol Blend Threshold: Left Shoulder", 0.5f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 42, IsAdvanced = true, Browsable = true }));
            PatrolBlendThresholdShortStock = config.Bind<float>(patrol, "Patrol Blend Threshold: Short-Stock", 0f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 41, IsAdvanced = true, Browsable = true }));
            PatrolTransitionFromActiveAim = config.Bind<float>(patrol, "Patrol Transition From: Active Aim", 1f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 40, IsAdvanced = true, Browsable = true }));
            PatrolTransitionFromHighReady = config.Bind<float>(patrol, "Patrol Transition From: High Ready", 1.1f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 39, IsAdvanced = true, Browsable = true }));
            PatrolTransitionFromLowReady = config.Bind<float>(patrol, "Patrol Transition From: Low Ready", 1.9f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 38, IsAdvanced = true, Browsable = true }));
            PatrolTransitionFromLeftShoulder = config.Bind<float>(patrol, "Patrol Transition From: Left Shoulder", 2f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 37, IsAdvanced = true, Browsable = true }));
            PatrolTransitionFromShortStock = config.Bind<float>(patrol, "Patrol Transition From: Short-Stock", 1f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 36, IsAdvanced = true, Browsable = true }));
            PatrolTransitionToActiveAim = config.Bind<float>(patrol, "Patrol Transition To Speed: Active Aim", 1f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 35, IsAdvanced = true, Browsable = true }));
            PatrolTransitionToHighReady = config.Bind<float>(patrol, "Patrol Transition To Speed: High Ready", 1.15f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 34, IsAdvanced = true, Browsable = true }));
            PatrolTransitionToLowReady = config.Bind<float>(patrol, "Patrol Transition To Speed: Low Ready", 0.65f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 33, IsAdvanced = true, Browsable = true }));
            PatrolTransitionToLeftShoulder = config.Bind<float>(patrol, "Patrol Transition To Speed: Left Shoulder", 0.85f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 32, IsAdvanced = true, Browsable = true }));
            PatrolTransitionToShortStock = config.Bind<float>(patrol, "Patrol Transition To Speed: Short-Stock", 1f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 31, IsAdvanced = true, Browsable = true }));

            LeftShoulderWalkSpeedBonus = config.Bind<float>(leftShoulder, "Left Shoulder Walk Speed Modifier", 0.88f, new ConfigDescription("Multiplier applied to walk speed while in Left Shoulder stance.", new AcceptableValueRange<float>(0.1f, 5f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 35, Browsable = true }));
            LeftShoulderSprintAccelBonus = config.Bind<float>(leftShoulder, "Left Shoulder Sprint Accel Modifier", 0.9f, new ConfigDescription("Multiplier applied to sprint acceleration while in Left Shoulder stance.", new AcceptableValueRange<float>(0.1f, 5f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 34, Browsable = true }));
            LeftShoulderStaminaRate = config.Bind<float>(leftShoulder, "Left Shoulder Stamina Rate", 0f, new ConfigDescription("Rate at which arm stamina changes while in Left Shoulder stance.", new AcceptableValueRange<float>(0f, 20f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 33, Browsable = true }));
            LeftShoulderTransitionFromIdle = config.Bind<float>(leftShoulder, "Left Shoulder Transition From: Idle", 2f, new ConfigDescription("Speed of entering Left Shoulder from idle (no previous stance).", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 32, Browsable = true }));
            LeftShoulderBlendThresholdActiveAim = config.Bind<float>(leftShoulder, "Left Shoulder Blend Threshold: Active Aim", 0f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 29, IsAdvanced = true, Browsable = true }));
            LeftShoulderBlendThresholdHighReady = config.Bind<float>(leftShoulder, "Left Shoulder Blend Threshold: High Ready", 0f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 28, IsAdvanced = true, Browsable = true }));
            LeftShoulderBlendThresholdLowReady = config.Bind<float>(leftShoulder, "Left Shoulder Blend Threshold: Low Ready", 0.55f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 27, IsAdvanced = true, Browsable = true }));
            LeftShoulderBlendThresholdPatrol = config.Bind<float>(leftShoulder, "Left Shoulder Blend Threshold: Patrol", 0f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 26, IsAdvanced = true, Browsable = true }));
            LeftShoulderBlendThresholdShortStock = config.Bind<float>(leftShoulder, "Left Shoulder Blend Threshold: Short-Stock", 0f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 25, IsAdvanced = true, Browsable = true }));
            LeftShoulderTransitionFromActiveAim = config.Bind<float>(leftShoulder, "Left Shoulder Transition From: Active Aim", 2f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 24, IsAdvanced = true, Browsable = true }));
            LeftShoulderTransitionFromHighReady = config.Bind<float>(leftShoulder, "Left Shoulder Transition From: High Ready", 1.3f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 23, IsAdvanced = true, Browsable = true }));
            LeftShoulderTransitionFromLowReady = config.Bind<float>(leftShoulder, "Left Shoulder Transition From: Low Ready", 3.1f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 22, IsAdvanced = true, Browsable = true }));
            LeftShoulderTransitionFromPatrol = config.Bind<float>(leftShoulder, "Left Shoulder Transition From: Patrol", 1.55f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 21, IsAdvanced = true, Browsable = true }));
            LeftShoulderTransitionFromShortStock = config.Bind<float>(leftShoulder, "Left Shoulder Transition From: Short-Stock", 1.5f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 20, IsAdvanced = true, Browsable = true }));
            LeftShoulderTransitionToActiveAim = config.Bind<float>(leftShoulder, "Left Shoulder Transition To Speed: Active Aim", 1f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 19, IsAdvanced = true, Browsable = true }));
            LeftShoulderTransitionToHighReady = config.Bind<float>(leftShoulder, "Left Shoulder Transition To Speed: High Ready", 1.15f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 18, IsAdvanced = true, Browsable = true }));
            LeftShoulderTransitionToLowReady = config.Bind<float>(leftShoulder, "Left Shoulder Transition To Speed: Low Ready", 1.45f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 17, IsAdvanced = true, Browsable = true }));
            LeftShoulderTransitionToPatrol = config.Bind<float>(leftShoulder, "Left Shoulder Transition To Speed: Patrol", 0.18f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 16, IsAdvanced = true, Browsable = true }));
            LeftShoulderTransitionToShortStock = config.Bind<float>(leftShoulder, "Left Shoulder Transition To Speed: Short-Stock", 0.9f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 50f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 15, IsAdvanced = true, Browsable = true }));
        }
    }
}
