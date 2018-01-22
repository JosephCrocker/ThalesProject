using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

[CustomEditor(typeof(PathController))]
public class PathEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PathController _pathController = (PathController)target;

        GUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                _pathController.AddNode();
            }
            if (GUILayout.Button("SAVE"))
            {
                _pathController.Save();
            }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
            if (GUILayout.Button("-"))
            {
                _pathController.RemoveNode();
            }
            if (GUILayout.Button("LOAD"))
            {
                string path = EditorUtility.OpenFilePanel("Load Path from XML", "", "xml");
                if (path.Length != 0)
                {
                    _pathController.Load(path);
                }
            }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Calculate Traversal"))
        {
            _pathController.CalculateTraversalTime();
        }
    }
}
