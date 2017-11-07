using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ForceChangeProjectile : NetworkBehaviour {

    public uint originalShooter; // does not sync on clients, but doesn't need to
    public float speed = 5f; 

    private Rigidbody rb; 


    public void Launch()
    {
        rb.velocity = transform.forward * speed; 
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

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

            gameObject.SetActive(false);    // todo: play effect when collision
        }
    }

}
