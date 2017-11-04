using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : MonoBehaviour {
    //To make this script work, simply add some empty gameobjects as locations for the dragon to move towards.
    Animator dragonAnimator;
    public Transform[] destinations;
    public float speed  = 10.0f;
    [SerializeField]
    private float currentSpeed;
    [SerializeField]
    private Transform nextTarget;
    [SerializeField]
    private float distance;

    void Start () {
        dragonAnimator = GetComponent<Animator>();
        nextTarget = destinations[0];
        distance = 0f;
        //currentSpeed = speed;
    }

    void Update () {

        if(distance < 1)
        {
            float choice = Random.Range(0, 3);               
            if(choice == 0)
            {
                dragonAnimator.SetBool("isFlying", true);
                currentSpeed = speed;
            }
            else if(choice == 1)
            {
                dragonAnimator.SetBool("isFlying", false);
                dragonAnimator.SetFloat("speed", 0.5f);
                currentSpeed = speed;
            }
            else if(choice == 2)
            {
                dragonAnimator.SetBool("isFlying", false);
                dragonAnimator.SetFloat("speed", 1.0f);
                currentSpeed = speed * 2.0f;
            }
            SetTarget();
        }
        else
        {
            MoveTowardsTarget(nextTarget);
        }
    }

    public void SetTarget()
    {
        int ranValue = Random.Range(0, destinations.Length);
        nextTarget = destinations[ranValue];

        distance = Vector3.Distance(transform.position, nextTarget.position);
        print(nextTarget.name + " val: " + ranValue);
        //print(distance);
    }

    public void MoveTowardsTarget(Transform target)
    {
        float step = speed * Time.deltaTime;
        distance = Vector3.Distance(transform.position, target.position);
        transform.LookAt(target);
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }


}
