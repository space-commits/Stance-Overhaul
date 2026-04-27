using RealismCommonLib.Events;
using RealismCommonLib.Utils;
using StanceOverhaul.Controllers.StateControllers;
using StanceOverhaul.Enums;
using System;
using UnityEngine;
using static RealismCommonLib.Plugin;

namespace StanceOverhaul.Stances
{
    public abstract class StanceBase : IStance, IDisposable
    {
        public virtual EStance StanceType => EStance.None;

        public abstract Vector3Curve EnterPositionCurve { get; }
        public abstract Vector3Curve EnterRotationCurve { get; }

        public abstract Vector3Curve ExitPositionCurve { get; }
        public abstract Vector3Curve ExitRotationCurve { get; }

        public virtual float BlendThreshold => 0.15f; //TODO should depend on incoming stance type
        public virtual float BaseSpeed => 2.5f; //TODO expose to config

        public StanceBase()
        {
            ReloadEvents.WeaponStateReset += OnWeaponStateReset;
            ReloadEvents.CheckAmmo += OnCheckAmmo;
            ReloadEvents.ChamberCheck += OnCheckChamber;
            ReloadEvents.Rechamber += OnRechamber;
            ReloadEvents.MagReload += OnMagReload;
            ReloadEvents.QuickMagReload += OnQuickMagReload;
            ReloadEvents.InternalMagReload += OnInternalMagReload;
            ReloadEvents.MalfFix += OnMalfFix;
        }

        public void Dispose()
        {
            ReloadEvents.WeaponStateReset -= OnWeaponStateReset;
            ReloadEvents.CheckAmmo -= OnCheckAmmo;
            ReloadEvents.ChamberCheck -= OnCheckChamber;
            ReloadEvents.Rechamber -= OnRechamber;
            ReloadEvents.MagReload -= OnMagReload;
            ReloadEvents.QuickMagReload -= OnQuickMagReload;
            ReloadEvents.InternalMagReload -= OnInternalMagReload;
            ReloadEvents.MalfFix -= OnMalfFix;
        }

        protected virtual void OnWeaponStateReset() { }

        protected virtual void OnMalfFix() { }

        protected virtual void OnRechamber() { }

        protected virtual void OnCheckChamber() { }

        protected virtual void OnCheckAmmo() { }

        protected virtual void OnInternalMagReload() { }

        protected virtual void OnMagReload() { }

        protected virtual void OnQuickMagReload() { }

        public virtual void OnEnter() {}

        public virtual void OnExit() {}

        public virtual void OnHoldUpdate(float deltaTime) {} //TODO: implement
    }
}
