using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class DragonLerp : NetworkBehaviour
{
    Vector3 nextPosition; //next position of the most recent update from server
    Vector3 previousPosition; //previous position before updates from server
    Quaternion rotation;
    public float updateRate = 0.2f; //how long we want to wait between position updates
    float progress, startTime;

    public override void OnStartServer()
    {
        previousPosition = transform.position;
        if (isServer) StartCoroutine(UpdatePosition());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    /*
    private void Start()
    {
        previousPosition = transform.position;
        if (isServer) StartCoroutine(UpdatePosition());
    }*/

    private void Update()
    {
        LerpPosition();
    }

    IEnumerator UpdatePosition()
    {
        while (enabled)
        {
            yield return new WaitForSeconds(updateRate); //don't block client
            CmdSendPosition(transform.position, transform.rotation);
        }
    }

    //current = Lerp(previous, next, progress)
    //(where progress varies from 0 to 1 over the expected duration of the move)
    void LerpPosition()
    {
        if (isServer) return; 

        float timePassed = Time.time - startTime;
        progress = timePassed / updateRate;
        //print("progress: " + progress + ", timePassed: " + timePassed + " update rate: " + updateRate);
        transform.position = Vector3.Lerp(previousPosition, nextPosition, progress); //Starting position, position that it's going to, a float for smoothing (smoothing based on your frame rate)
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.1f); //Time.deltatime * smooth
    }

    [Command]
    void CmdSendPosition(Vector3 position, Quaternion newRotation)
    {
        //print("CmdSendPosition " + position);
        nextPosition = position;
        rotation = newRotation;
        RpcReceivePosition(position, newRotation); //broadcast to all clients.
    }

    //we need a function for the client to receive the position
    [ClientRpc]
    void RpcReceivePosition(Vector3 position, Quaternion newRotation)
    {
        nextPosition = position;
        rotation = newRotation;
        startTime = Time.time; //for interpolation
        previousPosition = transform.position;
    }

}
