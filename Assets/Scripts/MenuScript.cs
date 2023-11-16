using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour
{

    public string songType;
    public string modeType;

    //to be given after the game
    public float finalScore;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void SetParty()
    {
        songType = "Party";
    }
    public void SetSorrow()
    {
        songType = "Sorrow";
    }

    public void SetWar()
    {
        songType = "War";
    }

    public void SetWisdom()
    {
        songType = "Wisdom";
    }

    public void TestButton()
    {
        Debug.Log("This button works.");
    }


    public void SetAlone()
    {
        modeType = "Alone";
    }
    public void SetTogether()
    {
        modeType = "Together";
    }






}
