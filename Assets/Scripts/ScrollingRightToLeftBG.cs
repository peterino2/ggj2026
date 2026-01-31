using UnityEngine;
using UnityEngine.UI;

// ok i really want to try generating objects purely through code, 
// I dont think unity is usually very good at that. is it?
public class ScrollingRightToLeftBG : MonoBehaviour
{
    public Sprite foregroundSprite;
    public Sprite midgroundSprite;
    public Sprite backgroundSprite;

    public float foregroundSpeed = 300f;
    public float midgroundSpeed = 150f;
    public float backgroundSpeed = 75f;

    public Material foregroundMaterial;
    public Material midgroundMaterial;
    public Material backgroundMaterial;

    public Camera canvasCamera;

    private RectTransform[][] layerImages;
    private float imageWidth;
    private float[] speeds;
    public Canvas canvas;
    public RectTransform canvasRect;

    private void Awake()
    {
        canvasRect = canvas.GetComponent<RectTransform>();
        imageWidth = canvasRect.rect.width;
        float imageHeight = canvasRect.rect.height;

        Sprite[] sprites = { backgroundSprite, midgroundSprite, foregroundSprite };
        Material[] materials = { backgroundMaterial, midgroundMaterial, foregroundMaterial };
        speeds = new float[] { backgroundSpeed, midgroundSpeed, foregroundSpeed };
        layerImages = new RectTransform[3][];

        for (int layer = 0; layer < 3; layer++)
        {
            // Skip layers with no sprite assigned
            if (sprites[layer] == null)
            {
                layerImages[layer] = null;
                continue;
            }

            layerImages[layer] = new RectTransform[3];

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

                Image img = imgObj.AddComponent<Image>();
                img.sprite = sprites[layer];
                if (materials[layer] != null)
                {
                    img.material = materials[layer];
                }
                img.preserveAspect = false;

                // Set sibling index for draw order (background first, foreground last)
                imgObj.transform.SetSiblingIndex(layer * 3 + i);

                layerImages[layer][i] = rect;
            }
        }
    }

    private void Update()
    {
        if (layerImages == null) return;
        
        speeds = new float[] { backgroundSpeed, midgroundSpeed, foregroundSpeed };
        
        for (int layer = 0; layer < 3; layer++)
        {
            // might not have a midground, skipping that shit
            if (layerImages[layer] == null) continue;

            for (int i = 0; i < 3; i++)
            {
                RectTransform rect = layerImages[layer][i];
                Vector2 pos = rect.anchoredPosition;
                pos.x -= speeds[layer] * Time.deltaTime;

                // Wrap around when offscreend
                if (pos.x < -imageWidth * 1.5f)
                {
                    pos.x += imageWidth * 3f;
                }

                rect.anchoredPosition = pos;
            }
        }
    }
}
