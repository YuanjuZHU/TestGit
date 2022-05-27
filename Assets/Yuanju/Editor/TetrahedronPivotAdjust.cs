using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


/// <summary>
/// this class is dedicated to the translation of the models that have pyramid referene, this class should be used once the models dragged to the scene
/// </summary>
public class TetrahedronPivotAdjust: EditorWindow
{
    public GameObject ReferenceTetrahedron;
    public Vector3 ReferenceCenter;
    GameObject[] objectsToAdjust;
    private readonly List<Vector3> tetrahedronCenters = new List<Vector3>();
    private readonly List<Vector3> Offsets = new List<Vector3>();

    List<GameObject> CreatedMarks = new List<GameObject>(); //this list is used for the facility of destroy the generated marks
    List<GameObject> Tretrahedrons = new List<GameObject>(); //this list is used for the facility of destroy the tretrahedrons

    [MenuItem("Window/Correct Pivot")]
    public static void ShowWindow()
    {
        UnityEditor.EditorWindow window = GetWindow<TetrahedronPivotAdjust>("Correct pivot");
        window.Show();

    }


    void OnGUI()
    {
        if (GUILayout.Button("Launch correction!"))
        {
            Offsets.Clear();
            tetrahedronCenters.Clear();

            FindReferenceCenters();
            ComputeOffsets();
            MoveGeneratorParts();
            UnpackPrefab();
            RemoveMarks();
            RemoveTetrahedrons();
        }
    }

    /// <summary>
    /// find the center of the trtrahedrons of the objects to be corrected
    /// </summary>
    private void FindReferenceCenters()
    {
        ReferenceTetrahedron = GameObject.Find("reference tetrahedron"); //the reference tetrahedron in the boiler body
        var markTarget = GameObject.CreatePrimitive(PrimitiveType.Cube);
        markTarget.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        markTarget.transform.position = ReferenceTetrahedron.GetComponentInChildren<MeshFilter>().sharedMesh.bounds.center;
        markTarget.transform.RotateAround(ReferenceTetrahedron.transform.position, Vector3.right, ReferenceTetrahedron.transform.eulerAngles.x);
        ReferenceCenter = markTarget.transform.position; //find the reference center of boiler body
        CreatedMarks.Add(markTarget);
        //Debug.Log("the position of the target object: " + markTarget.transform.position);


        objectsToAdjust = GameObject.FindGameObjectsWithTag("Pivot"); //all the objects that needs to be corrected should be tagged as "Pivot"
        //find the reference centers of the generator's parts that need to be corrected
        foreach (var GO in objectsToAdjust)
        {
            var tetrahedron= GO.transform.Find(" 2").gameObject;// this is the tetrahedorn in the objects to be corrected
            Tretrahedrons.Add(tetrahedron);
            //get the center of the model 
            var center = tetrahedron.GetComponentInChildren<MeshFilter>().sharedMesh.bounds.center;
            var markObjectToCorrect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            markObjectToCorrect.transform.position = center;
            markObjectToCorrect.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            markObjectToCorrect.transform.RotateAround(GO.transform.position, Vector3.right, tetrahedron.transform.eulerAngles.x);
            CreatedMarks.Add(markObjectToCorrect);
            tetrahedronCenters.Add(markObjectToCorrect.transform.position);
            //Debug.Log("the position of the object to be corrected: " + markObjectToCorrect.transform.position);
        }
    }


    /// <summary>
    /// compute the offsets to move the gameobjects to be corrected
    /// </summary>
    private void ComputeOffsets()
    {
        for (int i = 0; i < objectsToAdjust.Length; i++)
        {
            Offsets.Add(ReferenceCenter - tetrahedronCenters[i]);
        }

    }

    /// <summary>
    /// move the gameobjects to be corrected
    /// </summary>
    private void MoveGeneratorParts()
    {
        for (int i = 0; i < objectsToAdjust.Length; i++)
        {
            objectsToAdjust[i].transform.position += Offsets[i];
        }
    }


    /// <summary>
    /// remove the marks created
    /// </summary>
    public void RemoveMarks()
    {
        foreach (var item in CreatedMarks)
        {
            DestroyImmediate(item);
        }
    }

    /// <summary>
    /// unpack the objects to be corrected, if we do not unpack the gameobjects, the tetradrons can not be deleted
    /// </summary>
    public void UnpackPrefab()
    {
        foreach (var item in objectsToAdjust)
        {
            PrefabUtility.UnpackPrefabInstance(item, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);          
        }

    }

    /// <summary>
    /// remove the trtrahedrons
    /// </summary>
    public void RemoveTetrahedrons()
    {
        foreach (var item in Tretrahedrons)
        {

            DestroyImmediate(item);
        }
    }

}