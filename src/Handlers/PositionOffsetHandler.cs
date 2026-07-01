using EFT.Animations;
using EFT;
using System;
using System.Collections.Generic;
using System.Text;

namespace StanceOverhaul.Handlers
{
    internal class PositionOffsetHandler
    {
      /*  //I've no idea wtf is going on here but it sort of works
        private void HandleAltPistolPosition(Player player, Player.FirearmController fc, ProceduralWeaponAnimation pwa, float stanceMulti, float dt, Vector3 camTarget)
        {
            //left stance speed
            float leftResetSpeedModi = _isLeftStanceResetState ? 0.2f : 1f;

            //speed
            float fpsFactor = Mathf.Pow(RealismCommonLib.Plugin.FPSFactor, 0.25f);
            float speedFactorTarget = AimStateInstance.IsAiming ? PluginConfig.PistolPosResetSpeedMulti.Value * stanceMulti : PluginConfig.PistolPosSpeedMulti.Value * stanceMulti;
            float pidSpeed = fpsFactor * leftResetSpeedModi * PluginConfig.PistolPosResetSpeedMulti.Value;
            _pistolPosSpeed = Mathf.Lerp(_pistolPosSpeed, speedFactorTarget, dt * 10f);

            if (!AimStateInstance.IsAiming)
            {
                _gunXTarget = !IsBlindFiring ? 0.038f : 0f;
                _gunYTarget = -0.0385f;
                _gunZTarget = 0f;
            }

            DoLeftShoulder(player, fc, pwa, _pistolPosSpeed, dt, _leftStancePistolPositionTarget, _leftStancePistolRotaitonTarget, stanceMulti * 2.5f, 0.05f);

            if (RealismCommonLib.Plugin.FOVFixEnabled)
            {
                MoveGunToCameraPID(pwa, dt, stanceMulti, ref _gunXTarget, ref _gunCameraAlignmentTargetX, camTarget.x, 0.15f * pidSpeed, 0.0001f);
                MoveGunToCameraPID(pwa, dt, stanceMulti, ref _gunYTarget, ref _gunCameraAlignmentTargetY, camTarget.y, 0.3f * pidSpeed, ignoreLeftShoulder: true);
                MoveGunToCameraPID(pwa, dt, stanceMulti, ref _gunZTarget, ref _gunCameraAlignmentTargetZ, camTarget.z, 0.4f * pidSpeed, ignoreLeftShoulder: true);
            }

            _currentPistolXPos = Mathf.Lerp(_currentPistolXPos, _gunXTarget, dt * _pistolPosSpeed);
            _currentPistolYPos = Mathf.Lerp(_currentPistolYPos, _gunYTarget, dt * _pistolPosSpeed);
            _currentPistolZPos = Mathf.Lerp(_currentPistolZPos, _gunZTarget, dt * _pistolPosSpeed);

            _pistolLocalPosition.x = _currentPistolXPos + _leftStancePosition.x;
            _pistolLocalPosition.y = _currentPistolYPos + _leftStancePosition.y;
            _pistolLocalPosition.z = _currentPistolZPos + _leftStancePosition.z;

            pwa.HandsContainer.WeaponRoot.localPosition = _pistolLocalPosition;

        }

        private void HandleRiflePosition(Player player, Player.FirearmController fc, ProceduralWeaponAnimation pwa, float stanceMulti, float movementFactor, float dt, Vector3 camTarget)
        {
            //left stance speeds
            float leftResetPidModi = _isLeftStanceResetState ? 0f : 1f;

            //speeds
            float fpsFactor = Mathf.Pow(RealismCommonLib.Plugin.FPSFactor, 0.25f);   
            float posSpeed = AimStateInstance.IsAiming ? 30f * WeaponStateInstance.TotalFinalAimSpeed : 6f * WeaponStateInstance.TotalFinalAimSpeed;
            float pidSpeed = 30f * fpsFactor * leftResetPidModi;
            Vector3 stanceModifer = GetRifleStancePIDModifier();

            bool isCantedAiming = IsCantedAiming(pwa, false);
            bool adjustSpeedForCant = isCantedAiming && StoredStance == EStance.ActiveAiming;

            if (!AimStateInstance.IsAiming) 
            {
                _gunXTarget = BaseWeaponOffsetPosition.x + PluginConfig.WeapOffset.Value.x;
                _gunYTarget = BaseWeaponOffsetPosition.y + PluginConfig.WeapOffset.Value.y;
                _gunZTarget = BaseWeaponOffsetPosition.z + PluginConfig.WeapOffset.Value.z;
            }

            DoLeftShoulder(player, fc, pwa, stanceMulti, dt, _leftStanceRiflePositionTarget, _leftStanceRifleRotaitonTarget, stanceMulti * 4.5f);

            if (PluginConfig.EnableAltRifle.Value && RealismCommonLib.Plugin.FOVFixEnabled)
            {
                MoveGunToCameraPID(pwa, dt, WeaponStateInstance.TotalFinalAimSpeed, ref _gunXTarget, ref _gunCameraAlignmentTargetX, camTarget.x, 0.3f * pidSpeed * stanceModifer.x, 0.0001f);
                MoveGunToCameraPID(pwa, dt, WeaponStateInstance.TotalFinalAimSpeed, ref _gunYTarget, ref _gunCameraAlignmentTargetY, camTarget.y, 0.3f * pidSpeed * stanceModifer.y, 0.0001f, true);
                MoveGunToCameraPID(pwa, dt, WeaponStateInstance.TotalFinalAimSpeed, ref _gunZTarget, ref _gunCameraAlignmentTargetZ, camTarget.z, 0.3f * pidSpeed * stanceModifer.z, 0.0001f, true);
            }
          
            _currentRifleXPos = Mathf.Lerp(_currentRifleXPos, _gunXTarget, dt * posSpeed);
            _currentRifleYPos = Mathf.Lerp(_currentRifleYPos, _gunYTarget, dt * posSpeed); //if trying to fix stance ADS, animspeed might be fucking with things
            _currentRifleZPos = Mathf.Lerp(_currentRifleZPos, _gunZTarget, dt * posSpeed);

            _rifleLocalPosition.x = _currentRifleXPos + _leftStancePosition.x;
            _rifleLocalPosition.y = _currentRifleYPos + _leftStancePosition.y;
            _rifleLocalPosition.z = _currentRifleZPos + _leftStancePosition.z;

            pwa.HandsContainer.WeaponRoot.localPosition = _rifleLocalPosition;
        }*/
    }
}
