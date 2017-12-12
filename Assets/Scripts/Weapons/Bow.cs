using UnityEngine.Networking;
using UnityEngine;

public class Bow : NetworkBehaviour {

    public GameObject bow;
    public GameObject arrow;
    public GameObject arrowProjectilePrefab;
    public GameObject rHandWeapon;
    public Transform cameraPosition, projectileSpawnPosition;

    public float arrowSpeed = 15.0f;


	void Update () {
        if (Input.GetKeyDown(KeyCode.F1)) bow.SetActive(true);    
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3)) bow.SetActive(false);


        if (bow.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                arrow.SetActive(true);

            }

            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                arrow.SetActive(false);
                CmdFireArrow();
            }
        }
	}


    [Command]
    void CmdFireArrow()
    {
        GameObject arrowInstance = Instantiate(arrowProjectilePrefab, projectileSpawnPosition.position, transform.rotation);
        arrowInstance.GetComponent<Rigidbody>().velocity = cameraPosition.forward * arrowSpeed;
        NetworkServer.Spawn(arrowInstance);
        Destroy(arrowInstance, 30.0f);
    }

}
