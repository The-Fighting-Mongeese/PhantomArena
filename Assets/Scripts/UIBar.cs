using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBar : MonoBehaviour
{
    public RectTransform Foreground;
    public Text UIText;
    public float Max = 100.0f;

    public void Init(float max)
    {
        Max = max;
        UpdateUI(max);
    }

    public void UpdateUI(float cur)
    {
        Foreground.localScale = new Vector3(cur/Max, 1, 1);
        UIText.text = cur.ToString("n0") + " / " + Max;
    }

    // Use this for initialization
    void Awake ()
    {
        Foreground.localScale = new Vector3(1, 1, 1);
        UIText.text = Max + " / " + Max;
    }
	
}
