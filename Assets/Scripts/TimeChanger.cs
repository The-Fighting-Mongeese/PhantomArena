using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeChanger : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Minus))
        {
            Time.timeScale -= 0.1f;
        }
        else if (Input.GetKeyDown(KeyCode.Plus))
        {
            Time.timeScale += 0.1f;
        }
	}
}
