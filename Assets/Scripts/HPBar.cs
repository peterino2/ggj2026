using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public Image fillImage;
    public Image damageFillImage;
    
    public float maxHP = 100f;
    public float animationSpeed = 5f;
    public float damageDelay = 0.5f;
    
    private float currentHP;
    private float displayHP;
    private float damageDisplayHP;
    private float damageDelayTimer;
    
    public static HPBar instance;

    private void Start()
    {
        instance = this;
        currentHP = maxHP;
        displayHP = maxHP;
        damageDisplayHP = maxHP;
        UpdateFill();
    }

    private void Update()
    {
        if (displayHP != currentHP)
        {
            displayHP = Mathf.MoveTowards(displayHP, currentHP, animationSpeed * maxHP * Time.deltaTime);
            
            if (displayHP > currentHP)
            {
                displayHP = Mathf.Lerp(displayHP, currentHP, animationSpeed * Time.deltaTime);
            }
            else
            {
                displayHP = Mathf.Lerp(displayHP, currentHP, animationSpeed * 2f * Time.deltaTime);
            }
        }

        if (damageDisplayHP > displayHP)
        {
            damageDelayTimer -= Time.deltaTime;
            if (damageDelayTimer <= 0f)
            {
                damageDisplayHP = Mathf.Lerp(damageDisplayHP, displayHP, animationSpeed * Time.deltaTime);
            }
        }
        else
        {
            damageDisplayHP = displayHP;
        }

        UpdateFill();
    }

    private void UpdateFill()
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = displayHP / maxHP;
        }

        if (damageFillImage != null)
        {
            damageFillImage.fillAmount = damageDisplayHP / maxHP;
        }
    }

    public void SetHP(float newHP)
    {
        float previousHP = currentHP;
        currentHP = Mathf.Clamp(newHP, 0f, maxHP);
        
        if (currentHP < previousHP)
        {
            damageDelayTimer = damageDelay;
        }
    }

    public void TakeDamage(float damage)
    {
        SetHP(currentHP - damage);
    }

    public void Heal(float amount)
    {
        SetHP(currentHP + amount);
    }

    public void SetMaxHP(float newMax, bool healToFull = false)
    {
        maxHP = newMax;
        if (healToFull)
        {
            currentHP = maxHP;
            displayHP = maxHP;
            damageDisplayHP = maxHP;
        }
        else
        {
            currentHP = Mathf.Min(currentHP, maxHP);
        }
        UpdateFill();
    }

    public float GetCurrentHP()
    {
        return currentHP;
    }

    public float GetHPPercent()
    {
        return currentHP / maxHP;
    }

    public bool IsDead()
    {
        return currentHP <= 0f;
    }
}
