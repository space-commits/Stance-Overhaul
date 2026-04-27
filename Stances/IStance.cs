using System;
using RealismCommonLib.Utils;
using StanceOverhaul.Controllers;
using StanceOverhaul.Enums;
using UnityEngine;

namespace StanceOverhaul.Stances
{
    public interface IStance
    {
        public EStance StanceType { get; }

        public abstract Vector3Curve EnterRotationCurve { get; }
        public abstract Vector3Curve EnterPositionCurve { get; }

        public abstract Vector3Curve ExitRotationCurve { get; }
        public abstract Vector3Curve ExitPositionCurve { get; }

        public float BlendThreshold { get; }
        public float BaseSpeed { get; }

        void OnEnter();
        void OnExit();
        void OnHoldUpdate(float deltaTime);
    }
}
