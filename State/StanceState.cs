using RealismCommonLib.StateControllers.InstanceState;
using StanceOverhaul.Enums;
using StanceOverhaul.Handlers;
using StanceOverhaul.Stances;
using System.Text;
using UnityEngine;
using static RealismCommonLib.Plugin;

namespace StanceOverhaul.State
{
    internal class StanceState : IControllerHelper
    {
        private IStance? _requestedStance;
        private StanceSlot? _primary;
        private StanceSlot? _incoming;

        private bool _incomingPaused;

        private Vector3 _smoothedPosition;
        private Vector3 _smoothedRotation;

        public float SmoothSpeed { get; private set; } = 20f; //TODO: expose in config?

        public Vector3 StancePosition { get; private set; }
        public Vector3 StanceRotation { get; private set; }

        public EStanceType ActiveStanceType
        {
            get
            {
                return ActiveStance?.StanceType ?? EStanceType.None;
            }
        }

        /// <summary>
        /// The stance that is active or being transitioned to.
        /// Use this for checking if what the active or current stance is, not PrimaryStance, 
        /// as PrimaryStance can be heading to idle or being blended out to another stance.
        /// </summary>
        public IStance? ActiveStance
        {
            get
            {
                if (_incoming != null && !_incomingPaused && _incoming.IsAtOrHeadingToPose)
                    return _incoming.Stance;

                if (_primary != null && _primary.IsAtOrHeadingToPose)
                    return _primary.Stance;
                return null;
            }
        }

        /// <summary>
        /// The stance that is currently active, but can be heading to idle or belding to another stance.
        /// Do not use this for checking if a stance is active, use ActiveStance instead. 
        /// Use this if you need to know the stance that is currently being blended out of or heading to idle.
        /// </summary>
        public IStance? PrimaryStance
        {
            get
            {
                return _primary?.Stance;
            }
        }

        public bool IsIdle => ActiveStance == null;

        public void RunOnAwake()
        {
        }

        public void RunOnDestroy()
        {
        }

        public void RunOnUpdate(float deltaTime)
        {
            UpdateStanceState(deltaTime);
        }

        private void UpdateStanceState(float deltaTime)
        {
            //update primary
            _primary?.SlotUpdate(deltaTime);

            //check blend threshold - unpause incoming if met
            if (_incoming != null && _incomingPaused && _primary != null)
            {
                /*      if (_primary.Direction != 0
                          && _primary.IdleProximity <= _primary.Stance.BlendThreshold)
                      {
                          ModLogger.LogWarning("== threshold met");
                          _incomingPaused = false;
                      }*/

                if (_primary.IsHeadingToIdle
                && _primary.IdleProximity >= _primary.Stance.BlendIntoThreshold(_incoming.Stance.StanceType))
                {
                    ModLogger.LogWarning("== threshold met");
                    _incomingPaused = false;
                }

                ModLogger.LogWarning($"IdleProximity {_primary.IdleProximity} BlendThreshold {_primary.Stance.BlendIntoThreshold(_incoming.Stance.StanceType)}");
            }

            //upate incoming if not paused
            if (_incoming != null && !_incomingPaused)
                _incoming.SlotUpdate(deltaTime);

            //cleanup completed slots: discard slots that reached idle
            if (_primary?.IsAtIdle == true) //&& _incoming == null
                _primary = null;

            if (_incoming != null && !_incomingPaused && _incoming.IsAtIdle)
                _incoming = null;

            //promote incoming if primary is gone
            if (_primary == null && _incoming != null)
            {
                _primary = _incoming;
                _incoming = null;
                _incomingPaused = false;

                ModLogger.LogWarning("== incoming promoted to primary");
            }

            UpdateAimSpeed();
            //UpdateAimState();

            UpdateTransforms(deltaTime);
        }

        public void UpdateTransforms(float deltaTime)
        {
            //evaluate output from active slots
            var rawPos = Vector3.zero;
            var rawRot = Vector3.zero;

            if (_primary != null && _incoming != null && !_incomingPaused)
            {
                float weight = _incoming.Progress;
                rawPos = Vector3.Lerp(_primary.EvaluatePosition(), _incoming.EvaluatePosition(), weight);
                rawRot = Vector3.Lerp(_primary.EvaluateRotation(), _incoming.EvaluateRotation(), weight);
            }
            else if (_primary != null)
            {
                rawPos = _primary.EvaluatePosition();
                rawRot = _primary.EvaluateRotation();
            }
            else if (_incoming != null)
            {
                rawPos = _incoming.EvaluatePosition();
                rawRot = _incoming.EvaluateRotation();
            }

            //output smoothing
            float smoothFactor = Mathf.Clamp01(deltaTime * PluginConfig.test19.Value);
            _smoothedPosition = Vector3.Lerp(_smoothedPosition, rawPos, smoothFactor);
            _smoothedRotation = Vector3.Lerp(_smoothedRotation, rawRot, smoothFactor);

            StancePosition = _smoothedPosition;
            StanceRotation = _smoothedRotation;
        }

        //Move to StanceAimHandler
        public void UpdateAimSpeed()
        {
            float aimSpeedModifier = 1f;

            if (_incoming != null && PlayerStateInstance.PWA.IsAiming)
            {
                aimSpeedModifier = _incoming.EvaluateAimSpeed();
            }

            if (_primary != null && PlayerStateInstance.PWA.IsAiming)
            {
                aimSpeedModifier = _primary.EvaluateAimSpeed();
            }

            Plugin.StanceControllerInstance.PwaAimSpeed = Plugin.StanceControllerInstance.PwaOriginalAimSpeed * aimSpeedModifier;
        }

        //Move to StanceAimHandler
   /*     public bool AimingInterrupted { get; private set; }*/

        //Move to StanceAimHandler
 /*       public void UpdateAimState()
        {
            if (_primary == null || !_primary.Stance.InterruptsAiming) return;

            bool isAnimating = _primary.IsHeadingToIdle || _primary.IsHeadingToPose;

            // Only interrupt if aiming AND heading AND not already interrupted
            if (!AimingInterrupted && isAnimating && AimStateInstance.IsAiming)
            {
                InterruptAim();
                return;
            }

            if (!AimStateInstance.IsAiming && _primary.IsCloseToTerminalState(PluginConfig.test1.Value)) //_primary.Stance.AimResumeThreshold
            {
                UnInterruptAim();
            }
        }*/

    /*    public void InterruptAim()
        {
            PlayerStateInstance.FirearmController.ToggleAim();
            AimingInterrupted = true;
        }
        public void UnInterruptAim()
        {
            PlayerStateInstance.FirearmController.ToggleAim();
            AimingInterrupted = false;
        }*/

        public void RequestStance(IStance stance)
        {
            // no active stance: simple enter
            if (_primary == null && _incoming == null)
            {
                ModLogger.LogWarning("toggle stance from no stance");
                _primary = new StanceSlot(stance, ECurveType.Enter, 0f, +1, this);
                stance.OnEnter();
                return;
            }

            // same stance as primary: toggle exit or reverse
            if (_primary?.Stance == stance && _incoming == null)
            {
                //holding -> switch to exit curve
                if (_primary.Direction == 0)
                {
                    ModLogger.LogWarning("switch to exit curve from hold");
                    _primary.ActiveCurve = ECurveType.Exit;
                    _primary.Progress = 0f;
                    _primary.Direction = +1;
                    stance.OnExit();
                }
                else if (_primary.IsHeadingToIdle) // heading to idle -> reverse toward pose
                {
                    ModLogger.LogWarning("reverse direction towards pose");
                    _primary.Direction *= -1; //should this be +1 or -1?
                    /*stance.OnEnter();*/
                }
                else // heading to pose -> reverse toward idle
                {
                    ModLogger.LogWarning("reverse direction towards idle");
                    _primary.Direction *= -1; //should this be +1 or -1?
                    stance.OnExit();
                }
                return;
            }

            //cancel incoming during blend
            if (_incoming?.Stance == stance)
            {
                ModLogger.LogWarning($"cancel incoming during blend");
                BeginExit(_incoming);
                return;
            }

            //third stance during blend
            if (_incoming != null)
            {
                ModLogger.LogWarning("third stance during blend");
                //collapse: drop primary, promte incoming, start its exit
                _primary = _incoming;
                _incoming = null;
                BeginExit(_primary);
            }
            else // if (_primary != null)
            {
                ModLogger.LogWarning($"normal transition A -> B {stance.StanceType}");
                //normal transition A -> B
                BeginExit(_primary);
            }

            _incoming = new StanceSlot(stance, ECurveType.Enter, 0f, +1, this);
            _incomingPaused = true; //will be unpaused once blend threshold is met
            stance.OnEnter();
            ModLogger.LogWarning($"incoming queued and paused {stance.StanceType}");
        }

        public void BeginExit(StanceSlot slot)
        {
            if (slot.Direction == 0) //holding -> switch to exit curve
            {
                ModLogger.LogWarning($"switch to exit curve  {slot.Stance.StanceType}");
                slot.ActiveCurve = ECurveType.Exit;
                slot.Progress = 0f;
                slot.Direction = +1;
            }
            else if (slot.IsHeadingToIdle) // already heading to idle: no change needed
            {
                ModLogger.LogWarning($"headed to idle: on change  {slot.Stance.StanceType}");

                return;
            }
            else // heading to pose -> change direction to idle, stay on the same curve, just reverse
            {
                ModLogger.LogWarning($"reverse direction {slot.Stance.StanceType}");
                slot.Direction *= -1; //should this be +1 or -1?
            }

            slot.Stance.OnExit();
        }

        public void CancelAll()
        {
            ModLogger.LogWarning("cancel all");

            if (_primary != null)
                BeginExit(_primary);

            if (_incoming == null) return;

            if (_incomingPaused)
            {
                _incoming = null;
                _incomingPaused = false;
            }
            else
            {
                BeginExit(_incoming);
            }
        }
    }
}