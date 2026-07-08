
using StanceOverhaul.Handlers;
using StanceOverhaul.Enums;
using UnityEngine;
using EFT;
using StanceOverhaul.Events;
using StanceOverhaul.Stances;
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

        public bool IsDoingTacSprint
        {
            get { return _isDoingTacSprint; }
            private set
            {
                if (_isDoingTacSprint != value)
                {
                    ModLogger.LogWarning($"IsDoingTacSprint changed to {value}");
                    _isDoingTacSprint = value;
                    if (_isDoingTacSprint)
                        StanceEvents.RaiseOnTacSprintStarted();
                    else
                        StanceEvents.RaiseOnTacSprintEnded();
                }
            }
        }

        private bool _isDoingTacSprint = false;

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

        //TODO: weight limit should be factored by strength skill
        private bool CanDoTacSprint(IStance? stance = null)
        {
            return PluginConfig.EnableTacSprint.Value
                     && (stance?.CanDoTacSprint ?? false)
                     && WeaponMeetsCriteria
                     && !HealthConditionPreventsTacSprint;
        }

        private bool WeaponMeetsCriteria
        {
            get
            {
                return WeaponStateInstance.TotalWeaponWeight <= (WeaponStateInstance.IsBullpup ? TAC_SPRINT_WEIGHT_BULLPUP : TAC_SPRINT_WEIGHT_LIMIT)
                    && WeaponStateInstance.WeaponLength <= TAC_SPRINT_LENGTH_LIMIT && !PlayerStateInstance.IsScav
                    && !HealthConditionPreventsTacSprint
                    && WeaponStateInstance.TotalErgo > TAC_SPRINT_ERGO_LIMIT;
            }
        }


        public void RunOnAwake()
        {
            StanceEvents.OnStanceEntered += OnStanceEntered;
            StanceEvents.OnStanceExited += OnStanceExited;
        }

        public void RunOnDestroy()
        {
            StanceEvents.OnStanceEntered -= OnStanceEntered;
            StanceEvents.OnStanceExited -= OnStanceExited;
        }

        public void OnStanceEntered(IStance stance)
        {
            ChangeTacSprintState(stance);
        }

        public void OnStanceExited()
        {
            ChangeTacSprintState(null);
        }

        private void ChangeTacSprintState(IStance? stance)
        {
            if (CanDoTacSprint(stance) && PlayerStateInstance.Player.BodyAnimatorCommon.GetFloat(PlayerAnimator.WEAPON_SIZE_MODIFIER_PARAM_HASH) != 2f)
                PlayerStateInstance.Player.BodyAnimatorCommon.SetFloat(PlayerAnimator.WEAPON_SIZE_MODIFIER_PARAM_HASH, 2f);
            else if (PlayerStateInstance.Player.BodyAnimatorCommon.GetFloat(PlayerAnimator.WEAPON_SIZE_MODIFIER_PARAM_HASH) != WeaponStateInstance.WeaponLength)
                PlayerStateInstance.Player.BodyAnimatorCommon.SetFloat(PlayerAnimator.WEAPON_SIZE_MODIFIER_PARAM_HASH, WeaponStateInstance.WeaponLength);
        }

        public void RunOnUpdate(float deltaTime)
        {

            //We were doing tac sprint, but a condition changed to block it like health state blocking it
            //Need to change tac sprint state
            if (IsDoingTacSprint && !CanDoTacSprint(StanceControllerInstance.CurrentStance))
            {
                ModLogger.LogWarning("change tac sprint state");
                ChangeTacSprintState(null);
                IsDoingTacSprint = false; 
            }

            if (!IsDoingTacSprint && CanDoTacSprint(StanceControllerInstance.CurrentStance) && PlayerStateInstance.IsSprinting)
            {
                ModLogger.LogWarning("entered tac sprint");
                IsDoingTacSprint = true;
            }
            else if (!CanDoTacSprint(StanceControllerInstance.CurrentStance) || !PlayerStateInstance.IsSprinting)
            {
                IsDoingTacSprint = false; // the setter has a guard to only raise events if the value changes, so this is safe to call every frame
            }
        }
    }
}
