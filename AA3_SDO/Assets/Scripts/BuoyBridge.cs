using UnityEngine;

public class BuoyBridge : MonoBehaviour
{
    public enum WaterMode
    {
        Flat,
        Sinusoidal,
        Gerstner
    }

    [Header("Selected Water")]
    public WaterMode waterMode = WaterMode.Flat;

    [Header("Water References")]
    public SinusoidalWave sinusoidalWave;
    public GerstnerWaves gerstnerWave;

    [Header("Flat Water")]
    public float flatWaterLevel = 0f;

    private Arquimeds arquimeds;

    void Start()
    {
        arquimeds = GetComponent<Arquimeds>();
    }

    void Update()
    {
        if (arquimeds == null) return;

        float waveHeight = flatWaterLevel;

        if (waterMode == WaterMode.Sinusoidal && sinusoidalWave != null)
        {
            waveHeight = sinusoidalWave.GetWaveHeight(transform.position.x, transform.position.z);
        }
        else if (waterMode == WaterMode.Gerstner && gerstnerWave != null)
        {
            waveHeight = gerstnerWave.GetWaveHeight(transform.position.x, transform.position.z);
        }

        arquimeds.SetWaterLevel(waveHeight);
    }
}