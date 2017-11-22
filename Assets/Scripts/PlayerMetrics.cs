using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMetrics : NetworkBehaviour {

    public int netID;
    [SyncVar(hook = "OnChangePlayerName")]
    public string playername;
    [SyncVar(hook = "OnChangeKills")]
    public int kills;
    [SyncVar(hook = "OnChangeDeaths")]
    public int deaths;

    void OnChangePlayerName(string newName)
    {
        playername = newName;
    }

    void OnChangeKills(int newKills)
    {
        kills = newKills;
    }

    void OnChangeDeaths(int newDeaths)
    {
        deaths = newDeaths;
    }
}
