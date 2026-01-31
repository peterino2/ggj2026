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

    private List<Wave> activeWaves = new List<Wave>();
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
                activeWaves.Add(nextWaveSpawn.wave);
            }
        }

        // Update active waves
        for (int i = 0; i < activeWaves.Count; ++i)
        {
            Wave wave = activeWaves[i];
            wave.duration -= Time.deltaTime;
            if (wave.Update() || wave.duration < 0)
            {
                activeWaves.RemoveAt(i);
                --i;
            }
        }
    }
}
