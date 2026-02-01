using UnityEngine;
using UnityEngine.Pool;

public class Scrap : MonoBehaviour
{
    private float BaseMagnetizeSpeed = 750.0f;

    private float CollectionRangeSquared = 1.0f;

    private float XP = 1.0f;

    private Vector3 Velocity;

    private ObjectPool<Scrap> Owner;

    private bool IsMagnetized = false;

    private Transform PlayerTransform;

    public void SetXP(float xp)
    {
        XP = xp;
    }

    public void SetCollectionRange(float Range)
    {
        CollectionRangeSquared = Range * Range;
    }

    public void SetOwner(ObjectPool<Scrap> owner)
    {
        Owner = owner;
    }

    public void SetBaseMagnetizeSpeed(float speed)
    {
        BaseMagnetizeSpeed = speed;
    }

    public void SetVelocity(Vector3 velocity)
    {
        Velocity = velocity;
    }

    public void SetPlayerTransformReference(Transform playerTransform)
    {
        PlayerTransform = playerTransform;
    }

    public void Magnetize(float pulsePower)
    {
        IsMagnetized = true;
    }

    private bool IsCloseEnoughToPlayer()
    {
        return Vector3.SqrMagnitude(PlayerTransform.position - transform.position) <= CollectionRangeSquared;
    }

    private void Reset()
    {
        gameObject.SetActive(false);
        transform.position = new Vector3(2000, 0, 0);
        Owner.Release(this);
        IsMagnetized = false;
    }

    void Update()
    {
        if (IsMagnetized)
        {
            Vector3 NewDirection = Vector3.Normalize(PlayerTransform.position - transform.position);

            transform.position += NewDirection * BaseMagnetizeSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += Velocity * Time.deltaTime;
        }

        if(IsCloseEnoughToPlayer())
        {
            XPBarSystem.GetInstance().AddXP(XP);

            Reset();
        }
        else if(transform.position.x < -1500.0f)
        {
            Reset();
        }
    }
}
