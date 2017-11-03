﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollider : MonoBehaviour
{
    public event Action<GameObject> OnOpponentTrigger;
    public delegate void OpponentTriggerEvent(GameObject other);
    
    public GameObject host;
    
    private Collider col;
    private Collider hostCol; 


    private void Awake()
    {
        col = GetComponent<Collider>();
        hostCol = host.GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject != host)
        {
            OnOpponentTrigger(other.gameObject);
        }
    }

    public void ActivateCollider()
    {
        col.enabled = true;
    }

    public void DeactivateCollider()
    {
        col.enabled = false;
    }
        
}