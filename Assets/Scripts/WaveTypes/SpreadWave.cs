// Probably the most common time of wave.
// Enemies spawn at random location and at a certain frequency.

using UnityEngine;

public class SpreadWave : Wave
{
    public float frequency = 1; // In seconds. 0.1 means 10 enemies per second.

    private float spawnDelay = 0;

    public override void Update()
    {
        spawnDelay -= Time.deltaTime;
        while (spawnDelay < 0)
        {
            spawnDelay += frequency;
            SpawnRandom();
        }

        base.Update();
    }
}
