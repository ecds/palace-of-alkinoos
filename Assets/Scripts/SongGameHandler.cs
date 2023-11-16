using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SongGameHandler : MonoBehaviour
{

    //variables

    public float score = 0;
    public GameObject scoreText;

    public string currLyrics;
    public GameObject lyricsText, instructionsText;
    public bool gameStarted = false;


    public GameObject menu;


    //public string[] lyrics;
    public List<string> lyrics = new List<string>();

    private AudioSource adSource;

    //public AudioClip[] adClips;
    public List<AudioClip> adClips = new List<AudioClip>();
    public int numTimesPlayed = 0;

    public GameObject audioPeererMic, audioPeererSong;

    //rename this
    public AudioPeerColton peerScriptMic;
    public AudioPeerSong peerScriptSong;


    public GameObject loseScreen, winScreen, gameUI;


    public GameObject partySource, sorrowSouce, warSource, wisdomSource;
    private GameObject whatSong;


    //temp to remove infinity from score
    public int timesDone = 0;

    //Remove this after testing
    public float timeTilNextAudio;




    public bool playNext = true;
    public bool isListening = false;

    // Start is called before the first frame update
    void Start()
    {
        adSource = GetComponent<AudioSource>();


        var modeHolder = menu.GetComponent<MenuScript>();
        peerScriptMic = audioPeererMic.GetComponent<AudioPeerColton>();
        peerScriptSong = audioPeererSong.GetComponent<AudioPeerSong>();


        switch (modeHolder.songType)
        {
            case "Party":
                whatSong = partySource;
                break;
            case "Sorrow":
                whatSong = sorrowSouce;
                break;
            case "War":
                whatSong = warSource;
                break;
            case "Wisdom":
                whatSong = wisdomSource;
                break;
        }


        

        //Pull a list of lyrics and their assossiated sounds.
        foreach (Transform child in partySource.transform)
        {
            lyrics.Add(child.GetComponent<Text>().text);
            adClips.Add(child.GetComponent<AudioSource>().clip);
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (!gameStarted)
        {
            
        }
        if (gameStarted == false && Input.GetKeyDown(KeyCode.E))
        {


            gameStarted = true;
            PlayNextClip(false);
            instructionsText.SetActive(true);
            instructionsText.GetComponentInChildren<Text>().text = "Listen to what is said. Press Space to speak it back.";
        }

        if (gameStarted)
        {
            if (Input.GetKeyDown(KeyCode.Space) && isListening == false)
            {
                //Start listening
                peerScriptMic.ResetVars(true);
                peerScriptSong.ResetVars(true);

                //
                if (numTimesPlayed < adClips.Count)
                {
                    PlayNextClip(true);
                }


                Debug.Log("Listening");
                isListening = true;
                instructionsText.GetComponentInChildren<Text>().text = "Repeat what is said as closely as you can. Press Space to finish.";


            }
            else if (Input.GetKeyDown(KeyCode.Space) && isListening == true)
            {
                playNext = true;
                
                peerScriptMic.NoRecord();
                peerScriptSong.NoRecord();
                isListening = false;

                instructionsText.GetComponentInChildren<Text>().text = "Listen to what is said. Press Space to speak it back.";
                lyricsText.GetComponentInChildren<Text>().text = "Press E to start.";
                scoreText.GetComponentInChildren<Text>().text = "Score: " + score;
            }

            if (isListening) 
            {
                CompareFreq();

            }

            if (playNext && numTimesPlayed < adClips.Count)
            {
                PlayNextClip(false);
            }

            if (playNext && numTimesPlayed >= adClips.Count)
            {
                FinishGame();
            }


        }


    }


    void PlayNextClip(bool increment)
    {

        adSource.clip = adClips[numTimesPlayed];
        lyricsText.GetComponent<Text>().text = lyrics[numTimesPlayed];


        adSource.Play();
        peerScriptSong.PlayClip(numTimesPlayed);
        if (increment)
        {
            numTimesPlayed++;
        }

        playNext = false;
    }

    void PitchToScore()
    {
        // Get the difference from the player's pitch to the targeted pitch.
        //Perhaps done after the player speaks, or during.
    }



    void FinishGame()
    {
        var scoreHolder = menu.GetComponent<MenuScript>();
        scoreHolder.finalScore = score;

        winScreen.SetActive(true);

        winScreen.GetComponentInChildren<Text>().text = "Your score is: " + scoreHolder.finalScore;

        gameUI.SetActive(false);
        gameObject.SetActive(false);

        audioPeererSong.GetComponent<AudioSource>().clip = null;

        numTimesPlayed = 0;
        score = 0;
        scoreText.GetComponentInChildren<Text>().text = "Score: ";
        gameStarted = false;


    }

    void CompareFreq()
    {
        float newScore = 0;
        
        float[] songFreq = peerScriptSong.Get_FreqBand();
        float[] micFreq = peerScriptMic.Get_FreqBand();

        //Math to add to score
        //Higher the value, the more the closeness matters.

        for (int i = 0; i < songFreq.Length; i++)
        {

            float partScore = Mathf.Abs(songFreq[i] - micFreq[i]);

            //if (timesDone > 2)
            {
                partScore += partScore;

                Debug.Log(newScore += partScore / 1000);
            }


        }
        Mathf.Clamp01(newScore);
        score += newScore;



        //this one takes a long time. also broken
        /*
        AudioClip clip1 = peerScriptMic.GetAudio();
        AudioClip clip2 = peerScriptSong.GetAudio();

        clip1 = ResampleAudioClip(ref clip1, clip2);

        float[] samples1 = new float[clip1.samples];
        float[] samples2 = new float[clip2.samples];

        Debug.Log(samples1.Length + "     " +  samples2.Length);

        float threshold = 0.1f;

        clip1.GetData(samples1, 0);
        clip2.GetData(samples2, 0);

        float maxDifference = 0;

        adSource.clip = clip1;
        adSource.Play();

        // Compare the audio samples
        for (int i = 0; i < samples2.Length; i++)
        {
            float difference = Mathf.Abs(samples1[i] - samples2[i]);
            Debug.Log(difference);

            newScore += difference;



        }

        newScore = newScore / samples2.Length;
        score = 1.0f - Mathf.Clamp01(newScore / threshold);
        */

    }

    /*
    private AudioClip ResampleAudioClip(ref AudioClip audioClip, AudioClip referenceClip)
    {
        int desiredSampleCount = referenceClip.samples;
        float[] data = new float[desiredSampleCount];
        audioClip.GetData(data, 0);
        audioClip = AudioClip.Create("ResampledClip", desiredSampleCount, audioClip.channels, audioClip.frequency, false);
        audioClip.SetData(data, 0);
        return audioClip;
    }*/



    //temp example

    /*
    IEnumerator playAudioSequentially()
    {
        yield return null;

        //1.Loop through each AudioClip
        for (int i = 0; i < adClips.Length; i++)
        {
            //2.Assign current AudioClip to audiosource
            adSource.clip = adClips[i];

            //3.Play Audio
            adSource.Play();

            //4.Wait for it to finish playing
            while (adSource.isPlaying)
            {
                yield return null;
            }

            //5. Go back to #2 and play the next audio in the adClips array
        }
    }
    */
}


