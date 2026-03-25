using System;
using StanceOverhaul.Controllers;
using StanceOverhaul.Controllers.StateControllers;
using StanceOverhaul.Enums;
using static Val;

namespace StanceOverhaul.Stances
{
    public abstract class StanceBase : IStance
    {
        public EStanceState State => _state;
        public virtual EStance StanceType => EStance.None;

        public bool IsActive => _state == EStanceState.Active;
        public bool IsTransitioning => _state == EStanceState.Entering || _state == EStanceState.Exiting;
       
        public bool CanExit => _state == EStanceState.Active;

        protected EStanceState _state = EStanceState.Inactive;

        private bool _lastCanExit = false;

        public event Action<IStance> OnEnterStarted;
        public event Action<IStance> OnEnterCompleted;
        public event Action<IStance> OnExitStarted;
        public event Action<IStance> OnExitCompleted;
        public event Action<IStance> OnCanExitChanged;

        public virtual void Enter()
        {
            if (_state != EStanceState.Inactive)
                return;

            _state = EStanceState.Entering;
            OnEnterStarted?.Invoke(this);
        }

        public virtual void Exit(bool force = false)
        {
            if (_state == EStanceState.Inactive)
                return;

            if (!force && !CanExit)
                return;

            if (_state == EStanceState.Exiting)
                return;

            _state = EStanceState.Exiting;
            OnExitStarted?.Invoke(this);
        }

        public void StanceUpdate(float dt) 
        {
            switch (_state)
            {
                case EStanceState.Entering:
                    if (UpdateEnter(dt))
                    {
                        _state = EStanceState.Active;
                        OnEnterCompleted?.Invoke(this);
                    }
                    break;
                case EStanceState.Active:
                    UpdateActive(dt);
                    break;
                case EStanceState.Exiting:
                    if (UpdateExit(dt))
                    {
                        _state = EStanceState.Inactive;
                        OnExitCompleted?.Invoke(this);
                    }
                    break;
            }
            CheckCanExitChanged();
        }

        //TODO: this is not a good implementation, try find another way notify controller that stance can exit
        protected void CheckCanExitChanged()
        {
            bool currentExit = CanExit;
            if (currentExit != _lastCanExit)
            {
                _lastCanExit = currentExit;
                OnCanExitChanged?.Invoke(this);
            }
        }

        protected abstract bool UpdateEnter(float dt);
        protected abstract void UpdateActive(float dt);
        protected abstract bool UpdateExit(float dt);
    }
}
