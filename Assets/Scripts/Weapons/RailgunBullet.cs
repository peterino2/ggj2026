using UnityEngine;

public class RailgunBullet : MonoBehaviour
{
    public Vector3 Velocity { get; set; }

    public float Damage { get; set; }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position += Velocity * Time.deltaTime;

        Vector3 pos = transform.position;
        if (pos.z != -5f)
        {
            pos.z = -5f;
            transform.position = pos;
        }

        if (gameObject.transform.position.x > 2000.0f)
        {
            gameObject.transform.position = Vector3.zero;
            gameObject.SetActive(false);
        }
    }
}
