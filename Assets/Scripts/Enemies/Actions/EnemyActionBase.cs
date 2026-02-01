using UnityEngine;

public class EnemyActionBase : MonoBehaviour
{
 
    protected EnemyControllerBase controller;

    public void SetController(EnemyControllerBase EnemyController)
    {
        controller = EnemyController;
    }

    public virtual bool CanPerform()
    {
        return false;
    }

    public virtual bool PerformAction()
    {
        return true;
    }
}
