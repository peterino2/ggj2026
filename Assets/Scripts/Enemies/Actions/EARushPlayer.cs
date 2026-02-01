using System.Runtime.CompilerServices;
using UnityEngine;

public class EARushPlayer : EnemyActionBase
{
    public float AdjustmentTime = 0.2f;
    public float Speed = 300.0f;
    public float MaxAdjustmentAngle = 5.0f;

    private float TimeSinceLastAdjustment = 20000.0f;
    

    public override bool CanPerform()
    {
        return TimeSinceLastAdjustment > AdjustmentTime;
    }

    public override bool PerformAction()
    {
        if(!CanPerform())
        {
            return false;
        }

        if (transform.position.x < SpriteController.GetPlayer().transform.position.x + 50.0f)
        {
            return false;
        }

        Vector3 DesiredDirection = Vector3.Normalize(SpriteController.GetPlayer().transform.position - transform.position);
        Vector3 CurVelocity = controller.GetVelocity();

        if(CurVelocity.sqrMagnitude == 0.0f)
        {
            CurVelocity = new Vector3(-1.0f, 0, 0);
        }

        Vector3 CurrentDirection = Vector3.Normalize(CurVelocity);

        Vector3.Angle(DesiredDirection, CurrentDirection);

        controller.SetVelocity(Vector3.RotateTowards(CurrentDirection, DesiredDirection, Mathf.Deg2Rad * MaxAdjustmentAngle, 100000.0f) * Speed);

        TimeSinceLastAdjustment = 0.0f;

        return true;
    }

    private void Update()
    {
        TimeSinceLastAdjustment += Time.deltaTime;
    }
}
