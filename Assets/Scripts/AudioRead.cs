using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioRead : MonoBehaviour
{

    AudioSource audioSource;


    public AudioClip audioClip;
    public bool useMicrophone;

    public string selectedDevice;
    // Start is called before the first frame update
    void Start()
    {
        if (useMicrophone) 
        {
            selectedDevice = Microphone.devices[0].ToString();
            audioSource.clip = Microphone.Start(null, true, 10, AudioSettings.outputSampleRate);
        }
        else 
        {
            useMicrophone = false; 
        }


        if (!useMicrophone)
        {
            //audioSource.clip = audioClip;
        }
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
