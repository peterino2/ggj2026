using UnityEngine;

namespace Nodes
{
    public class LaserBeamNode : PowerNode
    {
        private LaserGun gun;
        public override void StartInner()
        {
            gun = SpriteController.GetPlayer().GetComponent<LaserGun>();
        }

        public override void OnPulse(float pulseStrength)
        {
            base.OnPulse(pulseStrength);
            gun.ApplyPulse(pulseStrength);
        }
    }
}