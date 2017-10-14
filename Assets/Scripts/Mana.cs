using UnityEngine;

public class Mana : MonoBehaviour {
    const int MAX_MANA = 100;
    const float RECHARGE_DELAY = 1;
    const float RECHARGE_RATE = 4;
    private float current_mana;
    private float time_last_mana_use;
    private UIBar _manaBar;

    private void Awake()
    {
        time_last_mana_use = Time.fixedTime;
        current_mana = MAX_MANA;
        _manaBar = GameObject.Find("CanvasUI").FindObject("ManaBar").GetComponent<UIBar>();
        _manaBar.Init(MAX_MANA);
    }

    public void Update()
    {
        //regenerates mana if greater than RECHARGE_DELAY seconds have passed
        //FIXME: current solution can skip up to Time.deltaTime of regeneration
        if (Time.fixedTime - time_last_mana_use - RECHARGE_DELAY > 0)
        {
            current_mana = Mathf.MoveTowards(current_mana, MAX_MANA, Time.deltaTime * RECHARGE_RATE);
            _manaBar.UpdateUI(current_mana);
        }
    }

    //tries to use mana, returns whether it was successful
    public bool TryUseMana(int amount)
    {
        if (current_mana >= amount)
        {
            current_mana -= amount;
            time_last_mana_use = Time.fixedTime;
            _manaBar.UpdateUI(current_mana);
            Debug.Log("used " + amount + " mana. " + current_mana + " mana remaining.");
            return true;
        }
        else
        {
            Debug.Log("failed to use " + amount + " mana. " + current_mana + " mana remaining.");
            return false;
        }
    }
}