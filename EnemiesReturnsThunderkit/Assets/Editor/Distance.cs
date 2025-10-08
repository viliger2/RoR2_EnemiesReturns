using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Distance : MonoBehaviour
{
    [MenuItem("GameObject/Distance Between Objects", false, 10000)]
    public static void MigrateBoxColliderValuesCommand(MenuCommand menuCommand) {
        if (Selection.objects.Length > 1)
		{
            var GameObject1 = (GameObject)Selection.objects[0];
            var GameObject2 = (GameObject)Selection.objects[1];
            Debug.Log(Vector3.Distance(GameObject1.transform.position, GameObject2.transform.position));
		}
    }
}
