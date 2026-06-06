using System;
using RealismCommonLib.Utils;
using StanceOverhaul.Controllers;
using StanceOverhaul.Enums;
using UnityEngine;

namespace StanceOverhaul.Stances
{
    public interface IStance
    {
        public EStanceType StanceType { get; }

        public abstract Vector3Curve EnterRotationCurve { get; }
        public abstract Vector3Curve EnterPositionCurve { get; }

        public abstract Vector3Curve ExitRotationCurve { get; }
        public abstract Vector3Curve ExitPositionCurve { get; }

        float BlendThreshold(EStanceType nextStance);
        float BaseSpeed(EStanceType? previousStance);
        float TransitionSpeedModifier(EStanceType? nextStance);

        void OnEnter();
        void OnExit();
        void OnHoldUpdate(float deltaTime);
    }
}
