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
        // transform.SetAllLayers(CombineLayers("Phantom", "Physical"));
        var foo = LayerMask.GetMask("Phantom", "Physical");
        Debug.Log(foo);
        transform.SetAllLayers(foo);
        Debug.Log(test.value);
    }

    float coreHeight = 0.8f;    // from center to top / bottom (actually half height than) 
    float coreRadius = 0.3f;


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



        // Warning! Not 100% sure if this mirrors exactly what Physics.CheckCapsule is
        // DebugExtension.DebugCapsule(transform.position + transform.up * coreHeight / 2, transform.position + transform.up * -coreHeight, coreRadius);
        
        // Handle phase change 
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (Physics.CheckCapsule(transform.position + transform.up * coreHeight / 2, transform.position + transform.up * -coreHeight / 2, coreRadius))
            {
                // always in something ! (player)
                Debug.Log("INSIDE SOMETHING");
            }
            return;
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

    public LayerMask test;

    int CombineLayers(params string[] layers)
    {
        int finalLayerMask = 0;
        foreach (string s in layers)
        {
            var layer = LayerMask.NameToLayer(s);

            Debug.Log(layer);
            finalLayerMask = finalLayerMask | layer;
            Debug.Log(finalLayerMask);
        }

        Debug.Log(finalLayerMask);
        Debug.Log(test.value);

        return finalLayerMask;
    }

    void OnDrawGizmosSelected()
    {
        DebugExtension.DrawCapsule(transform.position + (transform.up * coreHeight), transform.position - (transform.up * coreHeight), coreRadius);
    }

    [Command]
    public void CmdRespawn()
    {
        RpcRespawn();
    }

    [ClientRpc]
    public void RpcRespawn()
    {
        transform.position = Vector3.zero;
    }
}
