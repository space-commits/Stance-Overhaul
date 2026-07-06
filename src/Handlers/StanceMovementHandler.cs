using RealismCommonLib.ModifierHandlers;
using StanceOverhaul.Handlers;
using StanceOverhaul.Events;
using StanceOverhaul.Stances;
using Comfort.Common;
using static RealismCommonLib.Plugin;
using static StanceOverhaul.Plugin;

namespace StanceOverhaul.Controllers.StateControllers
{
    internal class StanceMovementHandler : IControllerHelper
    {
        private FloatMultiplierHandle _walkSpeed;
        private FloatMultiplierHandle _sprintSpeed;
        private FloatMultiplierHandle _preSprintAccelSpeed;
        private FloatMultiplierHandle _sprintAccelSpeed;

        public void RunOnAwake()
        {
            StanceEvents.OnStanceEntered += OnStanceEntered;
            StanceEvents.OnStanceExited += OnStanceExited;
    
            _walkSpeed = StatModifiers.WalkSpeedModifier.Add(1f);
            _sprintSpeed = StatModifiers.SprintSpeedModifier.Add(1f);
            _preSprintAccelSpeed = StatModifiers.PreSprintAccelModifier.Add(1f);
            _sprintAccelSpeed = StatModifiers.SprintAccelModifier.Add(1f);
        }

        public void RunOnDestroy()
        {
            StatModifiers.WalkSpeedModifier.Remove(_walkSpeed);
            StatModifiers.SprintSpeedModifier.Remove(_sprintSpeed);
            StatModifiers.PreSprintAccelModifier.Remove(_preSprintAccelSpeed);
            StatModifiers.SprintAccelModifier.Remove(_sprintAccelSpeed);
        }

        public void RunOnUpdate(float deltaTime)
        {
        }

        private void OnStanceEntered(IStance stance)
        {

            var physical = PlayerStateInstance.Player?.Physical;
            if (physical == null) return;

            BackendConfigSettingsClass.InertiaSettings inertia = Singleton<BackendConfigSettingsClass>.Instance.Inertia;

            //Todo: move to common lib
            physical.SprintAcceleration = inertia.SprintAccelerationLimits.InverseLerp(physical.Inertia);
            physical.PreSprintAcceleration = inertia.PreSprintAccelerationLimits.Evaluate(physical.Inertia);

            float tacSprintAccelBonus =  StanceControllerInstance.IsDoingTacSprint ? PluginConfig.TacSprintAccelBonus.Value : 1f;

            physical.SprintAcceleration *= stance.SprintAccelBonus * tacSprintAccelBonus;
            physical.PreSprintAcceleration *= stance.SprintAccelBonus * tacSprintAccelBonus;

            _walkSpeed.Multiplier = stance.WalkSpeedBonus;

            _sprintSpeed.Multiplier = StanceControllerInstance.IsDoingTacSprint ? PluginConfig.TacSprintSpeedBonus.Value : 1f;

            ModLogger.LogWarning("StanceControllerInstance.IsDoingTacSprint = " + StanceControllerInstance.IsDoingTacSprint);

            // _preSprintAccelSpeed.Multiplier = stance.SprintAccelBonus; //*  IsDoingTacSprint ? TAC_SPRINT_ACCEL : 1f;
            // _sprintAccelSpeed.Multiplier = stance.SprintAccelBonus; // *  IsDoingTacSprint ? TAC_SPRINT_ACCEL : 1f;

            //_sprintSpeed.Multiplier = IsDoingTacSprint ? TAC_SPRINT_SPEED_BONUS * (1f + PlayerStateInstance.Player.Skills.EnduranceBuffRestoration.Value) : 1f;
        }

        private void OnStanceExited()
        {
            var physical = PlayerStateInstance.Player?.Physical;
            if (physical == null) return;

            BackendConfigSettingsClass.InertiaSettings inertia = Singleton<BackendConfigSettingsClass>.Instance.Inertia;

            physical.SprintAcceleration = inertia.SprintAccelerationLimits.InverseLerp(physical.Inertia);
            physical.PreSprintAcceleration = inertia.PreSprintAccelerationLimits.Evaluate(physical.Inertia);

            _walkSpeed.Multiplier = 1f;
            _sprintSpeed.Multiplier = 1f;
            // _preSprintAccelSpeed.Multiplier = 1f;
            // _sprintAccelSpeed.Multiplier = 1f;
        }
    }
}
