using UnityEngine;

public class Railgun : WeaponBase
{
    [SerializeField] float BulletSpeed = 1250.0f;

    [SerializeField] RailgunBullet RailgunPrefab;

    [SerializeField] float ShotCooldown = 2.0f;

    [SerializeField] float DamagePerPulsePower = 100.0f;

    [SerializeField] float BaseMinPulsePowerPerShot = 5.0f;

    float TimeSinceLastShot = 0.0f;

    float CurPulsePower = 0.0f;

    RailgunBullet Bullet;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Bullet = Instantiate(RailgunPrefab);
        Bullet.gameObject.SetActive(false);
        Bullet.Velocity = new Vector3(BulletSpeed, 0.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        TimeSinceLastShot += Time.deltaTime;

        // TEMP TEST CODE
        // CurPulsePower += Time.deltaTime * 20.0f;

        if(TimeSinceLastShot >= ShotCooldown)
        {
            if(CurPulsePower >= BaseMinPulsePowerPerShot)
            {
                Bullet.gameObject.transform.SetParent(gameObject.transform.parent);
                Bullet.Damage = CurPulsePower * DamagePerPulsePower;
                Bullet.gameObject.transform.position = gameObject.transform.position;
                Bullet.gameObject.SetActive(true);

                CurPulsePower = 0.0f;

                TimeSinceLastShot = 0.0f;
            }
        }
    }

    public override void ApplyPulse(float pulsePower)
    {
        base.ApplyPulse(pulsePower);
        CurPulsePower += pulsePower;
    }
}
