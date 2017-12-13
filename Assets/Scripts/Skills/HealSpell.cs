using UnityEngine.Networking;
using UnityEngine;
using System;

public class HealSpell : AnimLessSkill
{
    public GameObject healAuraPrefab;
    public int healAmountPerTenthSecond = 1;
    public int manaCostInitial = 20;            // gate usage (force players to find health packs)
    public int manaCostPerTenthSecond = 1;
    private bool isAuraOn = false;

    private float nextTimeToFire = 0f;
    private float tickRatePerSecond = 10f;      // called 10 times a second roughly
    private Mana mana;
    private Health health;


    protected override void Awake()
    {
        base.Awake();
        mana = GetComponent<Mana>();
        health = GetComponent<Health>();
        health.OnDeath += Deactivate;
    }

    private void OnDestroy()
    {
        health.OnDeath -= Deactivate;
    }

    protected override void Update()
    {
        if (!isLocalPlayer) return;

        base.Update();

        if (isAuraOn)
        {
            if (Time.time >= nextTimeToFire)
            {
                if (mana.CurrentMana >= manaCostPerTenthSecond)
                {
                    mana.TryUseMana(manaCostPerTenthSecond);
                    nextTimeToFire = Time.time + 1f / tickRatePerSecond;

                    if (health.currentHealth < health.maxHealth)
                        health.CmdHeal(healAmountPerTenthSecond);
                }
                else
                {
                    Deactivate();
                }
            }
        }
    }

    [Command]
    void CmdCastHealAura(bool isActive)
    {
        RpcCastHealAura(isActive);
    }

    [ClientRpc]
    void RpcCastHealAura(bool isActive)
    {
        healAuraPrefab.SetActive(isActive);
    }

    private void Deactivate()
    {
        isAuraOn = false;
        CmdCastHealAura(false);
    }

    public override bool ConditionsMet()
    {
        if (isAuraOn)
        {
            return true;
        }
        else
        {
            return cooldownCounter <= cooldown && mana.CurrentMana >= manaCostInitial;
        }
    }

    public override void ConsumeResources()
    {
        if (!isAuraOn)
        {
            mana.TryUseMana(manaCostInitial);
        }
    }

    public override void Activate(GameObject other)
    {
        isAuraOn = !isAuraOn;       // flip
        CmdCastHealAura(isAuraOn);
    }
}
