
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Handles how long the game will last. This is to be placed on an object in scene - not on a player.
/// </summary>
public class GameManager : NetworkBehaviour {

    public bool initializeDatabase = true;

    public float gameDuration = 300f;   // seconds 

    public float timeLeft;

    public Text timeDisplay;

    public GameObject scoreboard;

    private NetworkClient client;

    private const short UpdateTimeLeftMsgCode = 420;


    void Awake()
    {
        if (initializeDatabase)
            DataService.InitializeDbConnection("PhantomArena.db");
    }

    void Start ()
    {
        if (isServer)
        {
            NetworkServer.RegisterHandler(UpdateTimeLeftMsgCode, OnServerReceiveGetTimeLeftMsg);
            timeLeft = gameDuration;
        }

        client = NetworkManagerCustom.singleton.client;
        client.RegisterHandler(UpdateTimeLeftMsgCode, OnClientReceiveGetTimeLeftMsg);
        SendGetTimeLeftMsg();
    }

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
        Debug.Log("Is server " + isServer);
        RpcGameOver();
    }

    [ClientRpc]
    void RpcGameOver()
    {
        Debug.Log("RpcGameOver");

        StartCoroutine(EndGameAfterDelay(5f));

        scoreboard.SetActive(true);
        this.enabled = false;
    }

    void UpdateTimeDisplay()
    {
        int secs = (int)(timeLeft % 60);
        int mins = (int)(timeLeft / 60);

        // can do a check here to return if seconds have not changed 

        var timeString = String.Format("{0:00}:{1:00}", mins, secs);
        timeDisplay.text = timeString;
    }

    IEnumerator EndGameAfterDelay(float time)
    {
        yield return new WaitForSeconds(time);

        if (isServer)
        {
            Debug.Log("FOUND SERVER");
            NetworkManagerCustom.singleton.StopHost();
        }

        // NUKE EVERYTHING AND RESTART.
        Destroy(NetworkManagerCustom.singleton.gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #region Network handlers 

    public void SendGetTimeLeftMsg()
    {
        Debug.Log("Client sending time left message request.");
        var msg = new EmptyMessage();
        client.Send(UpdateTimeLeftMsgCode, msg);
    }

    void OnServerReceiveGetTimeLeftMsg(NetworkMessage msg)
    {
        Debug.Log("Server received time left message request from: " + msg.conn.connectionId);
        var clientMsg = new IntegerMessage((int)timeLeft);
        NetworkServer.SendToClient(msg.conn.connectionId, UpdateTimeLeftMsgCode, clientMsg);
    }

    void OnClientReceiveGetTimeLeftMsg(NetworkMessage msg)
    {
        var time = msg.ReadMessage<IntegerMessage>().value;
        timeLeft = time;
        Debug.Log("Client received time left response: " + time);
    }

    #endregion
}
