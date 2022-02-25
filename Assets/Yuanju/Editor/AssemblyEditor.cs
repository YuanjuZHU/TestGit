using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum OPTION
{
    Button = 0,
    Switch = 1,
    Valve = 2
}

public class AssemblyEditor : EditorWindow
{
    //private Valve valve;
    private string _labelComposeClicked  = "  ";
    private string _labelDecomposeClicked  = "  ";
    //the component gameobject
    private GameObject _componentObj;
    public OPTION OptionComponent;
    //selected gameobject amount
    private int _selectedObjects = 0;
    //selected component amount
    private int _selectedComponents = 0;
    private List<Vector3> _posParts = new List<Vector3>();

    [MenuItem("Window/Assembler")]
    public static void ShowWindow()
    {
        UnityEditor.EditorWindow window = GetWindow<AssemblyEditor>("Assembler");
        window.Show();

    }

    void OnGUI()
    {
        GUILayout.Label("Compose a component:  "+ _selectedObjects + " part(s) selected", EditorStyles.boldLabel);
        _selectedObjects = 0;
        _selectedComponents = 0;
        _posParts.Clear();
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            if (Selection.gameObjects[i].tag == "Part")
            {
                _selectedObjects += 1;
                _posParts.Add(Selection.gameObjects[i].transform.localPosition);

            }
            if (Selection.gameObjects[i].tag == "Component")
                _selectedComponents += 1;
        }
        //enum chose between the components types(switch, button...)
        OptionComponent = (OPTION)EditorGUILayout.EnumPopup("Choose component type:", OptionComponent);
        //if the "Assemble" button is clicked
        if (GUILayout.Button("Compose"))
        {
            //if some gameobject is selected, and in the selection we can find any part instead of component
            if (Selection.gameObjects.Length != 0 && _selectedObjects > 0) 
            {
                //the assignment of the name and the tag of the component
                _componentObj = new GameObject(OptionComponent.ToString());

                Debug.Log("I'm the good local position: " + new Vector3(_posParts.Average(x => x.x), _posParts.Average(x => x.y),
                    _posParts.Average(x => x.z)));
                _componentObj.transform.SetParent(GameObject.Find("Generator V3").transform);
                _componentObj.transform.localPosition = new Vector3(_posParts.Average(x => x.x),
                    _posParts.Average(x => x.y), _posParts.Average(x => x.z));
                _componentObj.transform.localEulerAngles=new Vector3(0,0,0);
                _componentObj.transform.localScale=new Vector3(1,1,1);
                _componentObj.tag = "Component";
                //find the parts and set them to the component
                foreach (var item in Selection.gameObjects)
                {
                    if (item.tag != "Component")
                    {
                        Debug.Log("I'm the local position of the part: "+ item.transform.localPosition);
                        item.transform.SetParent(_componentObj.transform);
                    }
                }

                _labelComposeClicked = OptionComponent + " Assembled!";
            }

            else
            {
                _labelComposeClicked = "Assemble failed!";
            }

        }
        GUILayout.Label(_labelComposeClicked, EditorStyles.boldLabel);
        GUILayout.Label("======================================================================================================", EditorStyles.boldLabel);
        GUILayout.Label("Decompose a component:  " + _selectedComponents + " component(s) selected", EditorStyles.boldLabel);

        if (GUILayout.Button("Decompose"))
        {
            //loop for all the gameobjects selected
            foreach (var component in Selection.gameObjects)
            {
                //how many children under a parent gameobject(component)
                var componentChildCount = component.GetComponentsInChildren<MeshRenderer>(true);
                Debug.Log("componentChildCount " + componentChildCount.Length);
                if (component.tag == "Component")
                {
                    //reset the parent for each part reversely to avoid deleting an element in an array while iterating it
                    for (int i = component.transform.childCount - 1; i >= 0; i--)
                    {
                        component.transform.GetChild(i).gameObject.transform.SetParent(GameObject.Find("Generator V3").transform);
                    }
                    DestroyImmediate(component);
                    _labelDecomposeClicked = "Component(s) decomposed!";
                }
                else
                {
                    _labelDecomposeClicked = "Decompose failed, not a component !";
                }
            }

            Debug.Log("Disassemble Button was clicked");
        }
        GUILayout.Label(_labelDecomposeClicked, EditorStyles.boldLabel);
    }

}
