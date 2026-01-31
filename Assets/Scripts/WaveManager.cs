// This is the code that spawn enemies.

using UnityEngine;
using System.Collections.Generic;


// Defines a specific spawn in time.
[System.Serializable]
public struct WaveSpawn
{
    [SerializeReference]
    public Wave wave;
    public float delay; // Time delay between the previous spawn (Keyframe) and this one. We dont put absolute time here because we might tweak/add/remove waves and we want everything following to naturally "shift".
}


// Manager script that runs the waves and spawn the enemies
public class WaveManager : MonoBehaviour
{
    public List<WaveSpawn> pendingWaveSpawns = new List<WaveSpawn>();

    private float time;

    private void Update()
    {
        // Spawn new waves
        if (pendingWaveSpawns.Count > 0)
        {
            WaveSpawn nextWaveSpawn = pendingWaveSpawns[0];
            nextWaveSpawn.delay -= Time.deltaTime;
            if (nextWaveSpawn.delay <= 0)
            {
                pendingWaveSpawns.RemoveAt(0);

                // Once a wave is activated, its just a GameObject that exists until it has spewed everyone.
                GameObject.Instantiate(nextWaveSpawn.wave, Vector3.zero, Quaternion.identity);
            }
            else
            {
                pendingWaveSpawns[0] = nextWaveSpawn; // Ya I'm C# rusty... Structs are all by copies
            }
        }
    }
}
