using Unity.Burst.Intrinsics;
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

        Arquimeds arq = arquimeds;
        float queryX = arq != null ? arq.initialPosition.x : transform.position.x;
        float queryZ = arq != null ? arq.initialPosition.z : transform.position.z;

        if (waterMode == WaterMode.Sinusoidal && sinusoidalWave != null)
        {
            if (sinusoidalWave.IsWaveActive())
                waveHeight = sinusoidalWave.GetWaveHeight(transform.position.x, transform.position.z);
            else
                waveHeight = flatWaterLevel;
        }
        else if (waterMode == WaterMode.Gerstner && gerstnerWave != null)
        {
            if (gerstnerWave.IsWaveActive())
                waveHeight = gerstnerWave.GetWaveHeight(transform.position.x, transform.position.z);
            else
                waveHeight = flatWaterLevel;
        }

        arquimeds.SetWaterLevel(waveHeight);
    }
}