using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ForceChangeProjectile : NetworkBehaviour {

    public uint originalShooter; // does not sync on clients, but doesn't need to ... this might not work?
    public float speed = 5f;
    public float projectileLifetime = 2f;
    public float phaseLockDuration = 4f;

    [SerializeField]
    private GameObject explosionParticles;
    private Rigidbody rb;
    private float counter = 0;
    

    public void Launch()
    {
        rb.velocity = transform.forward * speed; 
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        counter += Time.deltaTime;
        if (counter > projectileLifetime)
            gameObject.SetActive(false);
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

            // play effects
            RpcBlast();

            // game logic
            identity.RpcChangePhase(LayerHelper.Opposite(identity.gameObject.layer));
            identity.RpcSetPhaseLock(true);
            CoroutineManager.Instance.StartCoroutine(UnlockPhase(identity, phaseLockDuration)); // Note: If hit by multiple blast then only first duration is respected
            gameObject.SetActive(false);    // todo: play effect when collision
        }
    }

    [ClientRpc]
    private void RpcBlast()
    {
        var currentPos = transform.position;
        explosionParticles.transform.parent = null;
        explosionParticles.SetActive(true);
    }

    private IEnumerator UnlockPhase(PlayerController other, float time)
    {
        yield return new WaitForSeconds(time);
        other.RpcSetPhaseLock(false);
    }

}
