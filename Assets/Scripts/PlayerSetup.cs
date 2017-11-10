﻿using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour
{    
    [SerializeField]
    Behaviour[] componentsToDisable;
    [SerializeField]
    GameObject[] objectsToDisable;

    private void Start()
    {
        if (!isLocalPlayer) //if this object isn't controlled by the system, then we have to disable all the components.
        {
            DisableComponents();
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
