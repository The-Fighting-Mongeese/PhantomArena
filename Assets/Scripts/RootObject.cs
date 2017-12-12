using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Roots object on awake
/// </summary>
public class RootObject : MonoBehaviour
{
    void Awake()
    {
        this.transform.parent = null;
    }
}
