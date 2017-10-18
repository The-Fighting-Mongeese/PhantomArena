using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// A strong attack that attacks in the opposite form the player current is in. 
/// </summary>
public class AntiPhaseAttack : Skill
{
    public int manaRequired = 50;
    public int damage = 50;
    public float cooldown = 5f;         // seconds 

    private PlayerController player;
    private ListenableStateBehaviour animState; 
    private Health health;
    private Mana mana;


    private void Start()
    {
        player = GetComponent<PlayerController>();
        animState = player.GetComponent<Animator>().GetBehaviour<ListenableStateBehaviour>();
        health = GetComponent<Health>();
        mana = GetComponent<Mana>();

    }

    [Command]
    private void CmdDamage(GameObject other)
    {
        other.GetComponent<Health>().RpcTakeTrueDamage(damage);
    }

    #region Implements: Skill

    public override bool ConditionsMet()
    {
        return (mana.CurrentMana >= manaRequired);
    }

    public override void ConsumeResources()
    {
        mana.TryUseMana(manaRequired);
    }

    public override void Activate(GameObject other)
    {
        CmdDamage(other);
    }

    #endregion


    #region Animation Events 

    public void AnimEvent_AttackStart()
    {
        player.phaseLocked = true;
    }

    // TODO: Probably cause some issues if this anim is cut off early (won't trigger this last anim event) 
    public void AnimEvent_AttackEnd()
    {
        player.phaseLocked = false;
    }

    // start
    // end
    // interrupt { end } 

    #endregion
}
