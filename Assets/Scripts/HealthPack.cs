using UnityEngine;
using System.Collections;

//daniel
public class HealthPack : MonoBehaviour {

    const float ROTATION_SPEED = 45f;
    const float VERTICAL_BOB_FREQUENCY = 0.5f;
    const float VERTICAL_BOB_MAGNITUDE = 0.5f;
    const float VERTICAL_OFFSET = 1f;
    private Vector3 startingPosition;

    private void Start()
    {
        startingPosition = transform.position;
    }

    private void Update()
    {
        transform.Rotate(0, 0, ROTATION_SPEED * (Time.deltaTime));
        Vector3 pos = startingPosition;
        pos.y += VERTICAL_BOB_MAGNITUDE * Mathf.Sin(Time.fixedTime * 2 * Mathf.PI * VERTICAL_BOB_FREQUENCY) + VERTICAL_OFFSET;
        transform.position = pos;
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
