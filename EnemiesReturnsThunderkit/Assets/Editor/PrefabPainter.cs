using UnityEngine;
using UnityEditor;

public class PrefabPainter : EditorWindow
{
    private GameObject prefab;
    private Transform parent;

    private bool enablePainting = true;

    private float brushSize = 1f;
    private float density = 1f;

    private enum RotationMode
    {
        None,
        YAxisOnly,
        Full
    }

    private enum ScalingMode
    {
        Full,
        RandomEachAxis
    }

    private RotationMode rotationMode = RotationMode.YAxisOnly;

    private ScalingMode scalingMode = ScalingMode.Full;

    private Vector2 randomScaleRange = new Vector2(1f, 1f);

    [MenuItem("Tools/Prefab Painter")]
    public static void ShowWindow()
    {
        GetWindow<PrefabPainter>("Prefab Painter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Prefab Painting Tool", EditorStyles.boldLabel);

        // 🔘 Enable toggle
        enablePainting = EditorGUILayout.Toggle("Enable Painting", enablePainting);

        EditorGUILayout.Space();

        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
        parent = (Transform)EditorGUILayout.ObjectField("Parent", parent, typeof(Transform), true);

        brushSize = EditorGUILayout.Slider("Brush Size", brushSize, 0.1f, 10f);
        density = EditorGUILayout.Slider("Density", density, 1f, 20f);

        rotationMode = (RotationMode)EditorGUILayout.EnumPopup("Rotation Mode", rotationMode);
        scalingMode = (ScalingMode)EditorGUILayout.EnumPopup("Scaling Mode", scalingMode);

        randomScaleRange = EditorGUILayout.Vector2Field("Random Scale", randomScaleRange);

        EditorGUILayout.HelpBox("Hold SHIFT + Left Click in Scene View to paint.", MessageType.Info);

        if (!enablePainting)
        {
            EditorGUILayout.HelpBox("Painting is currently DISABLED.", MessageType.Warning);
        }
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (!enablePainting)
            return;

        Event e = Event.current;

        if (prefab == null)
            return;

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Handles.color = new Color(0, 1, 0, 0.5f);
            Handles.DrawSolidDisc(hit.point, hit.normal, brushSize);

            if (e.shift && e.type == EventType.MouseDown && e.button == 0)
            {
                Paint(hit.point);
                e.Use();
            }
        }

        sceneView.Repaint();
    }

    private void Paint(Vector3 center)
    {
        int count = Mathf.RoundToInt(density);

        for (int i = 0; i < count; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * brushSize;
            Vector3 offset = new Vector3(randomCircle.x, 0, randomCircle.y);

            Vector3 spawnPos = center + offset;

            Ray downRay = new Ray(spawnPos + Vector3.up * 10f, Vector3.down);
            if (Physics.Raycast(downRay, out RaycastHit hit, 50f))
            {
                GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                Undo.RegisterCreatedObjectUndo(obj, "Paint Prefab");

                obj.transform.position = hit.point;
                obj.transform.up = hit.normal;

                if (parent != null)
                {
                    obj.transform.SetParent(parent);
                }

                ApplyRotation(obj.transform);

                ApplyRandomScale(obj.transform);
                //float scale = Random.Range(randomScaleRange.x, randomScaleRange.y);
                //obj.transform.localScale = Vector3.one * scale;
            }
        }
    }

    private void ApplyRotation(Transform t)
    {
        switch (rotationMode)
        {
            case RotationMode.None:
                break;

            case RotationMode.YAxisOnly:
                t.Rotate(Vector3.up, Random.Range(0f, 360f), Space.World);
                break;

            case RotationMode.Full:
                t.rotation = Random.rotation;
                break;
        }
    }

    private void ApplyRandomScale(Transform t)
    {
        switch (scalingMode)
        {
            case ScalingMode.Full:
                float scale = Random.Range(randomScaleRange.x, randomScaleRange.y);
                t.localScale = Vector3.one * scale;
                break;
            case ScalingMode.RandomEachAxis:
                t.localScale = new Vector3(Random.Range(randomScaleRange.x, randomScaleRange.y), Random.Range(randomScaleRange.x, randomScaleRange.y), Random.Range(randomScaleRange.x, randomScaleRange.y));
                break;
        }
    }
}