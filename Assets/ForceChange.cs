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
    // private int netId; 


    protected override void Awake()
    {
        base.Awake();
        mana = GetComponent<Mana>();
    }

    public void AnimEvent_FC_LaunchProjectile()
    {
        if (!isLocalPlayer) return;
        CmdLaunchProjectile();
    }

    [Command]
    public void CmdLaunchProjectile()
    {
        Activate(null);

    }

    public override void Activate(GameObject other)
    {
        // Activate is only called on server. No local checking. 

        // TODO: rotation should be from camera rotation
        var projectile = Instantiate(forceChangeProjectilePrefab, projectileSpawnLoc.position, transform.rotation);
        projectile.GetComponent<ForceChangeProjectile>().originalShooter = netId.Value;
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
