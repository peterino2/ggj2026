using UnityEngine;

public class EAFollowPlayerYPosition : EnemyActionBase
{
    public float AdjustmentTime = 0.2f;
    public float Speed = 100.0f;

    private float TimeSinceLastAdjustment = 20000.0f;

    public override bool CanPerform()
    {
        return TimeSinceLastAdjustment > AdjustmentTime;
    }

    public override bool PerformAction()
    {
        if (!CanPerform())
        {
            return false;
        }

        Vector3 Velocity = controller.GetVelocity();

        if (controller.transform.position.y > SpriteController.GetPlayer().transform.position.y + 10.0f)
        {
            Velocity.y = -Speed;
        }
        else if(controller.transform.position.y < SpriteController.GetPlayer().transform.position.y - 10.0f)
        {
            Velocity.y = Speed;
        }
        else
        {
            Velocity.y = 0;
        }

        controller.SetVelocity(Velocity);

        TimeSinceLastAdjustment = 0.0f;

        return true;
    }

    private void Update()
    {
        TimeSinceLastAdjustment += Time.deltaTime;
    }
}
