using UnityEngine;

namespace DefaultNamespace.NodeEffects
{
    public class GeneratorEffect : NodeEffect
    {
        private float timeToFire = 0.0f;

        public override void TickEffect()
        {
            if (parentNode.currentState == PowerNode.NodeState.Docked)
            {
                timeToFire -= Time.deltaTime;

                if (timeToFire <= 0.0f)
                {
                    timeToFire += 1.0f / parentNode.FireRate;
                    OnPulse(parentNode.NodePower);
                }
            }
        }

        public override void OnPulse(float pulseStrength)
        {
            ForwardPulse(pulseStrength);
        }
    }
}
