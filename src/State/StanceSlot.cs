using StanceOverhaul.Enums;
using StanceOverhaul.Stances;
using UnityEngine;
using StanceOverhaul.Events;
using static StanceOverhaul.Plugin;

namespace StanceOverhaul.State;

internal class StanceSlot
{
    public StanceTransitionContext Transition { get; set; }
    public IStance Stance { get; }
    public ECurveType ActiveCurve { get; set; }
    public float Progress { get; set; } // 0..1
    public int Direction { get; set; } // +1 or -1
    public float PreviousProgress { get; private set; }

    internal StanceSlot(IStance stance, ECurveType activeCurve, float progress, int direction, StanceTransitionContext transition)
    {
        Stance = stance;
        ActiveCurve = activeCurve;
        Direction = direction;
        Progress = progress;
        Transition = transition;
    }

    public bool IsAtIdle =>
        (ActiveCurve == ECurveType.Enter && Progress <= 0f) ||
        (ActiveCurve == ECurveType.Exit && Progress >= 1f);

    /// <summary>
    /// Is at the terminal state of the stance, and hasn't started transitioning away from it yet.
    /// </summary>
    public bool IsAtPose =>
        (ActiveCurve == ECurveType.Enter && Progress >= 1f) ||
        (ActiveCurve == ECurveType.Exit && Progress <= 0f);

    public bool IsHeadingToIdle =>
        (ActiveCurve == ECurveType.Enter && Direction == -1) ||
        (ActiveCurve == ECurveType.Exit && Direction == +1);

    public bool IsHeadingToPose =>
        (ActiveCurve == ECurveType.Enter && Direction == +1) ||
        (ActiveCurve == ECurveType.Exit && Direction == -1);


    /// <summary>
    /// Is at the terminal state of the stance, or heading towards it, and not heading away from it.
    /// </summary>
    public bool IsAtOrHeadingToActivePose =>
        (ActiveCurve == ECurveType.Enter && (Direction == +1 || Progress >= 1f)) ||
        (ActiveCurve == ECurveType.Exit && Direction == -1);

    public float IdleProximity =>
        ActiveCurve == ECurveType.Exit ? Progress : 1f - Progress;

    public float DistanceToPose =>
        1f - IdleProximity;

    public Vector3 EvaluatePosition()
    {
        return ActiveCurve == ECurveType.Exit
            ? Stance.ExitPositionCurve.Evaluate(Progress)
            : Stance.EnterPositionCurve.Evaluate(Progress);
    }

    public Vector3 EvaluateRotation()
    {
        return ActiveCurve == ECurveType.Exit
            ? Stance.ExitRotationCurve.Evaluate(Progress)
            : Stance.EnterRotationCurve.Evaluate(Progress);
    }

    public float EvaluateAimSpeed()
    {
        return ActiveCurve == ECurveType.Exit
            ? Stance.ExitAimSpeedCurve.Evaluate(Progress)
            : ActiveCurve == ECurveType.Enter ?
            Stance.EnterAimSpeedCurve.Evaluate(Progress)
            : 1f;
    }

    public void SlotUpdate(float deltaTime)
    {
        if (Direction == 0) return; // holding

        var speed = Stance.TransitionFromModifier(Transition.From) *
                    Stance.TransitionToSpeedModifier(Transition.To) *
                    PluginConfig.GlobalStanceSpeed.Value;

        PreviousProgress = Progress;

        Progress = Mathf.Clamp01(
            Progress +
            Direction *
            deltaTime *
            StanceControllerInstance.StatsHandlerInstance.GetStanceSpeedModifier(speed));

        if (PreviousProgress < Stance.StanceHitShoulderThreshold && Progress >= Stance.StanceHitShoulderThreshold)
        {
            StanceEvents.RaiseOnStanceHitShoulder();
        }

        // reached pose end -> enter holding
        if (IsAtPose)
            Direction = 0;
    }
}