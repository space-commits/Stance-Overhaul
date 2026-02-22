using EFT;
using RealismCommonLib.ModifierHandlers;
using RealismCommonLib.StateControllers;
using StanceOverhaul.Enums;
using static RealismCommonLib.Plugin;

namespace StanceOverhaul.Controllers.StateControllers
{
    public class StanceMovementState : IStateController
    {
        private const float PATROL_STANCE_WALK_SPEED = 1.33f;
        private const float LOW_READY_WALK_SPEED = 1.15f;
        private const float HIGH_READY_WALK_SPEED = 1.06f;
        private const float SHORT_STOCK_WALK_SPEED = 0.94f;
        private const float PATROL_STANCE_SPRINT_ACCEL = 1.45f;
        private const float SHORT_STOCK_SPRINT_ACCEL = 0.9f;
        private const float LOW_READY_SPRINT_ACCEL = 1.25f;
        private const float TAC_SPRINT_ACCEL = 1.37f;
        private const float HIGH_READY_SPRINT_ACCEL = 1.2f;
        private const float TAC_SPRINT_SPEED_BONUS = 1.15f;


        private StanceController _stanceController;

        private FloatMultiplierHandle _walkSpeed;
        private FloatMultiplierHandle _sprintSpeed;
        private FloatMultiplierHandle _preSprintAccelSpeed;
        private FloatMultiplierHandle _sprintAccelSpeed;

        public StanceMovementState(StanceController stanceController)
        {
            _stanceController = stanceController;
        }

        public void RunOnAwake()
        {
            _walkSpeed = StatModifiers.MaxWalkSpeedModifier.Add(1f);
            _sprintSpeed = StatModifiers.MaxSprintSpeedModifier.Add(1f);
            _preSprintAccelSpeed = StatModifiers.PreSprintAccelModifier.Add(1f);
            _sprintAccelSpeed = StatModifiers.SprintAccelModifier.Add(1f);
        }

        public void RunOnDestroy()
        {
            StatModifiers.MaxWalkSpeedModifier.Remove(_walkSpeed);
            StatModifiers.MaxSprintSpeedModifier.Remove(_sprintSpeed);
            StatModifiers.PreSprintAccelModifier.Remove(_preSprintAccelSpeed);
            StatModifiers.SprintAccelModifier.Remove(_sprintAccelSpeed);
        }

        public void RunOnUpdate()
        {
            UpdateWalkSpeed();
            UpdateSprintSpeed();
        }

        private void UpdateWalkSpeed()
        {
            float stanceFactor = GetStanceWalkSpeedFactor(_stanceController.TargetStance);
            _walkSpeed.Multiplier = stanceFactor;
        }

        private float GetStanceWalkSpeedFactor(EStance stance)
        {
            switch (stance)
            {
                case EStance.PatrolStance:
                    return PATROL_STANCE_WALK_SPEED;
                case EStance.LowReady:
                    return LOW_READY_WALK_SPEED;
                case EStance.HighReady:
                    return HIGH_READY_WALK_SPEED;
                case EStance.ShortStock:
                    return SHORT_STOCK_WALK_SPEED;
                default:
                    return 1f;
            }
        }

        private void UpdateSprintSpeed()
        {
            float stanceSpeedBonus =
                _stanceController.IsDoingTacSprint ? TAC_SPRINT_SPEED_BONUS * (1f + PlayerStateInstance.Player.Skills.EnduranceBuffRestoration.Value)
                : 1f;
            _sprintSpeed.Multiplier = stanceSpeedBonus;

            float stanceAccelBonus = GetStanceSprintAccelBonus(_stanceController.TargetStance, _stanceController.IsDoingTacSprint);
            _preSprintAccelSpeed.Multiplier = stanceAccelBonus;
            _sprintAccelSpeed.Multiplier = stanceAccelBonus;
        }

        private float GetStanceSprintAccelBonus(EStance stance, bool isTacSprint)
        {
            if (stance == EStance.PatrolStance)
                return PATROL_STANCE_SPRINT_ACCEL;
            if (stance == EStance.ShortStock)
                return SHORT_STOCK_SPRINT_ACCEL;
            if (stance == EStance.LowReady)
                return LOW_READY_SPRINT_ACCEL;
            if (isTacSprint)
                return TAC_SPRINT_ACCEL;
            if (stance == EStance.HighReady)
                return HIGH_READY_SPRINT_ACCEL;
            return 1f;
        }
    }
}
