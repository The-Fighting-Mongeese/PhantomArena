using System.Collections;
using UnityEngine.Networking;
using UnityEngine;

/**
 * 
 * In order for this script to work, add 2 or more empty gameobjects
 * as locations for the dragon to move to.
 * 
*/
public class Dragon : NetworkBehaviour {

    public Transform[] destinations;
    public Transform dragonMouthLocation;
    public GameObject dragonBreathPrefab;
    public AudioSource flyingSound, roarSound;

    public float movementSpeed  = 10.0f;
    public float missleSpeed = 10.0f;
    public float minIdleTime = 5.0f;
    public float maxIdleTime = 10.0f;
    public float timeBetweenScans = 3.0f;
    public float aggroRadius = 20.0f;
    public float maxAggroDistance = 40.0f;
    public float fireRate = 1f;

    private Animator dragonAnimator;
    private Transform nextTarget;
    private Transform playerTransform;
    private float distance = 0.0f;
    private int destinationIndex = 0;
    private bool resting;
    private bool isAttacking;
    private float nextTimeToFire = 0f;

    const float dragonWalk = 0.5f; 
    const float dragonIdle = 0.0f;

    void Start () {
        dragonAnimator = GetComponent<Animator>();
        if(isServer)
            StartCoroutine(ScanSurroundings(aggroRadius, timeBetweenScans));
    }

    void Update () {
        if (!isServer) return;

        if (isAttacking)
        {
            AttackPlayer();
            if(Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                CmdFireBall();
            }
        }
        else
        {
            if (distance < 1)
            {
                if(!resting) StartCoroutine(Rest());

                if (!isAttacking)
                {
                    dragonAnimator.SetBool("isFlying", false);
                    CmdPlayDragonSound(2, false);
                    GetComponent<Rigidbody>().useGravity = true;
                }
            }
            else
            {
                MoveTowardsTarget(nextTarget);
            }

        }  
    }

    void SetTarget()
    {
        nextTarget = destinations[destinationIndex];
        distance = Vector3.Distance(transform.position, nextTarget.position);
        destinationIndex++;

        if(destinationIndex >= destinations.Length)
            destinationIndex = 0; 
    }

    void MoveTowardsTarget(Transform target)
    {
        float step = movementSpeed * Time.deltaTime;
        distance = Vector3.Distance(transform.position, target.position);
        transform.LookAt(target);
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }

    IEnumerator Rest()
    {
        resting = true;
        float timeResting = Random.Range(minIdleTime, maxIdleTime);
        dragonAnimator.SetFloat("speed", dragonIdle);
        yield return new WaitForSeconds(timeResting);
        dragonAnimator.SetFloat("speed", dragonWalk);
        SetTarget(); //sets the next target after resting
        resting = false;
    }

    IEnumerator ScanSurroundings(float radius, float timeBetweenScans)
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenScans);

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

            int i = 0;

            while(i < hitColliders.Length)
            {
                if(hitColliders[i].tag == "Player")
                {
                    isAttacking = true;

                    CmdPlayDragonSound(1, true);
                    CmdPlayDragonSound(2, true);
                    GetComponent<Rigidbody>().useGravity = false;
                    dragonAnimator.SetBool("isFlying", true);
                    playerTransform = hitColliders[i].transform;
                }
                i++;
            }
        }
    }

    [Command]
    void CmdFireBall()
    {
        GameObject fireBall = Instantiate(dragonBreathPrefab, dragonMouthLocation.position, dragonBreathPrefab.transform.rotation);
        fireBall.transform.LookAt(playerTransform);
        //fireBall.GetComponent<Rigidbody>().velocity = (playerTransform.position - transform.position).normalized * missleSpeed;
        fireBall.GetComponent<Rigidbody>().velocity = fireBall.transform.forward * missleSpeed;
        NetworkServer.Spawn(fireBall);
        Destroy(fireBall, 25f);
    }

    void AttackPlayer()
    {

        Vector3 targetOffset = new Vector3(0, 10, 0);

        float step = movementSpeed * Time.deltaTime;

        distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance > 8.0f)
        {
            transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position + targetOffset, step);
        }
        else
        {
            transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));
        }

        if (distance > maxAggroDistance)    isAttacking = false;

    }

    [Command]
    void CmdPlayDragonSound(int val, bool play)
    {
        RpcPlayDragonSound(val, play);
    }

    [ClientRpc]
    void RpcPlayDragonSound(int val, bool play)
    {
        if(val == 1)
            if (play) roarSound.Play();
            else roarSound.Stop();    
       else if(val == 2)
            if (play) flyingSound.Play();
            else flyingSound.Stop();
        
    }
}
