using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
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
