// Probably the most common time of wave.
// Enemies spawn at random location and at a certain frequency.

using UnityEngine;

[CreateAssetMenu(fileName = "NewWave", menuName = "Waves/Spread")]
public class SpreadWave : Wave
{
    public float frequency = 1; // In seconds. 0.1 means 10 enemies per second.

    private float spawnDelay = 0;

    public override bool Update()
    {
        spawnDelay -= Time.deltaTime;
        while (spawnDelay < 0)
        {
            spawnDelay += frequency;
            SpawnRandom();
        }
        
        return false;
    }
}
