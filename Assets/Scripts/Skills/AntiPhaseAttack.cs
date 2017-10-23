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
    private SkillStateMachine animState; 
    private Health health;
    private Mana mana;
    // TODO: Probably move this out or find a better way to cache.
    private int initialPhase, phantomLayer, physicalLayer; 


    private void Awake()
    {
        player = GetComponent<PlayerController>();
        animState = player.GetComponent<Animator>().GetBehaviour<SkillStateMachine>("AntiPhaseAttack");
        health = GetComponent<Health>();
        mana = GetComponent<Mana>();

        phantomLayer = LayerMask.NameToLayer("Phantom");
        physicalLayer = LayerMask.NameToLayer("Physical");
    }

    private void OnEnable()
    {
        // Bind to animator state (Should not be called in Awake, supposed to be in Start according to Unity Manual)
        animState.OnStateEntered += SkillStart;
        animState.OnStateExited += SkillEnd;
    }

    private void OnDisable()
    {
        // Unbind from animator state 
        animState.OnStateEntered -= SkillStart;
        animState.OnStateExited -= SkillEnd;
    }

    public void AnimEvent_APA_ColliderActivate()
    {
        player.weapon.ActivateCollider();
    }

    public void AnimEvent_APA_ColliderDeactivate()
    {
        player.weapon.DeactivateCollider();
    }

    private void SkillStart()
    {
        Debug.Log("SkillStart() called");
        // listen to weapon triggers
        player.weapon.OnOpponentTrigger += Activate;

        // save initial phase so we know which one to revert to later
        initialPhase = player.gameObject.layer;
        var oppositePhase = initialPhase == phantomLayer ? physicalLayer : phantomLayer;
        // change phase of weapon  
        player.weapon.GetComponent<PhasedMaterial>().ShowPhase(oppositePhase);
        player.weapon.gameObject.layer = oppositePhase;
    }

    private void SkillEnd()
    {
        // stop listening to weapon triggers
        player.weapon.OnOpponentTrigger -= Activate;

        // revert back to intial phase 
        player.weapon.GetComponent<PhasedMaterial>().ShowPhase(initialPhase);
        player.weapon.gameObject.layer = initialPhase;
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

}


