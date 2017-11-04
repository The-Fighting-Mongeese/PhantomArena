using UnityEngine;
using UnityEngine.Networking;

public class Arrow : NetworkBehaviour {

    public AudioSource arrowImpactSound;
    public int arrowDamage = 25;

    private void OnCollisionEnter(Collision collision)
    {
        arrowImpactSound.Play();

        GetComponent<Rigidbody>().detectCollisions = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;

        Health target = collision.transform.GetComponent<Health>();

        if (target != null)
        {
            if(isServer)
            target.CmdTakeTrueDamage(arrowDamage);
        }
    }
}
