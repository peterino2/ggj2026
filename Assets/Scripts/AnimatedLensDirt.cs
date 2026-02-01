using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AnimatedLensDirt : MonoBehaviour
{
    public Volume volume;
    public Texture2D[] dirtTextures;
    public float swapInterval = 0.1f;
    public float dirtIntensityMin = 5f;
    public float dirtIntensityMax = 10f;
    public float intensityPulseSpeed = 1f;
    
    private Bloom bloom;
    private int currentIndex = 0;
    private float timer = 0f;

    private void Start()
    {
        if (volume != null && volume.profile != null)
        {
            volume.profile.TryGet(out bloom);
        }
    }

    private void Update()
    {
        if (bloom == null) return;

        // Swap textures over time
        if (dirtTextures != null && dirtTextures.Length > 1)
        {
            timer += Time.deltaTime;
            if (timer >= swapInterval)
            {
                timer = 0f;
                currentIndex = (currentIndex + 1) % dirtTextures.Length;
                bloom.dirtTexture.value = dirtTextures[currentIndex];
            }
        }

        // Pulse dirt intensity
        float pulse = Mathf.Lerp(dirtIntensityMin, dirtIntensityMax, 
            (Mathf.Sin(Time.time * intensityPulseSpeed) + 1f) * 0.5f);
        bloom.dirtIntensity.value = pulse;
    }

    public void SetDirtIntensity(float intensity)
    {
        if (bloom != null)
        {
            bloom.dirtIntensity.value = intensity;
        }
    }
}
