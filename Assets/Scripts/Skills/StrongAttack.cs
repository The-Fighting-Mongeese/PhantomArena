using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// A strong but slow attack that consumes a lot of stamina. 
/// </summary>
public class StrongAttack : Skill
{
    public int staminaRequired = 50;
    public int damage = 50;
    public float cooldown = 5f;         // seconds 
    public ParticleSystem particles;
    public WeaponTrail trails;
    public AudioSource sfx; 

    private Stamina stamina;


    protected override void Awake()
    {
        base.Awake();
        stamina = GetComponent<Stamina>();
    }


    #region Implements: Skill

    public override bool ConditionsMet()
    {
        return (stamina.CurrentStamina >= staminaRequired);
    }

    public override void ConsumeResources()
    {
        stamina.TryUseStamina(staminaRequired);
    }

    public override void Activate(GameObject other)
    {
        if (!isLocalPlayer) return;
        base.CmdDamage(other, damage);
    }

    protected override void SkillStart()
    {
        particles.Play();
        sfx.gameObject.SetActive(true);


        if (!isLocalPlayer) return;
        player.weapon.OnOpponentTrigger += Activate;    // listen to weapon hits 
        player.skillLocked = true;

        transform.rotation = Quaternion.LookRotation(player.rig.FlatForward());    // face camera 
    }

    protected override void SkillEnd()
    {
        particles.Stop();
        sfx.gameObject.SetActive(false);

        if (!isLocalPlayer) return;
        player.weapon.OnOpponentTrigger -= Activate;    // stop listening to weapon hits 
        player.weapon.DeactivateCollider();             // ensure weapon is deactivated
        player.skillLocked = false;
    }

    #endregion
}
