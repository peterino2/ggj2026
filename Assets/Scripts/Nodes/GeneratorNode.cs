using UnityEngine;

namespace Nodes
{
    public class GeneratorNode : PowerNode
    {
        void Awake()
        {
            NodeName = "Generator";
            Description = "Periodically generates a pulse in the target direction";
        }
        
        public override void OnPulse()
        {
            base.OnPulse();
        }
    }
}