using UnityEngine;
using UnityEngine.Pool;

public class AutoGun : WeaponBase
{
    private ObjectPool<AutoGunBullet> BulletPool;

    private float CurrentPulsePower = 0.0f;

    [SerializeField] AutoGunBullet AutoGunBulletPrefab;

    [SerializeField] float CriticalPulsePoint = 1000.0f;

    [SerializeField] float PulsePerBullet = 0.3f;

    [SerializeField] float BaseFireRate = 0.25f;

    [SerializeField] float BulletSpeed = 30.0f;

    private float TimeSinceLastShot = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BulletPool = new ObjectPool<AutoGunBullet>(CreateBullet, null, null, null, true, 10, 100000);
    }

    private AutoGunBullet CreateBullet()
    {
        return Instantiate(AutoGunBulletPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        TimeSinceLastShot += Time.deltaTime;

        float NumQueuedBullets = CurrentPulsePower / PulsePerBullet;

        if (NumQueuedBullets < 1.0f)
        {
            return;
        }

        if (CurrentPulsePower > CriticalPulsePoint)
        {

        }
        else
        {
            float TimeBetweenShots = BaseFireRate / NumQueuedBullets;

            if(TimeBetweenShots < TimeSinceLastShot)
            {
                TimeSinceLastShot = 0.0f;
                AutoGunBullet Bullet = BulletPool.Get();
                Bullet.Velocity.Set(BulletSpeed, 0.0f, 0.0f);
                Bullet.gameObject.SetActive(true);
            }
        }
    }

    public override void ApplyPulse(float PulsePower)
    {
        base.ApplyPulse(PulsePower);

        CurrentPulsePower += PulsePower;
    }
}
