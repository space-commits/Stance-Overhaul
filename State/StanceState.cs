using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StanceOverhaul.Stances;
using StanceOverhaul.Enums;

namespace StanceOverhaul.Controllers.StateControllers
{
    public class StanceState : IControllerHelper
    {
        private IStance _current;
        private IStance _queued;

        public IStance CurrentStance => _current;

        public EStance CurrentStanceType => _current?.StanceType ?? EStance.None;

        public bool NoActiveStances => _current.State == EStanceState.Inactive && _queued == null; 

        public void RunOnAwake()
        {
        }

        public void RunOnDestroy()
        {
        }

        public void RunOnUpdate(float deltaTime)
        {
            _current?.StanceUpdate(deltaTime);
        }

        public void CancelStances() 
        {
            _current?.Exit(force: true);
            _queued = null;
        }

        public void RequestStance(IStance stance)
        {
            //toggle off if pressing key for current stance
            if (_current == stance)
            {
                _queued = null;
                _current.Exit(force: true);
                return;
            }

            _queued = stance;
            TryProcessQueue();
        }

        private void TryProcessQueue()
        {
            if (_current == null)
            {
                ActivateQueued();
                return;
            }

            _current.OnExitCompleted += OnCurrentExitComplete;
            _current.Exit(force: _current.IsTransitioning || !_current.CanExit);
        }

        private void ActivateQueued()
        {
            if (_queued == null)
                return;

            _current = _queued;
            _queued = null;

            _current.OnCanExitChanged += OnCurrentCanExitChanged;

            _current.OnEnterCompleted += OnEnterComplete;
            _current.Enter();
        }

        private void OnCurrentCanExitChanged(IStance stance)
        {
            TryProcessQueue();
        }

        private void OnCurrentExitComplete(IStance stance)
        {
            _current.OnExitCompleted -= OnCurrentExitComplete;
            ActivateQueued();
        }

        private void OnEnterComplete(IStance stance)
        {
            stance.OnEnterCompleted -= OnEnterComplete;
            //optional: notify other systems
        }
    }
}