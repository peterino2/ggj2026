using UnityEngine;

namespace DefaultNamespace.NodeEffects
{
    public class RailgunEffect : NodeEffect
    {
        private Railgun gun;

        public override void StartEffect()
        {
            gun = SpriteController.GetPlayer().GetComponent<Railgun>();
        }

        public override void OnPulse(float pulseStrength)
        {
            ForwardPulse(pulseStrength);
            gun.ApplyPulse(pulseStrength);
        }
    }
}
