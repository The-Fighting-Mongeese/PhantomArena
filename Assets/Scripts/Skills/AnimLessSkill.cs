using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// Simple version of skill that does not require an animation state / clip. Use for prototyping. 
public abstract class AnimLessSkill : Skill
{
    protected virtual void Awake()
    {
        // do not call base here

        player = GetComponent<PlayerController>();

        if (phantomLayer == 0)
            phantomLayer = LayerMask.NameToLayer("Phantom");
        if (physicalLayer == 0)
            physicalLayer = LayerMask.NameToLayer("Physical");
    }

    protected override void OnEnable()
    {
        // do not call base here
    }

    protected override void OnDisable()
    {
        // do not call base here
    }
}
