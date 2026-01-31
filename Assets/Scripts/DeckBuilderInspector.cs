using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

// totally cheating hard here, lifting this
public class DeckBuilderInspector : MonoBehaviour
{
    public Camera mainCamera;
    public RectTransform cardPanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI repeatDelayText;
    public TextMeshProUGUI repeatMultiplierText;

    public Material commonMaterial;
    public Material rareMaterial;
    public Material legendMaterial;

    private PowerNode currentHoveredNode;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        
        if (cardPanel != null)
            cardPanel.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Mouse.current == null) return;

        Vector2 screenPos = Mouse.current.position.ReadValue();
        
        if (Deckbuilder.GetInstance().hoveredNodeValid)
        {
            PowerNode hoveredNode = GetHoveredPowerNode(screenPos);
            if (hoveredNode == null)
            {
                HideCard();
            }
            else
            {
                bool shouldInspect = false;
            
                if (hoveredNode.currentState == PowerNode.NodeState.Floating)
                {
                    shouldInspect = true;
                }
                else if (Deckbuilder.GetInstance().IsMouseInGridBounds(screenPos))
                {
                    shouldInspect = true;
                }
            
                if (hoveredNode != currentHoveredNode && shouldInspect)
                {
                    currentHoveredNode = hoveredNode;
            
                    if (currentHoveredNode != null)
                    {
                        ShowCard(currentHoveredNode);
                    }
                    else
                    {
                        HideCard();
                    }
                }
            }
        }
        else
        {
            HideCard();
        }

        if (cardPanel.gameObject.activeSelf)
        {
            PositionCard(screenPos);
        }
    }

    private PowerNode GetHoveredPowerNode(Vector2 screenPos)
    {
        Deckbuilder db = Deckbuilder.GetInstance();
        PowerNode node = db.hoveredNode;
        
        if (node.IsPointerOverNode(screenPos))
            return node;

        return null;
    }

    public Color commonColor;
    public Color rareColor;
    public Color legendColor;

    private void ShowCard(PowerNode node)
    {
        if (cardPanel == null) return;

        cardPanel.gameObject.SetActive(true);

        if (nameText != null)
        {
            nameText.text = node.NodeName;

            if (node.rarity == PowerNode.Rarity.Common)
            {
                nameText.color = commonColor;
                nameText.material = commonMaterial;
            }

            if (node.rarity == PowerNode.Rarity.Rare)
            {
                nameText.color = rareColor;
                nameText.material = rareMaterial;
            }

            if (node.rarity == PowerNode.Rarity.Legendary)
            {
                nameText.color = legendColor;
                nameText.material = legendMaterial;
            }
        }
        
        if (descriptionText != null)
            descriptionText.text = node.Description;
        
        if (repeatDelayText != null)
            repeatDelayText.text = $"Delay: {node.RepeatDelay:F1}s";
        
        if (repeatMultiplierText != null)
            repeatMultiplierText.text = $"Power Factor: {node.RepeatMultiplier:F2}x";
    }

    private void HideCard()
    {
        if (cardPanel != null)
            cardPanel.gameObject.SetActive(false);
        
        currentHoveredNode = null;
    }

    private void PositionCard(Vector2 screenPos)
    {
        Vector2 apos = Deckbuilder.GetInstance().canvasCamera.ScreenToWorldPoint(screenPos);
        Vector2 offset = cardPanel.rect.size / 2;
        // hacky resolution stuff again
        Vector2 position = (apos + offset);
        cardPanel.transform.localPosition = new Vector3(position.x, position.y, -20);
    }
}
