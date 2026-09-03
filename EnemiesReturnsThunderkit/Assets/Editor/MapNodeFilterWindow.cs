using UnityEngine;
using UnityEditor;
using RoR2.Navigation;

public class MapNodeFilterWindow : EditorWindow
{
    private string targetGateName = "";
    
    [MenuItem("Tools/MapNode Filter")]
    public static void ShowWindow()
    {
        GetWindow<MapNodeFilterWindow>("MapNode Filter");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Filter MapNodes by Gate Name", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        
        targetGateName = EditorGUILayout.TextField("Target Gate Name:", targetGateName);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Disable Non-Matching MapNodes"))
        {
            DisableNonMatchingMapNodes();
        }
        
        if (GUILayout.Button("Enable All MapNodes"))
        {
            EnableAllMapNodes();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Find Matching MapNodes"))
        {
            FindMatchingMapNodes();
        }
    }
    
    private void DisableNonMatchingMapNodes()
    {
        if (string.IsNullOrEmpty(targetGateName))
        {
            EditorUtility.DisplayDialog("Error", "Please enter a target gate name.", "OK");
            return;
        }
        
        MapNode[] allMapNodes = FindObjectsOfType<MapNode>(true);
        int disabledCount = 0;
        
        // Group for undo
        Undo.IncrementCurrentGroup();
        
        foreach (MapNode mapNode in allMapNodes)
        {
            if (mapNode.gateName != targetGateName)
            {
                Undo.RecordObject(mapNode.gameObject, "Disable MapNode");
                mapNode.gameObject.SetActive(false);
                disabledCount++;
            }
        }
        
        Debug.Log($"Disabled {disabledCount} MapNodes where gateName != '{targetGateName}'.");
    }
    
    private void EnableAllMapNodes()
    {
        MapNode[] allMapNodes = FindObjectsOfType<MapNode>(true);
        
        Undo.IncrementCurrentGroup();
        
        foreach (MapNode mapNode in allMapNodes)
        {
            Undo.RecordObject(mapNode.gameObject, "Enable MapNode");
            mapNode.gameObject.SetActive(true);
        }
        
        Debug.Log($"Enabled {allMapNodes.Length} MapNode GameObjects.");
    }
    
    private void FindMatchingMapNodes()
    {
        if (string.IsNullOrEmpty(targetGateName))
        {
            EditorUtility.DisplayDialog("Error", "Please enter a target gate name.", "OK");
            return;
        }
        
        MapNode[] allMapNodes = FindObjectsOfType<MapNode>(true);
        var matchingNodes = new System.Collections.Generic.List<GameObject>();
        
        foreach (MapNode mapNode in allMapNodes)
        {
            if (mapNode.gateName == targetGateName)
            {
                matchingNodes.Add(mapNode.gameObject);
            }
        }
        
        if (matchingNodes.Count > 0)
        {
            Selection.objects = matchingNodes.ToArray();
            Debug.Log($"Found {matchingNodes.Count} MapNodes with gateName '{targetGateName}'. Selected in Hierarchy.");
        }
        else
        {
            Debug.Log($"No MapNodes found with gateName '{targetGateName}'.");
        }
    }
}