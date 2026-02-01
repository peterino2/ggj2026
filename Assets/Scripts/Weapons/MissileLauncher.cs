using UnityEngine;
using UnityEngine.Pool;

public class MissileLauncher : WeaponBase
{
    [SerializeField] Missile MissilePrefab;

    [SerializeField] float TimeBetweenShots = 3.0f;

    [SerializeField] float PulsePowerPerMissile = 3.0f;

    [SerializeField] float InitialVelocity = 500.0f;

    [SerializeField] float Damage = 50.0f;

    private ObjectPool<Missile> MissilePool;

    private float CurPulsePower = 0.0f;

    private float TimeSinceLastShot = 0.0f;    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MissilePool = new ObjectPool<Missile>(CreateMissile);
    }

    private Missile CreateMissile()
    {
        Missile NewMissile = Instantiate(MissilePrefab);
        NewMissile.transform.SetParent(gameObject.transform.parent);
        NewMissile.SetOwner(MissilePool);
        return NewMissile;
    }

    private void FireMissile(Vector3 Velocity)
    {
        Missile MissileToFire = MissilePool.Get();

        MissileToFire.Damage = Damage;
        MissileToFire.SetVelocity(Velocity);
        MissileToFire.gameObject.transform.position = gameObject.transform.position;
        MissileToFire.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        // DEBUG TEST REMOVE
        // CurPulsePower += 100.0f * Time.deltaTime;

        TimeSinceLastShot += Time.deltaTime;

        if(TimeSinceLastShot < TimeBetweenShots)
        {
            return;
        }

        TimeSinceLastShot = 0.0f;

        while (CurPulsePower > PulsePowerPerMissile)
        {
            CurPulsePower -= PulsePowerPerMissile;

            float Angle = Random.Range(-Mathf.PI / 4.0f, Mathf.PI / 4.0f);
            Vector3 Velocity = new Vector3(-Mathf.Cos(Angle), Mathf.Sin(Angle));
            Velocity *= InitialVelocity;
            FireMissile(Velocity);
        }
    }

    public override void ApplyPulse(float pulsePower)
    {
        base.ApplyPulse(pulsePower);

        CurPulsePower += pulsePower;
    }
}
