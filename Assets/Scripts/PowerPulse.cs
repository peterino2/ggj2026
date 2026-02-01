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
        
        public float minBrightness = 0.2f;
        public float maxBrightness = 2.0f;
        public float powerScaleFactor = 1.0f;
        
        public RectTransform rt;
        private SpriteRenderer spriteRenderer;
        private MaterialPropertyBlock propertyBlock;
        private static readonly int ClipRectID = Shader.PropertyToID("_ClipRect");
        
        private float initialLifetime;
        private Color baseColor;

        private void Awake()
        {
            rt = GetComponent<RectTransform>();
            pulsepos = rt.position;
            spriteRenderer = GetComponent<SpriteRenderer>();
            propertyBlock = new MaterialPropertyBlock();
            initialLifetime = lifetime;
            baseColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
        }

        private void Start()
        {
            ApplyClipRect();
            UpdateBrightness();
        }

        private void ApplyClipRect()
        {
            if (spriteRenderer == null) return;
            
            Deckbuilder db = Deckbuilder.GetInstance();
            if (db == null || db.gridBounds == null) return;
            
            Vector3[] corners = new Vector3[4];
            db.gridBounds.GetWorldCorners(corners);
            Vector4 clipRect = new Vector4(corners[0].x, corners[0].y, corners[2].x, corners[2].y);
            
            spriteRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetVector(ClipRectID, clipRect);
            spriteRenderer.SetPropertyBlock(propertyBlock);
        }

        private void UpdateBrightness()
        {
            if (spriteRenderer == null) return;
            
            float lifetimeRatio = lifetime / initialLifetime;
            float powerBrightness = Mathf.Clamp(pulsePower * powerScaleFactor, minBrightness, maxBrightness);
            float finalBrightness = powerBrightness * lifetimeRatio;
            
            Color newColor = baseColor * finalBrightness;
            newColor.a = baseColor.a * lifetimeRatio;
            spriteRenderer.color = newColor;
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

            UpdateBrightness();

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