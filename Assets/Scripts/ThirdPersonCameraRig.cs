using UnityEngine;
using UnityEngine.Networking;

public class ThirdPersonCameraRig : NetworkBehaviour
{
    public float maximumVerticalAngle = 45.0f;  // the maximum and minimum angles that the player can look vertically
    public float minimumVerticalAngle = -45.0f;

    public float lookSensitivity = 2.5f; // how fast the player rotates
    
    [SerializeField]
    private Transform verticalControl;           // Note: Camera should be a child of this object
    private Transform player;
    private float verticalRotation = 0;
    private Vector3 distanceToPlayer;
    

    private void Awake()
    {
        distanceToPlayer = Camera.main.transform.localPosition;

        // store current distance to player
        player = transform.parent;
        transform.parent = null; // unparent so this rig doesn't inherit rotation
        distanceToPlayer = transform.position - player.position;
    }

    private void Update()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        // turn rig (not body) horizontally
        Vector3 horizontalRotation = new Vector3(0f, x, 0f) * lookSensitivity;
        transform.Rotate(horizontalRotation);

        // turn camera vertically
        verticalRotation -= Input.GetAxis("Mouse Y") * lookSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, minimumVerticalAngle, maximumVerticalAngle);
        verticalControl.localEulerAngles = new Vector3(verticalRotation, 0, 0);

        // make sure rig is on player
        transform.position = player.position + distanceToPlayer;
    }

    public Vector3 FlatForward()
    {
        var dir = Camera.main.transform.forward;
        dir.y = 0;
        return dir.normalized;
    }

    public Vector3 FlatRight()
    {
        var dir = Camera.main.transform.right;
        dir.y = 0;
        return dir.normalized;
    }

}
