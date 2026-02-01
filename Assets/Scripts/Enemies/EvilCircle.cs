using UnityEngine;

public class EvilCircle : MonoBehaviour
{
    public float speed = 250;

    public void Update()
    {
        transform.position = transform.position + Vector3.left * speed * Time.deltaTime;
        
        Vector3 pos = transform.position;
        if (pos.z != -10f)
        {
            pos.z = -10f;
            transform.position = pos;
        }
    }
}
