using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class PowerPulse : MonoBehaviour
    {
        public float pulsePower = 1.0f;
        public float lifetime = 1.5f;
        public float speed = 30.0f;
        public PowerNode instigator;
        public Vector2 dir = new Vector2(1,0);
        public Vector2 pulsepos = new Vector2(0, 0);
        
        public RectTransform rt;

        private void Awake()
        {
            rt = GetComponent<RectTransform>();
            pulsepos = rt.position;
        }

        private void Update()
        {
            pulsepos += Time.deltaTime * speed * dir.normalized;
            Deckbuilder db = Deckbuilder.GetInstance();
            
            lifetime -= Time.deltaTime;
            if (lifetime <= 0.0f)
            {
                Destroy(gameObject);
            }

            rt.position = new Vector3(pulsepos.x, pulsepos.y, -1);
            
            foreach (var node in db.dockedNodes)
            {
                if (node == instigator)
                {
                    continue;
                }
                
                if (RectTransformUtility.RectangleContainsScreenPoint(node.rectTransform, rt.position))
                {
                    node.OnPulse(pulsePower);
                    Destroy(gameObject); // TODO pooling
                }
            }
        }
    }
}