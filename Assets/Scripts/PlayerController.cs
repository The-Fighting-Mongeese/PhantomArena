using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    [SerializeField]
    private float speed = 10.0f;

    [SerializeField]
    float coreHeight = 0.5f;    // from center to top / bottom (actually half height than) 

    [SerializeField]
    float coreRadius = 0.3f;

    private Rigidbody rb;

    private Renderer mesh;

    // TODO: Probably move this out or find a better way to cache.
    private int phantomLayer, physicalLayer;
    // TODO: Probably move this to a static Constant class
    private Color physicalColor = new Color(174 / 255f, 51 / 255f, 4 / 255f);
    private Color phantomColor = new Color(37 / 255f, 162 / 255f, 195 / 255f);


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mesh = GetComponent<Renderer>();
        phantomLayer = LayerMask.NameToLayer("Phantom");
        physicalLayer = LayerMask.NameToLayer("Physical");
    }

    void Update ()
    {
        // Handle movement 
        //if (!isLocalPlayer) return;
        float deltaX = Input.GetAxis("Horizontal") * speed;
        float deltaZ = Input.GetAxis("Vertical") * speed;
        Vector3 movement = new Vector3(deltaX, 0, deltaZ);
        movement = Vector3.ClampMagnitude(movement, speed);     // limits diagonal movement to the same speed as movement along an axis
        movement *= Time.deltaTime;
        transform.Translate(movement);                          // TODO: Change to force or velocity changing, or move to FixedUpdate to prevent going into walls. 

        // Handle phase change 
        if (Input.GetKeyDown(KeyCode.F))
        {
            // Check if player is phasing into environment
            var objectsInCore = Physics.OverlapCapsule(
                transform.position + transform.up * coreHeight / 2, 
                transform.position + transform.up * -coreHeight / 2, 
                coreRadius);

            if (objectsInCore.Length > 1)  // there will always be one for the player
            {
                Debug.Log("Player is phasing into something.");
                // TODO: Kill player
                CmdRespawn();
                return;
            }

            // TODO: Check phase change cooldown / resource use 

            // All checks ok, change phase
            CmdChangePhase((gameObject.layer == physicalLayer) ? phantomLayer : physicalLayer);
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
        transform.position = Vector3.zero;
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

    void OnDrawGizmosSelected()
    {
        // Drawing core bounds (Note: the calculations are correct, do not use half coreHeight)
        DebugExtension.DrawCapsule(transform.position + (transform.up * coreHeight), transform.position - (transform.up * coreHeight), coreRadius);
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
