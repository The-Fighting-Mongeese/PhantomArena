﻿using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Health : NetworkBehaviour
{
    public GameObject model;
    public GameObject ragdoll;
    public AudioSource deathSfx;
    public ParticleSystem hitVfx;

    public int maxHealth = 100;
    public Image fillImg; 
    public Text hpText;             // health bar over all other players 

    [SyncVar(hook ="OnChangeHealth")]
    public int currentHealth;

    public float respawnDelay = 5f;

    public bool alive = true;

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
        if (!alive) return;
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
        Debug.Log("Current life " + currentHealth + " amount " + amount);
        if (!alive) return;

        currentHealth -= amount;    // syncvar - does not require Rpc call
        if (currentHealth <= 0)
        {
            Debug.Log("Dieing");
            alive = false;
            currentHealth = 0;

            // play death animation if it has one 
            var animc = GetComponent<AnimateController>();
            if (animc != null)
            {
                animc.RpcNetworkedTrigger("Die");   // don't use SetTrigger here or the anim will run twice for local player
                RpcSetBool("Dead", true);
            }

            // Update death score
            if (gameObject.CompareTag("Player"))
                GetComponent<PlayerMetrics>().deaths++;

            if (gameObject.CompareTag("Tower"))
            {
                RpcTowerRespawn();
                alive = true;
                currentHealth = maxHealth;
            }
            else
            {
                // respawn after delay
                CoroutineManager.Instance.StartCoroutine(RespawnAfterDelay(respawnDelay));
            }
        }
    }

    //temp to be fixed, for receiving damage and adding score
    [Command]
    public void CmdTakeTrueDamage2(uint attackerNetId, int amount)
    {
        if (!alive) return;

        if (hitVfx != null)         // play hit vfx if it has one
            hitVfx.Play();

        currentHealth -= amount;    // syncvar - does not require Rpc call
        if (currentHealth <= 0)
        {
            Debug.Log("Dieing");
            alive = false;
            currentHealth = 0;

            // play death animation if it has one 
            var animc = GetComponent<AnimateController>();
            if (animc != null)
            {
                animc.RpcNetworkedTrigger("Die");   // don't use SetTrigger here or the anim will run twice for local player
                RpcSetBool("Dead", true);
            }

            // play death sfx if it has one 
            if (deathSfx != null)
                deathSfx.Play();

            // Update death score
            if (gameObject.tag == "Player")
            {
                var players = GameObject.FindGameObjectsWithTag("Player");
                foreach(var p in players)
                {
                    if(p.GetComponent<NetworkIdentity>().netId.Value == attackerNetId)
                    {
                        p.GetComponent<PlayerMetrics>().kills++;
                    }
                }

                GetComponent<PlayerMetrics>().deaths++;
            }

            if (gameObject.CompareTag("Tower"))
            {
                RpcTowerRespawn();
                alive = true;
                currentHealth = maxHealth;
            }
            else
            {
                // respawn after delay
                CoroutineManager.Instance.StartCoroutine(RespawnAfterDelay(respawnDelay));
            }
        }
    }


    [Command]
    void CmdRespawn()
    {
        currentHealth = maxHealth;
        alive = true;
        RpcRespawn();
    }

    [ClientRpc]
    void RpcRespawn()
    {
        // disable ragdoll if it has one
        var ragdollhelper = GetComponent<RagdollHelper>();
        if (ragdollhelper != null)
            ragdollhelper.SetRagdollEnabled(false);

        // stop death animation if it has one 
        var animc = GetComponent<AnimateController>();
        if (animc != null)
            animc.anim.SetBool("Dead", false);

        // reset to spawn position
        GameObject[] spawnLocations = GameObject.FindGameObjectsWithTag("PlayerSpawn");
        transform.position = spawnLocations[Random.Range(0, spawnLocations.Length)].transform.position;

        // reset 
        gameObject.SetActive(true);
    }

    // This should be ran on the server only
    private IEnumerator RespawnAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        CmdRespawn();
    }

    [ClientRpc]
    private void RpcSetBool(string name, bool torf)
    {
        var animc = GetComponent<AnimateController>();
        if (animc != null)
        {
            animc.anim.SetBool(name, torf); // this will be ran twice, but with the same value there will be no problem
        }
    }

    [ClientRpc]
    void RpcTowerRespawn()
    {
        CoroutineManager.Instance.StartCoroutine(TowerRespawnAfterDelay(2.0f));
    }

    private IEnumerator TowerRespawnAfterDelay(float seconds)
    {
        gameObject.SetActive(false);
        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(true);
    }

}

