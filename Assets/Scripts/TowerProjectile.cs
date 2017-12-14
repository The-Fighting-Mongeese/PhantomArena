using UnityEngine;
using UnityEngine.Networking;

public class TowerProjectile : NetworkBehaviour
{
    public float Speed = 10.0f;
    public int Damage = 20;

    private GameObject _owner;



    public void SetOwner(GameObject owner)
    {
        _owner = owner;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * Speed);
    }

    [Command]
    private void CmdDamage(GameObject other)
    {
        other.GetComponent<Health>().CmdTakeTrueDamage(Damage);
    }

    private void Explode()
    {
        NetworkServer.UnSpawn(gameObject);
        NetworkServer.Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Tower"))
            return;
        if (collision.CompareTag("Player") && isServer)
        {
            CmdDamage(collision.gameObject);
        }
        Explode();
    }

}