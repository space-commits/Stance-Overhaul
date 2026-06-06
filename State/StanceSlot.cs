using StanceOverhaul.Enums;
using StanceOverhaul.Stances;
using UnityEngine;

namespace StanceOverhaul.State;

internal class StanceSlot
{
    private StanceState _stanceState;
    public IStance Stance;
    public ECurveType ActiveCurve { get; set; }
    public float Progress { get; set; } // 0..1
    public int Direction { get; set; } // +1 or -1

    internal StanceSlot(IStance stance, ECurveType activeCurve, float progress, int direction, StanceState stanceState)
    {
        Stance = stance;
        ActiveCurve = activeCurve;
        Direction = direction;
        Progress = progress;
        _stanceState = stanceState;
    }

    public bool IsAtIdle =>
        (ActiveCurve == ECurveType.Enter && Progress <= 0f) ||
        (ActiveCurve == ECurveType.Exit && Progress >= 1f);

    public bool IsAtPose =>
        (ActiveCurve == ECurveType.Enter && Progress >= 1f) ||
        (ActiveCurve == ECurveType.Exit && Progress <= 0f);

    public bool IsHeadingToIdle =>
        (ActiveCurve == ECurveType.Enter && Direction == -1) ||
        (ActiveCurve == ECurveType.Exit && Direction == +1);

    public bool IsHeadingToPose =>
        (ActiveCurve == ECurveType.Enter && Direction == +1) ||
        (ActiveCurve == ECurveType.Exit && Direction == -1);

    public bool IsAtOrHeadingToPose =>
        IsAtPose || IsHeadingToPose;

    public float IdleProximity =>
        ActiveCurve == ECurveType.Exit ? Progress : 1f - Progress;

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

    public void SlotUpdate(float deltaTime)
    {
        if (Direction == 0) return; // holding

        Progress = Mathf.Clamp01(
            Progress + deltaTime * 
            Stance.BaseSpeed(_stanceState.PrimaryStance?.StanceType) *
            Stance.TransitionSpeedModifier(_stanceState.ActiveStanceType) *
            PluginConfig.test20.Value * Direction);

        // reached pose end -> enter holding
        if (IsAtPose)
            Direction = 0;
    }
}