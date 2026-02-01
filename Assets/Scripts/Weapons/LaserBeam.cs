using UnityEngine;

public class LaserBeam : BulletBase
{
    float CurrentPower = 0.0f;

    public float PowerDamageScale = 1000.0f;

    public override float GetDamage()
    {
        return CurrentPower * PowerDamageScale;
    }

    public void SetCurrentPower(float Power)
    {
        CurrentPower = Power;
        gameObject.transform.localScale = new Vector3(1.0f, CurrentPower, 1.0f);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
