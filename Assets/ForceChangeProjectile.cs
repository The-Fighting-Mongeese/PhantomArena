using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ForceChangeProjectile : NetworkBehaviour {

    public uint originalShooter; // does not sync on clients, but doesn't need to


    private void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;

        Debug.Log("Force change projectile trigger");   
        if (other.CompareTag("Player"))
        {
            var identity = other.GetComponent<PlayerController>();

            if (identity == null)
                return;

            if (originalShooter == identity.netId.Value)
                return;

            identity.RpcChangePhase(LayerHelper.Opposite(identity.gameObject.layer));
        }
    }

}
