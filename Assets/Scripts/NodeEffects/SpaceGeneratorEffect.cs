using UnityEngine;
using UnityEngine.InputSystem;

namespace DefaultNamespace.NodeEffects
{
    public class SpaceGeneratorEffect : NodeEffect
    {
        private float timeToFire = 0.0f;
        private bool canFire = true;

        public override void TickEffect()
        {
            if (parentNode.currentState != PowerNode.NodeState.Docked)
                return;

            if (!canFire)
            {
                timeToFire -= Time.deltaTime;
                if (timeToFire <= 0.0f)
                {
                    canFire = true;
                }
            }

            if (canFire && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                canFire = false;
                timeToFire = 1.0f / parentNode.FireRate;
                OnPulse(parentNode.NodePower);
            }
        }

        public override void OnPulse(float pulseStrength)
        {
            ForwardPulse(pulseStrength);
        }
    }
}
