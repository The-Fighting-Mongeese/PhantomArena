using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour {

    public Camera playerCam;

    private void LateUpdate()
    {
        transform.LookAt(transform.position + playerCam.transform.rotation * Vector3.back, 
            playerCam.transform.rotation * Vector3.down);
    }

}
