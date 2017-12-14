using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility class that provides basic info and functions about PhantomArena layers.
/// Please ensure this script is ran first in ScriptExecutionOrder
/// </summary>
public class LayerHelper : MonoBehaviour {

    public static int PhysicalLayer { get; set; }
    public static int PhantomLayer { get; set; }
    public static int GaiaLayer { get; set; }
    public static int WalkablePhysical { get; set; }
    public static int WalkablePhantom { get; set; }

    void Awake()
    {
        PhantomLayer    = LayerMask.NameToLayer("Phantom");
        PhysicalLayer   = LayerMask.NameToLayer("Physical");
        GaiaLayer       = LayerMask.NameToLayer("Gaia");

        WalkablePhysical = LayerMask.GetMask("Physical", "Gaia");
        WalkablePhantom  = LayerMask.GetMask("Phantom", "Gaia");
    }

    public static int Opposite(int layer)
    {
        return (layer == PhantomLayer) ? PhysicalLayer : PhantomLayer;
    }
}
