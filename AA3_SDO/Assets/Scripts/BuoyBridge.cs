using UnityEngine;

public class BuoyBridge : MonoBehaviour
{
    public SinusoidalWave sinusoidalWave;

    public GerstnerWaves gerstnerWave;

    private Arquimeds arquimeds;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        arquimeds = GetComponent<Arquimeds>();
    }

    // Update is called once per frame
    void Update()
    {
        if (arquimeds == null) return;

        float waveHeight = 0f;

        if (sinusoidalWave != null)
            waveHeight = sinusoidalWave.GetWaveHeight(transform.position.x, transform.position.z);
        else if (gerstnerWave != null)
            waveHeight = gerstnerWave.GetWaveHeight(transform.position.x, transform.position.z);

        arquimeds.SetWaterLevel(waveHeight);

    }
}
