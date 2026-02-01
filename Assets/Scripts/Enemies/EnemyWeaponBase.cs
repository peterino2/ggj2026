using UnityEngine;

public class EnemyWeaponBase : MonoBehaviour
{
    protected bool bCanFire = false;

    public virtual bool CanFire()
    {
        return bCanFire;
    }

    public virtual void Fire()
    {

    }
}
