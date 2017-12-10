using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManagerCustom : NetworkManager {

    public int totalNumberOfPlayers = 0;


    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        totalNumberOfPlayers++;
        PlayerManager.RegisterPlayer(conn.connectionId);
        //print("client OnServerConnect to server: " + conn.connectionId + " | " + conn.address);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        totalNumberOfPlayers--;
        //print("client disconnected from server: " + conn.connectionId);
        PlayerManager.UnRegisterPlayer(conn.connectionId);
    }

    public override void OnDestroyMatch(bool success, string extendedInfo)
    {
        base.OnDestroyMatch(success, extendedInfo);

        Debug.Log("On destroy match called");
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        totalNumberOfPlayers = 0;
        PlayerManager.Cleanup();

        Debug.Log("ON stop server called");
    }


}
