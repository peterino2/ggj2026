using UnityEngine;

namespace DefaultNamespace.NodeEffects
{
    public class AutoGunEffect : NodeEffect
    {
        private AutoGun gun;

        public override void StartEffect()
        {
            gun = SpriteController.GetPlayer().GetComponent<AutoGun>();
        }

        public override void OnPulse(float pulseStrength)
        {
            ForwardPulse(pulseStrength);
            gun.ApplyPulse(pulseStrength * parentNode.NodePower);
        }
    }
}
