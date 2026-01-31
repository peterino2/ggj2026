using UnityEngine;

public class Wave : ScriptableObject
{
    public float duration = 0; // How long this wave last. Default is just 1 frame
    public GameObject enemy; // Enemy to spawn

    public virtual bool Update() // Return true if you want the wave to stop.
    {
        return true;
    }

    public void SpawnRandom() // Same as spawn, but pick the Y position for you
    {
        Spawn(Random.Range(50, 1080 - 50));
    }

    public void Spawn(float yPos)
    {
        if (enemy == null) return; // Bruh...

        Vector3 spawnPosition = new Vector3(1920 + 100, yPos);
        GameObject.Instantiate(enemy, spawnPosition, Quaternion.identity);
    }
}
