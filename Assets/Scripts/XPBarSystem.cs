using System;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class XPBarSystem : MonoBehaviour
{
    public LootSelectionUI lootUI;
    public Image fillImage;
    
    public float xpPerSecond = 10f;
    public float maxXP = 100f;
    public RollType defaultRollType = RollType.Default;
    
    private float currentXP = 0f;
    private bool isPaused = false;

    public float Level = 1.0f;

    static private XPBarSystem Instance;

    static public XPBarSystem GetInstance()
    {
        return Instance;
    }

    public void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (isPaused || Time.timeScale == 0f) return;
        
        Gamemode gm = Gamemode.Instance;
        if (gm == null || !gm.IsTimerRunning()) return;
        
        currentXP += xpPerSecond * Time.deltaTime;
        
        if (fillImage != null)
        {
            fillImage.fillAmount = currentXP / maxXP;
        }
        
        if (currentXP >= maxXP)
        {
            TriggerLoot();
        }
    }

    private void TriggerLoot()
    {
        currentXP = 0f;
        
        if (fillImage != null)
        {
            fillImage.fillAmount = 0f;
        }
        
        if (lootUI != null)
        {
            lootUI.ShowSelection(defaultRollType);
        }

        Level += 1.0f;
    }

    public void AddXP(float amount)
    {
        currentXP += amount;
    }

    public void SetPaused(bool paused)
    {
        isPaused = paused;
    }

    public float GetXPPercent()
    {
        return currentXP / maxXP;
    }
}
