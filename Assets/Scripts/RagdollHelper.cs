using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RagdollHelper : MonoBehaviour
{
    public GameObject[] joints;
    private Collider col;
    private Rigidbody rb;
    private Animator anim;
    private NetworkAnimator nanim;


	// Use this for initialization
	void Start ()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        nanim = GetComponent<NetworkAnimator>();
        anim = GetComponent<Animator>();
        SetRagdollEnabled(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8))
            SetRagdollEnabled(true);
        if (Input.GetKeyDown(KeyCode.Alpha7))
            SetRagdollEnabled(false);
    }

    public void AnimEvent_SetRagdollEnabled(int enabled)
    {
        SetRagdollEnabled(enabled == 1);
    }

    public void SetRagdollEnabled(bool enabled)
    {
        foreach (GameObject joint in joints)
        {
            joint.GetComponent<Rigidbody>().isKinematic = !enabled;
            joint.GetComponent<Collider>().enabled = enabled;
            joint.layer = LayerHelper.GaiaLayer;
        }
        rb.isKinematic = enabled;   // prevent parent object from moving
        col.enabled = !enabled;     // prevent parent object from colliding with itself (ragdoll colliders) 
        nanim.enabled = !enabled;   // disable animator control over limbs 
        anim.enabled = !enabled;
    }
}
