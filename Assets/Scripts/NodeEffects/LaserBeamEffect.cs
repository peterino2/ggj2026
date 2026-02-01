using UnityEngine;

namespace DefaultNamespace.NodeEffects
{
    public class LaserBeamEffect : NodeEffect
    {
        private LaserGun gun;

        public override void StartEffect()
        {
            gun = SpriteController.GetPlayer().GetComponent<LaserGun>();
        }

        public override void OnPulse(float pulseStrength)
        {
            ForwardPulse(pulseStrength);
            gun.ApplyPulse(pulseStrength);
        }
    }
}
