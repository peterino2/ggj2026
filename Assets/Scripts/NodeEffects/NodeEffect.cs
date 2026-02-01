using UnityEngine;

namespace DefaultNamespace.NodeEffects
{
    public abstract class NodeEffect : MonoBehaviour
    {
        protected PowerNode parentNode;

        public virtual void Initialize(PowerNode node)
        {
            parentNode = node;
        }

        public virtual void StartEffect()
        {
        }

        public virtual void TickEffect()
        {
        }

        public abstract void OnPulse(float pulseStrength);

        protected void ForwardPulse(float pulseStrength)
        {
            if (pulseStrength < parentNode.minPulsePower)
                return;

            foreach (var dir in parentNode.PulseDirections)
            {
                parentNode.GeneratePulse(dir, pulseStrength * parentNode.powerFactor);
            }
        }
    }
}