using UnityEngine;

namespace DefaultNamespace.NodeEffects
{
    public class MagnetEffect : NodeEffect
    {
        private Magnet gun;

        public override void StartEffect()
        {
            gun = SpriteController.GetPlayer().GetComponent<Magnet>();
        }

        public override void OnPulse(float pulseStrength)
        {
            ForwardPulse(pulseStrength);
            gun.ApplyPulse(pulseStrength * parentNode.NodePower);
        }
    }
}
