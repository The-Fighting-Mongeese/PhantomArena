using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ForceChange : Skill
{
    public int manaRequired = 50;
    public ForceChangeProjectile forceChangeProjectilePrefab;
    public Transform projectileSpawnLoc;
    public AudioSource sfx;
    public ParticleSystem vfx;

    private Mana mana;
    // private int netId; 


    protected override void Awake()
    {
        base.Awake();
        mana = GetComponent<Mana>();
    }

    public void AnimEvent_FC_LaunchProjectile()
    {
        vfx.Play();
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
        return (mana.CurrentMana >= manaRequired && cooldownCounter <= 0);
    }

    public override void ConsumeResources()
    {
        mana.TryUseMana(manaRequired);
        cooldownCounter = cooldown;
    }

    protected override void SkillStart()
    {
        // base.SkillStart(); // do manual locking

        sfx.Play();

        if (!isLocalPlayer) return;

        player.skillLocked = true;
        player.phaseLocked = true;

        transform.rotation = Quaternion.LookRotation(player.rig.FlatForward());    // face camera 
    }

    protected override void SkillEnd()
    {
        // base.SkillEnd(); // do manual unlocking

        player.skillLocked = false;
        player.phaseLocked = false;
    }

}
