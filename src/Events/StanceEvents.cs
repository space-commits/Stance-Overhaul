using RealismCommonLib.Events;
using StanceOverhaul.Stances;
using System;

namespace StanceOverhaul.Events;

public static class StanceEvents
{
    /// <summary>Raised when a stance's OnEnter() fires. Carries the entering stance.</summary>
    public static event Action<IStance>? OnStanceEntered;

    /// <summary>Raised when a stance's OnExit() fires. Handler should revert to idle behaviour.</summary>
    public static event Action? OnStanceExited;

    internal static void RaiseOnStanceEntered(IStance stance)
    {
        BaseEventHandler.RaiseEvent(OnStanceEntered, stance);
    }

    internal static void RaiseOnStanceExited()
    {
        BaseEventHandler.RaiseEvent(OnStanceExited);
    }
}
