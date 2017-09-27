using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    [SerializeField]
    private float speed = 10.0f;

    [SerializeField]
    private PlayerCore core;

    private Rigidbody rb;

    // TODO: Probably move this out or find a better way to cache.
    private int phantomLayer, physicalLayer;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        phantomLayer = LayerMask.NameToLayer("Phantom");
        physicalLayer = LayerMask.NameToLayer("Physical");
    }


    void Update () {
        // Handle movement 
        //if (!isLocalPlayer) return;
        float deltaX = Input.GetAxis("Horizontal") * speed;
        float deltaZ = Input.GetAxis("Vertical") * speed;
        Vector3 movement = new Vector3(deltaX, 0, deltaZ);
        movement = Vector3.ClampMagnitude(movement, speed); //limits diagonal movement to the same speed as movement along an axis
        movement *= Time.deltaTime;
        // rb.velocity = movement;
        transform.Translate(movement);

        // Handle phase change 
        if (Input.GetKeyDown(KeyCode.F))
        {
            // If core is inside something, player is currently phasing through environment. 
            if (core.CoreInside)
            {
                // Die - phased into environment. 
                CmdRespawn();   
            }
            CmdRespawn();
            transform.SetAllLayers((gameObject.layer == physicalLayer) ? phantomLayer : physicalLayer);
        }
	}

    [Command]
    public void CmdRespawn()
    {
        RpcRespawn();
    }

    [ClientRpc]
    public void RpcRespawn()
    {
        Debug.Log("RPC RESPAWNS");
        if (isLocalPlayer)  // b/c of local authority. 
        {
            transform.position = Vector3.zero;
        }
    }
}
