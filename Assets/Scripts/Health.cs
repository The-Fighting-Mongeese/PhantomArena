using System.Collections;
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

    public float respawnDelay = 5f;

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
        if (!isServer)
        {
            Debug.LogError("Command not ran on server " + this);
        }

        Debug.Log("Current life " + currentHealth + " amount " + amount);

        currentHealth -= amount;    // syncvar - does not require Rpc call
        if (currentHealth <= 0)
        {
            Debug.Log("Dieing");
            currentHealth = 0;

            // play death animation if it has one 
            var animc = GetComponent<AnimateController>();
            if (animc != null)
            {
                animc.networkAnimator.SetTrigger("Die");
                animc.anim.SetBool("Dead", true);
            }

            // respawn after delay
            CoroutineManager.Instance.StartCoroutine(RespawnAfterDelay(respawnDelay));
        }
    }

    // DEPRECATED
    [ClientRpc]
    public void RpcTakeTrueDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            //die
            currentHealth = maxHealth; //heal
            CmdRespawn();
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

    // This should be ran on the server only
    private IEnumerator RespawnAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        // disable ragdoll if it has one
        var ragdollhelper = GetComponent<RagdollHelper>();
        if (ragdollhelper != null)
            ragdollhelper.SetRagdollEnabled(false);

        // stop death animation if it has one 
        var animc = GetComponent<AnimateController>();
        if (animc != null)
            animc.anim.SetBool("Dead", false);

        // reset to full HP
        currentHealth = maxHealth;

        // reset to spawn position
        Transform spawnLocation = GameObject.Find("NetworkSpawnLocation").transform;
        transform.position = spawnLocation.position;

        // reset 
        gameObject.SetActive(true);
    }

}

