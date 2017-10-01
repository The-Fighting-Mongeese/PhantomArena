using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Health : NetworkBehaviour
{

    public int maxHealth = 100;
    public Image fillImg; 
    public Text hpText;

    [SyncVar(hook ="OnChangeHealth")]
    public int currentHealth;


    private void Awake()
    {
        currentHealth = maxHealth;
        hpText.text = currentHealth + "/" + maxHealth;
    }

    
    void OnChangeHealth(int health)
    {
        fillImg.fillAmount = (float)health / maxHealth;
        hpText.text = health + "/" + maxHealth;
        //print("change hp: " + currentHealth + " / " + maxHealth + " divided=" +(float)currentHealth / maxHealth);
    }


    public void Heal(int amountHealed)
    {
        if (!isServer) return;  
        currentHealth += amountHealed;
        
    }

    [Command]
    public void CmdHeal(int amountHealed)
    {
        print( GetComponent<Chat>().pName + " cmd heal called");
        RpcHeal(amountHealed);
    }

    [ClientRpc]
    public void RpcHeal(int amountHealed)
    {
        currentHealth += amountHealed;
        hpText.text = currentHealth + "/" + maxHealth;
        print(GetComponent<Chat>().pName + " rpc heal called");
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
            currentHealth = maxHealth; //heal
            RpcRespawn();
        }
        
    }

    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            transform.position = Vector3.zero;
        }
    }


}

