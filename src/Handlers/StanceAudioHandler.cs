using StanceOverhaul.Events;
using static RealismCommonLib.Plugin;

namespace StanceOverhaul.Handlers;

public class StanceAudioHandler : IControllerHelper
{
      public void RunOnAwake()
        {
            StanceEvents.OnStanceHitShoulder += OnShoulderHit;
        }

        public void RunOnDestroy()
        {
            StanceEvents.OnStanceHitShoulder -= OnShoulderHit;
        }

        public void RunOnUpdate(float deltaTime)
        {
        }

        private void OnShoulderHit()
        {
            AudioControllerInstance.PlayADSSound(PluginConfig.StanceSfxModifier.Value, false);
        }

}
