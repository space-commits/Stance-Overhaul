using RealismCommonLib.Events;
using RealismCommonLib.Utils;
using StanceOverhaul.Controllers.StateControllers;
using StanceOverhaul.Enums;
using System;
using UnityEngine;
using static RealismCommonLib.Plugin;

namespace StanceOverhaul.Stances
{
    public abstract class StanceBase2 : IStance2, IDisposable
    {
        public virtual EStance StanceType => EStance.None;

        public Vector3Curve RotationCurve { get; protected set; }
        public Vector3Curve PositionCurve { get; protected set; }

        public float BlendThreshold => 0.5f;

        public StanceBase2()
        {
        }

        public void Dispose()
        {
        }

        public virtual void OnEnter()
        {
        }

        public virtual void OnExit()
        {
        }

        public virtual void OnHoldUpdate(float deltaTime)
        {      
        }
    }
}
