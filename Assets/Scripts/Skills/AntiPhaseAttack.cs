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

    private Mana mana;
    private int initialPhase;


    protected override void Awake()
    {
        base.Awake();
        mana = GetComponent<Mana>();
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
        base.CmdDamage(other, damage);
    }

    protected override void SkillStart()
    {
        // save initial phase so we know which one to revert to later
        initialPhase = player.gameObject.layer;
        var oppositePhase = initialPhase == phantomLayer ? physicalLayer : phantomLayer;

        // change phase of weapon  
        player.weapon.GetComponent<PhasedMaterial>().ShowPhase(oppositePhase);
        player.weapon.gameObject.layer = oppositePhase;

        if (!isLocalPlayer) return;
        
        // listen to weapon triggers
        player.weapon.OnOpponentTrigger += Activate;

        player.skillLocked = true;

        transform.rotation = Quaternion.LookRotation(player.rig.FlatForward());    // face camera 
    }

    protected override void SkillEnd()
    {
        // revert back to intial phase 
        player.weapon.GetComponent<PhasedMaterial>().ShowPhase(initialPhase);
        player.weapon.gameObject.layer = initialPhase;

        if (!isLocalPlayer) return;

        // stop listening to weapon triggers
        player.weapon.OnOpponentTrigger -= Activate;

        // ensure collider is deactivated 
        player.weapon.DeactivateCollider();

        player.skillLocked = false;
    }

    #endregion

}


