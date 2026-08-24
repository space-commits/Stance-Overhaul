using StanceOverhaul.Enums;
using StanceOverhaul.Events;
using StanceOverhaul.Stances;
using UnityEngine;
using RealismCommonLib.Events;
using RealismCommonLib.ModifierHandlers;
using static RealismCommonLib.Plugin;
using static StanceOverhaul.Plugin;

namespace StanceOverhaul.SubSystem;

public class StanceReloadSpeedSystem : ISubSystem
{
    private FloatMultiplierHandle _magReload;
    private FloatMultiplierHandle _checkAmmo;
    private FloatMultiplierHandle _checkChamber;
    private FloatMultiplierHandle _pumpBolt;
    private FloatMultiplierHandle _malfFix;
    private FloatMultiplierHandle _rechamber;
    private FloatMultiplierHandle _internalMagReload;
    private FloatMultiplierHandle _revolverReload;
    private EStanceReloadType _stanceReloadType = EStanceReloadType.None;

    public bool IsInReloadOperation
    {
        get
        {
            return _stanceReloadType != EStanceReloadType.None;
        }
    }

    public void RunOnAwake()
    {
        if (!PluginConfig.EnableReloadSpeedChanges.Value) return;
        ReloadEvents.WeaponStateReset += OnWeaponStateReset;
        ReloadEvents.CheckAmmo += OnCheckAmmo;
        ReloadEvents.ChamberCheck += OnChamberCheck;
        ReloadEvents.MalfFix += OnMalfFix;
        ReloadEvents.Rechamber += OnRechamber;
        ReloadEvents.MagReload += OnMagReload;
        ReloadEvents.QuickMagReload += OnQuickMagReload;
        ReloadEvents.InternalMagReload += OnInternalMagReload;
        ReloadEvents.BoltActionPump += PumpBolt;
        
        AssignReloadHandlers();
    }

    public void RunOnDestroy()
    {
        if (!PluginConfig.EnableReloadSpeedChanges.Value) return;
        ReloadEvents.WeaponStateReset -= OnWeaponStateReset;
        ReloadEvents.CheckAmmo -= OnCheckAmmo;
        ReloadEvents.ChamberCheck -= OnChamberCheck;
        ReloadEvents.MalfFix -= OnMalfFix;
        ReloadEvents.Rechamber -= OnRechamber;
        ReloadEvents.MagReload -= OnMagReload;
        ReloadEvents.QuickMagReload -= OnQuickMagReload;
        ReloadEvents.InternalMagReload -= OnInternalMagReload;
        ReloadEvents.BoltActionPump -= PumpBolt;

        UnassignReloadHandlers();
    }

    private void AssignReloadHandlers()
    {
        _magReload = StatModifiers.MagReloadSpeed.Add(1f);
        _checkAmmo = StatModifiers.CheckAmmoSpeed.Add(1f);
        _checkChamber = StatModifiers.CheckChamberSpeed.Add(1f);
        _pumpBolt = StatModifiers.PumpBoltSpeed.Add(1f);
        _malfFix = StatModifiers.MalfFixSpeed.Add(1f);
        _rechamber = StatModifiers.RechamberSpeed.Add(1f);
        _revolverReload = StatModifiers.RevolverReloadModifier.Add(1f);
        _internalMagReload = StatModifiers.InternalReloadModifier.Add(1f);
    }

    private void UnassignReloadHandlers()
    {
        StatModifiers.MagReloadSpeed.Remove(_magReload);
        StatModifiers.CheckAmmoSpeed.Remove(_checkAmmo);
        StatModifiers.CheckChamberSpeed.Remove(_checkChamber);
        StatModifiers.PumpBoltSpeed.Remove(_pumpBolt);
        StatModifiers.MalfFixSpeed.Remove(_malfFix);
        StatModifiers.RechamberSpeed.Remove(_rechamber);
        StatModifiers.InternalReloadModifier.Remove(_internalMagReload);
        StatModifiers.RevolverReloadModifier.Remove(_revolverReload);
    }


    public void RunOnUpdate(float deltaTime)
    {
    }

    private void OnInternalMagReload()
    {
        ApplyInternalReloadSpeedBonus();
    }

    private void OnMagReload()
    {
        _stanceReloadType = EStanceReloadType.Magazine;
        ApplyMagReloadSpeedBonuses();
        ModLogger.LogWarning($"OnMagReload: stance={StanceControllerInstance?.CurrentStance?.StanceType}, reloadType={_stanceReloadType}");
    }

    private void OnQuickMagReload()
    {
        OnMagReload();
    }

    private void ApplyMagReloadSpeedBonuses()
    {
        _magReload.Multiplier = StanceControllerInstance.CurrentStance?.MagazineReloadSpeedModifier ?? 1f;
    }

    private void OnWeaponStateReset()
    {
        _stanceReloadType = EStanceReloadType.None;
        _checkAmmo.Multiplier = 1f;
        _checkChamber.Multiplier = 1f;
        _rechamber.Multiplier = 1f;
        _malfFix.Multiplier = 1f;
        _magReload.Multiplier = 1f;
        _internalMagReload.Multiplier = 1f;
        _pumpBolt.Multiplier = 1f;
        _revolverReload.Multiplier = 1f;
    }

    private void OnCheckAmmo()
    {
        _stanceReloadType = EStanceReloadType.CheckAmmo;
        OnWeaponManip();
    }

    private void OnChamberCheck()
    {
        _stanceReloadType = EStanceReloadType.CheckChamber;
        OnWeaponManip();
    }

    private void OnRechamber()
    {
        _stanceReloadType = EStanceReloadType.Rechamber;
        OnWeaponManip();
    }

    private void OnMalfFix()
    {
        _stanceReloadType = EStanceReloadType.ClearMalfunction;
        OnWeaponManip();
    }

    private void OnWeaponManip()
    {
        var bonus = StanceControllerInstance.CurrentStance?.WeaponManipSpeedModifier ?? 1f;
        _checkAmmo.Multiplier = bonus;
        _checkChamber.Multiplier = bonus;
        _rechamber.Multiplier = bonus;
        _malfFix.Multiplier = bonus;
    }


    private void ApplyInternalReloadSpeedBonus()
    {
        float bonus = 1f;

        if (ReloadStateInstance.IsAttemptingRevolverReload)
        {
            _stanceReloadType = EStanceReloadType.Revolver;
            _revolverReload.Multiplier = StanceControllerInstance.CurrentStance?.RevolverReloadSpeedModifier ?? 1f;
            return;
        }
        else if (WeaponStateInstance.IsShotgun)
        {
            _stanceReloadType = EStanceReloadType.Tube;
            bonus = StanceControllerInstance.CurrentStance?.TubeReloadSpeedModifier ?? 1f;
        }
        else
        {
            _stanceReloadType = EStanceReloadType.Top;
            bonus = StanceControllerInstance.CurrentStance?.TopReloadSpeedModifier ?? 1f;
        }

        _internalMagReload.Multiplier = bonus;
    }

    private void PumpBolt()
    {
        _stanceReloadType = EStanceReloadType.PumpBolt;
        _pumpBolt.Multiplier = StanceControllerInstance.CurrentStance?.PumpBoltSpeedModifier ?? 1f;
        ModLogger.LogWarning($"PumpBolt: stance={StanceControllerInstance?.CurrentStance?.StanceType}, reloadType={_stanceReloadType}");
    }
}