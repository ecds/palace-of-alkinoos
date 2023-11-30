using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SongGameHandler : MonoBehaviour
{

    //variables

    public float score = 0;
    private float p1Score = 0, p2Score = 0;
    public GameObject scoreText, p1ScoreText, p2ScoreText;

    public string currLyrics;
    public GameObject lyricsText, instructionsText;
    public bool gameStarted = false;
    public bool singlePlayer;


    public GameObject menu;


    //public string[] lyrics;
    public List<string> lyrics = new List<string>();

    private AudioSource adSource;
    
    public AudioSource cSong;

    //public AudioClip[] adClips;
    public List<AudioClip> adClips = new List<AudioClip>();
    public int numTimesPlayed = 0;

    private bool whichPlayer = false;

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

        switch (modeHolder.modeType)
        {
            case "Alone":
                singlePlayer = true;
                break;
            case "Together":
                singlePlayer = false;
                break;
        }

        switch (modeHolder.songType)
        {
            case "Party":
                whatSong = partySource;
                cSong = GameObject.Find("Suitors Are Gone").GetComponent<AudioSource>();
                break;
            case "Sorrow":
                whatSong = sorrowSouce;
                cSong = GameObject.Find("Cyclops Sadness").GetComponent<AudioSource>();
                break;
            case "War":
                whatSong = warSource;
                cSong = GameObject.Find("Suitors Are Gone").GetComponent<AudioSource>();
                break;
            case "Wisdom":
                whatSong = wisdomSource;
                cSong = GameObject.Find("Cyclops Sadness").GetComponent<AudioSource>();
                break;
        }

        cSong.Play();
        

        //Pull a list of lyrics and their assossiated sounds.
        foreach (Transform child in whatSong.transform)
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
                    if (singlePlayer)
                    {
                        PlayNextClip(true);
                    }
                    if (!singlePlayer)
                    {
                        //this, in multiplayer, lets you play the same clip twice, setting which player has a higher score.
                        if (!whichPlayer)
                        {
                            PlayNextClip(false);
                        }
                        else
                        {
                            PlayNextClip(true);
                        }
                    }
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
                if (singlePlayer)
                {
                    lyricsText.GetComponentInChildren<Text>().text = "Press E to start.";
                    scoreText.GetComponentInChildren<Text>().text = "Score: " + score;
                }
                if (!singlePlayer)
                {
                    if (!whichPlayer)
                    {
                        p1ScoreText.GetComponentInChildren<Text>().text = "Score: " + p1Score;
                    }                    
                    else
                    {
                        p2ScoreText.GetComponentInChildren<Text>().text = "Score: " + p2Score;
                    }

                    whichPlayer = !whichPlayer;

                }
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
        cSong.Stop();

        var scoreHolder = menu.GetComponent<MenuScript>();
        scoreHolder.finalScore = score;
        if (!singlePlayer)
        {
            scoreHolder.p1FinalScore = p1Score;
            scoreHolder.p2FinalScore = p2Score;
        }


        winScreen.SetActive(true);

        if (singlePlayer)
        {
            winScreen.GetComponentInChildren<Text>().text = "Your score is: " + scoreHolder.finalScore;
        }
        else
        {
            string whowonyoudecide = "";
            if (p1Score > p2Score)
            {
                whowonyoudecide = "Player 1 wins!";
            }
            else
            {
                whowonyoudecide = "Player 2 wins!";
            }
            winScreen.GetComponentInChildren<Text>().text = "Player 1's score is: " + scoreHolder.p1FinalScore +
                "\nPlayer 2's score is: " + scoreHolder.p2FinalScore + "\n"+ whowonyoudecide;
        }


        //cleanup
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
        
        if (!whichPlayer)
        {
            p1Score += newScore;
        }
        if (whichPlayer)
        {
            p2Score += newScore;
        }


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


