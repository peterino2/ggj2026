using UnityEngine;
using UnityEngine.Pool;

public class AutoGun : WeaponBase
{
    private ObjectPool<AutoGunBullet> BulletPool;

    private float CurrentPulsePower = 0.0f;

    // Bullet to fire
    [SerializeField] AutoGunBullet AutoGunBulletPrefab;

    // Critical number of queued bullets before it starts firing at angles
    [SerializeField] float CriticalBulletPoint = 10.0f;

    // Pulse power cost per shot
    [SerializeField] float PulsePerBullet = 0.3f;

    // IDK what this does random lever
    [SerializeField] float BaseFireRate = 0.25f;

    // How fast bullet go
    [SerializeField] float BulletSpeed = 300.0f;
    [SerializeField] Transform bulletSpawnPosition;

    private float TimeSinceLastShot = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BulletPool = new ObjectPool<AutoGunBullet>(CreateBullet, null, null, null, true, 10, 100000);
    }

    private AutoGunBullet CreateBullet()
    {
        AutoGunBullet NewBullet = Instantiate(AutoGunBulletPrefab);
        NewBullet.SetOwner(BulletPool);
        return NewBullet;
    }

    private void FireBullet(Vector3 Velocity)
    {
        AutoGunBullet Bullet = BulletPool.Get();
        Bullet.gameObject.transform.SetParent(gameObject.transform.parent, false);
        Bullet.gameObject.transform.position = bulletSpawnPosition.position;
        Bullet.Velocity = Velocity;
        Bullet.gameObject.SetActive(true);

        CurrentPulsePower -= PulsePerBullet;
    }

    // Update is called once per frame
    void Update()
    {
        TimeSinceLastShot += Time.deltaTime;

        //TEMP TESTING ONLY
        // CurrentPulsePower += Time.deltaTime;

        float NumQueuedBullets = CurrentPulsePower / PulsePerBullet;

        if (NumQueuedBullets < 1.0f)
        {
            return;
        }

        if (NumQueuedBullets > CriticalBulletPoint)
        {
            if(TimeSinceLastShot < 4.0f / NumQueuedBullets)
            {
                return;
            }

            float ShotsPerShot = NumQueuedBullets - CriticalBulletPoint;

            float Divisor = 4.0f;

            if (ShotsPerShot < 5.0f)
            {
                Divisor += 5.0f - ShotsPerShot;
            }

            if(ShotsPerShot > 1.0f)
            {
                ShotsPerShot = Mathf.Sqrt(ShotsPerShot); 
            }

            float AngleRange = Mathf.PI / Divisor;

            ShotsPerShot = Mathf.Min(ShotsPerShot, 10.0f);

            for (float i = ShotsPerShot; i > 0.0f; i -= 1.0f)
            {
                float Angle = Random.Range(-AngleRange, AngleRange);

                Vector3 Velocity = new Vector3(Mathf.Cos(Angle), Mathf.Sin(Angle), 0.0f);
                Velocity *= BulletSpeed;

                AudioPlayer.Instance.Play(SoundType.AutoGun);
                FireBullet(Velocity);
            }

            TimeSinceLastShot = 0.0f;
        }
        else
        {
            float TimeBetweenShots = BaseFireRate / NumQueuedBullets;

            if(TimeBetweenShots < TimeSinceLastShot)
            {
                TimeSinceLastShot = 0.0f;
                FireBullet(new Vector3(BulletSpeed, 0.0f, 0.0f));
                AudioPlayer.Instance.Play(SoundType.AutoGun);
            }
        }
    }

    public override void ApplyPulse(float PulsePower)
    {
        Debug.Log("Autogun pulsed " + PulsePower);
        base.ApplyPulse(PulsePower);

        CurrentPulsePower += PulsePower;
    }
}
