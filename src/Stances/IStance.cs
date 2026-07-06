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
        public EStaminaMode StaminaMode { get; }
        public float StaminaRate { get; }
        public float WalkSpeedBonus { get; }
        public float SprintAccelBonus { get; }
        public bool DoesTacSprint { get; }
        public float HipfireBonus { get; }

        public abstract Vector3Curve EnterRotationCurve { get; }
        public abstract Vector3Curve EnterPositionCurve { get; }

        public abstract Vector3Curve ExitRotationCurve { get; }
        public abstract Vector3Curve ExitPositionCurve { get; }

        public abstract AnimationCurve ExitAimSpeedCurve { get; }
        public abstract AnimationCurve EnterAimSpeedCurve { get; }

        float BlendIntoThreshold(EStanceType nextStance);
        float TransitionFromModifier(EStanceType? previousStance);
        float TransitionToSpeedModifier(EStanceType? nextStance);

        void OnEnter();
        void OnExit();
        void OnHoldUpdate(float deltaTime);
    }
}
