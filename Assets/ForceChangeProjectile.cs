using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ForceChangeProjectile : NetworkBehaviour {

    public int originalShooter; // i don't think this will sync ... 

    private void Update()
    {
        Debug.Log(originalShooter);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Force change projectile trigger");   
    }

}
