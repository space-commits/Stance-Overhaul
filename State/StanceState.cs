using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StanceOverhaul.Stances;
using StanceOverhaul.Enums;
using static RealismCommonLib.Plugin;

namespace StanceOverhaul.Controllers.StateControllers
{
    internal class StanceState : IControllerHelper
    {
        private IStance _current;
        private IStance _next;

        public IStance CurrentStance => _current;
        public IStance NextStance => _next;

        public EStance CurrentStanceType => _current?.StanceType ?? EStance.None;
        public EStance NextStanceType => _next?.StanceType ?? EStance.None;

        public bool NoActiveStances => _current.State == EStanceState.Inactive && _next == null;

        public void RunOnAwake()
        {
        }

        public void RunOnDestroy()
        {
        }

        public void RunOnUpdate(float deltaTime)
        {
            _current?.StanceUpdate(deltaTime);
            ProcessTransitions();
        }

        private void ProcessTransitions()
        {
            if (_next != null &&
                _next != _current && 
                (_current == null || _current.State == EStanceState.Inactive)) 
            {
                _current = _next;
                _next = null;
                _current.Enter();
            }
        }

        public void RequestStance(IStance stance)
        {
            _next = stance;

            if (_next == _current && _current.IsActive) 
            {
                ModLogger.LogWarning("toggle off");

                _next = null;
                _current.TryExit(force: true);
                return;
            }

            if (_next == _current && !_current.IsActive)
            {
                ModLogger.LogWarning("ractivating");
                _current.Enter();
            }

            if (_next != _current)
            {
                ModLogger.LogWarning("not current");

                if (_current != null)
                    _current.TryExit();
                return;
            }

        }

        public void CancelAll()
        {
            _next = null;

            if (_current != null)
                _current.TryExit(force: true);
        }
    }
}