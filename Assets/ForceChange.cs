using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ForceChange : Skill
{
    public int manaRequired = 50;
    public float cooldown = 5f;          
    public GameObject forceChangeProjectilePrefab;
    public Transform projectileSpawnLoc;

    private Mana mana;


    protected override void Awake()
    {
        base.Awake();
        mana = GetComponent<Mana>();
    }

    public void AnimEvent_FC_LaunchProjectile()
    {
        if (!isLocalPlayer) return;
        Activate(null);
    }

    public override void Activate(GameObject other)
    {
        if (!isLocalPlayer) return;

        // TODO: rotation should be from camera rotation
        var projectile = Instantiate(forceChangeProjectilePrefab, projectileSpawnLoc.position, transform.rotation);
        projectile.GetComponent<ForceChangeProjectile>().originalShooter = 100;
        // TODO: Magic numbers 
        projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * 5f;

        NetworkServer.Spawn(projectile);

        Destroy(projectile, 2f);
    }

    public override bool ConditionsMet()
    {
        return (mana.CurrentMana >= manaRequired);
    }

    public override void ConsumeResources()
    {
        mana.TryUseMana(manaRequired);
    }

}
