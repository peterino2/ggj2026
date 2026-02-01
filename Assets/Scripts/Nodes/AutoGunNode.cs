using UnityEngine;

namespace Nodes
{
    public class AutoGunNode : PowerNode
    {
        private AutoGun gun;
        public override void StartInner()
        {
            gun = SpriteController.GetPlayer().GetComponent<AutoGun>();
        }

        public override void OnPulse(float pulseStrength)
        {
            base.OnPulse(pulseStrength);
            gun.ApplyPulse(pulseStrength);
        }
    }
}