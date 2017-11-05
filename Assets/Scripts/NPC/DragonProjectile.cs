using UnityEngine.Networking;
using UnityEngine;

public class DragonProjectile : NetworkBehaviour {

    public GameObject explosionEffectPrefab; //assign via inspector
    public float explosionRadius = 15.0f;
    public int damage = 20;

    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Dragon") return;

        GetComponent<ParticleSystem>().Stop();
        GetComponent<SphereCollider>().enabled = false;

        GameObject impactGO = Instantiate(explosionEffectPrefab, transform.position, explosionEffectPrefab.transform.rotation);

        var colls = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (var col in colls)
        {

            Health target = col.transform.GetComponent<Health>();
            if (target != null && target.tag != "Dragon")
                if (isServer)
                    target.CmdTakeTrueDamage(damage);           
        }

        Destroy(impactGO, 4f);
        Destroy(gameObject);
    }
}
