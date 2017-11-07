using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

//Interpolation of player movements.
public class PlayerLerp : NetworkBehaviour {

    Vector3 playerNext; //next position of the most recent update from server
    Vector3 playerPrevious; //previous position before updates from server
    Quaternion playerRotation;
    public float updateRate = 0.2f; //how long we want to wait between position updates
    float progress, startTime;

    private void OnEnable()
    {
        if(isLocalPlayer) StartCoroutine(UpdatePosition());
    }

    private void OnDisable(){
        StopAllCoroutines();
    }

    private void Start()
    {
        if (isLocalPlayer) StartCoroutine(UpdatePosition());
    }

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
        if (isLocalPlayer) return; //it's only for remote players.

        float timePassed = Time.time - startTime;
        progress = timePassed / updateRate;
        //print("progress: " + progress + ", timePassed: " + timePassed + " update rate: " + updateRate);
        transform.position = Vector3.Lerp(playerPrevious, playerNext, progress); //Starting position, position that it's going to, a float for smoothing (smoothing based on your frame rate)
        transform.rotation = Quaternion.Slerp(transform.rotation, playerRotation, 0.1f); //Time.deltatime * smooth
    }

    [Command]
    void CmdSendPosition(Vector3 position, Quaternion newRotation)
    {
        playerNext = position;
        playerRotation = newRotation;
        RpcReceivePosition(position, newRotation); //broadcast to all clients.
    }

    //we need a function for the client to receive the position
    [ClientRpc]
    void RpcReceivePosition(Vector3 position, Quaternion newRotation)
    {
        playerNext = position; 
        playerRotation = newRotation;
        startTime = Time.time; //for interpolation
        playerPrevious = transform.position;
    }

}
//https://gamedev.stackexchange.com/questions/113362/best-way-to-interpolate-player-movements-in-a-very-fast-paced-unity-game