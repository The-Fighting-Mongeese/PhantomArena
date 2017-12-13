﻿using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;  

public class PlayerSetup : NetworkBehaviour
{    
    [SerializeField]
    Behaviour[] componentsToDisable;
    [SerializeField]
    GameObject[] objectsToDisable;

    public GameObject playerHealthbar;

    private void Start()
    {

        if (!isLocalPlayer) //if this object isn't controlled by the system, then we have to disable all the components.
        {
            DisableComponents();
        }

        if (isLocalPlayer)
        {
            playerHealthbar.SetActive(false);

            if (SceneManager.GetActiveScene().name == "DanielTerrainScene")
            {
                print("haiya");
                GameObject initialSpawn = GameObject.Find("PlayerSpawn");
                if (initialSpawn != null)
                {
                    transform.position = initialSpawn.transform.position;
                }
            }

        }
    }

    void DisableComponents()
    {
        foreach (Behaviour b in componentsToDisable)
            b.enabled = false;
        foreach (GameObject go in objectsToDisable)
            go.SetActive(false);
    }


}
