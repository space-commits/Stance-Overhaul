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


        public static ConfigEntry<KeyboardShortcut> ActiveAimKeybind { get; set; }
        public static ConfigEntry<KeyboardShortcut> LowReadyKeybind { get; set; }
        public static ConfigEntry<KeyboardShortcut> HighReadyKeybind { get; set; }
        public static ConfigEntry<KeyboardShortcut> ShortStockKeybind { get; set; }
        public static ConfigEntry<KeyboardShortcut> PatrolKeybind { get; set; }
        public static ConfigEntry<KeyboardShortcut> MeleeKeybind { get; set; }
        public static ConfigEntry<KeyboardShortcut> StanceWheelComboKeyBind { get; set; }
        public static ConfigEntry<float> StanceSfxModifier { get; set; }
        public static ConfigEntry<bool> EnableAnimationFixes { get; set; }
        public static ConfigEntry<bool> OverrideCollision { get; set; }
        public static ConfigEntry<bool> OverrideMounting { get; set; }
        public static ConfigEntry<bool> UseMouseWheelStance { get; set; }
        public static ConfigEntry<bool> UseMouseWheelPlusKey { get; set; }

        public static ConfigEntry<bool> EnableFSAimBlock { get; set; }
        public static ConfigEntry<bool> EnableNVGAimBlock { get; set; }
        public static ConfigEntry<bool> EnableThermalAimBlock { get; set; }

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

        public static ConfigEntry<float> LeftShoulderOffset { get; set; }
        public static ConfigEntry<float> StanceRotationSpeedMulti { get; set; }
        public static ConfigEntry<float> StanceTransitionSpeedMulti { get; set; }
        public static ConfigEntry<float> ThirdPersonPositionSpeed { get; set; }
        public static ConfigEntry<float> ThirdPersonRotationSpeed { get; set; }

        public static ConfigEntry<float> ActiveAimPosSpeedMulti { get; set; }
        public static ConfigEntry<float> ActiveAimResetSpeedMulti { get; set; }
        public static ConfigEntry<float> ActiveAimRotationSpeedMulti { get; set; }
        public static ConfigEntry<float> PistolRotationSpeedMulti { get; set; }
        public static ConfigEntry<float> HighReadyRotationMulti { get; set; }
        public static ConfigEntry<float> LowReadyRotationMulti { get; set; }
        public static ConfigEntry<float> ShortStockRotationMulti { get; set; }
        public static ConfigEntry<float> ShortStockAdditionalRotationSpeedMulti { get; set; }
        public static ConfigEntry<float> ActiveAimAdditionalRotationSpeedMulti { get; set; }
        public static ConfigEntry<float> HighReadyAdditionalRotationSpeedMulti { get; set; }
        public static ConfigEntry<float> LowReadyAdditionalRotationSpeedMulti { get; set; }
        public static ConfigEntry<float> PistolAdditionalRotationSpeedMulti { get; set; }
        public static ConfigEntry<float> ActiveAimResetRotationSpeedMulti { get; set; }
        public static ConfigEntry<float> PistolResetRotationSpeedMulti { get; set; }
        public static ConfigEntry<float> HighReadyResetRotationMulti { get; set; }
        public static ConfigEntry<float> LowReadyResetRotationMulti { get; set; }
        public static ConfigEntry<float> ShortStockResetRotationSpeedMulti { get; set; }
        public static ConfigEntry<float> HighReadySpeedMulti { get; set; }
        public static ConfigEntry<float> HighReadyResetSpeedMulti { get; set; }
        public static ConfigEntry<float> LowReadySpeedMulti { get; set; }
        public static ConfigEntry<float> LowReadyResetSpeedMulti { get; set; }
        public static ConfigEntry<float> PistolPosSpeedMulti { get; set; }
        public static ConfigEntry<float> PistolPosResetSpeedMulti { get; set; }
        public static ConfigEntry<float> ShortStockSpeedMulti { get; set; }
        public static ConfigEntry<float> ShortStockResetSpeedMulti { get; set; }

        public static ConfigEntry<Vector3> WeapOffset { get; set; }
        public static ConfigEntry<Vector3> ActiveAimRotation { get; set; }
        public static ConfigEntry<Vector3> PistolRotation { get; set; }
        public static ConfigEntry<Vector3> ActiveThirdPersonPosition { get; set; }
        public static ConfigEntry<Vector3> ActiveThirdPersonRotation { get; set; }
        public static ConfigEntry<Vector3> ActiveAimAdditionalRotation { get; set; }
        public static ConfigEntry<Vector3> ActiveAimResetRotation { get; set; }
        public static ConfigEntry<Vector3> HighReadyThirdPersonPosition { get; set; }
        public static ConfigEntry<Vector3> HighReadyThirdPersonRotation { get; set; }
        public static ConfigEntry<Vector3> HighReadyAdditionalRotation { get; set; }
        public static ConfigEntry<Vector3> HighReadyResetRotation { get; set; }
        public static ConfigEntry<Vector3> LowReadyThirdPersonPosition { get; set; }
        public static ConfigEntry<Vector3> LowReadyThirdPersonRotation { get; set; }
        public static ConfigEntry<Vector3> LowReadyAdditionalRotation { get; set; }
        public static ConfigEntry<Vector3> LowReadyResetRotation { get; set; }
        public static ConfigEntry<Vector3> PistolAdditionalRotation { get; set; }
        public static ConfigEntry<Vector3> PistolResetRotation { get; set; }
        public static ConfigEntry<Vector3> ShortStockAdditionalRotation { get; set; }
        public static ConfigEntry<Vector3> ShortStockResetRotation { get; set; }
        public static ConfigEntry<Vector3> PistolThirdPersonPosition { get; set; }
        public static ConfigEntry<Vector3> PistolThirdPersonRotation { get; set; }
        public static ConfigEntry<Vector3> PistolOffset { get; set; }
        public static ConfigEntry<Vector3> ActiveAimOffset { get; set; }
        public static ConfigEntry<Vector3> LowReadyOffset { get; set; }
        public static ConfigEntry<Vector3> LowReadyRotation { get; set; }
        public static ConfigEntry<Vector3> HighReadyOffset { get; set; }
        public static ConfigEntry<Vector3> HighReadyRotation { get; set; }
        public static ConfigEntry<Vector3> ShortStockThirdPersonPosition { get; set; }
        public static ConfigEntry<Vector3> ShortStockThirdPersonRotation { get; set; }
        public static ConfigEntry<Vector3> ShortStockOffset { get; set; }
        public static ConfigEntry<Vector3> ShortStockRotation { get; set; }
        public static ConfigEntry<Vector3> ShortStockReadyOffset { get; set; }
        public static ConfigEntry<Vector3> ShortStockReadyRotation { get; set; }

        public static void InitConfigBindings(ConfigFile config)
        {
            string dev = "0. Dev.";
            string weapAimAndPos = "1. Weapon Stances And Position.";
            string stanceBinds = "2. Weapon Stances Keybinds.";
            string activeAim = "3. Active Aim.";
            string highReady = "4. High Ready.";
            string lowReady = "5. Low Ready.";
            string pistol = "6. Pistol Position And Stance.";
            string shortStock = "7. Short-Stocking.";
            string thirdPerson = "8. Third Person Animations.";

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
            IdleStamDrainModi = config.Bind<float>(weapAimAndPos, "Idle Stam Drain Modifer", 1f, new ConfigDescription("", new AcceptableValueRange<float>(0.1f, 5f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 200, Browsable =true }));
            EnableStanceStamChanges = config.Bind<bool>(weapAimAndPos, "Enable Stance Stamina And Movement Effects",true, new ConfigDescription("Enabled Stances And Mounting To Affect Stamina And Movement Speed. Stamina Drain May Not Work Correctly If Disabled. High + Low Ready, Short-Stocking And Pistol Idle Will Regenerate Stamina Faster And Optionally Idle With Rifles Drains Stamina. High Ready Has Faster Sprint Speed And Sprint Accel, Low Ready Has Faster Sprint Accel. Arm Stamina Won't Drain Regular Stamina If It Reaches 0.", null, new ConfigurationManagerAttributes { Order = 183, Browsable =true }));
            AllowActiveAimReload = config.Bind<bool>(weapAimAndPos, "Allow Reload From Active Aim", false, new ConfigDescription("Allows Reload From Magazine While In Active Aim With Speed Bonus.", null, new ConfigurationManagerAttributes { Order = 190, Browsable =true }));
            EnableMountUI = config.Bind<bool>(weapAimAndPos, "Enable Mounting UI",true, new ConfigDescription("If Enabled, An Icon On Screen Will Indicate If Player Is Bracing, Mounting And What Side Of Cover They Are On.", null, new ConfigurationManagerAttributes { Order = 179, Browsable =true }));
            LeftShoulderOffset = config.Bind<float>(weapAimAndPos, "Left Shoulder Offset", -0.13f, new ConfigDescription("", new AcceptableValueRange<float>(-0.2f, 0.1f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 153, Browsable =true }));
            WeapOffset = config.Bind<Vector3>(weapAimAndPos, "Rifle Position Offset", new Vector3(-0.04f, -0.015f, 0f), new ConfigDescription("Config option 'alt rife' is required. Adjusts The Starting Position Of Rifle On Screen.", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 152, IsAdvanced = true, Browsable =true }));
            StanceRotationSpeedMulti = config.Bind<float>(weapAimAndPos, "Stance Rotation Speed Multi", 1f, new ConfigDescription("Adjusts The Speed Of Stance Rotation Changes.", new AcceptableValueRange<float>(0.1f, 10f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 146, IsAdvanced = true, Browsable =true }));
            StanceTransitionSpeedMulti = config.Bind<float>(weapAimAndPos, "Stance Transition Speed.", 15.0f, new ConfigDescription("Adjusts The Position Change Speed Between Stances", new AcceptableValueRange<float>(1f, 35f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 145, IsAdvanced = true, Browsable =true }));
            StanceSfxModifier = config.Bind<float>(weapAimAndPos, "Stance Sfx Volume Modifier", 2f, new ConfigDescription("Gear rattle volume modifer when doing stance related things", new AcceptableValueRange<float>(0.1f, 20f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 153, Browsable =true }));

            EnableFSAimBlock = config.Bind<bool>(weapAimAndPos, "Enable Faceshield Aim Block", true, new ConfigDescription("Faceshields Block ADS Unless The Specfic Stock/Weapon/Faceshield Allows It.", null, new ConfigurationManagerAttributes { Order = 4, Browsable = true }));
            EnableNVGAimBlock = config.Bind<bool>(weapAimAndPos, "Enable NVG Aim Block", true, new ConfigDescription("Magnified Optics Block ADS When Using NVGs.", null, new ConfigurationManagerAttributes { Order = 5, Browsable = true }));
            EnableThermalAimBlock = config.Bind<bool>(weapAimAndPos, "Enable Thermal Aim Block", true, new ConfigDescription("Can't Aim With Sights When Using Thermal Goggles.", null, new ConfigurationManagerAttributes { Order = 5, Browsable = true }));

            ActiveAimKeybind = config.Bind(stanceBinds, "Active Aim Keybind", new KeyboardShortcut(KeyCode.Mouse4), new ConfigDescription("Cants The Weapon Sideways, Improving Hipfire Accuracy.", null, new ConfigurationManagerAttributes { Order = 90, Browsable =true }));
            ToggleActiveAim = config.Bind<bool>(stanceBinds, "Use Toggle For Active Aim", false, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 100, Browsable =true }));
            HighReadyKeybind = config.Bind(stanceBinds, "High Ready Keybind", new KeyboardShortcut(KeyCode.Mouse3, new[] { KeyCode.LeftAlt }), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 110, Browsable =true }));
            LowReadyKeybind = config.Bind(stanceBinds, "Low Ready Keybind", new KeyboardShortcut(KeyCode.Mouse3, new[] { KeyCode.LeftControl }), new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 120, Browsable =true }));
            ShortStockKeybind = config.Bind(stanceBinds, "Short-Stock Keybind", new KeyboardShortcut(KeyCode.J), new ConfigDescription("Tucks The Weapon's Stock Under Player's Arm, Shortening The Overall Length Of The Wweapon To Prevent Muzzle Being Pushed Away From Target.", null, new ConfigurationManagerAttributes { Order = 130, Browsable =true }));
            PatrolKeybind = config.Bind(stanceBinds, "Patrol/Neutral Stance Keybind", new KeyboardShortcut(KeyCode.K), new ConfigDescription("Puts The Weapon In A Neutral Position, Improving Arm Stam Regen And Walk Speed. For Maximum Larping.", null, new ConfigurationManagerAttributes { Order = 155, Browsable =true }));
            MeleeKeybind = config.Bind(stanceBinds, "Melee Keybind", new KeyboardShortcut(KeyCode.None), new ConfigDescription("Strike With Muzzle Or Bayonet Of Equipped Weapon.", null, new ConfigurationManagerAttributes { Order = 150, Browsable =true }));
            UseMouseWheelStance = config.Bind<bool>(stanceBinds, "Enable Mouse Wheel Stance Switching",true, new ConfigDescription("Switches Between High And Low Ready Via Mouse Wheel.", null, new ConfigurationManagerAttributes { Order = 160, Browsable =true }));
            UseMouseWheelPlusKey = config.Bind<bool>(stanceBinds, "Require Key + Mouse Wheel",true, new ConfigDescription("Require Keybind + Mouse Wheel To Change Stance.", null, new ConfigurationManagerAttributes { Order = 170, Browsable =true }));
            StanceWheelComboKeyBind = config.Bind(stanceBinds, "Keybind To Use With Mouse Wheel", new KeyboardShortcut(KeyCode.LeftControl), new ConfigDescription("Key Used In Combination With Mouse Wheel If Enabled ", null, new ConfigurationManagerAttributes { Order = 180, Browsable =true }));

            ThirdPersonRotationSpeed = config.Bind<float>(thirdPerson, "Third Person Rotation Speed Multi", 1.5f, new ConfigDescription("Speed Of Stance Rotation Change In Third Person.", new AcceptableValueRange<float>(0.1f, 20f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 1000, IsAdvanced = true, Browsable =true }));
            ThirdPersonPositionSpeed = config.Bind<float>(thirdPerson, "Third Person Position Speed Multi", 1.0f, new ConfigDescription("Speed Of Stance Position Change In Third Person.", new AcceptableValueRange<float>(0.1f, 20f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 1100, IsAdvanced = true, Browsable =true }));

            PistolThirdPersonPosition = config.Bind<Vector3>(thirdPerson, "Pistol Third Person Position", new Vector3(-0.03f, 0.04f, -0.05f), new ConfigDescription("", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 260, IsAdvanced = true, Browsable =true }));
            PistolThirdPersonRotation = config.Bind<Vector3>(thirdPerson, "Pistol Third Person Rotation", new Vector3(0f, 15f, 0f), new ConfigDescription("", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 230, IsAdvanced = true, Browsable =true }));

            ShortStockThirdPersonPosition = config.Bind<Vector3>(thirdPerson, "Short-Stock Third Person Position", new Vector3(0.03f, 0.065f, -0.075f), new ConfigDescription("", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 200, IsAdvanced = true, Browsable =true }));
            ShortStockThirdPersonRotation = config.Bind<Vector3>(thirdPerson, "Short-Stock Third Person Rotation", new Vector3(0f, -15f, 0f), new ConfigDescription("", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 170, IsAdvanced = true, Browsable =true }));

            ActiveThirdPersonPosition = config.Bind<Vector3>(thirdPerson, "Active Aim Third Person Position", new Vector3(-0.02f, -0.02f, 0.02f), new ConfigDescription("", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 140, IsAdvanced = true, Browsable =true }));
            ActiveThirdPersonRotation = config.Bind<Vector3>(thirdPerson, "Active Aim Third Person Rotation", new Vector3(0f, -35f, 0f), new ConfigDescription("", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 110, IsAdvanced = true, Browsable =true }));

            HighReadyThirdPersonPosition = config.Bind<Vector3>(thirdPerson, "High Ready Third Person Position", new Vector3(0.02f, 0.05f, -0.045f), new ConfigDescription("", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 80, IsAdvanced = true, Browsable =true }));
            HighReadyThirdPersonRotation = config.Bind<Vector3>(thirdPerson, "High Ready Third Person Rotation", new Vector3(-8f, -25f, 0f), new ConfigDescription("", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 50, IsAdvanced = true, Browsable =true }));

            LowReadyThirdPersonPosition = config.Bind<Vector3>(thirdPerson, "Low Ready Third Person Position", new Vector3(0.01f, -0.025f, 0f), new ConfigDescription("", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 20, IsAdvanced = true, Browsable =true }));
            LowReadyThirdPersonRotation = config.Bind<Vector3>(thirdPerson, "Low Ready Third Person Rotation", new Vector3(24f, 10f, -1f), new ConfigDescription("", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 8, IsAdvanced = true, Browsable =true }));

            ActiveAimAdditionalRotationSpeedMulti = config.Bind<float>(activeAim, "Active Aim Additonal Rotation Speed Multi.", 2f, new ConfigDescription("", new AcceptableValueRange<float>(0.0f, 10f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 145, IsAdvanced = true, Browsable =true }));
            ActiveAimResetRotationSpeedMulti = config.Bind<float>(activeAim, "Active Aim Reset Rotation Speed Multi.", 3.5f, new ConfigDescription("", new AcceptableValueRange<float>(0.0f, 10f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 145, IsAdvanced = true, Browsable =true }));
            ActiveAimRotationSpeedMulti = config.Bind<float>(activeAim, "Active Aim Rotation Speed Multi.", 2f, new ConfigDescription("", new AcceptableValueRange<float>(0.0f, 10f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 144, IsAdvanced = true, Browsable =true }));
            ActiveAimPosSpeedMulti = config.Bind<float>(activeAim, "Active Aim Speed Multi", 15f, new ConfigDescription("", new AcceptableValueRange<float>(1f, 100f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 143, IsAdvanced = true, Browsable =true }));
            ActiveAimResetSpeedMulti = config.Bind<float>(activeAim, "Active Aim Reset Speed Multi", 6f, new ConfigDescription("", new AcceptableValueRange<float>(1f, 100f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 142, IsAdvanced = true, Browsable =true }));

            ActiveAimOffset = config.Bind<Vector3>(activeAim, "Active Aim Position", new Vector3(-0.02f, 0.008f, 0f), new ConfigDescription("Weapon Position When In Stance.", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 135, IsAdvanced = true, Browsable =true }));
            ActiveAimRotation = config.Bind<Vector3>(activeAim, "Active Aim Rotation", new Vector3(0.0f, -35f, 0f), new ConfigDescription("Weapon Rotation When In Stance.", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 122, IsAdvanced = true, Browsable =true }));
            ActiveAimAdditionalRotation = config.Bind<Vector3>(activeAim, "Active Aiming Additional Rotation", new Vector3(0f, -35f, 0f), new ConfigDescription("Additional Seperate Weapon Rotation When Going Into Stance.", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 111, IsAdvanced = true, Browsable =true }));
            ActiveAimResetRotation = config.Bind<Vector3>(activeAim, "Active Aiming Reset Rotation", new Vector3(-0.5f, 20.5f, -2f), new ConfigDescription("Weapon Rotation When Going Out Of Stance.", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 102, IsAdvanced = true, Browsable =true }));

            HighReadyAdditionalRotationSpeedMulti = config.Bind<float>(highReady, "High Ready Additonal Rotation Speed Multi.", 0.1f, new ConfigDescription("How Fast The Weapon Rotates Going Out Of Stance.", new AcceptableValueRange<float>(0f, 100f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 94, IsAdvanced = true, Browsable =true }));
            HighReadyResetRotationMulti = config.Bind<float>(highReady, "High Ready Reset Rotation Speed Multi.", 1.5f, new ConfigDescription("How Fast The Weapon Rotates Going Out Of Stance.", new AcceptableValueRange<float>(0f, 100f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 93, IsAdvanced = true, Browsable =true }));
            HighReadyRotationMulti = config.Bind<float>(highReady, "High Ready Rotation Speed Multi.", 2f, new ConfigDescription("How Fast The Weapon Rotates Going Into Stance.", new AcceptableValueRange<float>(1f, 100f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 92, IsAdvanced = true, Browsable =true }));
            HighReadyResetSpeedMulti = config.Bind<float>(highReady, "High Ready Reset Speed Multi", 6.5f, new ConfigDescription("How Fast The Weapon Moves Going Out Of Stance", new AcceptableValueRange<float>(0f, 100f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 91, IsAdvanced = true, Browsable =true }));
            HighReadySpeedMulti = config.Bind<float>(highReady, "High Ready Speed Multi", 6f, new ConfigDescription("How Fast The Weapon Moves Going Into Stance", new AcceptableValueRange<float>(0f, 100f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 90, IsAdvanced = true, Browsable =true }));

            HighReadyOffset = config.Bind<Vector3>(highReady, "High Ready Position", new Vector3(0.005f, 0.035f, -0.04f), new ConfigDescription("Weapon Position When In Stance.", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 85, IsAdvanced = true, Browsable =true }));
            HighReadyRotation = config.Bind<Vector3>(highReady, "High Ready Rotation", new Vector3(-8.0f, -20f, 0f), new ConfigDescription("Weapon Rotation When In Stance.", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 72, IsAdvanced = true, Browsable =true }));
            HighReadyAdditionalRotation = config.Bind<Vector3>(highReady, "High Ready Additional Rotation", new Vector3(-50.0f, -25f, -5f), new ConfigDescription("Additional Seperate Weapon Rotation When Going Into Stance.", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 69, IsAdvanced = true, Browsable =true }));
            HighReadyResetRotation = config.Bind<Vector3>(highReady, "High Ready Reset Rotation", new Vector3(0f, 2f, 0f), new ConfigDescription("Weapon Rotation When Going Out Of Stance.", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 66, IsAdvanced = true, Browsable =true }));

            LowReadyAdditionalRotationSpeedMulti = config.Bind<float>(lowReady, "Low Ready Additonal Rotation Speed Multi", 0.75f, new ConfigDescription("How Fast The Weapon Rotates.", new AcceptableValueRange<float>(0f, 100f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 64, IsAdvanced = true, Browsable =true }));
            LowReadyResetRotationMulti = config.Bind<float>(lowReady, "Low Ready Reset Rotation Speed Multi", 2.25f, new ConfigDescription("How Fast The Weapon Rotates.", new AcceptableValueRange<float>(0f, 100f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 63, IsAdvanced = true, Browsable =true }));
            LowReadyRotationMulti = config.Bind<float>(lowReady, "Low Ready Rotation Speed Multi", 1.5f, new ConfigDescription("How Fast The Weapon Rotates.", new AcceptableValueRange<float>(0f, 100f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 62, IsAdvanced = true, Browsable =true }));
            LowReadySpeedMulti = config.Bind<float>(lowReady, "Low Ready Speed Multi.", 14f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 100.0f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 61, IsAdvanced = true, Browsable =true }));
            LowReadyResetSpeedMulti = config.Bind<float>(lowReady, "Low Ready Reset Speed Multi", 8.7f, new ConfigDescription("", new AcceptableValueRange<float>(0f, 100.0f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 60, IsAdvanced = true, Browsable =true }));

            LowReadyOffset = config.Bind<Vector3>(lowReady, "Low Ready Position", new Vector3(0f, -0.01f, 0f), new ConfigDescription("Weapon Position When In Stance.", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 55, IsAdvanced = true, Browsable =true }));
            LowReadyRotation = config.Bind<Vector3>(lowReady, "Low Ready Rotation", new Vector3(8f, -5f, -1f), new ConfigDescription("Weapon Rotation When In Stance.", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 42, IsAdvanced = true, Browsable =true }));
            LowReadyAdditionalRotation = config.Bind<Vector3>(lowReady, "Low Ready Additional Rotation", new Vector3(12.0f, -1f, 0f), new ConfigDescription("Additional Seperate Weapon Rotation When Going Into Stance.", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 39, IsAdvanced = true, Browsable =true }));
            LowReadyResetRotation = config.Bind<Vector3>(lowReady, "Low Ready Reset Rotation", new Vector3(-1.0f, 0f, 0f), new ConfigDescription("Weapon Rotation When Going Out Of Stance.", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 36, IsAdvanced = true, Browsable =true }));

            PistolAdditionalRotationSpeedMulti = config.Bind<float>(pistol, "Pistol Additional Rotation Speed Multi", 0.1f, new ConfigDescription("How Fast The Weapon Rotates.", new AcceptableValueRange<float>(0.0f, 20f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 35, IsAdvanced = true, Browsable =true }));
            PistolResetRotationSpeedMulti = config.Bind<float>(pistol, "Pistol Reset Rotation Speed Multi", 0.5f, new ConfigDescription("How Fast The Weapon Rotates.", new AcceptableValueRange<float>(0.0f, 20f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 34, IsAdvanced = true, Browsable =true }));
            PistolRotationSpeedMulti = config.Bind<float>(pistol, "Pistol Rotation Speed Multi", 1f, new ConfigDescription("How Fast The Weapon Rotates.", new AcceptableValueRange<float>(0.0f, 20f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 33, IsAdvanced = true, Browsable =true }));
            PistolPosSpeedMulti = config.Bind<float>(pistol, "Pistol Position Speed Multi", 6.0f, new ConfigDescription("", new AcceptableValueRange<float>(1.0f, 100.0f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 32, IsAdvanced = true, Browsable =true }));
            PistolPosResetSpeedMulti = config.Bind<float>(pistol, "Pistol Position Reset Speed Multi", 8.0f, new ConfigDescription("", new AcceptableValueRange<float>(1.0f, 100.0f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 30, IsAdvanced = true, Browsable =true }));

            PistolOffset = config.Bind<Vector3>(pistol, "Pistol Position", new Vector3(0f, 0.04f, -0.015f), new ConfigDescription("Weapon Position When In Stance.", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 25, IsAdvanced = true, Browsable =true }));
            PistolRotation = config.Bind<Vector3>(pistol, "Pistol Rotation", new Vector3(0.0f, -5f, 0f), new ConfigDescription("Weapon Rotation When In Stance.", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 12, IsAdvanced = true, Browsable =true }));
            PistolAdditionalRotation = config.Bind<Vector3>(pistol, "Pistol Ready Additional Rotation", new Vector3(0.0f, 0f, 0f), new ConfigDescription("Additional Seperate Weapon Rotation When Going Into Stance.", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 6, IsAdvanced = true, Browsable =true }));
            PistolResetRotation = config.Bind<Vector3>(pistol, "Pistol Ready Reset Rotation", new Vector3(-5f, 0f, 0f), new ConfigDescription("Weapon Rotation When Going Out Of Stance.", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 3, IsAdvanced = true, Browsable =true }));

            ShortStockAdditionalRotationSpeedMulti = config.Bind<float>(shortStock, "Short-Stock Additional Rotation Speed Multi", 1.5f, new ConfigDescription("How Fast The Weapon Rotates.", new AcceptableValueRange<float>(0.1f, 5f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 35, IsAdvanced = true, Browsable =true }));
            ShortStockResetRotationSpeedMulti = config.Bind<float>(shortStock, "Short-Stock Reset Rotation Speed Multi", 1.0f, new ConfigDescription("How Fast The Weapon Rotates.", new AcceptableValueRange<float>(0.1f, 5f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 34, IsAdvanced = true, Browsable =true }));
            ShortStockRotationMulti = config.Bind<float>(shortStock, "Short-Stock Rotation Speed Multi", 2.0f, new ConfigDescription("How Fast The Weapon Rotates.", new AcceptableValueRange<float>(0.1f, 5f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 33, IsAdvanced = true, Browsable =true }));
            ShortStockSpeedMulti = config.Bind<float>(shortStock, "Short-Stock Position Speed Multi.", 4f, new ConfigDescription("", new AcceptableValueRange<float>(1f, 100.0f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 32, IsAdvanced = true, Browsable =true }));
            ShortStockResetSpeedMulti = config.Bind<float>(shortStock, "Short-Stock Position Reset Speed Mult", 3.8f, new ConfigDescription("", new AcceptableValueRange<float>(1f, 100.0f), new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 30, IsAdvanced = true, Browsable =true }));

            ShortStockOffset = config.Bind<Vector3>(shortStock, "Short-Stock Position", new Vector3(0.02f, 0.1f, -0.025f), new ConfigDescription("Weapon Position When In Stance.", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 25, IsAdvanced = true, Browsable =true }));
            ShortStockRotation = config.Bind<Vector3>(shortStock, "Short-Stock Rotation", new Vector3(0f, -15f, 0f), new ConfigDescription("Weapon Rotation When In Stance.", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 12, IsAdvanced = true, Browsable =true }));
            ShortStockAdditionalRotation = config.Bind<Vector3>(shortStock, "Short-Stock Ready Additional Rotation", new Vector3(-3.0f, -15f, 1f), new ConfigDescription("Additional Seperate Weapon Rotation When Going Into Stance.", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 6, IsAdvanced = true, Browsable =true }));
            ShortStockResetRotation = config.Bind<Vector3>(shortStock, "Short-Stock Ready Reset Rotation", new Vector3(-1.5f, 2f, 0f), new ConfigDescription("Weapon Rotation When Going Out Of Stance.", null, new ConfigurationManagerAttributes { ShowRangeAsPercent = false, Order = 3, IsAdvanced = true, Browsable =true }));
        }
    }
}
