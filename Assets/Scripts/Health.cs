using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Health : NetworkBehaviour
{

    public int maxHealth = 100;
    public Image fillImg; 

    private void Awake()
    {
        currentHealth = maxHealth;
    }


    [SyncVar]
    public int currentHealth;

    public void TakeTrueDamage(int amount)
    {
        currentHealth -= amount;
        if(currentHealth <= 0)
        {
            //die
        }
        print("hp left: " + currentHealth);

        fillImg.fillAmount -= 0.02f;

    }


}

