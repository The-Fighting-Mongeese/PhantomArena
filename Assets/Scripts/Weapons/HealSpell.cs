using UnityEngine.Networking;
using UnityEngine;


public class HealSpell : NetworkBehaviour
{

    public GameObject healAuraPrefab;
    public float timeAliveSeconds = 5.0f;
    public int healAmountPerTenthSecond = 1;
    public int manaCostPerTenthSecond = 1;
    private bool isAuraOn;

    private float nextTimeToFire = 0f;
    private float tickRatePerSecond = 10f; //called 10 times a second roughly

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2)) {
            isAuraOn = !isAuraOn;
            CmdCastHealAura(isAuraOn);
        }

        if (isAuraOn)
        {
            if (Time.time >= nextTimeToFire)
            {
                if (GetComponent<Health>().currentHealth < GetComponent<Health>().maxHealth
                    && GetComponent<Mana>().TryUseMana(manaCostPerTenthSecond))
                {
                    nextTimeToFire = Time.time + 1f / tickRatePerSecond;
                    GetComponent<Health>().CmdHeal(healAmountPerTenthSecond);
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
}
