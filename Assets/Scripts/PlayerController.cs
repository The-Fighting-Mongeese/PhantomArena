using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float speed = 10.0f;

    // TODO: Probably move this out or find a better way to cache.
    private int phantomLayer, physicalLayer;


    void Start()
    {
        phantomLayer = LayerMask.NameToLayer("Phantom");
        physicalLayer = LayerMask.NameToLayer("Physical");
    }


    void Update () {
        //if (!isLocalPlayer) return;
        float deltaX = Input.GetAxis("Horizontal") * speed;
        float deltaZ = Input.GetAxis("Vertical") * speed;
        Vector3 movement = new Vector3(deltaX, 0, deltaZ);
        movement = Vector3.ClampMagnitude(movement, speed); //limits diagonal movement to the same speed as movement along an axis
        movement *= Time.deltaTime;
        transform.Translate(movement);

        if (Input.GetKeyDown(KeyCode.F))
        {
            transform.SetAllLayers((gameObject.layer == physicalLayer) ? phantomLayer : physicalLayer);
        }
	}
}
