using System;
using StanceOverhaul.Controllers;
using StanceOverhaul.Enums;

namespace StanceOverhaul.Stances
{
    public interface IStance
    {
        public EStance StanceType { get; }

        bool IsActive { get; }
        bool IsTransitioning { get; }
        bool CanExit { get; }

        public EStanceState State { get; }

        void Enter();
        void Exit(bool force = false);

        void StanceUpdate(float deltaTime);

        event Action<IStance> OnEnterStarted;
        event Action<IStance> OnEnterCompleted;

        event Action<IStance> OnCanExitChanged;

        event Action<IStance> OnExitStarted;
        event Action<IStance> OnExitCompleted;
    }
}
