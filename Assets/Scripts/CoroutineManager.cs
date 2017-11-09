using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object that is always enabled to run coroutines.
/// </summary>
public class CoroutineManager : MonoBehaviour
{
    private static CoroutineManager instance = null;
    public static CoroutineManager Instance
    {
        get
        {
            return instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instance == null) instance = this;
    }
}