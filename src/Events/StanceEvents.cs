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

    public static event Action? OnTacSprintStarted;

    public static event Action? OnTacSprintEnded;

    public static event Action? OnStanceHitShoulder;

    public static event Action? OnStanceReloadReset;
    public static event Action? OnStanceCheckAmmo;
    public static event Action? OnStanceChamberCheck;
    public static event Action? OnStanceChamber;
    public static event Action? OnStanceMagReload;
    public static event Action? OnStanceTubeReload;
    public static event Action? OnStanceTopReload;
    public static event Action? OnStanceReload;

    internal static void RaiseOnStanceEntered(IStance stance)
    {
        BaseEventHandler.RaiseEvent(OnStanceEntered, stance);
    }

    internal static void RaiseOnStanceExited()
    {
        BaseEventHandler.RaiseEvent(OnStanceExited);
    }

    internal static void RaiseOnTacSprintStarted()
    {
        BaseEventHandler.RaiseEvent(OnTacSprintStarted);
    }

    internal static void RaiseOnTacSprintEnded()
    {
        BaseEventHandler.RaiseEvent(OnTacSprintEnded);
    }

    internal static void RaiseOnStanceHitShoulder()
    {
        BaseEventHandler.RaiseEvent(OnStanceHitShoulder);
    }

    /// <summary>
    /// Chains from common lib reload events but allows StanceReloadSpeedSystem to handle timing
    /// </summary>
    internal static void RaiseStanceReloadReset()
    {
        BaseEventHandler.RaiseEvent(OnStanceReloadReset);
    }
    internal static void RaiseStanceChamberCheck()
    {
        BaseEventHandler.RaiseEvent(OnStanceChamberCheck);
    }
    internal static void RaiseStanceCheckAmmo()
    {
        BaseEventHandler.RaiseEvent(OnStanceCheckAmmo);
    }
    internal static void RaiseStanceChamber()
    {
        BaseEventHandler.RaiseEvent(OnStanceChamber);
    }
    internal static void RaiseStanceMagReload()
    {
        BaseEventHandler.RaiseEvent(OnStanceMagReload);
    }
    internal static void RaiseStanceTubeReload()
    {
        BaseEventHandler.RaiseEvent(OnStanceTubeReload);
    }
    internal static void RaiseStanceTopReload()
    {
        BaseEventHandler.RaiseEvent(OnStanceTopReload);
    }

    /// <summary>
    /// Chains from common lib reload events but allows StanceReloadSpeedSystem to handle timing
    /// </summary>
    internal static void RaiseStanceReload()
    {
        BaseEventHandler.RaiseEvent(OnStanceReload);
    }
}
