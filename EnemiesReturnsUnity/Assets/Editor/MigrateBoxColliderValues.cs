using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MigrateBoxColliderValues : Editor
{
    [MenuItem("GameObject/Migrate Box Collider Values", false, 10000)]
    public static void MigrateBoxColliderValuesCommand(MenuCommand menuCommand) {
        GameObject obj = (GameObject)menuCommand.context;
        var boxCollider = obj.GetComponent<BoxCollider>();
        obj.transform.localPosition = boxCollider.center;
        obj.transform.localScale = boxCollider.size;
        boxCollider.center = Vector3.zero;
        boxCollider.size = Vector3.one;
    }
}
