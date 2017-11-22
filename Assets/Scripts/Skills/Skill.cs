using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Skill : NetworkBehaviour
{
    protected PlayerController player;
    protected SkillStateMachine animState;
    protected static int phantomLayer, physicalLayer;  // TODO: Probably move this out or find a better way to cache.
    [SerializeField]
    private string animStateId;


    // Minimum amount of references that are required. Ensure base.Awake() is called if you do override.
    protected virtual void Awake()
    {
        player = GetComponent<PlayerController>();
        animState = player.GetComponent<Animator>().GetBehaviour<SkillStateMachine>(animStateId);
        if (phantomLayer == 0)
            phantomLayer = LayerMask.NameToLayer("Phantom");
        if (physicalLayer == 0)
            physicalLayer = LayerMask.NameToLayer("Physical");
    }

    protected virtual void OnEnable()
    {
        // Bind to animator state (Should not be called in Awake, supposed to be in Start according to Unity Manual)
        animState.OnStateEntered += SkillStart;
        animState.OnStateExited += SkillEnd;
    }

    protected virtual void OnDisable()
    {
        // Unbind from animator state 
        animState.OnStateEntered -= SkillStart;
        animState.OnStateExited -= SkillEnd;
    }


    // Simple damage function inheriting scripts can use. 
    [Command]
    protected void CmdDamage(GameObject other, int damage)
    {
        //other.GetComponent<Health>().CmdTakeTrueDamage(damage);
        other.GetComponent<Health>().CmdTakeTrueDamage2(GetComponent<NetworkIdentity>().netId.Value, damage); //temp code for score board
    }

    // Simple function animation clips can use to activate/deactivate weapon hit detection
    // Warning: This function is called multiple times (once for each Skill script), not recomended on release version.
    public void AnimEvent_ActivateWeaponCollider() { if (isLocalPlayer) player.weapon.ActivateCollider(); }
    public void AnimEvent_DeactivateWeaponCollider() { if (isLocalPlayer) player.weapon.DeactivateCollider(); }
    public void AnimEvent_SetSkillLocked(int locked) { if (isLocalPlayer) player.skillLocked = locked > 0 ? true : false; }
    public void AnimEvent_SetMoveLocked(int locked) { if (isLocalPlayer) player.moveLocked = locked > 0 ? true : false; }

    // Interface functions inheriting scripts need to implement. 
    public abstract bool ConditionsMet();               // to check whether or not this skill can be casted
    public abstract void ConsumeResources();            // use required conditions to activate this skill            
    public abstract void Activate(GameObject other);    // if the attack hits, activate its effects 
    protected virtual void SkillStart() { }             // called when enterting animation state (skill starts)
    protected virtual void SkillEnd() { }               // called when exiting animation state (skill end or interrupted)
}
