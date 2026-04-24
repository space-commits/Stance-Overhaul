using System;
using RealismCommonLib.Utils;
using StanceOverhaul.Controllers;
using StanceOverhaul.Enums;
using UnityEngine;

namespace StanceOverhaul.Stances
{
    public interface IStance2
    {
        public EStance StanceType { get; }

        public abstract Vector3Curve RotationCurve { get; }
        public abstract Vector3Curve PositionCurve { get; }

        public float BlendThreshold { get; }

        void OnEnter();
        void OnExit();
        void OnHoldUpdate(float deltaTime);
    }
}
