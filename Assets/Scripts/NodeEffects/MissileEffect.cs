using UnityEngine;

namespace DefaultNamespace.NodeEffects
{
    public class MissileEffect : NodeEffect
    {
        private MissileLauncher launcher;

        public override void StartEffect()
        {
            launcher = SpriteController.GetPlayer().GetComponent<MissileLauncher>();
        }

        public override void OnPulse(float pulseStrength)
        {
            ForwardPulse(pulseStrength);
            launcher.ApplyPulse(pulseStrength);
        }
    }
}
