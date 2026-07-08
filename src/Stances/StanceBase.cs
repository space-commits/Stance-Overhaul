using RealismCommonLib.Events;
using RealismCommonLib.Utils;
using StanceOverhaul.Controllers.StateControllers;
using StanceOverhaul.Enums;
using StanceOverhaul.Events;
using System;
using UnityEngine;
using static RealismCommonLib.Plugin;

namespace StanceOverhaul.Stances
{
    public abstract class StanceBase : IStance, IDisposable
    {
        public virtual EStanceType StanceType => EStanceType.None;
        public virtual EStaminaMode StaminaMode => EStaminaMode.Neutral;
        public virtual float StaminaRate => 0f;
        public virtual float WalkSpeedBonus => 1f;
        public virtual float SprintAccelBonus => 1f;
        public virtual float HipfireBonus => 1f;
        public virtual bool CanDoTacSprint => false;

        public abstract Vector3Curve EnterPositionCurve { get; }
        public abstract Vector3Curve EnterRotationCurve { get; }
        public abstract Vector3Curve ExitPositionCurve { get; }
        public abstract Vector3Curve ExitRotationCurve { get; }

        public virtual AnimationCurve ExitAimSpeedCurve { get; }
            = new AnimationCurve
            (
                new Keyframe { time = 0f, value = 0f },
                new Keyframe { time = 0.25f, value = 0f },
                new Keyframe { time = 0.5f, value = 0.1f },
                new Keyframe { time = 0.7f, value = 0.25f },
                new Keyframe { time = 0.85f, value = 0.5f },
                new Keyframe { time = 0.95f, value = 0.8f },
                new Keyframe { time = 1f, value = 1f }
            );

        public virtual AnimationCurve EnterAimSpeedCurve { get; }
            = new AnimationCurve
            (
                new Keyframe { time = 0f, value = 0f },
                new Keyframe { time = 0.25f, value = 0f },
                new Keyframe { time = 0.5f, value = 0.1f },
                new Keyframe { time = 0.7f, value = 0.25f },
                new Keyframe { time = 0.85f, value = 0.5f },
                new Keyframe { time = 0.95f, value = 0.8f },
                new Keyframe { time = 1f, value = 1f }
            );

        /// <summary>
        /// Blend threshold for this stance when transitioning to another stance.
        /// Value of 0 means it will immediately let the next stance's animation take over, 
        /// while a value of 1 means it will fully play this stance's animation before transitioning.
        /// </summary>
        public virtual float BlendIntoThreshold(EStanceType nextStance) { return 0.5f; }

        ///<summary>
        ///Speed of this stance when transitioning from another stance.
        ///</summary>
        public virtual float TransitionFromModifier(EStanceType? previousStance) { return 1f; }

        ///<summary>
        ///Speed of this stance when transitioning to another stance. Should be 1 if there is no next stance.
        ///</summary>
        public virtual float TransitionToSpeedModifier(EStanceType? nextStance) { return 1f; }

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

        public virtual void OnEnter()
        {
            StanceEvents.RaiseOnStanceEntered(this);
        }

        public virtual void OnExit()
        {
            StanceEvents.RaiseOnStanceExited();
        }

        public virtual void OnHoldUpdate(float deltaTime) { } //TODO: implement
    }
}
