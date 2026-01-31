using UnityEngine;

public class Wave : MonoBehaviour
{
    public float duration = 0; // How long this wave last. Default is just 1 frame
    public GameObject enemy; // Enemy to spawn

    public virtual void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            GameObject.Destroy(gameObject);
        }
    }

    public void SpawnRandom() // Same as spawn, but pick the Y position for you
    {
        float spawnHalfRange = 1080 / 2 - 50;
        Spawn(Random.Range(-spawnHalfRange, spawnHalfRange));
    }

    public void Spawn(float yPos)
    {
        if (enemy == null) return; // Bruh...

        Vector3 spawnPosition = new Vector3(1920 / 2, yPos);
        GameObject.Instantiate(enemy, spawnPosition, Quaternion.identity);
    }
}
