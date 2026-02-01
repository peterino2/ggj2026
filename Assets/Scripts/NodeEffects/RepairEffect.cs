using UnityEngine;

namespace DefaultNamespace.NodeEffects
{
    public class RepairEffect : NodeEffect
    {
        public override void OnPulse(float pulseStrength)
        {
            SpriteController.gPlayer.takeHealing(pulseStrength * parentNode.NodePower);
        }
    }
}
