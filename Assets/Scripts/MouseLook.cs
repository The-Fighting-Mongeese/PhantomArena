using UnityEngine;

public class MouseLook : MonoBehaviour {

    public float maximumVerticalAngle = 45.0f, //the maximum and minimum angles that the player can look vertically
        minimumVerticalAngle = -45.0f;

    public float lookSensitivity = 5.0f; //how fast the player rotates

    public float rotationX = 0;

    public Camera playerCam;

    private void Start()
    {
        playerCam = GameObject.Find("PlayerCamera").GetComponent<Camera>(); //finds the player camera

        Rigidbody body = GetComponent<Rigidbody>();
        if(body != null)
        {
            body.freezeRotation = true;
        }
    }

    private void Update()
    {
        //Rotation via turning around
        float _rotationY = Input.GetAxis("Mouse X");
        Vector3 _rotation = new Vector3(0f, _rotationY, 0f) * lookSensitivity;
        transform.Rotate(_rotation);

        //Calculating camera rotation as a 3D vector (looking up and down)
        rotationX -= Input.GetAxis("Mouse Y") * lookSensitivity;
        rotationX = Mathf.Clamp(rotationX, minimumVerticalAngle, maximumVerticalAngle);
        playerCam.transform.localEulerAngles = new Vector3(rotationX, 0, 0); //Rotate the camera only when looking up and down so as to prevent flying movements.

    }

}
