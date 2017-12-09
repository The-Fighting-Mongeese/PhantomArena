using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour {
    Light thisLight;
    float startingIntensity;


    // Use this for initialization
    void Start () {
        thisLight = GetComponent<Light>();
        startingIntensity = thisLight.intensity;
	}

    // Update is called once per frame
    private void Update() {
        thisLight.intensity = startingIntensity + 0.5f * Mathf.Sin(Time.fixedTime);
    }
}
