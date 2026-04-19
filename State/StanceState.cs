using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StanceOverhaul.Stances;
using StanceOverhaul.Enums;
using static RealismCommonLib.Plugin;
using RealismCommonLib.Utils;

namespace StanceOverhaul.Controllers.StateControllers
{
    internal class StanceState : IControllerHelper
    {
        private IStance? _current;
        private IStance? _next;
        private IStance? _queued; //TODO: when a stance is toggled during a transition, queue up that stance and it becomes _next stance when appropriate

        public EStance CurrentStanceType
        {
            get 
            {
                return 
                    _current == null || _current.State == EStanceState.Inactive ? EStance.None : 
                    _current.StanceType;
            }
        }

        public IStance? CurrentStance => _current;

        public EStance NextStanceType => _next?.StanceType ?? EStance.None;

        public bool NoActiveStances => _current?.State == EStanceState.Inactive && _next == null;

        private float _blendAlpha = 0f;

        //TODO: move blending to own class
        public Vector3 CurrentStancePosition
        {
            get
            {
                return _current != null ? _current.StancePosition : Vector3.zero;
            }
        }

        public Vector3 CurrentStanceRotation
        {
            get
            {
                return _current != null ? _current.StanceRotation : Vector3.zero;
            }
        }

        public Vector3 NextStancePosition
        {
            get
            {
                return _next != null ? _next.StancePosition : Vector3.zero;
            }
        }

        public Vector3 NextStanceRotation
        {
            get
            {
                return _next != null ? _next.StanceRotation : Vector3.zero;

            }
        }

        public void RunOnAwake()
        {
        }

        public void RunOnDestroy()
        {
        }

        public void RunOnUpdate(float deltaTime)
        {
            BlendStances(deltaTime);
            ProcessTransitions();
        }

        private void BlendStances(float deltaTime)
        {
            if (_current == null) return;

            _current.StanceUpdate(deltaTime);

            if (_next == null)
            {
                _blendAlpha = 0f;
                Plugin.StanceControllerInstance.StancePositionSpring.Zero = CurrentStancePosition;
                Plugin.StanceControllerInstance.StanceRotationSpring.Zero = CurrentStanceRotation;
                return;
            }

            if (_next.State != EStanceState.Inactive) 
            {
                ModLogger.LogWarning("blending");

                _blendAlpha = Mathf.Clamp01(_blendAlpha + deltaTime * PluginConfig.test20.Value);

                _next.StanceUpdate(deltaTime);

                Plugin.StanceControllerInstance.StancePositionSpring.Zero = Vector3.Lerp(CurrentStancePosition, NextStancePosition, _blendAlpha);
                Plugin.StanceControllerInstance.StanceRotationSpring.Zero = Vector3.Slerp(CurrentStanceRotation, NextStanceRotation, _blendAlpha);
            }
        }


        //TODO: fix this mess, very hard to debug or make sense of it.
        // Need to refactor request processing and transitions to allow two stances to exist
        private void ProcessTransitions()
        {
            if (_current == null) return;

            if (_current.State == EStanceState.Exiting
                && _current.CanTransition == true
                && _next?.State == EStanceState.Inactive)
            {
                ModLogger.LogWarning("starting transition");

                _next.Enter();
            }

            if (_next != null
                && _next?.State != EStanceState.Inactive
                && (MathUtils.IsGreaterThanOrEqualTo(_blendAlpha, 1f) || _current.State == EStanceState.Inactive))
            {
                ModLogger.LogWarning($"next stanfe state {_next?.State}");
                ModLogger.LogWarning($"ending transition {_blendAlpha}");

                _blendAlpha = 0f;
                _current = _next;
                _next = null;
            }

        }

        public void RequestStance(IStance stance)
        {

            if (stance != _current && _current == null)
            {
                ModLogger.LogWarning("toggling on for first time");
                _current = stance;
                _next = null;
                _current.Enter();
                return;
            }

            if (stance == _current && _current.IsActive) 
            {
                ModLogger.LogWarning("toggle off");

                _current.TryExit(force: true);
                return;
            }

            if (stance == _current && !_current.IsActive)
            {
                ModLogger.LogWarning("reactivating");
                _current.Enter();
                return;
            }

            if (stance != _current)
            {
                ModLogger.LogWarning("not current");

                _next = stance;

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