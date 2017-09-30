using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Health : NetworkBehaviour
{

    public int maxHealth = 100;
    public Image fillImg; 
    [SyncVar]
    public int currentHealth;


    private void Awake()
    {
        currentHealth = maxHealth;
    }

    [Command]
    public void CmdTakeTrueDamage(int amount)
    {
        RpcTakeTrueDamage(amount);
    }

    [ClientRpc]
    public void RpcTakeTrueDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            //die
        }
        print("hp left: " + currentHealth);

        fillImg.fillAmount -= 0.02f;
    }


}

