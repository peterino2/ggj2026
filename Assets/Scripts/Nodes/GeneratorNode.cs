using UnityEngine;

namespace Nodes
{
    public class GeneratorNode : PowerNode
    {
        // generates a pulse once every delay seconds

        private float TimeToFire = 0.0f;
        
        private void Update()
        {
            UpdateInner();
        }

        // DO NOT implement Update(), parent's update calls TickNode
        public override void TickNode()
        {
            if(currentState == NodeState.Docked)
            {
                TimeToFire -= Time.deltaTime;

                if (TimeToFire <= 0.0f)
                {
                    TimeToFire += 1.0f / FireRate;
                    OnPulse(1.0f);
                }
            }
        }
        
        public override void OnPulse(float pulseStrength)
        {
            base.OnPulse(pulseStrength);
        }
    }
}