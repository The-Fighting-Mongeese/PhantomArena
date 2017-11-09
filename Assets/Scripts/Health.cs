using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Health : NetworkBehaviour
{
    public GameObject model;
    public GameObject ragdoll;

    public int maxHealth = 100;
    public Image fillImg; 
    public Text hpText;             // health bar over all other players 

    [SyncVar(hook ="OnChangeHealth")]
    public int currentHealth;

    private UIBar _healthBar;       // health bar HUD (for current player only)


    private void Awake()
    {
        _healthBar = GameObject.Find("CanvasUI").FindObject("HealthBar").GetComponent<UIBar>();
        _healthBar.Init(maxHealth);
        currentHealth = maxHealth;
        hpText.text = currentHealth + "/" + maxHealth;
    }

    
    void OnChangeHealth(int health)
    {
        currentHealth = health;
        fillImg.fillAmount = (float)health / maxHealth;
        hpText.text = health + "/" + maxHealth;

        if (isLocalPlayer)
            _healthBar.UpdateUI(health);
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

            // GetComponent<Collider>().enabled = false;
            // GetComponent<RagdollHelper>().SetRagdollEnabled(true);
            // Instantiate(ragdoll);

            // CmdRespawn();
        }
        
    }

    [Command]
    void CmdRespawn()
    {
        RpcRespawn();
    }

    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            Transform spawnLocation = GameObject.Find("NetworkSpawnLocation").transform;
            transform.position = spawnLocation.position;
        }
    }


}

