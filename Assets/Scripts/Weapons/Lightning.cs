﻿using UnityEngine.Networking;
using UnityEngine;

//Lightning attack

public class Lightning : NetworkBehaviour {

    public GameObject lightningStrikePrefab;
    public float lightningAliveTime = 10.0f;
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            CmdLightningBolt();
        }
	}


    [Command]
    void CmdLightningBolt()
    {
        Vector3 lightningInitialPosition = new Vector3(transform.position.x, lightningStrikePrefab.transform.position.y, transform.position.z);
        GameObject lightningStrike = Instantiate(lightningStrikePrefab, lightningInitialPosition + transform.forward * 4, lightningStrikePrefab.transform.rotation);
        NetworkServer.Spawn(lightningStrike);
        Destroy(lightningStrike, lightningAliveTime);
    }

}
