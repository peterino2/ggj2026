using UnityEngine;

public class EAMoveToXPosition : EnemyActionBase
{
    public float AdjustmentTime = 0.2f;
    public float Speed = 100.0f;


    [SerializeField] public Transform bossPosition;



    public override bool CanPerform()
    {
        if (controller.transform.position.x > bossPosition.position.x)
        {

            return true;
        }
        else {
            Vector3 Velo = controller.GetVelocity();
            Velo.x = 0;
            controller.SetVelocity(Velo);
            return false;
        }
            
    }

    public override bool PerformAction()
    {
        if (!CanPerform())
        {
            Vector3 Velo =controller.GetVelocity();
            Velo.x = 0;

            controller.SetVelocity(Velo);
            return false;
        }

        Vector3 Velocity = controller.GetVelocity();
        Velocity.x = -Speed;

        controller.SetVelocity(Velocity);

        return true;
    }
}
