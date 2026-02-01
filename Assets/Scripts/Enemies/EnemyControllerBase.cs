using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class EnemyControllerBase : MonoBehaviour
{

    protected Vector3 Velocity = Vector3.zero;

    [SerializeField] List<EnemyActionBase> ActionList;

    public virtual Vector3 GetVelocity()
    {
        return Velocity;
    }

    public virtual void SetVelocity(Vector3 velocity)
    {
        Velocity = velocity;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (EnemyActionBase action in ActionList)
        {
            action.SetController(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Velocity * Time.deltaTime;

        foreach(EnemyActionBase action in ActionList)
        {
            if(action.CanPerform())
            {
                if(action.PerformAction())
                {
                    return;
                }
            }
        }
    }
}
