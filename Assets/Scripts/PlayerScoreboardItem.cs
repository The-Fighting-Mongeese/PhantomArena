using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreboardItem : MonoBehaviour {

    public Text usernameText;
    public Text killsText;
    public Text deathsText;

    public void Setup(string username, int kills, int deaths)
    {
        usernameText.text = username;
        killsText.text = "Kills: " + kills;
        deathsText.text = "Deaths: " + deaths;
    }
}
