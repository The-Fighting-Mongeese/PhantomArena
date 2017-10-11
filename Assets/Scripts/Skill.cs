using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    public abstract bool ConditionsMet();               // to check whether or not this skill can be casted
    public abstract void ConsumeResources();            // use required conditions to activate this skill            
    public abstract void Activate(GameObject other);    // if the attack hits, activate its effects 
}
