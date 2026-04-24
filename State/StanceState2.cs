using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StanceOverhaul.Stances;
using StanceOverhaul.Enums;
using static RealismCommonLib.Plugin;
using RealismCommonLib.Utils;
using System;

namespace StanceOverhaul.Controllers.StateControllers
{
    public enum SlotPhase    
    {
        Entering,
        Holding,
        Exiting
    }

    internal class StanceSlot 
    {
        public IStance2 Stance;
        public SlotPhase Phase;
        public float Progress { get; set; }
        public float Speed { get; set; }

        internal StanceSlot(IStance2 stance, SlotPhase phase, float progress)
        {
            Stance = stance;
            Phase = phase;
            this.Progress = Progress;
        }

        public bool IsComplete => Phase == SlotPhase.Exiting && Progress <= 0f;

        public Vector3 EvaluatePosition() 
        {
            return Stance.PositionCurve.Evaluate(Progress);
        }

        public Vector3 EvaluateRotation()
        {
            return Stance.RotationCurve.Evaluate(Progress);
        }

        public void SlotUpdate(float deltaTime) 
        {
            if (Phase == SlotPhase.Holding) return;

            float direction = Phase == SlotPhase.Entering ? 1f : -1f;
            Progress = Mathf.Clamp01(Progress + deltaTime * PluginConfig.test20.Value * direction);
            
            if (Phase == SlotPhase.Entering && Progress >= 1f)
                Phase = SlotPhase.Holding;
        }
    }

    internal class StanceState2 : IControllerHelper
    {
        private StanceSlot? _primary;
        private StanceSlot? _incoming;
        private bool _incomingPaused;

        public Vector3 StancePosition { get; private set; }
        public Vector3 StanceRotation { get; private set; }
  

        public void RunOnAwake()
        {
        }

        public void RunOnDestroy()
        {
        }

        public void RunOnUpdate(float deltaTime)
        {
            //tick primary
            _primary?.SlotUpdate(deltaTime);

            //check blend threshold - unpause incoming if met
            if (_incoming != null && _incomingPaused)
            {
                if (_primary != null 
                    && _primary.Phase == SlotPhase.Exiting 
                    && _primary.Progress <= _primary.Stance.BlendThreshold) 
                {
                    _incomingPaused = false;
                }
            }

            if (_incoming != null && !_incomingPaused) 
            {
                _incoming.SlotUpdate(deltaTime);
            }

            //cleanup completed slots

            if (_primary?.IsComplete == true)
            {
                _primary = null;
            }
            if (_incoming != null && _primary == null) 
            {
                _primary = _incoming;
                _incoming = null;
                _incomingPaused = false;
            }

            //evaluate output from active slots
            var pos = Vector3.zero;
            var rot = Vector3.zero;
            if (_primary != null) 
            {
                pos += _primary.EvaluatePosition();
                rot += _primary.EvaluateRotation();
            }
            if (_incoming != null && !_incomingPaused) 
            {
                pos += _incoming.EvaluatePosition();
                rot += _incoming.EvaluateRotation();
            }

            StancePosition = pos;
            StanceRotation = rot;

            Plugin.StanceControllerInstance.StancePositionSpring.Zero = StancePosition;
            Plugin.StanceControllerInstance.StanceRotationSpring.Zero = StanceRotation;
        }

        public void RequestStance(IStance2 stance)
        {
            // no active stance
            if (_primary == null && _incoming == null)
            {
                _primary = new StanceSlot(stance, SlotPhase.Entering, 0);
                return;
            }

            // re-toggle during exit, reverse back to entering
            if (_primary?.Stance == stance && _primary.Phase == SlotPhase.Exiting && _incoming == null)
            {
                _primary.Phase = SlotPhase.Entering;
                //progress stays where it is, just starts incrementing again
                return;         
            }

            //same stnace toggled, begin exit to idle
            if (_primary?.Stance == stance) 
            {
                BeginExit(_primary);
                return;
            }

            //cancel incoming during blend
            if (_incoming != null)
            {
                //collapse: drop primary, promote incoming as new primary (start its exit)
                _primary = _incoming;
                _incoming = null;
                BeginExit(_primary);
            }
            else 
            {
                //norm transition a to b
                BeginExit(_primary);
            }

            // queue new incoming
            _incoming = new StanceSlot(stance, SlotPhase.Entering, 0);
            _incomingPaused = true;
            _incoming.Stance.OnEnter();
        }

        public void BeginExit(StanceSlot slot) 
        {
            //progress stays where it is: update will start decrementing
            // no remapping needed because the same curve is used in both directions
            slot.Phase = SlotPhase.Exiting;
            slot.Stance.OnExit();
        }
    }
}