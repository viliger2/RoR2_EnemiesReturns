using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ApplyRandomRotation : Editor
{
    [MenuItem("GameObject/Apply Random Rotation/Y", false, 10000)]
    public static void ApplyRandomRotationYRun(MenuCommand menuCommand) {
        GameObject obj = (GameObject)menuCommand.context;
        obj.transform.Rotate(0f, UnityEngine.Random.Range(0, 360), 0f);
    }

    [MenuItem("GameObject/Apply Random Rotation/Z", false, 10001)]
    public static void ApplyRandomRotationZRun(MenuCommand menuCommand) {
        GameObject obj = (GameObject)menuCommand.context;
        obj.transform.Rotate(0f, 0f, UnityEngine.Random.Range(0, 360));
    }


    [MenuItem("GameObject/Apply Random Rotation/X", false, 9999)]
    public static void ApplyRandomRotationXRun(MenuCommand menuCommand) {
        GameObject obj = (GameObject)menuCommand.context;
        obj.transform.Rotate(UnityEngine.Random.Range(0, 360), 0f, 0f);
    }    

    [MenuItem("GameObject/Apply Random Rotation/Full", false, 9000)]
    public static void ApplyRandomRotationFullRun(MenuCommand menuCommand) {
        GameObject obj = (GameObject)menuCommand.context;
        obj.transform.Rotate(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360));
    }        
}
