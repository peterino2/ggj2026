using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

// ok i really want to try generating objects purely through code, 
// I dont think unity is usually very good at that. is it?
public class ScrollingRightToLeftBG : MonoBehaviour
{
    public Sprite[] sprites = new Sprite[] { };
    public float[] SpeedList = new float[] {300f, 150f, 75f, 25f, 15f};
    public Vector2[][] AnchoredPositions = new Vector2[][] {};

    public Material bgMaterial;

    public Camera canvasCamera;

    private RectTransform[][] layerImages;
    private float imageWidth;
    public Canvas canvas;
    public RectTransform canvasRect;

    private const int LayerCount = 5;

    private void Awake()
    {
        canvasRect = canvas.GetComponent<RectTransform>();
        imageWidth = canvasRect.rect.width;
        float imageHeight = canvasRect.rect.height;

        // Sprite[] sprites = { backgroundSprite, midgroundSprite, foregroundSprite };
        layerImages = new RectTransform[LayerCount][];
        
        AnchoredPositions  = new Vector2[LayerCount][];

        for (int layer = 0; layer < LayerCount; layer++)
        {
            // Skip layers with no sprite assigned
            if (sprites[layer] == null)
            {
                layerImages[layer] = null;
                continue;
            }

            layerImages[layer] = new RectTransform[3];
            AnchoredPositions[layer] = new Vector2[3];

            for (int i = 0; i < 3; i++)
            {
                GameObject imgObj = new GameObject($"Layer{layer}_Image{i}");
                imgObj.transform.SetParent(transform, false);
                imgObj.transform.localPosition = imgObj.transform.localPosition + new Vector3(0,0,1);

                // no idea if these anchor settings are correct. just copy pastaing off the forums
                RectTransform rect = imgObj.AddComponent<RectTransform>();
                rect.sizeDelta = new Vector2(imageWidth, imageHeight);
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = new Vector2((i - 1) * imageWidth, 0);
                AnchoredPositions[layer][i] = rect.anchoredPosition;

                Image img = imgObj.AddComponent<Image>();
                img.sprite = sprites[layer];
                if (bgMaterial != null)
                {
                    img.material = bgMaterial;
                }
                img.preserveAspect = false;

                // Set sibling index for draw order (background first, foreground last)
                imgObj.transform.SetSiblingIndex(layer * 3 + i);

                layerImages[layer][i] = rect;
            }
        }
    }

    private Vector2 parallaxPos = Vector2.zero;
    private void Update()
    {
        if (layerImages == null) return;

        parallaxPos = Vector2.zero;
        Vector2 mouse2d = Mouse.current.position.ReadValue();
        parallaxPos = mouse2d - new Vector2(1920, 1080) / 2.0f;
        parallaxPos.x = 0;
        parallaxPos = parallaxPos * 50 / 10000;
        
        for (int layer = 0; layer < LayerCount; layer++)
        {
            // might not have a midground, skipping that shit
            if (layerImages[layer] == null) continue;

            for (int i = 0; i < 3; i++)
            {
                RectTransform rect = layerImages[layer][i];
                Vector2 pos = AnchoredPositions[layer][i];
                pos.x -= SpeedList[layer] * Time.deltaTime;

                // Wrap around when offscreend
                if (pos.x < -imageWidth * 1.5f)
                {
                    pos.x += imageWidth * 3f;
                }

                AnchoredPositions[layer][i] = pos;
                
                // parallax calculation based off of ship position?
                rect.anchoredPosition = AnchoredPositions[layer][i] + parallaxPos * (LayerCount - layer);
            }
        }
    }
}
