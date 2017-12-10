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
    public Color[] vfxTrailColors;
    public AudioSource sfx;

    private Mana mana;
    private int initialPhase;
    private MeleeWeaponTrail trail; 

    protected override void Awake()
    {
        base.Awake();
        mana = GetComponent<Mana>();
        trail = player.weapon.GetComponentInChildren<MeleeWeaponTrail>();
    }


    #region Implements: Skill

    public override bool ConditionsMet()
    {
        return (mana.CurrentMana >= manaRequired && cooldownCounter <= 0);
    }

    public override void ConsumeResources()
    {
        mana.TryUseMana(manaRequired);
        cooldownCounter = cooldown;
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

        // show vfx
        trail._colors = vfxTrailColors;
        trail.Emit = true;

        // play sfx 
        sfx.Play();

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

        // stop vfx
        trail.Emit = false;

        if (!isLocalPlayer) return;

        // stop listening to weapon triggers
        player.weapon.OnOpponentTrigger -= Activate;

        // ensure collider is deactivated 
        player.weapon.DeactivateCollider();

        player.skillLocked = false;
    }

    #endregion

}


