using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TellMeTheFuckingBoundingBox : Editor
{
    [MenuItem("GameObject/Bounding Box Size", false, 10000)]
    public static void BoundingBoxSize(MenuCommand menuCommand) {
        GameObject obj = (GameObject)menuCommand.context;
        var renderer = obj.GetComponent<Renderer>();
        if(renderer)
        {
            var size = renderer.bounds.size;
            Debug.Log($"bounds size is {size}, num for scaling is {size.x * size.y * size.z};");
        }
    }
}
