using UnityEngine;
using UnityEngine.Networking;

public class Stamina : NetworkBehaviour {

    public float CurrentStamina { get { return current_stamina; } }

    const int MAX_STAMINA = 100;
    const float RECHARGE_DELAY = 1;
    const float RECHARGE_RATE = 4;
    private float current_stamina;
    private float time_last_stamina_use;

    private UIBar _staminaBar;

    private void Awake()
    {
        time_last_stamina_use = Time.fixedTime;
        current_stamina = MAX_STAMINA;
        _staminaBar = GameObject.Find("CanvasUI").FindObject("StaminaBar").GetComponent<UIBar>();
        _staminaBar.Init(MAX_STAMINA);
    }

    public void Update()
    {
        if (!isLocalPlayer)
            return;
        //regenerates stamina if greater than RECHARGE_DELAY seconds have passed
        //FIXME: current solution can skip up to Time.deltaTime of regeneration
        if (Time.fixedTime - time_last_stamina_use - RECHARGE_DELAY > 0)
        {
            current_stamina = Mathf.MoveTowards(current_stamina, MAX_STAMINA, Time.deltaTime * RECHARGE_RATE);
            _staminaBar.UpdateUI(current_stamina);
        }
    }

    //tries to use stamina, returns whether it was successful
    public bool TryUseStamina(int amount)
    {
        if (current_stamina >= amount)
        {
            current_stamina -= amount;
            time_last_stamina_use = Time.fixedTime;
            _staminaBar.UpdateUI(current_stamina);
            Debug.Log("used " + amount + " stamina. " + current_stamina + " stamina remaining.");
            return true;
        }
        else
        {
            Debug.Log("failed to use " + amount + " stamina. " + current_stamina + " stamina remaining.");
            return false;
        }
    }
}
