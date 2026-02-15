using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ApplyRandomRotation : Editor
{
    [MenuItem("GameObject/Apply Random Rotation", false, 10000)]
    public static void ApplyRandomRotationRun(MenuCommand menuCommand) {
        GameObject obj = (GameObject)menuCommand.context;
        obj.transform.Rotate(0f, UnityEngine.Random.Range(0, 360), 0f);
    }
}
