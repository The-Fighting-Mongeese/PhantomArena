﻿using System.Collections;
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
    public ParticleSystem particles;
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
        return (stamina.CurrentStamina >= staminaRequired && cooldownCounter <= 0);
    }

    public override void ConsumeResources()
    {
        stamina.TryUseStamina(staminaRequired);
        cooldownCounter = cooldown;
    }

    public override void Activate(GameObject other)
    {
        if (!isLocalPlayer) return;
        base.CmdDamage(other, damage);
    }

    protected override void SkillStart()
    {
        base.SkillStart();

        particles.Play();
        sfx.gameObject.SetActive(true);

        if (!isLocalPlayer) return;

        player.weapon.OnOpponentTrigger += Activate;    // listen to weapon hits 
        transform.rotation = Quaternion.LookRotation(player.rig.FlatForward());    // face camera 
    }

    protected override void SkillEnd()
    {
        base.SkillEnd();

        particles.Stop();
        sfx.gameObject.SetActive(false);

        if (!isLocalPlayer) return;

        player.weapon.OnOpponentTrigger -= Activate;    // stop listening to weapon hits 
        player.weapon.DeactivateCollider();             // ensure weapon is deactivated
    }

    #endregion
}
