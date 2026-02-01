using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class LootSelectionUI : MonoBehaviour
{
    public NodeLootSpawner lootSpawner;
    public CanvasGroup panelCanvasGroup;
    public Transform spawnPoint;

    public Button[] choiceButtons;
    public TextMeshProUGUI[] nameTexts;
    public TextMeshProUGUI[] descriptionTexts;
    public TextMeshProUGUI[] statsTexts;
    public RawImage[] iconImages;

    public Material commonMaterial;
    public Material rareMaterial;
    public Material legendaryMaterial;

    public Color commonColor;
    public Color rareColor;
    public Color legendColor;

    private RolledNode[] currentChoices;
    private bool isShowing = false;

    private void Start()
    {
        HideSelection();
        
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            int index = i;
            choiceButtons[i].onClick.AddListener(() => OnOptionSelected(index));
        }
    }

    private void Update()
    {
        if (isShowing) return;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.nKey.wasPressedThisFrame)
            {
                ShowSelection(RollType.Default);
            }
            else if (Keyboard.current.kKey.wasPressedThisFrame)
            {
                ShowSelection(RollType.High);
            }
        }
    }

    public void ShowSelection(RollType rollType)
    {
        currentChoices = lootSpawner.RollChoices(rollType, 3);

        for (int i = 0; i < currentChoices.Length && i < choiceButtons.Length; i++)
        {
            RolledNode choice = currentChoices[i];
            
            if (nameTexts[i] != null)
            {
                nameTexts[i].fontSharedMaterial = GetRarityMaterial(choice.rarity);
                nameTexts[i].color = GetRarityColor(choice.rarity);
                
                switch (choice.rarity)
                {
                    case PowerNode.Rarity.Rare:
                        nameTexts[i].text = choice.archetype.name + " (Rare)";
                        break;
                    case PowerNode.Rarity.Legendary:
                        nameTexts[i].text = choice.archetype.name + " (Legendary)";
                        break;
                    default:
                        nameTexts[i].text = choice.archetype.name;
                        break;
                }
            }

            if (descriptionTexts[i] != null)
            {
                descriptionTexts[i].text = choice.archetype.description;
            }

            if (statsTexts[i] != null)
            {
                statsTexts[i].text = FormatStats(choice);
            }

            if (iconImages != null && i < iconImages.Length && iconImages[i] != null)
            {
                Sprite icon = lootSpawner.GetIcon(choice.archetype.iconKey);
                if (icon != null)
                {
                    iconImages[i].texture = icon.texture;
                    iconImages[i].enabled = true;
                }
                else
                {
                    iconImages[i].enabled = false;
                }
            }

            choiceButtons[i].gameObject.SetActive(true);
        }

        panelCanvasGroup.alpha = 1f;
        panelCanvasGroup.interactable = true;
        panelCanvasGroup.blocksRaycasts = true;
        isShowing = true;
    }

    public void HideSelection()
    {
        panelCanvasGroup.alpha = 0f;
        panelCanvasGroup.interactable = false;
        panelCanvasGroup.blocksRaycasts = false;
        isShowing = false;
        currentChoices = null;
    }

    public void OnOptionSelected(int index)
    {
        if (currentChoices == null || index >= currentChoices.Length) return;

        RolledNode selected = currentChoices[index];
        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : GetDefaultSpawnPosition();
        
        lootSpawner.SpawnFromRolledNode(selected, spawnPos);
        HideSelection();
    }

    private Vector3 GetDefaultSpawnPosition()
    {
        Camera cam = lootSpawner.spawnCamera != null ? lootSpawner.spawnCamera : Camera.main;
        Vector3 viewportPos = new Vector3(0.75f, 0.5f, 10f);
        return cam.ViewportToWorldPoint(viewportPos);
    }

    private Material GetRarityMaterial(PowerNode.Rarity rarity)
    {
        switch (rarity)
        {
            case PowerNode.Rarity.Rare:
                return rareMaterial;
            case PowerNode.Rarity.Legendary:
                return legendaryMaterial;
            default:
                return commonMaterial;
        }
    }

    private Color GetRarityColor(PowerNode.Rarity rarity)
    {
        switch (rarity)
        {
            case PowerNode.Rarity.Rare:
                return rareColor;
            case PowerNode.Rarity.Legendary:
                return legendColor;
            default:
                return commonColor;
        }
    }

    private string FormatStats(RolledNode rolled)
    {
        string stats = "";
        
        if (rolled.archetype.effectType == "GeneratorEffect")
        {
            stats += $"Fire Rate: {rolled.fireRate:F1}/s\n";
            stats += $"Power: {rolled.nodePower:F1}\n";
        }
        
        stats += $"Power Factor: {rolled.powerFactor:F2}x";

        if (rolled.pulseDirections != null && rolled.pulseDirections.Length > 0)
        {
            stats += $"\nOutputs: {rolled.pulseDirections.Length}";
        }

        return stats;
    }
}
