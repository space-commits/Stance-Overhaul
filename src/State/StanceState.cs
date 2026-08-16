using RealismCommonLib.StateControllers.InstanceState;
using StanceOverhaul.Enums;
using StanceOverhaul.SubSystem;
using StanceOverhaul.Stances;
using System.Text;
using UnityEngine;
using static RealismCommonLib.Plugin;

namespace StanceOverhaul.State
{

    /// <summary>
    /// Represents a transition from one stance to another, including the source and target stances.
    /// Used to ensure that stance slots are aware of the context of the transition they are part of
    /// </summary>
    internal class StanceTransitionContext
    {
        /// <summary>
        /// The stance that is being transitioned from, if there is a stance transition occuring.
        /// </summary>
        public EStanceType From { get; }

        /// <summary>
        /// The stance that is being transitioned to, if there is a stance transition occuring.
        /// </summary>
        public EStanceType To { get; }

        public StanceTransitionContext(EStanceType from, EStanceType to)
        {
            From = from;
            To = to;
        }
    }

    internal class StanceState : SubSystem.ISubSystem
    {
        private StanceSlot? _primary;
        private StanceSlot? _incoming;

        private bool _incomingPaused;

        private Vector3 _smoothedPosition;
        private Vector3 _smoothedRotation;

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
        /// Use this for checking what the active or current stance is, not PrimaryStance, 
        /// as PrimaryStance can be heading to idle or being blended out to another stance.
        /// </summary>
        public IStance? ActiveStance
        {
            get
            {
                if (_incoming != null && !_incomingPaused && _incoming.IsAtOrHeadingToActivePose)
                    return _incoming.Stance;

                if (_primary != null && _primary.IsAtOrHeadingToActivePose)
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
                if (_primary.IsHeadingToIdle
                && _primary.IdleProximity >= _incoming.Stance.BlendIntoThreshold(_primary.Stance.StanceType))
                {
                    _incomingPaused = false;
                }
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
            float smoothFactor = Mathf.Clamp01(deltaTime * PluginConfig.StanceBlendSpeed.Value);
            _smoothedPosition = Vector3.Lerp(_smoothedPosition, rawPos, smoothFactor);
            _smoothedRotation = Vector3.Lerp(_smoothedRotation, rawRot, smoothFactor);

            StancePosition = _smoothedPosition;
            StanceRotation = _smoothedRotation;
        }

        //Move to StanceAimHandler
        public void UpdateAimSpeed()
        {
            float aimSpeedModifier = 1f;

            if (AimStateInstance.IsAiming)
            {
                if (_incoming != null && _incomingPaused == false)
                    aimSpeedModifier = _incoming.EvaluateAimSpeed();
                else if (_primary != null)
                    aimSpeedModifier = _primary.EvaluateAimSpeed();
            }

            Plugin.StanceControllerInstance.PwaAimSpeed = Plugin.StanceControllerInstance.PwaOriginalAimSpeed * aimSpeedModifier;
        }

        public void RequestStance(IStance stance)
        {
            // no active stance: simple enter
            if (_primary == null && _incoming == null)
            {
                var transition = new StanceTransitionContext(EStanceType.None, stance.StanceType);

                _primary = new StanceSlot(stance, ECurveType.Enter, 0f, +1, transition);
                stance.OnEnter();

                return;
            }

            // same stance as primary: toggle exit or reverse
            if (_primary?.Stance == stance && _incoming == null)
            {
                //holding -> switch to exit curve
                if (_primary.Direction == 0)
                {
                    _primary.Transition = new StanceTransitionContext(stance.StanceType, EStanceType.None); ;

                    _primary.ActiveCurve = ECurveType.Exit;
                    _primary.Progress = 0f;
                    _primary.Direction = +1;
                    stance.OnExit();
                }
                else if (_primary.IsHeadingToIdle) // heading to idle -> reverse toward pose
                {
                    _primary.Transition = new StanceTransitionContext(EStanceType.None, stance.StanceType);

                    _primary.Direction *= -1;
                    stance.OnEnter();
                }
                else // heading to pose -> reverse toward idle
                {
                    _primary.Transition = new StanceTransitionContext(stance.StanceType, EStanceType.None);

                    _primary.Direction *= -1;
                    stance.OnExit();
                }
                return;
            }

            //same stance is incoming, discard current stance and promote incoming to primary, start its exit
            if (_incoming?.Stance == stance)
            {
                _primary = _incoming;
                _incoming = null;
                _incomingPaused = false;

                var transition = new StanceTransitionContext(_primary.Stance.StanceType, EStanceType.None);
                BeginExit(_primary, transition);

                return;
            }

            //third stance during blend. A = Null, B = Primary, C = Incoming. B -> C.
            if (_incoming != null)
            {
                //collapse: drop primary, promote incoming, start its exit
                //current incoming becomes the active stance.  Old primary is abandoned.
                _primary = _incoming; //incoming becomes primary, and will be blended out to the new stance
                _incoming = null;

                //start exit of new primary
                var transition = new StanceTransitionContext(_primary.Stance.StanceType, stance.StanceType);
                BeginExit(_primary, transition);

                //start new incoming
                _incoming = new StanceSlot(stance, ECurveType.Enter, 0f, +1, transition);
                _incomingPaused = true;
                stance.OnEnter();

                return;
            }

            //normal transition A -> B
            if (_primary != null)
            {
                var transition = new StanceTransitionContext(_primary.Stance.StanceType, stance.StanceType);

                BeginExit(_primary, transition);

                _incoming = new StanceSlot(stance, ECurveType.Enter, 0f, +1, transition);
                _incomingPaused = true;
                stance.OnEnter();
            }
        }

        public void BeginExit(StanceSlot slot, StanceTransitionContext transition)
        {
            slot.Transition = transition;

            if (slot.Direction == 0) //holding -> switch to exit curve
            {
                slot.ActiveCurve = ECurveType.Exit;
                slot.Progress = 0f;
                slot.Direction = +1;
            }
            else if (slot.IsHeadingToIdle) // already heading to idle: no change needed
            {
                return;
            }
            else // heading to pose -> change direction to idle, stay on the same curve, just reverse
            {
                slot.Direction *= -1;
            }

            slot.Stance.OnExit();
        }

        public void CancelAll()
        {
            if (_primary != null)
            {
                var transition = new StanceTransitionContext(_primary.Stance.StanceType, EStanceType.None);
                BeginExit(_primary, transition);
            }

            // Incoming hasn't started yet, discard it.
            if (_incomingPaused)
            {
                _incoming = null;
                _incomingPaused = false;
            }
            else if (_incoming != null) // incoming is already blending, exit it
            {
                var transition = new StanceTransitionContext(_incoming.Stance.StanceType, EStanceType.None);
                BeginExit(_incoming, transition);
            }
        }
    }
}