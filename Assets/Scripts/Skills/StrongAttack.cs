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

    private PlayerController player;
    private Health health;
    private Stamina stamina;
    

    private void Start()
    {
        player = GetComponent<PlayerController>();
        health = GetComponent<Health>();
        stamina = GetComponent<Stamina>();
    }

    [Command]
    private void CmdDamage(GameObject other)
    {
        other.GetComponent<Health>().RpcTakeTrueDamage(damage);
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
        CmdDamage(other);
    }

    #endregion
}
