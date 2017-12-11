using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillIndicator : MonoBehaviour {
    public Text buttonNameText;
    public Text cooldownCounterText;
    public Image skillIconImage;

    public void SetButtonNameText(string buttonName) {
        buttonNameText.text = buttonName;
    }

    public void UpdateUI(bool conditionsMet, float cooldownCounter) {
        if (conditionsMet) {
            skillIconImage.color = Color.white;
        } else {
            skillIconImage.color = Color.gray;
        }

        if (cooldownCounter != 0) {
            cooldownCounterText.text = cooldownCounter.ToString("f1");
        } else {
            cooldownCounterText.text = string.Empty;
        }
    }


}
