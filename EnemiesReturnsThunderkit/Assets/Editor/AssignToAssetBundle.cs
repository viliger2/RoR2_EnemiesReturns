using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundleAssignerWindow : EditorWindow
{
    private DefaultAsset folder;
    private string bundleName = "";

    [MenuItem("Tools/Asset Bundles/Assign Folder To AssetBundle")]
    public static void ShowWindow()
    {
        GetWindow<AssetBundleAssignerWindow>("Assign AssetBundle");
    }

    private void OnGUI()
    {
        GUILayout.Label("Assign AssetBundle", EditorStyles.boldLabel);

        folder = (DefaultAsset)EditorGUILayout.ObjectField(
            "Folder",
            folder,
            typeof(DefaultAsset),
            false);

        bundleName = EditorGUILayout.TextField("Bundle Name", bundleName);

        GUI.enabled = folder != null;

        if (GUILayout.Button("Assign"))
        {
            AssignBundle();
        }

        GUI.enabled = true;
    }

    private void AssignBundle()
    {
        string folderPath = AssetDatabase.GetAssetPath(folder);

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogError("Please select a valid folder.");
            return;
        }

        string finalBundleName = string.IsNullOrWhiteSpace(bundleName)
            ? Path.GetFileName(folderPath).ToLowerInvariant()
            : bundleName.Trim().ToLowerInvariant();

        string[] guids = AssetDatabase.FindAssets("", new[] { folderPath });

        int count = 0;

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            if (AssetDatabase.IsValidFolder(assetPath))
                continue;

            AssetImporter importer = AssetImporter.GetAtPath(assetPath);

            if (importer == null)
                continue;

            importer.assetBundleName = finalBundleName;
            count++;
        }

        AssetDatabase.RemoveUnusedAssetBundleNames();
        AssetDatabase.SaveAssets();

        Debug.Log($"Assigned {count} assets in '{folderPath}' to AssetBundle '{finalBundleName}'.");
    }
}