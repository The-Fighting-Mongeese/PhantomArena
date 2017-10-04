using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBar : MonoBehaviour
{
    public RectTransform Foreground;
    public Text UIText;
    public int Max = 100;


    public void UpdateUI(int cur)
    {
        Foreground.localScale = new Vector3(cur/Max, 1, 1);
        UIText.text = cur + " / " + Max;
    }

    // Use this for initialization
    void Awake ()
    {
        Foreground.localScale = new Vector3(1, 1, 1);
        UIText.text = Max + " / " + Max;
    }
	
}
