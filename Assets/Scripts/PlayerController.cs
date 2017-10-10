﻿using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Stamina))]
[RequireComponent(typeof(Mana))]
public class PlayerController : NetworkBehaviour {
    private float JUMP_DURATION = 1.0f;

    [SerializeField]
    private float speed = 10.0f;

    [SerializeField]
    float coreHeight = 0.5f;    // from center to top / bottom (actually half height than) 

    [SerializeField]
    float coreRadius = 0.3f;

    //private Rigidbody rb;

    private Renderer mesh;

    // TODO: Probably move this out or find a better way to cache.
    private int phantomLayer, physicalLayer;
    // TODO: Probably move this to a static Constant class
    private Color physicalColor = new Color(174 / 255f, 51 / 255f, 4 / 255f);
    private Color phantomColor = new Color(37 / 255f, 162 / 255f, 195 / 255f);

    private Rigidbody rb;
    private Health health; 


    void Start()
    {
        mesh = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();
        phantomLayer = LayerMask.NameToLayer("Phantom");
        physicalLayer = LayerMask.NameToLayer("Physical");
    }

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Health s = GetComponent<Health>();
            s.CmdTakeTrueDamage(20);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Mana m = GetComponent<Mana>();
            m.TryUseMana(50);
        }

        // Handle phase change 
        if (Input.GetKeyDown(KeyCode.F))
        {
            AttemptPhaseChange();
        }
    }

    private void FixedUpdate()
    {
        if (IsGrounded())
        {
            Vector3 vel = (transform.forward * Input.GetAxisRaw("Vertical") + transform.right * Input.GetAxisRaw("Horizontal")).normalized * speed;
            if (Input.GetKey(KeyCode.Space))
            {
                vel.y = 9.81f * 0.5f * JUMP_DURATION;
            }
            else
            {
                vel.y = rb.velocity.y;
            }
            rb.velocity = vel;
        }
    }

    [Command]
    public void CmdChangePhase(int layer)
    {
        RpcChangePhase(layer);
    }

    [ClientRpc]
    public void RpcChangePhase(int layer)
    {
        if (layer == physicalLayer)
        {
            transform.SetAllLayers(physicalLayer);  // Change layer 
            mesh.material.color = physicalColor;    // Change appearance
        }
        else
        {
            transform.SetAllLayers(phantomLayer);
            mesh.material.color = phantomColor;
        }
    }

    private bool AttemptPhaseChange()
    {
        // Check if player is phasing into something
        var objectsInCore = Physics.OverlapCapsule(
            transform.position + transform.up * coreHeight / 2,
            transform.position + transform.up * -coreHeight / 2,
            coreRadius);

        // Die if phasing into something other than a player (eg. environment) 
        foreach (var o in objectsInCore)
        {
            if (!o.CompareTag("Player"))
            {
                health.CmdTakeTrueDamage(1000);
                return false;
            }
        }

        // TODO: Check phase change cooldown / resource use 

        // All checks ok, change phase
        CmdChangePhase((gameObject.layer == physicalLayer) ? phantomLayer : physicalLayer);

        return true;
    }

    void OnDrawGizmosSelected()
    {
        // Drawing core bounds (Note: the calculations are correct, do not use half coreHeight)
        DebugExtension.DrawCapsule(transform.position + (transform.up * coreHeight), transform.position - (transform.up * coreHeight), coreRadius);
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, 2 * coreHeight + 0.01f);
    }
}

/* Function to combine layers. 
int CombineLayers(params string[] layers)
{
    int finalLayerMask = 0;
    foreach (string s in layers)
    {
        var layer = LayerMask.NameToLayer(s);
        finalLayerMask = finalLayerMask | layer;
    }
    return finalLayerMask;
}
*/
