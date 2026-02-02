using UnityEngine;
using UnityEngine.Pool;

public class EnemyBullet : MonoBehaviour
{
    public Vector3 Velocity;

    public float Damage = 10.0f;

    public ObjectPool<EnemyBullet> Owner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Velocity * Time.deltaTime;

        if(transform.position.x < -1500.0f)
        {
            Owner.Release(this);
            gameObject.SetActive(false);
            transform.position = Vector3.zero;
        }
    }

    public virtual void OnPlayerHit()
    {
        SpriteController.GetPlayer().takeDamage(Damage);
        Owner.Release(this);
        gameObject.SetActive(false);
        transform.position = Vector3.zero;
    }
}
