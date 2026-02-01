using System;
using UnityEngine;
using UnityEngine.Pool;

public class AutoGunBullet : MonoBehaviour
{
    public Vector3 Velocity { get; set; }

    private ObjectPool<AutoGunBullet> Owner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void SetOwner(ObjectPool<AutoGunBullet> NewOwner)
    {
        Owner = NewOwner;
    }

    // Update is called once per frame
    void Update()
    {
        if (Owner == null)
        {
            Debug.LogError("No owner associated with bullet!");
            Destroy(gameObject);
            return;
        }

        gameObject.transform.position += Velocity * Time.deltaTime;

        Vector3 pos = transform.position;
        if (pos.z != -5f)
        {
            pos.z = -5f;
            transform.position = pos;
        }

        if(gameObject.transform.position.x > 2000.0f)
        {
            Owner.Release(this);
            gameObject.transform.position = Vector3.zero;
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) {
            // Owner.Release(this);
            gameObject.transform.position = Vector3.zero;
            gameObject.SetActive(false);
        }
    }
}
