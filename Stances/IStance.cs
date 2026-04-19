using System;
using StanceOverhaul.Controllers;
using StanceOverhaul.Enums;
using UnityEngine;

namespace StanceOverhaul.Stances
{
    public interface IStance
    {
        public EStance StanceType { get; }

        public Vector3 StanceRotation { get; }
        public Vector3 StancePosition { get; }

        public bool CanTransition { get; }

        bool IsActive { get; }

        public EStanceState State { get; }

        void Enter();
        void TryExit(bool force = false);

        void StanceUpdate(float deltaTime);

        event Action<IStance> OnEnterStarted;
        event Action<IStance> OnEnterCompleted;

        event Action<IStance> OnExitStarted;
        event Action<IStance> OnExitCompleted;
    }
}
