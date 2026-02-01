// This is the code that spawn enemies.

using UnityEngine;
using System.Collections.Generic;
using System;


// Defines a specific spawn in time.
[System.Serializable]
public struct WaveSpawn
{
    [SerializeReference]
    public Wave wave;
    public float delay; // Time delay between the previous spawn (Keyframe) and this one. We dont put absolute time here because we might tweak/add/remove waves and we want everything following to naturally "shift".
}

public struct SpawnRange
{
    public float min, max;
    public float x;

    public SpawnRange(float _min, float _max, float _x)
    {
        min = _min;
        max = _max;
        x = _x;
    }
}


// Manager script that runs the waves and spawn the enemies
public class WaveManager : MonoBehaviour
{
    public List<WaveSpawn> pendingWaveSpawns = new List<WaveSpawn>();
    public GameObject maxSpawnLimit;
    public GameObject minSpawnLimit;

    private SpawnRange spawnRange;
    private float time;

    public Transform spawnParent;

    [SerializeField] Wave spawnStraightPrefab;
    [SerializeField] Wave spawnCirc2Prefab;
    [SerializeField] Wave spawnWave3Prefab;
    [SerializeField] Wave spawnSpreadWavePrefab;

    private void Start()
    {
        if (maxSpawnLimit != null && minSpawnLimit != null)
        {
            spawnRange = new SpawnRange(minSpawnLimit.transform.position.y, maxSpawnLimit.transform.position.y, minSpawnLimit.transform.position.x);
        }

        float diff = Gamemode.Instance.GetDifficulty();

        WaveSpawn ws = new WaveSpawn();
        ws.wave = spawnStraightPrefab;
        ws.delay = 2;
        pendingWaveSpawns.Add(ws);
        AddWaves(3, diff);
        
    }
    private void AddWaves(int waves, float diff) {
        for (int i = 0; i < waves; i++)
        {
            WaveSpawn ws = new WaveSpawn();
            ws.wave = spawnStraightPrefab;
            ws.delay = Mathf.Clamp(12 - diff, 4.0f, 11.0f);
            pendingWaveSpawns.Add(ws);
            Debug.Log("diff is " + diff);
            WaveSpawn ws2 = new WaveSpawn();
            ws2.wave = spawnCirc2Prefab;
            ws2.delay = Mathf.Clamp(12 - diff, 4.0f, 11.0f);
            pendingWaveSpawns.Add(ws2);

            WaveSpawn ws3 = new WaveSpawn();
            ws3.wave = spawnStraightPrefab;
            ws3.delay = Mathf.Clamp(12 - diff, 4.0f, 11.0f);
            pendingWaveSpawns.Add(ws3);

            WaveSpawn ws4 = new WaveSpawn();
            ws4.wave = spawnSpreadWavePrefab;
            ws4.delay = Mathf.Clamp(12 - diff, 4.0f, 11.0f);
            pendingWaveSpawns.Add(ws4);
        }
    }

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
                Wave wave = GameObject.Instantiate(nextWaveSpawn.wave, Vector3.zero, Quaternion.identity);
                wave.spawnRange = spawnRange;
            }
            else
            {
                pendingWaveSpawns[0] = nextWaveSpawn; // Ya I'm C# rusty... Structs are all by copies
            }
        }
        else if (pendingWaveSpawns.Count == 0)
        {
            AddWaves(3, Gamemode.Instance.GetDifficulty());
        }
    }
}
