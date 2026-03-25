using EFT;
using RealismCommonLib.ModifierHandlers;
using StanceOverhaul.Stances;
using System;
using System.Collections.Generic;
using System.Text;

namespace StanceOverhaul.Controllers.StateControllers;

public class StanceStaminaHandler : IControllerHelper
{
    public void RunOnAwake()
    {
    }

    public void RunOnDestroy()
    {
    }

    public void RunOnUpdate(float deltaTime)
    {
    }


    //TODO: replace all this with events and similar
    //Related events should trigger an update
    //this method makes baby Jesus cry

    /*    
     *            private EStance _lastRecordedStanceStamina = EStance.None; //used for stamina drate rate updates

     *    private bool _regenStam = false;
        private bool _drainStamStam = false;
        private bool _neutralStam = false;
        private bool _wasBracingStam = false;
        private bool _wasMountingStam = false;
        private bool _wasAimingStam = false;
        public bool HaveResetStamDrain = false;
        public bool CanResetAimDrain = false;*/

    /*  public void SetStanceStamina(Player player)
      {
          bool isInRegenableStance = TargetStance == EStance.HighReady || TargetStance == EStance.LowReady || TargetStance == EStance.PatrolStance || TargetStance == EStance.ShortStock || (IsIdle() && !PluginConfig.EnableIdleStamDrain.Value);
          bool isInRegenableState = (!player.Physical.HoldingBreath && (IsMounting || IsBracing)) || player.IsInPronePose || TargetStance == EStance.PistolCompressed || PlayerStateInstance.IsUsingStationaryWeapon;
          bool doRegen = ((isInRegenableStance && !AimStateInstance.IsAiming && !FiringStateInstance.IsFiringFromStance) || isInRegenableState) && !PlayerStateInstance.IsSprinting;
          bool shouldDoIdleDrain = IsIdle() && PluginConfig.EnableIdleStamDrain.Value;
          bool shouldInterruptRegen = isInRegenableStance && (AimStateInstance.IsAiming || FiringStateInstance.IsFiringFromStance);
          bool doNeutral = PlayerStateInstance.IsSprinting || player.IsInventoryOpened || (TargetStance == EStance.ActiveAiming && player.Pose == EPlayerPose.Duck);
          bool doDrain = ((shouldInterruptRegen || !isInRegenableStance || shouldDoIdleDrain) && !isInRegenableState && !doNeutral) || (IsDoingTacSprint && PluginConfig.EnableIdleStamDrain.Value);
          EStance stance = TargetStance;

          if (HaveResetStamDrain || DidWeaponSwap || AimStateInstance.IsAiming != _wasAimingStam || _regenStam != doRegen || _drainStamStam != doDrain || _neutralStam != doNeutral || _lastRecordedStanceStamina != TargetStance || IsMounting != _wasMountingStam || IsBracing != _wasBracingStam)
          {
              if (doDrain)
              {
                  player.Physical.Aim(1f);
              }
              else if (doRegen)
              {
                  player.Physical.Aim(0f);
              }
              else if (doNeutral)
              {
                  player.Physical.Aim(1f);
              }
              HaveResetStamDrain = false;
          }

          //drain
          if (doDrain)
          {
              player.Physical.HandsStamina.Multiplier = GetDrainRate(player);
          }
          //regen
          else if (doRegen)
          {
              player.Physical.HandsStamina.Multiplier = GetRestoreRate();
          }
          //no drain or regen
          else if (doNeutral)
          {
              player.Physical.HandsStamina.Multiplier = 0f;
          }

          _regenStam = doRegen;
          _drainStamStam = doDrain;
          _neutralStam = doNeutral;
          _wasBracingStam = IsBracing;
          _wasMountingStam = IsMounting;
          _wasAimingStam = AimStateInstance.IsAiming;
          _lastRecordedStanceStamina = TargetStance;
      }

      //TODO: refactor stamina system
      public void ResetStanceStamina()
      {
          _regenStam = false;
          _drainStamStam = false;
          _neutralStam = false;
          _wasBracingStam = false;
          _wasMountingStam = false;
          _wasAimingStam = false;
          _lastRecordedStanceStamina = EStance.None;
      }

      public void UnarmedStanceStamina(Player player)
      {
          player.Physical.Aim(0f);
          player.Physical.HandsStamina.Multiplier = 1f;
          ResetStanceStamina();
      }*/

    /*    private float GetRestoreRate()
        {
            float baseRestoreRate = 0f;
            if (IsMounting && WeaponStateInstance.BipodIsDeployed)
            {
                baseRestoreRate = 5f;
            }
            if (TargetStance == EStance.PatrolStance || IsMounting)
            {
                baseRestoreRate = 4f;
            }
            else if (TargetStance == EStance.LowReady || TargetStance == EStance.PistolCompressed || IsBracing)
            {
                baseRestoreRate = 2.4f;
            }
            else if (TargetStance == EStance.HighReady)
            {
                baseRestoreRate = 1.85f;
            }
            else if (TargetStance == EStance.ShortStock)
            {
                baseRestoreRate = 1.3f;
            }
            else if (IsIdle() && !PluginConfig.EnableIdleStamDrain.Value)
            {
                baseRestoreRate = 1f;
            }
            else
            {
                baseRestoreRate = 1f;
            }
            float formfactor = WeaponStateInstance.IsBullpup ? 1.05f : 1f;
            return (1f - ((WeaponStateInstance.ErgoFactor * formfactor) / 100f)) * baseRestoreRate * HealthStateInstance.HealthStamRegenFactor;
        }

        private float GetDrainRate(Player player)
        {
            float baseDrainRate = 0f;
            if (player.Physical.HoldingBreath)
            {
                baseDrainRate = IsMounting && WeaponStateInstance.BipodIsDeployed ? 0.025f : IsMounting ? 0.05f : IsBracing ? 0.1f : 0.5f;
            }
            else if (AimStateInstance.IsAiming)
            {
                baseDrainRate = 0.15f;
            }
            else if (IsDoingTacSprint)
            {
                baseDrainRate = 0.15f;
            }
            else if (TargetStance == EStance.ActiveAiming)
            {
                baseDrainRate = 0.075f;
            }
            else
            {
                baseDrainRate = 0.1f;
            }
            float formfactor = WeaponStateInstance.IsBullpup ? 0.4f : 1f;
            return WeaponStateInstance.ErgoFactor * formfactor * baseDrainRate * ((1f - HealthStateInstance.HealthStamRegenFactor) + 1f) * (1f - (SkillStateInstance.StrengthSkillAimBuff)) * PluginConfig.IdleStamDrainModi.Value;
        }*/
}