using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// A fast but weak attack. 
/// </summary>
public class BasicAttack : Skill
{
    public int staminaRequired = 10;
    public int damage = 20;
    public Color[] vfxTrailColors; 

    private Stamina stamina;
    private MeleeWeaponTrail trail;


    protected override void Awake()
    {
        base.Awake();
        stamina = GetComponent<Stamina>();
        trail = player.weapon.GetComponentInChildren<MeleeWeaponTrail>();
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

        trail._colors = vfxTrailColors;
        trail.Emit = true;

        if (!isLocalPlayer) return;

        player.weapon.OnOpponentTrigger += Activate;    // listen to weapon hits 
        transform.rotation = Quaternion.LookRotation(player.rig.FlatForward());    // face camera 
    }

    protected override void SkillEnd()
    {
        base.SkillEnd();

        trail.Emit = false;

        if (!isLocalPlayer) return;

        player.weapon.OnOpponentTrigger -= Activate;    // stop listening to weapon hits 
        player.weapon.DeactivateCollider();             // ensure weapon is deactivated
    }

    #endregion
}
