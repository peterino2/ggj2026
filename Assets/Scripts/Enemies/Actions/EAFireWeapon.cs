using UnityEngine;

public class EAFireWeapon : EnemyActionBase
{
    public float ShotCooldown = 0.75f;
    public EnemyWeaponBase Weapon;

    private float TimeSinceLastShot = 20000.0f;
    public override bool CanPerform()
    {
        return TimeSinceLastShot > ShotCooldown;
    }

    public override bool PerformAction()
    {
        if(!CanPerform())
        {
            return false;
        }

        if(!Weapon.CanFire())
        {
            return false;
        }

        Weapon.Fire();
        TimeSinceLastShot = 0.0f;
        return true;
    }

    private void Update()
    {
        TimeSinceLastShot += Time.deltaTime;   
    }
}
