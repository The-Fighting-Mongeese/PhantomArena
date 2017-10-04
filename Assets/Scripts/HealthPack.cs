using UnityEngine;
using System.Collections;

//daniel
public class HealthPack : MonoBehaviour {

    int ticks = 50;
    int flag = 0, flagDown;

    private void Awake()
    {
        flagDown = ticks;
    }

    private void Update()
    {
        transform.Rotate(0, 0, 45f * Time.deltaTime);

        if(flag < ticks)
        {
            transform.Translate(Vector3.up * 1 * Time.deltaTime, Space.World);
            flag++;
        }
        else if(flag >= ticks)
        {
            transform.Translate(Vector3.down * 1 * Time.deltaTime, Space.World);
            if(flagDown-- <= 0 || transform.position.y < 0.1)
            {
                flag = 0;
                flagDown = ticks;
            }
        }

        
    }


    private IEnumerator DisableHealthPack()
    {

        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(5.0f);
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<BoxCollider>().enabled = true;

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {
            other.GetComponent<Health>().Heal(20);
            StartCoroutine(DisableHealthPack());
        }
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            collision.collider.GetComponent<Health>().Heal(20);   
            StartCoroutine(DisableHealthPack());
        }
    } */




}
