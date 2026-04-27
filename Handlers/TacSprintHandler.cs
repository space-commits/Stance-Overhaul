using EFT;
using StanceOverhaul.Handlers;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static RealismCommonLib.Plugin;
using static StanceOverhaul.Plugin;

namespace StanceOverhaul.Controllers.StateControllers
{
    public class TacSprintHandler : IControllerHelper
    {
        public const float TAC_SPRINT_WEIGHT_LIMIT = 5.1f;
        public const float TAC_SPRINT_WEIGHT_BULLPUP = 5.75f;
        public const int TAC_SPRINT_LENGTH_LIMIT = 6;
        public const float TAC_SPRINT_ERGO_LIMIT = 35f;
        public const float TAC_SPRINT_RADIATION_LIMIT = 50f;
        public const float TAC_SPRINT_TOXICITY_LIMIT = 50f;

        public bool HealthConditionPreventsTacSprint
        {
            get
            {
                return
                    HazardsStateInstance.TotalToxicity > TAC_SPRINT_TOXICITY_LIMIT ||
                    HazardsStateInstance.TotalRadiation > TAC_SPRINT_RADIATION_LIMIT ||
                    HazardsStateInstance.IsCoughingInGas ||
                    StanceControllerInstance.HealthStateAffectsStance;
            }
        }

        public void RunOnAwake() 
        {
        }

        public void RunOnDestroy()
        {
        }

        public void RunOnUpdate(float deltaTime)
        {
        }

        public bool IsDoingTacSprint = false;

        private float _tacSprintTime = 0.0f;
        private bool _canDoTacSprintTimer = false;

        //TODO: weight limit should be factored by strength skill
    /*    private bool CanDoTacSprint
        {
            get
            {
                return PluginConfig.EnableTacSprint.Value
                    && PlayerStateInstance.IsSprinting
                    && TargetStance != EStance.ActiveAiming
                    && (TargetStance == EStance.HighReady || StoredStance == EStance.HighReady)
                    && WeaponStateInstance.TotalWeaponWeight <= (WeaponStateInstance.IsBullpup ? TAC_SPRINT_WEIGHT_BULLPUP : TAC_SPRINT_WEIGHT_LIMIT)
                    && WeaponStateInstance.WeaponLength <= TAC_SPRINT_LENGTH_LIMIT && !PlayerStateInstance.IsScav
                    && !HealthConditionPreventsTacSprint
                    && WeaponStateInstance.TotalErgo > TAC_SPRINT_ERGO_LIMIT;
            }
        }*/
  /*      private void DoTacSprint(Player.FirearmController fc, Player player)
        {
            if (CanDoTacSprint)
            {
                IsDoingTacSprint = true;
                player.BodyAnimatorCommon.SetFloat(PlayerAnimator.WEAPON_SIZE_MODIFIER_PARAM_HASH, 2f);
                _tacSprintTime = 0f;
                _canDoTacSprintTimer = true;
            }
            else if (PluginConfig.EnableTacSprint.Value && _canDoTacSprintTimer)
            {
                _tacSprintTime += Time.deltaTime;
                if (_tacSprintTime >= 0.5f)
                {
                    player.BodyAnimatorCommon.SetFloat(PlayerAnimator.WEAPON_SIZE_MODIFIER_PARAM_HASH, WeaponStateInstance.WeaponLength);
                    _tacSprintTime = 0f;
                    _canDoTacSprintTimer = false;
                }
                IsDoingTacSprint = false;
            }
            else
            {
                IsDoingTacSprint = false;
            }
        }*/


    }
}
