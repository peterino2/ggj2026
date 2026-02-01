using UnityEngine;
using UnityEngine.Pool;

public class Missile : MonoBehaviour
{
    [SerializeField] float XAcceleration = 500.0f;

    [SerializeField] float YDeceleration = 250.0f;

    Vector3 Velocity;

    ObjectPool<Missile> Owner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void SetVelocity(Vector3 NewVelocity)
    {
        Velocity = NewVelocity;
    }

    public void SetOwner(ObjectPool<Missile> objectPool)
    {
        Owner = objectPool;
    }

    // Update is called once per frame
    void Update()
    {
        if(Velocity.y > 0.0f)
        {
            Velocity.y = Mathf.Max(0.0f, Velocity.y - YDeceleration * Time.deltaTime);
        }
        else if(Velocity.y < 0.0f)
        {
            Velocity.y = Mathf.Min(0.0f, Velocity.y + YDeceleration * Time.deltaTime);
        }

        Velocity.x = Velocity.x + XAcceleration * Time.deltaTime;

        transform.position += Velocity * Time.deltaTime;

        if(transform.position.x > 2000.0f)
        {
            gameObject.SetActive(false);
            transform.position = Vector3.zero;

            Owner.Release(this);
        }
    }
}
