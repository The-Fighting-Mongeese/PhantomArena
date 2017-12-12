using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningStrike : MonoBehaviour {

    public int damagePerTenthSecond = 1; //damage per tick, each tick 0.1 seconds roughly
    private float nextTimeToFire = 0f;
    private float fireRate = 10f; //10 times a second

    public Transform target;

    private void Update()
    {
        if (target == null) return;
        Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, target.position.z);
        float step = 5f * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
    }


    private void OnTriggerStay(Collider other)
    {
        
        if (other.transform.GetComponent<Health>() != null)
        {
            target = other.transform;

            if (Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                other.transform.GetComponent<Health>().CmdTakeTrueDamage(damagePerTenthSecond);
            }
        }

    }
}
