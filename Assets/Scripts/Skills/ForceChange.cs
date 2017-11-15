using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ForceChange : Skill
{
    public int manaRequired = 50;
    public float cooldown = 5f;          
    public ForceChangeProjectile forceChangeProjectilePrefab;
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
        CmdLaunchProjectile(Camera.main.transform.rotation);
    }

    [Command]
    public void CmdLaunchProjectile(Quaternion direction)
    {
        var projectile = Instantiate<ForceChangeProjectile>(forceChangeProjectilePrefab, projectileSpawnLoc.position, direction);
        projectile.originalShooter = netId.Value;
        projectile.Launch();    // apply forward movement using prefabs speed value

        NetworkServer.Spawn(projectile.gameObject);

        Destroy(projectile.gameObject, projectile.projectileLifetime + 5f); // Ensure this well off in the future, after the projectile has fully disabled itself
    }

    public override void Activate(GameObject other)
    {
        // Prefab handles activation 
    }

    public override bool ConditionsMet()
    {
        return (mana.CurrentMana >= manaRequired);
    }

    public override void ConsumeResources()
    {
        mana.TryUseMana(manaRequired);
    }

    protected override void SkillStart()
    {
        if (!isLocalPlayer) return;
        player.skillLocked = true;
        player.moveLocked = true;
    }

    protected override void SkillEnd()
    {
        if (!isLocalPlayer) return;
        player.skillLocked = false;
        player.moveLocked = false;
    }

}
