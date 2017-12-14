using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformLayerExtensions {

    public static void SetAllLayers(this Transform trans, string layerName)
    {
        trans.gameObject.layer = LayerMask.NameToLayer(layerName);
        foreach (Transform child in trans)
            child.gameObject.layer = LayerMask.NameToLayer(layerName);
    }

    public static void SetAllLayers(this Transform trans, int layer)
    {
        trans.gameObject.layer = layer;
        foreach (Transform child in trans)
            child.gameObject.layer = layer;
    }
}
