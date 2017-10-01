using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

//dan
public class HealthPackSpawner : NetworkBehaviour {

    public GameObject healthPackPrefab;
    GameObject[] healthPackSpawnLocations;

    List<GameObject> healthPacksInGame = new List<GameObject>();

    /*
    void Start()
    {
        healthPackSpawnLocations = GameObject.FindGameObjectsWithTag("HealthPackSpawn");
        CmdSpawnHealthPack();
    } */


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


    private IEnumerator SpawnHealthPack() //not being used currently
    {
        while (true)
        {         
            foreach(var healthPack in healthPacksInGame)
                Destroy(healthPack);
            
            healthPacksInGame.Clear();
            CmdSpawnHealthPack();

            yield return new WaitForSeconds(10.0f);
        }
    }

}
