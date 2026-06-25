using UnityEditor;
using UnityEngine;

public class FindReferences
{
    [MenuItem("Tools/Find References")]
    static void Find()
    {
        string targetPath = "Packages/com.unity.postprocessing/PostProcessing/Gizmos/PostProcessLayer.png";

        string[] allAssets = AssetDatabase.GetAllAssetPaths();

        foreach (string assetPath in allAssets)
        {
            string[] deps = AssetDatabase.GetDependencies(assetPath, true);

            foreach (string dep in deps)
            {
                if (dep == targetPath)
                {
                    Debug.Log($"{assetPath} depends on {targetPath}");
                    break;
                }
            }
        }
    }
}
