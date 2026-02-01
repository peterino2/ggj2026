using UnityEngine;

public class Wave : MonoBehaviour
{
    public float duration = 0; // How long this wave last. Default is just 1 frame
    public GameObject enemy; // Enemy to spawn
    public SpawnRange spawnRange;

    public virtual void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            GameObject.Destroy(gameObject);
        }
    }

    public float getRandomY()
    {
        return Random.Range(spawnRange.min, spawnRange.max);
    }

    public void SpawnRandom() // Same as spawn, but pick the Y position for you
    {
        Spawn(getRandomY());
    }

    public void Spawn(float yPos)
    {
        if (enemy == null) return; // Bruh...

        Vector3 spawnPosition = new Vector3(spawnRange.x, yPos);
        GameObject.Instantiate(enemy, spawnPosition, Quaternion.identity);
    }
}
