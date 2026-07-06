
using StanceOverhaul.Handlers;
using StanceOverhaul.Enums;
using UnityEngine;
using EFT;
using static RealismCommonLib.Plugin;
using static StanceOverhaul.Plugin;
using StanceOverhaul.State;

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

        public bool IsDoingTacSprint { get; private set; }
        private float _tacSprintTime = 0.0f;
        private bool _canDoTacSprintTimer = false;

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
        private bool CanDoTacSprint
        {
            get
            {
                return PluginConfig.EnableTacSprint.Value
                    && PlayerStateInstance.IsSprinting
                    && StanceControllerInstance.CurrentStanceType == EStanceType.HighReady;
            }
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
        }

        public void RunOnDestroy()
        {
        }

        public void RunOnUpdate(float deltaTime)
        {
            if (CanDoTacSprint && WeaponMeetsCriteria && !HealthConditionPreventsTacSprint)
            {

                PlayerStateInstance.Player.BodyAnimatorCommon.SetFloat(PlayerAnimator.WEAPON_SIZE_MODIFIER_PARAM_HASH, 2f);
                _tacSprintTime = 0f;
                _canDoTacSprintTimer = true;

                IsDoingTacSprint = true;
                return;
            }
            else if (_canDoTacSprintTimer)
            {
                _tacSprintTime += Time.deltaTime;
                if (_tacSprintTime >= 0.5f)
                {
                    PlayerStateInstance.Player.BodyAnimatorCommon.SetFloat(PlayerAnimator.WEAPON_SIZE_MODIFIER_PARAM_HASH, WeaponStateInstance.WeaponLength);
                    _tacSprintTime = 0f;
                    _canDoTacSprintTimer = false;
                }
            }

            IsDoingTacSprint = false;
        }
    }
}
