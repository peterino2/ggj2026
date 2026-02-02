using System.Globalization;
using UnityEngine;
using UnityEngine.Pool;

public class EnemySpreadWeapon : EnemyWeaponBase
{
    ObjectPool<EnemyBullet> BulletPool;

    [SerializeField] float Speed = 200.0f;

    [SerializeField] EnemyBullet BulletPrefab;

    [SerializeField] int NumberOfBullets = 3;

    [SerializeField] float ArcHalfAngle = 45.0f;

    private void Start()
    {
        BulletPool = new ObjectPool<EnemyBullet>(CreateBullet);
    }

    private EnemyBullet CreateBullet()
    {
        EnemyBullet Bullet = Instantiate(BulletPrefab);

        Bullet.Owner = BulletPool;
        return Bullet;
    }

    public override bool CanFire()
    {
        return true;
    }

    public override void Fire()
    {
        float AngleDiff = ArcHalfAngle * 2.0f / NumberOfBullets;

        AngleDiff = Mathf.Deg2Rad * AngleDiff;

        float CurAngle = 180.0f + ArcHalfAngle;
        CurAngle = Mathf.Deg2Rad * CurAngle - AngleDiff / 2.0f;

        for(int i = 0; i < NumberOfBullets; i++)
        {
            EnemyBullet bullet = BulletPool.Get();

            bullet.Velocity = new Vector3(Mathf.Cos(CurAngle), Mathf.Sin(CurAngle), 0.0f);
            bullet.Velocity *= Speed;
            bullet.transform.position = transform.position;
            bullet.gameObject.SetActive(true);

            CurAngle -= AngleDiff;
        }
    }
}
