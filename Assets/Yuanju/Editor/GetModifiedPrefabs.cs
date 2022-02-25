using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;



public class GetModifiedPrefabs: EditorWindow
{


    [MenuItem("Window/Modify Models")]
    public static void ShowWindow()
    {
        UnityEditor.EditorWindow window = GetWindow<GetModifiedPrefabs>("Get good Prefabs");
        window.Show();

    }




    void OnGUI()
    {
        if (GUILayout.Button("Start modify!"))
        {
            InstantiateAndSaveAsPrefabs();
            AdjustCenterToPivot();
            ComputeTranslationOffset();
            MovePartsToCorrectPosition();
        }
    }

    private int partNumber;
    private readonly List<GameObject> partInstances = new List<GameObject>();
    /// <summary>
    /// create the instances with the prefabs, put them into a list for the purpose of the modification save them in "Assets/SteamGeneratorSavonaModified/"
    /// </summary>
    public void InstantiateAndSaveAsPrefabs()
    {
        AssetDatabase.StartAssetEditing();
        // Find all prefabs that have 'generatore.3d' in their filename, and are placed in 'SteamGeneratorSavona' folder
        var guids = AssetDatabase.FindAssets("", new[] { "Assets/Yuanju/ToModify" });

        foreach (var g in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(g);
            var prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            var instance = Instantiate(prefab);
            partNumber++;
            //name the instances
            instance.name = prefab.name;
            //instance.tag = "Part";
            partInstances.Add(instance);
            //AdjustCenterToPivot(instance); //operates on the components of instance
            //PrefabUtility.ReplacePrefab(instance, prefab, ReplacePrefabOptions.ConnectToPrefab);
            PrefabUtility.SaveAsPrefabAsset(instance,"Assets/Yuanju/Modified " + instance.name + ".prefab");
        }

        AssetDatabase.StopAssetEditing();
    }

    /// <summary>
    /// Move the mesh to its pivot
    /// </summary>
    private readonly List<Vector3> centers = new List<Vector3>();
    private void AdjustCenterToPivot()
    {
        foreach (var instance in partInstances)
        {
            //get the center of the model 
            var center = instance.GetComponentInChildren<MeshFilter>() /*GetComponent<MeshCollider>()*/.sharedMesh
                .bounds.center;
            var mesh = instance.GetComponentInChildren<MeshFilter>().sharedMesh; //GetComponent<MeshFilter>().mesh;
            //record the center
            centers.Add(center);
            //deal with the mesh
            var vertices = mesh.vertices;
            //the vertices are locals                                                                                                        网格顶点是本地坐标
            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i] -= center;
            }
            mesh.vertices = vertices;
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();

            instance.GetComponentInChildren<MeshFilter>().mesh = mesh;
            DestroyImmediate(instance.GetComponentInChildren<MeshCollider>());
            //var meshCollider = instance.AddComponent<MeshCollider>();
            //meshCollider.convex = true;
            //meshCollider.sharedMesh = instance.GetComponentInChildren<MeshFilter>().mesh;
        }
    }

    /// <summary>
    /// compute the offsets to place the parts to the right positions
    /// </summary>
    private List<Vector3> translationVectors = new List<Vector3>();
    private void ComputeTranslationOffset()
    {
        var generatorCenter =
            new Vector3(centers.Average(x => x.x), centers.Average(x => x.y), centers.Average(x => x.z));
        foreach (var center in centers)
        {
            translationVectors.Add(center - generatorCenter);
        }
    }

    /// <summary>
    /// Translate the parts to the correct positions
    /// </summary>
    private void MovePartsToCorrectPosition()
    {
        //Debug.Log("I'm the parts amount"+ partInstances.Count);
        for (var j = 0; j < partInstances.Count; j++)
        {
            partInstances[j].transform.Translate(translationVectors[j]);
            Debug.Log("My transform was moved");
        }
    }

}