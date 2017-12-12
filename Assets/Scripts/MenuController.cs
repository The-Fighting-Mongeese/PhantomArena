using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {
    [SerializeField]
    Button button1, button2, button3;


	// Use this for initialization
	void Start () {
        button1.Select();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Button1OnClick() {
        SceneManager.LoadScene("JasonWorkSpace");
    }
    public void Button2OnClick() {
        SceneManager.LoadScene("DanielTerrainScene");
    }
    public void Button3OnClick() {
        SceneManager.LoadScene("MarkusWorkSpace");
    }
}
