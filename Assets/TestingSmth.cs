using System.Collections;
using System.Collections.Generic;
using System.Numerics;

using UnityEngine;

public class TestingSmth : MonoBehaviour
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
    public AudioSource audSource1;

    public GameObject audSourceObject2;
    public AudioSource audSource2;


    public float sampleRate = 44100;
    public float waveLengthInSeconds = 2.0f;
    int timeIndex = 0;

    float phase = 0;


    void Start()

    {
        numberOfBins = 64;
        spectrumData = new float[numberOfBins];
        frequencies = new float[numberOfBins];
        audSource1 = audSourceObject1.GetComponent<AudioSource>();
        audSource2 = audSourceObject2.GetComponent<AudioSource>();
    }

    void Update()
    {
        audSource1.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);
        numSamples = (int)(sampleRate * audSource2.clip.channels);
        UpdateFrequencies();
    }

    
    void UpdateFrequencies()
    {

        for (int i = 0; i < frequencies.Length; i++)
        {
            frequencies[i] = gameLogic.getFrequencyBin(spectrumData.Length, i) + Mathf.Sin(Time.time) * 100.0f;
            frequencies[i] = Mathf.Max(0, frequencies[i]);
        }

        numSamples = (int)(sampleRate * audSource2.clip.channels);
        samples = new float[numSamples];

        for (int i = 0; i < frequencies.Length; i++)
        {
            float increment = frequencies[i] * 2 * Mathf.PI / sampleRate;
            float phase = 0;

            for (int j = 0; j < numSamples; j++)
            {
                float sample = spectrumData[i] * 3000000 * Mathf.Sin(phase);
                samples[j] += sample;

                // Update the phase for the next sample
                phase += increment;
            }
        }

        audSource2.clip = AudioClip.Create("DynamicWaves", numSamples, audSource2.clip.channels, (int)sampleRate, false);
        audSource2.clip.SetData(samples, 0);
        audSource2.Play();


    }
    

    /*
    void OnAudioFilterRead(float[] data, int channels)
    {
        Debug.Log("Check");
        samples = new float[numSamples];

        for (int i = 0; i < frequencies.Length; i++)
        {
            frequencies[i] = gameLogic.getFrequencyBin(spectrumData.Length, i) + Mathf.Sin(Time.time) * 100.0f;
            frequencies[i] = Mathf.Max(0, frequencies[i]);
        }


        for (int i = 0; i < frequencies.Length; i++)
        {

            for (int j = 0; j < data.Length; j += channels)
            {


                phase += 2 * Mathf.PI * frequencies[i] / sampleRate;

                data[j] += spectrumData[i] * Mathf.Sin(phase);

                if (phase >= 2 * Mathf.PI)
                {
                    phase -= 2 * Mathf.PI;
                }
            }
        }

    
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
*/

}
