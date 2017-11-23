using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

//dan
public class HealthPackSpawner : NetworkBehaviour {

    public GameObject healthPackPrefab;

    private GameObject[] healthPackSpawnLocations;

    private List<GameObject> healthPacksInGame = new List<GameObject>();

    
    void Start()
    {
        if (!isServer) return;  // Object should be set to server only, but just in case 
        healthPackSpawnLocations = GameObject.FindGameObjectsWithTag("HealthPackSpawn");
        CmdSpawnHealthPack();
    } 

    [Command]
    public void CmdSpawnHealthPack()
    {
        healthPackSpawnLocations = GameObject.FindGameObjectsWithTag("HealthPackSpawn");
        foreach (var spawnLocation in healthPackSpawnLocations)
        {
            GameObject healthPack = Instantiate(healthPackPrefab, spawnLocation.transform);
            NetworkServer.Spawn(healthPack);
            healthPacksInGame.Add(healthPack);
        }
    }

  }
