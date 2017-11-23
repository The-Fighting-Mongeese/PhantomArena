using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Networking;

public class PlayerManager : NetworkBehaviour
{

    public static Dictionary<int, PlayerMetrics> playerList = new Dictionary<int, PlayerMetrics>(); //playerIDs stored as _netid



    public static void RegisterPlayer(int _netID)
    {
        playerList.Add(_netID, null);
    }


    public static PlayerMetrics GetPlayer(int _playerID)
    {
        return playerList[_playerID];
    }

    public static void UnRegisterPlayer(int _playerID)
    {
        playerList.Remove(_playerID);
    }


    public static PlayerMetrics[] GetAllPlayers()
    {
        return playerList.Values.ToArray();
    }


    private void Update()
    {
        if (!isServer) return;

        if (Input.GetKeyDown(KeyCode.K))
        {
            /*
            print(playerList.Count);

            foreach (KeyValuePair<int, PlayerMetrics> p in playerList)
            {
                print(p.ToString());
                if (p.Value == null)
                {
                    print(p.Value);
                    var players = GameObject.FindGameObjectsWithTag("Player");
                    foreach (var player in players)
                    {
                        print(player.GetComponent<NetworkIdentity>().connectionToClient.connectionId);

                        if (player.GetComponent<NetworkIdentity>().connectionToClient.connectionId == p.Key)
                        {
                            PlayerMetrics _playerMetrics = player.GetComponent<PlayerMetrics>();
                            playerList[p.Key] = _playerMetrics;
                        }

                    }
                }
            }
            */
            foreach (var p in GetAllPlayers())
            {
                print(p.playername);
            }
        }
    }

    private void Start()
    {
        if(isServer)
            StartCoroutine(UpdatePlayers());
    }

    IEnumerator UpdatePlayers()
    {
        while (true)
        {
            Debug.Log("Update players");

            foreach (KeyValuePair<int, PlayerMetrics> p in playerList.ToArray())
            {
                if (p.Value == null)
                {
                    var players = GameObject.FindGameObjectsWithTag("Player");
                    foreach (var player in players)
                    {
                        if (player.GetComponent<NetworkIdentity>().connectionToClient.connectionId == p.Key)
                        {
                            PlayerMetrics _playerMetrics = player.GetComponent<PlayerMetrics>();
                            playerList[p.Key] = _playerMetrics;
                        }

                    }
                }
            }

            yield return new WaitForSeconds(1.5f);
        }
    }
    /*
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
    {
        print("server is adding a player! " + conn.connectionId);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        print("client is connecting: " + conn.connectionId);
    }*/

    public static void Cleanup()
    {
        Debug.Log("Cleanup");
        playerList.Clear();
    }

    public static PlayerMetrics GetTopPlayer()
    {
        PlayerMetrics currentTop = playerList.Values.First();
        foreach (PlayerMetrics pm in playerList.Values)
        {
            if (pm.kills > currentTop.kills)
            {
                currentTop = pm;
            }
        }
        return currentTop;
    }

}
