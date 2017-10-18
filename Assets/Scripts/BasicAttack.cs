using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : Skill
{
    public int staminaRequired = 10;
    public int damage = 20;
    public float cooldown = 0.5f;         // seconds 

    private PlayerController player;
    private Health health;
    private Stamina stamina;


    private void Start()
    {
        player = GetComponent<PlayerController>();
        health = GetComponent<Health>();
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
        other.GetComponent<Health>().CmdTakeTrueDamage(damage);
    }

    #endregion
}
