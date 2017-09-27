using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCore : MonoBehaviour {

    public bool CoreInside { get { return inCount > 0;  } } 

    [SerializeField]
    private int inCount = 0;            // keeps track of how many objects the core is in right now 


    void OnTriggerEnter(Collider other)
    {
        Debug.Log("CORE TRIGGER ENTER");
        Debug.Log(other);
        inCount++;
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("CORE TRIGGER EXIT");
        inCount--;
    }

}

