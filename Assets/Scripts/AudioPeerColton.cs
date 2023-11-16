using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioPeerColton : MonoBehaviour
{
    AudioSource _audioSource, gameAudioSource;
    public AudioClip _audioClip, gameAudioClip;
    //mic stuff
    public bool _useMicrophone;
    public string _selectedDevice;
    public AudioMixerGroup _mixerGroupMicrophone, _mixerGroupMaster;

    public bool doRecord = false;



    public static float[] _samples = new float[512];

    //use freqBand and bandBuffer for values 0 - (>1) for all 8 frequencies (Check ParamCube for example of use)
    public static float[] _freqBand = new float[8];

    public static float[] _bandBuffer = new float[8];
    private float[] _bufferDecrease = new float[8];

    private float[] _freqBandHighest = new float[8];

    //use audioBand or audioBandBuffer for values 0 - 1 for all 8 frequencies
    public static float[] _audioBand = new float[8];
    public static float[] _audioBandBuffer = new float[8];

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.enabled = false;
        ResetVars(false);
        
    }

    public float[] Get_FreqBand()
    {
        return _audioBandBuffer;
    }

    // Update is called once per frame
    void Update()
    {
        if (doRecord)
        {
            GetSpectrumAudioSource();
            MakeFrequencyBands();
            BandBuffer();
            CreateAudioBands();
            //Debug.Log("AAA: " + _freqBand[4]);
        }
        if (!doRecord)
        {
            _audioSource.enabled = false;
        }

    }

    public AudioClip GetAudio()
    {
        return _audioSource.clip;
    }


    public void ComparePitch()
    {
        for (int i = 0; i < 8; i++)
        {
            if (_freqBand[i] > _freqBandHighest[i])
            {
                _freqBandHighest[i] = _freqBand[i];
            }
        }


    }

    void CreateAudioBands()
    {
        for (int i = 0; i < 8; i++)
        {
            if (_freqBand[i] > _freqBandHighest[i])
            {
                _freqBandHighest[i] = _freqBand[i];
            }
            _audioBand[i] = (_freqBand[i] / _freqBandHighest[i]);
            _audioBandBuffer[i] = (_bandBuffer[i] / _freqBandHighest[i]);
        }
    }

    void GetSpectrumAudioSource()
    {
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
    }

    void BandBuffer()
    {
        for (int g = 0; g < 8; ++g)
        {
            if (_freqBand[g] > _bandBuffer[g])
            {
                _bandBuffer = _freqBand;
                _bufferDecrease[g] = 0.005f;
            }

            if (_freqBand[g] < _bandBuffer[g])
            {
                _bufferDecrease[g] = (_bandBuffer[g] - _freqBand[g]) / 8;
                _bandBuffer[g] -= _bufferDecrease[g];
                _bufferDecrease[g] *= 1.2f;
            }
        }
    }

    void MakeFrequencyBands()
    {
        int count = 0;
        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i + 1);

            if (i == 7)
                sampleCount += 2;

            for (int j = 0; j < sampleCount; j++)
            {
                average += _samples[count] * (count + 1);
                count++;
            }

            average /= count;
            _freqBand[i] = average * 10;
        }
    }

    public void NoRecord()
    {
        doRecord = false;
        Debug.Log("Do not record anymore");
    }

    public void ResetVars(bool record)
    {
        _samples = new float[512];
        if (record)
        {
            doRecord = true;
        }

    //use freqBand and bandBuffer for values 0 - (>1) for all 8 frequencies (Check ParamCube for example of use)
        _freqBand = new float[8];

        _bandBuffer = new float[8];
        _bufferDecrease = new float[8];

        _freqBandHighest = new float[8];

    //use audioBand or audioBandBuffer for values 0 - 1 for all 8 frequencies
        _audioBand = new float[8];
        _audioBandBuffer = new float[8];


        if (_useMicrophone)
        {
            Debug.Log("Devices:");
            foreach (var device in Microphone.devices)
            {
                Debug.Log("Name: " + device);
            }
            if (Microphone.devices.Length > 0)
            {

                _selectedDevice = Microphone.devices[0].ToString();
                _audioSource.clip = Microphone.Start(_selectedDevice, true, 10, AudioSettings.outputSampleRate);
                _audioSource.volume = 0f;
                _audioSource.outputAudioMixerGroup = _mixerGroupMicrophone;


                
            }
            else
            {
                _useMicrophone = false;
            }
        }
        if (!_useMicrophone)
        {
            _audioSource.clip = _audioClip;
            _audioSource.volume = .1f;
            _audioSource.outputAudioMixerGroup = _mixerGroupMaster;
        }
        _audioSource.enabled = true;
        _audioSource.Play();
    }
}
