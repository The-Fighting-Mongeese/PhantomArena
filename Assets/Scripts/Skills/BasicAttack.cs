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
    public float cooldown = 0.5f;         // seconds 

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
        if (!isLocalPlayer) return;
        player.weapon.OnOpponentTrigger += Activate;    // listen to weapon hits 

        player.skillLocked = true;
        player.moveLocked = true;
    }

    protected override void SkillEnd()
    {
        if (!isLocalPlayer) return;
        player.weapon.OnOpponentTrigger -= Activate;    // stop listening to weapon hits 
        player.weapon.DeactivateCollider();             // ensure weapon is deactivated

        player.skillLocked = false;
        player.moveLocked = false;
    }

    #endregion
}
