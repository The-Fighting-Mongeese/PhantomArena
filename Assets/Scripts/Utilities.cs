using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class Utilities
{
    public static T GetBehaviour<T>(this Animator animator, String id) where T : SkillStateMachine
    {
        return animator.GetBehaviours<T>().ToList().First(behaviour => behaviour.id == id);
    }
}