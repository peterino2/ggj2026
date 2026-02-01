using UnityEngine;

namespace Nodes
{
    public class RailGunNode : PowerNode
    {
        private Railgun gun;
        public override void StartInner()
        {
            gun = SpriteController.GetPlayer().GetComponent<Railgun>();
        }

        public override void OnPulse(float pulseStrength)
        {
            base.OnPulse(pulseStrength);
            gun.ApplyPulse(pulseStrength);
        }
    }
}