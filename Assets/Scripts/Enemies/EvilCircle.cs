using UnityEngine;

public class EvilCircle : MonoBehaviour
{
    public float speed = 250;

    public void Update()
    {
        transform.position = transform.position + Vector3.left * speed * Time.deltaTime;
    }
}
