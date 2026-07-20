using System;
using System.Collections.Generic;
using System.Text;

namespace StanceOverhaul.Stances
{
    internal class Mounting
    {
        //thanks and credit to lualeet's deadzone mod for this code, 0 jank compared to Realism's previous mounting system

        /*   
         *   
         *           //TODO: make logic support BSG mounting + realism mounting at the same time
              //Replace with Enum and treat as stance
              public bool IsMountings
              {
                  get
                  {
                      return _isRealismMounting;
                  }
                  set
                  {
                      if (value != _isRealismMounting)
                      {
                          Player player = PlayerStateInstance.Player;
                          FirearmController fc = player.HandsController as FirearmController;
                          if (fc == null)
                          {
                              value = false;
                              return;
                          }
                          _isRealismMounting = value;
                          if (player.ProceduralWeaponAnimation != null) player.ProceduralWeaponAnimation.method_23();
                          float accuracy = fc.Item.GetTotalCenterOfImpact(false); //forces accuracy to update
                          AccessTools.Field(typeof(Player.FirearmController), "float_3").SetValue(fc, accuracy); //update weapon accuracy
                          player.ProceduralWeaponAnimation.UpdateTacticalReload(); //gives better chamber animations
                          //this causes camera to detatch from weapon, breaks pretty badly
                          //it's needed to enable animation change (player grip changes), maybe there is a check for this anim state that caused the issue that can be disabled
                          //player.MovementContext.PlayerAnimator.SetProneBipodMount(player.MovementContext.IsInPronePose && WeaponStateInstance.BipodIsDeployed && value);
                          fc.FirearmsAnimator.SetMounted(value);
                          //player.ProceduralWeaponAnimation.SetMountingData(value, BracingDirection != EBracingDirection.Top);
                      }
                  }
              }
         *   
         *   private Quaternion _makeQuaternionDelta(Quaternion from, Quaternion to) => to * Quaternion.Inverse(from); //yeah I don't know what this is either
              private float _mountAimSmoothed = 0f;
              public float _cumulativeMountPitch = 0f;
              public float _cumulativeMountYaw = 0f;
              static Vector2 _lastMountYawPitch;
              public EBracingDirection BracingDirection = EBracingDirection.None;
              public bool IsBracing = false;
              public bool _isRealismMounting = false;
              public float BracingSwayBonus = 1f;
              public float BracingRecoilBonus = 1f;
      */


        /*        static void SetRotationWrapped(ref float yaw, ref float pitch)
                {
                    // I prefer using (-180; 180) euler angle range over (0; 360)
                    // However, wrapping the angles is easier with (0; 360), so temporarily cast it
                    if (yaw < 0) yaw += 360;
                    if (pitch < 0) pitch += 360;

                    pitch %= 360;
                    yaw %= 360;

                    // Now cast it back
                    if (yaw > 180) yaw -= 360;
                    if (pitch > 180) pitch -= 360;
                }

                public void SetRotationClamped(ref float yaw, ref float pitch, float maxAngle)
                {
                    Vector2 clampedVector
                        = Vector2.ClampMagnitude(
                            new Vector2(yaw, pitch),
                            maxAngle
                        );

                    yaw = clampedVector.x;
                    pitch = clampedVector.y;
                }

                public void UpdateAimSmoothed(ProceduralWeaponAnimation pwa, float deltaTime)
                {
                    _mountAimSmoothed = Mathf.Lerp(_mountAimSmoothed, pwa.IsAiming ? 1f : 0f, deltaTime * 6f);
                }

                public void UpdateMountRotation(Vector2 currentYawPitch, float clamp)
                {
                    Quaternion lastRotation = Quaternion.Euler(_lastMountYawPitch.x, _lastMountYawPitch.y, 0);
                    Quaternion currentRotation = Quaternion.Euler(currentYawPitch.x, currentYawPitch.y, 0);

                    _lastMountYawPitch = currentYawPitch;
                    lastRotation = Quaternion.SlerpUnclamped(currentRotation, lastRotation, 0.115f);

                    Vector3 delta = _makeQuaternionDelta(lastRotation, currentRotation).eulerAngles;

                    _cumulativeMountYaw += delta.x;
                    _cumulativeMountPitch += delta.y;

                    SetRotationWrapped(ref _cumulativeMountYaw, ref _cumulativeMountPitch);
                    SetRotationClamped(ref _cumulativeMountYaw, ref _cumulativeMountPitch, clamp);
                }

                public void ApplyPivotPoint(ProceduralWeaponAnimation pwa, Player player, float pivotPoint, float aimPivot)
                {
                    float aimMultiplier = 1f - ((1f - aimPivot) * _mountAimSmoothed);

                    Transform weaponRootAnim = pwa.HandsContainer.WeaponRootAnim;

                    if (weaponRootAnim == null) return;

                    weaponRootAnim.LocalRotateAround(Vector3.up * -pivotPoint, new Vector3( _cumulativeMountPitch * aimMultiplier, 0, _cumulativeMountYaw * aimMultiplier));

                    // Not doing this messes up pivot for all offsets after this
                    weaponRootAnim.LocalRotateAround(
                        Vector3.up * pivotPoint,
                        Vector3.zero
                    );
                }

                public void MountingPivotUpdate(Player player, ProceduralWeaponAnimation pwa, float clamp, float deltaTime, float pivotPoint = 0.75f, float aimPivot = 0.25f)
                {
                    Vector2 currentYawPitch = new(player.MovementContext.Yaw, player.MovementContext.Pitch);

                    UpdateMountRotation(currentYawPitch, clamp);
                    UpdateAimSmoothed(pwa, deltaTime);
                    ApplyPivotPoint(pwa, player, pivotPoint, aimPivot);
                }

                static readonly System.Diagnostics.Stopwatch aimWatch = new();

                public float GetDeltaTime()
                {
                    float deltaTime = aimWatch.Elapsed.Milliseconds / 1000f;
                    aimWatch.Reset();
                    aimWatch.Start();
                    return deltaTime;
                }

                public void ToggleMounting(Player player, ProceduralWeaponAnimation pwa, Player.FirearmController fc)
                {
                   *//* if (player.IsInPronePose && WeaponStateInstance.BipodIsDeployed)
                    {
                        IsMounting = true;
                    }*//*
                    if (IsMounting && PlayerStateInstance.IsMoving)
                    {
                        IsMounting = false;
                    }
                }*/
    }
}
