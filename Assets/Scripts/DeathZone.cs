using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        Health target = other.transform.GetComponent<Health>();
        if(target != null)
        {
            target.CmdTakeTrueDamage2(0, target.currentHealth);
        }
    }
}
