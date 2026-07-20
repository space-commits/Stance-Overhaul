using StanceOverhaul.Handlers;
using UnityEngine;
using EFT;
using static RealismCommonLib.Plugin;

namespace StanceOverhaul.Controllers.StateControllers;

//TODO: factor in balance
public class StatsHandler : IControllerHelper
{
    private const float MinimumWeaponWeight = 0.8f;
    private const float MaximumWeaponWeight = 13.0f;

    // Maximum reduction ergonomics can provide
    // 0.50 = 50% reduction of excess weight
    private const float MaximumErgoReduction = 0.50f;

    // Controls diminishing returns
    // 1.0 = linear
    // >1.0 = stronger diminishing returns
    private const float ErgoExponent = 2f;

    private const float WeightExponent = 1.35f;

    private float _effectiveWeaponWeight;

    /// <summary>
    /// Weight of the weapon after ergonomics is applied as a reducing factor.
    /// </summary>
    public float EffectiveWeaponWeight
    {
        get
        {
            return _effectiveWeaponWeight;
        }
        private set
        {
            _effectiveWeaponWeight = value;
        }
    }

    public void RunOnAwake()
    {
        WeaponStateInstance.OnWeaponStateChangedFc += UpdateStats;
    }

    public void RunOnDestroy()
    {
        WeaponStateInstance.OnWeaponStateChangedFc -= UpdateStats;
    }

    public void RunOnUpdate(float deltaTime)
    {
    }

    private void UpdateStats(Player.FirearmController fc)
    {
        CalculateEffectiveWeight();
    }

    public void CalculateEffectiveWeight()
    {
        var ergonomics = WeaponStateInstance.TotalErgo;
        var weight = WeaponStateInstance.TotalWeaponWeight;

        // Convert ergonomics into 0-1 range
        float ergoNormalized = Mathf.Clamp(ergonomics / 100f, 0f, 1f);


        // Convert ergonomics into weight reduction
        //
        // 0 ergo    = 0% reduction
        // 100 ergo  = MaximumErgoReduction
        //
        float ergoReduction = MaximumErgoReduction * (1f - Mathf.Pow(1f - ergoNormalized, ErgoExponent));


        // Only reduce weight above the minimum possible weapon weight
        //
        // Example:
        // 8kg weapon:
        // excess weight = 6kg
        // 50% reduction = remove 3kg
        //
        float excessWeight =
            Mathf.Max(
                weight - MinimumWeaponWeight,
                0f
            );


        float effectiveWeight =
            MinimumWeaponWeight +
            excessWeight * (1f - ergoReduction);


        EffectiveWeaponWeight = effectiveWeight;
    }

    public float GetDrainRate(float baseDrainRate)
    {
        var weightExp = Mathf.Pow(EffectiveWeaponWeight, WeightExponent);
        return baseDrainRate + weightExp * 0.15f;
    }

    public float GetRegenerationRate(float baseRegenRate)
    {
        var weightExp = Mathf.Pow(EffectiveWeaponWeight, WeightExponent);
        return baseRegenRate / (1 + weightExp * 0.15f);
    }

    public float GetStanceSpeedModifier(float baseStanceSpeed)
    {
        var weightExp = Mathf.Pow(EffectiveWeaponWeight, WeightExponent);
        return baseStanceSpeed / (1 + weightExp * 0.25f);
    }
}