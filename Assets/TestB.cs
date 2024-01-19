using System.Collections;
using System.Collections.Generic;
using System.Numerics;


using UnityEngine;

public class TestB : MonoBehaviour
{
    public GameLogic gameLogic;

    public int startBin = 0; // Start index of the frequency bins you want to filter
    public int endBin = 256;  // End index of the frequency bins you want to filter
    public float attenuationFactor = 1f; // Adjust this factor to control the amount of attenuation

    public float[] spectrumData;
    public float[] samples;
    public float[] frequencies;
    public int numSamples;
    public int numberOfBins;

    public GameObject audSourceObject1;
    private AudioSource audSource1;

    public GameObject audSourceObject2;
    private AudioSource audSource2;


    public int sampleRate = 44100;
    public float waveLengthInSeconds = 2.0f;
    int timeIndex = 0;

    float phase = 0;

    public float amplitude = 0.5f;
    public float frequency = 440.0f;


    void Start()

    {
        numberOfBins = 64;
        spectrumData = new float[numberOfBins];
        frequencies = new float[numberOfBins];
        audSource1 = audSourceObject1.GetComponent<AudioSource>();
        audSource2 = audSourceObject2.GetComponent<AudioSource>();;
        audSource2.spatialBlend = 0; //force 2D sound
    }

    void FixedUpdate()
    {
        audSource1.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

        audSource2.Play();

    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        frequency = gameLogic.getFrequencyBin(spectrumData.Length, 0);

        for (int i = 0; i < data.Length; i += channels)
        {
            phase += 2 * Mathf.PI * frequency / sampleRate;

            data[i] = Mathf.Sin(phase);

            if (phase >= 2 * Mathf.PI)
            {
                phase -= 2 * Mathf.PI;
            }
        }
    }



    void GenerateSineWave()
    {
        int numSamples = (int)(sampleRate * audSource2.clip.channels);
        float[] samples = new float[numSamples];

        float increment = frequency * 2 * Mathf.PI / sampleRate;

        for (int i = 0; i < numSamples; i++)
        {
            float time = i / (float)sampleRate;
            samples[i] = amplitude * Mathf.Sin(2 * Mathf.PI * frequency * time);
        }

        audSource2.clip = AudioClip.Create("SineWave", numSamples, audSource2.clip.channels, (int)sampleRate, false);
        audSource2.clip.SetData(samples, 0);
        audSource2.Play();
    }

    void UpdateFrequencies()
    {

        frequencies[0] = gameLogic.getFrequencyBin(spectrumData.Length, 0) + Mathf.Sin(Time.time) * 100.0f;

        numSamples = (int)(sampleRate * audSource2.clip.channels);
        samples = new float[numSamples];

        float increment = frequencies[0] * 2 * Mathf.PI / sampleRate;
        float phase = 0;

            for (int j = 0; j < numSamples; j++)
            {
                float sample = spectrumData[0] * 3000000 * Mathf.Sin(phase);
                samples[j] += sample;

                // Update the phase for the next sample
                phase += increment;
            }
      

        audSource2.clip = AudioClip.Create("DynamicWaves", numSamples, audSource2.clip.channels, (int)sampleRate, false);
        audSource2.clip.SetData(samples, 0);
        audSource2.Play();


    }
}
