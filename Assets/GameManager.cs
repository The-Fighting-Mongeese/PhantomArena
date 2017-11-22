
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour {

    public float gameDuration = 300f;   // seconds 

    public float timeLeft;

    public Text timeDisplay; 


	// Use this for initialization
	void Start ()
    {
        timeLeft = gameDuration;
	}
	
	// Update is called once per frame
	void Update ()
    {
        timeLeft -= Time.deltaTime;
        
        if (timeLeft <= 0)
        {
            timeLeft = 0;

            if (isServer)
            {
                GameOver();
            }
        }

        UpdateTimeDisplay();
	}


    void GameOver()
    {
        Debug.Log(PlayerManager.GetTopPlayer());
        UpdateTimeDisplay();
        this.enabled = false;
    }

    [Command]
    void CmdRetrieveCurrentTime()
    {
        RpcRetrieveCurrentTime(timeLeft);
    }

    [ClientRpc]
    void RpcRetrieveCurrentTime(float time)
    {
        timeLeft = time;
    }

    void UpdateTimeDisplay()
    {
        int secs = (int)(timeLeft % 60);
        int mins = (int)(timeLeft / 60);

        // can do a check here to return if seconds have not changed 

        var timeString = String.Format("{0:00}:{1:00}", mins, secs);
        timeDisplay.text = timeString;
    }

}
