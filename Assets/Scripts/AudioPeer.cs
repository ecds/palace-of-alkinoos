using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioPeer : MonoBehaviour
{
    AudioSource _audioSource;
    public AudioClip _audioClip;
//mic stuff
    public bool _useMicrophone;
    public string _selectedDevice;
    public AudioMixerGroup _mixerGroupMicrophone, _mixerGroupMaster;



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
        _audioSource = GetComponent<AudioSource> ();
        _audioSource.enabled = false;
        if(_useMicrophone)
        {
            if(Microphone.devices.Length > 0)
            {
                _selectedDevice = Microphone.devices[0].ToString();
                _audioSource.clip = Microphone.Start(_selectedDevice, true, 10, AudioSettings.outputSampleRate);
                _audioSource.volume = 1f;
                _audioSource.outputAudioMixerGroup = _mixerGroupMicrophone;
            }
            else
            {
                _useMicrophone = false;
            }
        }
        if(!_useMicrophone)
        {
            _audioSource.clip = _audioClip;
            _audioSource.volume = .1f;
            _audioSource.outputAudioMixerGroup = _mixerGroupMaster;
        }
        _audioSource.enabled = true;
        _audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        BandBuffer();
        CreateAudioBands();
    }

    void CreateAudioBands()
    {
        for(int i = 0; i < 8; i++)
        {
            if(_freqBand[i] > _freqBandHighest[i])
            {
                _freqBandHighest[i] = _freqBand[i];
            }
            _audioBand[i] = (_freqBand[i] / _freqBandHighest[i]);
            _audioBandBuffer[i] = (_bandBuffer[i] / _freqBandHighest[i]);
        }
    }

    void GetSpectrumAudioSource()
    {
        _audioSource.GetSpectrumData (_samples, 0, FFTWindow.Blackman);
    }

    void BandBuffer()
    {
        for(int g = 0; g < 8; ++g)
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
        for(int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i + 1);

            if(i == 7)
                sampleCount +=2;

            for(int j = 0; j < sampleCount; j++)
            {
                average += _samples[count] * (count + 1);
                count++;
            }

            average /=count;
            _freqBand[i] = average * 10;
        }
    }
}
