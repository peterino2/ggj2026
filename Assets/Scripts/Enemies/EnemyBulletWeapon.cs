using UnityEngine;
using UnityEngine.Pool;

public class EnemyBulletWeapon : EnemyWeaponBase
{
    ObjectPool<EnemyBullet> BulletPool;

    [SerializeField] float Speed;

    [SerializeField] EnemyBullet BulletPrefab;

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
        EnemyBullet Bullet = BulletPool.Get();

        Bullet.transform.position = transform.position;
        Bullet.gameObject.SetActive(true);


    }
}
