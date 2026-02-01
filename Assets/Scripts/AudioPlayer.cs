using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    AutoGun,
    Railgun,
    Laser,
    Missile,
    Hit,
    Explosion
}

[System.Serializable]
public class SoundConfig
{
    public SoundType soundType;
    public AudioClip clip;
    public int maxVoices = 5;
    public float minInterval = 0.05f;
    public float volumeMin = 0.8f;
    public float volumeMax = 1.0f;
    public float pitchMin = 0.95f;
    public float pitchMax = 1.05f;
}

public class AudioPlayer : MonoBehaviour
{
    public static AudioPlayer Instance { get; private set; }

    public SoundConfig[] soundConfigs;

    private Dictionary<SoundType, AudioSource[]> audioSourcePools = new Dictionary<SoundType, AudioSource[]>();
    private Dictionary<SoundType, int> currentVoiceIndex = new Dictionary<SoundType, int>();
    private Dictionary<SoundType, float> lastPlayTime = new Dictionary<SoundType, float>();
    private Dictionary<SoundType, SoundConfig> configLookup = new Dictionary<SoundType, SoundConfig>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializePools();
    }

    private void InitializePools()
    {
        foreach (var config in soundConfigs)
        {
            configLookup[config.soundType] = config;
            
            AudioSource[] sources = new AudioSource[config.maxVoices];
            for (int i = 0; i < config.maxVoices; i++)
            {
                GameObject sourceObj = new GameObject($"AudioSource_{config.soundType}_{i}");
                sourceObj.transform.SetParent(transform);
                AudioSource source = sourceObj.AddComponent<AudioSource>();
                source.clip = config.clip;
                source.playOnAwake = false;
                source.spatialBlend = 0f;
                sources[i] = source;
            }
            
            audioSourcePools[config.soundType] = sources;
            currentVoiceIndex[config.soundType] = 0;
            lastPlayTime[config.soundType] = -999f;
        }
    }

    public void Play(SoundType soundType, float volumeMultiplier = 1f)
    {
        if (!configLookup.TryGetValue(soundType, out SoundConfig config))
        {
            Debug.LogWarning($"Sound type {soundType} not configured");
            return;
        }

        float timeSinceLastPlay = Time.time - lastPlayTime[soundType];
        if (timeSinceLastPlay < config.minInterval)
        {
            return;
        }

        lastPlayTime[soundType] = Time.time;

        AudioSource[] sources = audioSourcePools[soundType];
        int voiceIndex = currentVoiceIndex[soundType];
        AudioSource source = sources[voiceIndex];

        currentVoiceIndex[soundType] = (voiceIndex + 1) % config.maxVoices;

        float volume = Random.Range(config.volumeMin, config.volumeMax) * volumeMultiplier;
        float pitch = Random.Range(config.pitchMin, config.pitchMax);

        source.volume = volume;
        source.pitch = pitch;
        source.Play();
    }
}
