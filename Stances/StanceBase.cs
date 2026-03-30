using RealismCommonLib.Events;
using RealismCommonLib.Utils;
using StanceOverhaul.Enums;
using System;
using UnityEngine;
using static RealismCommonLib.Plugin;

namespace StanceOverhaul.Stances
{
    public abstract class StanceBase : IStance, IDisposable
    {
        public EStanceState State => _state;
        public virtual EStance StanceType => EStance.None;

        public bool IsActive => _state == EStanceState.Active || _state == EStanceState.Entering;

        public Vector3 StancePosition { get; protected set; } = Vector3.zero;
        public Vector3 StanceRotation { get; protected set; } = Vector3.zero;

        protected bool _canExit = false;
        protected bool _exitRequested = false;

        protected EStanceState _state = EStanceState.Inactive;

        public event Action<IStance> OnEnterStarted;
        public event Action<IStance> OnEnterCompleted;
        public event Action<IStance> OnExitStarted;
        public event Action<IStance> OnExitCompleted;

        private const float MANIP_TIMER = 0.25f;
        public bool PauseStance { get; protected set; } = false;
        public DelayTimer ManipTimer { get; } = new DelayTimer(MANIP_TIMER);

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

        protected virtual void OnWeaponStateReset() 
        {
            PauseStance = false;
            ManipTimer.Start();
        }

        protected virtual void OnMalfFix() { }

        protected virtual void OnRechamber() {}

        protected virtual void OnCheckChamber() {}

        protected virtual void OnCheckAmmo() {}

        protected virtual void OnInternalMagReload() {}

        protected virtual void OnMagReload() {}

        protected virtual void OnQuickMagReload() {}

        public virtual void Enter()
        {
            if (_state != EStanceState.Inactive)
                return;

            ModLogger.LogWarning("stance Enter");

            _state = EStanceState.Entering; 
            OnEnterStarted?.Invoke(this);
        }

        public virtual void TryExit(bool force = false)
        {
            ModLogger.LogWarning("stance Exit");

            if (_state == EStanceState.Inactive || _state == EStanceState.Exiting)
                return;

            if (force)
                _canExit = true;

            _exitRequested = true;
        }

        protected void BeginExit()
        {      
            ModLogger.LogWarning("begin exit");
            if (_state == EStanceState.Exiting)
                return;

            ModLogger.LogWarning("set state to exiting");
            _state = EStanceState.Exiting;
            OnExitStarted?.Invoke(this);
        }

        public void StanceUpdate(float dt) 
        {
            UpdateManipTimer();

            if (_exitRequested && _canExit)
            {
                _exitRequested = false;
                _canExit = false;
                BeginExit();
            }

            switch (_state)
            {
                case EStanceState.Entering:
                    if (UpdateEnter(dt))
                    {
                        OnEnter();
                    }
                    break;
                case EStanceState.Active:
                    UpdateActive(dt);
                    break;
                case EStanceState.Exiting:
                    if (UpdateExit(dt))
                    {
                        OnExit();
                    }
                    break;
            }
        }

        private void OnEnter()
        {
            ModLogger.LogWarning("OnEnter");
            _state = EStanceState.Active;
            OnEnterCompleted?.Invoke(this);
        }

        private void OnExit()
        {
            ModLogger.LogWarning("OnExit");
            _state = EStanceState.Inactive;
            PauseStance = false;
            ManipTimer.Stop();
            OnExitCompleted?.Invoke(this);
        }

        private void UpdateManipTimer()
        {
            if (ManipTimer.Update())
            {
                PauseStance = false;
            }
        }

        protected void SetCanExit(bool value)
        {
            if (_canExit == value) return;

            _canExit = value;
        }

        protected abstract bool UpdateEnter(float dt);
        protected abstract void UpdateActive(float dt);
        protected abstract bool UpdateExit(float dt);
    }
}
