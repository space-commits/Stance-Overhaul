using EFT.Animations;
using EFT;
using StanceOverhaul.SpringAnimators;
using StanceOverhaul.Stances;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StanceOverhaul.Controllers.StanceControllers
{
    internal class ExtraDetailsAnimator : ISpringAnimator
    {

    /*    private Vector3 _posePosOffest = Vector3.zero;
        private Vector3 _poseRotOffest = Vector3.zero;
        private Quaternion _poseQuatOffset = Quaternion.identity;


        public bool ShouldDoFaceGearCantedOffset()
        {
            return (GearStateInstance.HasGasMask || (GearStateInstance.FaceShieldIsActive && GearStateInstance.GearBlocksMouth));
        }

        //non-stance related rotational and postion changes for immersion
        public void DoExtraPosAndRot(ProceduralWeaponAnimation pwa, Player player)
        {
            //position
            float stockOffset = !WeaponStateInstance.IsPistol && !WeaponStateInstance.HasShoulderContact ? -0.04f : 0f;
            float stockPosOffset = WeaponStateInstance.StockPosition * 0.01f;
            float posOffsetMulti = WeaponStateInstance.HasShoulderContact ? -0.04f : 0.04f;
            float posePosOffset = (1f - player.MovementContext.PoseLevel) * posOffsetMulti;

            float targetPosXOffset = pwa.IsAiming ? 0f : 0f;
            float targetPosYOffset = pwa.IsAiming ? 0f : 0f;
            float targetPosZOffset = pwa.IsAiming ? 0f : Mathf.Clamp(posePosOffset + stockOffset + stockPosOffset, -0.05f, 0.05f);
            Vector3 targetPos = new Vector3(targetPosXOffset, targetPosYOffset, targetPosZOffset);

            _posePosOffest = Vector3.Lerp(_posePosOffest, targetPos, 5f * Time.deltaTime);

            //rotation
            bool isMountedWithBipod = WeaponStateInstance.BipodIsDeployed && IsMounting;
            bool doCantedSightOffset = IsCantedAiming(pwa, true);
            bool doMaskOffset =
                !doCantedSightOffset &&
                !isMountedWithBipod &&
                ShouldDoFaceGearCantedOffset() &&
                !WeaponStateInstance.WeaponCanFSADS &&
                pwa.IsAiming &&
                WeaponStateInstance.HasShoulderContact &&
                WeaponStateInstance.TreatAsPistol;
            bool doLongMagOffset = WeaponStateInstance.HasLongMag && player.IsInPronePose && !isMountedWithBipod;
            float cantedOffsetBase = -0.41f;
            float magOffset = doCantedSightOffset ? 0f : doLongMagOffset && !pwa.IsAiming ? -0.35f : doLongMagOffset && pwa.IsAiming ? -0.12f : 0f;
            float ergoOffset = WeaponStateInstance.ErgoFactor * -0.001f;
            float poseRotOffset = (1f - player.MovementContext.PoseLevel) * -0.03f;
            poseRotOffset += player.IsInPronePose ? -0.03f : 0f;
            float maskFactor = doMaskOffset ? -0.025f + ergoOffset : 0f;
            float baseRotOffset = pwa.IsAiming || IsMounting || IsBracing ? 0f : poseRotOffset + ergoOffset;
            float cantedSightOffset = doCantedSightOffset ? cantedOffsetBase : 0f;

            float rotX = 0f;
            float rotY = Mathf.Clamp(baseRotOffset + maskFactor + magOffset, -0.5f, 0f) + cantedSightOffset;
            float rotZ = 0f;
            Vector3 targetRot = new Vector3(rotX, rotY, rotZ);

            _poseRotOffest = Vector3.Lerp(_poseRotOffest, targetRot, 5f * Time.deltaTime); //speeds should be affected by stance multi? or player crouch speed?

            Quaternion newRot = Quaternion.identity;
            newRot.x = _poseRotOffest.x;
            newRot.y = _poseRotOffest.y;
            newRot.z = _poseRotOffest.z;
            _poseQuatOffset = newRot;
        }*/
    }
}
