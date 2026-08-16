using EFT.Animations;
using System;
using System.Collections.Generic;
using System.Text;

namespace StanceOverhaul.SubSystem.Aiming
{
    internal class AimPIDHandler
    {

        /*    private void MoveGunToCameraPID(ProceduralWeaponAnimation pwa, float dt, float stanceMulti, ref float gunAxesTarget, ref float gunCameraAlignmentTarget, float camTargetAxes, float speedModifer, float tolerance = 0.001f, bool ignoreLeftShoulder = false)
            {
                if (!AimStateInstance.IsAiming)
                {
                    gunCameraAlignmentTarget = camTargetAxes;
                }

                if (IsColliding || PistolIsColliding || !pwa.OverlappingAllowsBlindfire || StopCameraMovement || (ShouldDoLeftShoulder && !ignoreLeftShoulder)) return;

                bool skipPIDForRifle = FiringStateInstance.IsFiringMovement && !PluginConfig.EnableAltRifleRecoil.Value && !TreatWeaponAsPistolStance;
                bool skipPIDForPistol = FiringStateInstance.IsFiringMovement && TreatWeaponAsPistolStance;
                if (AimStateInstance.IsAiming && !skipPIDForRifle && !skipPIDForPistol)
                {
                    float speed = speedModifer * stanceMulti;

                    // Calculate difference
                    float error = gunCameraAlignmentTarget - camTargetAxes;

                    if (Mathf.Abs(error) > tolerance)
                    {
                        // Convert error into a vertical offset
                        // (positive error = move weapon upward, negative = downward)
                        float adjustment = error * speed * dt;

                        gunAxesTarget += adjustment;
                    }
                }
            }

            private Vector3 GetRifleStancePIDModifier()
            {
                if (StoredStance == EStance.HighReady)
                    return new Vector3(0.6f, 0.35f, 1f);
                if (StoredStance == EStance.LowReady)
                    return new Vector3(0.8f, 0.7f, 1f);
                if (StoredStance == EStance.ShortStock)
                    return new Vector3(0.5f, 0.3f, 1f);
                if (StoredStance == EStance.ActiveAiming)
                    return new Vector3(1.5f, 0.75f, 1f);

                return Vector3.one;
            }*/


    }
}
