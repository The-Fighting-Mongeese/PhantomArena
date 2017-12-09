using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDebug : MonoBehaviour {

    public AudioRandom ar;

	// Use this for initialization
	void Start () {
        ar = GetComponent<AudioRandom>();
	}
	
	// Update is called once per frame
	void Update () {
		
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ar.Play();
        }
	}
}
