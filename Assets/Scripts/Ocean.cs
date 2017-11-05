using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ocean : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        Health target = other.transform.GetComponent<Health>();
        if(target != null)
        {
            target.CmdTakeTrueDamage(target.currentHealth);
        }
    }
}
