using BepInEx;
using BepInEx.Bootstrap;
using Comfort.Common;
using EFT;
using RealismCommonLib.Controllers;
using RealismCommonLib.StateControllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace StanceOverhaul
{
    [BepInPlugin("com.fontaine.stanceoverhaul", "Fontaine-StanceOverhaul", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {

        public static StanceController StanceControllerInstance;

        void Awake()
        {
        }

        void Update()
        {
            
        }
    }
}



