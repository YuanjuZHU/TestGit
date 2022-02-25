using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CheckIstrigger : EditorWindow
{
    [MenuItem("Tools/Check Istrigger")]
    public static void ShowWindow()
    {
        UnityEditor.EditorWindow window = GetWindow<CheckIstrigger>("Check");
        window.Show();

    }

    void OnGUI()
    {
        if (GUILayout.Button("Check"))
        {
            var parts = GameObject.FindGameObjectsWithTag("Part");
            foreach (var part in parts)
            {
                part.GetComponent<MeshCollider>().isTrigger = true;
            }
        }
    }
}
