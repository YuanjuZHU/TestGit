using System.Collections;
using System.Collections.Generic;
using Leap.Unity.Interaction;
using UnityEditor;
using UnityEngine;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System;
using System.Reflection;

//[CustomEditor(typeof(ActuatorSettings))]
[System.Serializable]
public class ReadCSVSetUpActuatorsEditor : EditorWindow
{
    public static DataTable dt;
    public static List<string> ElementTypes=new List<string>();
    public ActuatorSettings actuatorSettings;
    private string path;
    //private ActuatorSettings myTarget;

    [MenuItem("Window/ReadCSVSetUpActuatorsEditor")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        ReadCSVSetUpActuatorsEditor window = (ReadCSVSetUpActuatorsEditor)EditorWindow.GetWindow(typeof(ReadCSVSetUpActuatorsEditor));
        window.Show();
    }

    //get the info in Awake
    void Awake()
    {
        //todo expose the path in the inspector
        //dt = ReadDataFromCsv("Assets/Yuanju/elements of Tirreno Power models new.csv");  //read the CSV file and put the data into a data table
        //select the data rows who have the same name as the touched gameobject(eg. Select("name="+touched gameobject.parsedName) )
        //for (int i = 0; i < dt.Columns.Count; i++)
        //{
        //  Debug.Log("data columns[i] names: " + dt.Rows[dt.Rows.Count - 1][1]);
        //}

        //myTarget = (ActuatorSettings)target;
        ////dt.TableName = "Generator elements pre-setup";
        //myTarget.MyDatatable = dt;

        ////ElementTypes = GetDataFromTable(dt, "TYPE", true);
        ////foreach (var type in ElementTypes)
        ////{
        ////    CreateTag(type);
        ////}

        ////AttachAllTags();
        actuatorSettings = GameObject.Find("quadro electtrico manager").GetComponent<ActuatorSettings>();
    }

    #region Deal with data table
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="ColumnTitle"></param>
    /// <returns></returns>
    private List<string> GetDataFromTable(DataTable dt, string ColumnTitle, bool isExcludeDuplication )
    {
        DataRow[] drs = dt.Select(); //get all the rows of the data table
        List<string> myList = new List<string>();
        //find the types of the components
        foreach (var dr in drs)
        {
            //if (!myList.Contains(dr[ColumnTitle].ToString())) / this is a filter to make sure an element is only added once if there are duplications in the column
            //{
            if (!isExcludeDuplication)
            {
                myList.Add(dr[ColumnTitle].ToString());
            }

            else if (!myList.Contains(dr[ColumnTitle].ToString()))
            {
                myList.Add(dr[ColumnTitle].ToString());
            }

            //}
        }
        return myList;
    }
    /// <summary>
    /// read csv to dt
    /// </summary>
    /// <param name="file">csv</param>
    /// <returns>dt</returns>
    public static DataTable ReadDataFromCsv(string file)
    {
        DataTable dt = null;

        if (File.Exists(file))
        {
            dt = new DataTable();
            FileStream fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Read);

            StreamReader sr = new StreamReader(fs, Encoding.Default);

            string head = sr.ReadLine();
            string[] headNames = head.Split('&');
            for (int i = 0; i < headNames.Length; i++)
            {
                dt.Columns.Add(headNames[i], typeof(string));
            }
            while (!sr.EndOfStream)
            {
                #region ==read file in a loop==
                string lineStr = sr.ReadLine();
                if (lineStr == null || lineStr.Length == 0)
                    continue;
                string[] values = lineStr.Split('&');
                #region ==add row data==
                DataRow dr = dt.NewRow();
                for (int i = 0; i < values.Length; i++)
                {
                    dr[i] = values[i];
                }
                dt.Rows.Add(dr);
                #endregion
                #endregion
            }
            fs.Close();
            sr.Close();

        }
        return dt;
    }

    #endregion


    #region Create tags
    public static void CreateTag(string tag)
    {
        if(!UnityEditorInternal.InternalEditorUtility.tags.Contains(tag))
        {
            UnityEditorInternal.InternalEditorUtility.AddTag(tag);
        }

        //var asset = AssetDatabase.LoadMainAssetAtPath("ProjectSettings/TagManager.asset");
        //if (asset != null)
        //{ // sanity checking
        //    var so = new SerializedObject(asset);
        //    var tags = so.FindProperty("tags");

        //    var numTags = tags.arraySize;
        //    // do not create duplicates
        //    for (int i = 0; i < numTags; i++)
        //    {
        //        var existingTag = tags.GetArrayElementAtIndex(i);
        //        if (existingTag.stringValue == tag) return;
        //    }

        //    tags.InsertArrayElementAtIndex(numTags);
        //    tags.GetArrayElementAtIndex(numTags).stringValue = tag;
        //    so.ApplyModifiedProperties();
        //    so.Update();
        //}
    }
    #endregion

    #region Attach tags to corresponding gameobjects

    public void AttachAllTags()
    {
        var names= GetDataFromTable(dt, "NAME",false);
        var tags= GetDataFromTable(dt, "TYPE",false);

        for (int i = 0; i < names.Count; i++)
        {
            var go = GameObject.Find(names[i]);
            if (go!=null)
            {
                go.tag = tags[i];
            }
            //GameObject.Find(names[i]).tag = tags[i];
        }

    }

    #endregion


    public void OnGUI()
    {
        
        //base.OnInspectorGUI();

        //dt = ReadDataFromCsv(myTarget.path);

        if (GUILayout.Button("choose csv file:", GUILayout.Width(100), GUILayout.Height(20)))
        {
            //var note1 = EditorGUI.TextArea(new Rect(lastRect.width + 50f, lastRect.height + 45, 400f, 20f), "there is no file yet");
            path = EditorUtility.OpenFilePanel("Choose the configuration file", Application.dataPath, "csv");
            if (path.Length != 0)
            {
                var fileContent = File.ReadAllBytes(path);
                Debug.Log("the path of the file: "+ path);
            }
            dt = ReadDataFromCsv(path);
            dt.TableName = "Generator elements pre-setup";


            ElementTypes = GetDataFromTable(dt, "TYPE", true);
            foreach (var type in ElementTypes)
            {
                CreateTag(type);
            }

            AttachAllTags();

        }
        EditorGUILayout.TextField("file path:", path);
        actuatorSettings.path = path;
        //var note = EditorGUI.TextArea(new Rect(lastRect.width, lastRect.height, lastRect.width, lastRect.height), "this is a text");



        GUILayout.Label("Attach the actuator classes to gameobjects:");
        if (GUILayout.Button("Set up the components", GUILayout.Width(400), GUILayout.Height(40)))
        {
            dt = ReadDataFromCsv(path);
            dt.TableName = "Generator elements pre-setup";
            //ElementTypes = GetDataFromTable(dt, "TYPE", true);
            AttachActuatorClass();
            Debug.Log("\"Set up the components\" has been clicked");
        }

        GUILayout.Label("Set up the mesh colliders for the actuators:");
        if (GUILayout.Button("Set up colliders", GUILayout.Width(400), GUILayout.Height(40)))
        {
            dt = ReadDataFromCsv(path);
            dt.TableName = "Generator elements pre-setup";
            //ElementTypes = GetDataFromTable(dt, "TYPE", true);
            SetUpColliders();
            Debug.Log("\"Set up colliders\" has been clicked");
        }


        GUILayout.Label("Set the rotating axis of the hinge joints:");
        if (GUILayout.Button("Configure hinge joints", GUILayout.Width(400), GUILayout.Height(40)))
        {
            dt = ReadDataFromCsv(path);
            dt.TableName = "Generator elements pre-setup";
            //ElementTypes = GetDataFromTable(dt, "TYPE", true);
            ConfigureHingeJoints();
            Debug.Log("\"Configure hinge joints\" has been clicked");
        }

        GUILayout.Label("Fill in some properties of the actuator classes:");
        if (GUILayout.Button("Configure object(actuator) classes", GUILayout.Width(400), GUILayout.Height(40)))
        {
            dt = ReadDataFromCsv(path);
            dt.TableName = "Generator elements pre-setup";
            //ElementTypes = GetDataFromTable(dt, "TYPE", true);
            ConfigureActuatorClass();
            Debug.Log("\"Configure object(actuator) classes\" has been clicked");
        }
    }

    public void AttachActuatorClass()
    {
        var names = GetDataFromTable(dt, "NAME", false);
        var types = GetDataFromTable(dt, "TYPE", false);


        for (int i = 0; i < names.Count; i++)
        {
            var go = GameObject.Find(names[i]);
            if (go != null)
            {
                //var assemblyPath = this.GetType().Assembly;
                Assembly assembly = Assembly.Load("Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"/*"Assembly-CSharp"*/);
                //Debug.Log("is assembly got? " + assembly);
                //Type mytype = assembly.GetType("Switch")
                //todo the types in .csv file should be the same as the names of the object classes
                Type mytype = assembly.GetType(go.tag); // find object class by using the tag on the game object in assembly
                if (go.GetComponent<IGeneratorComponent>() == null && mytype != null) 
                {
                    go.AddComponent(mytype);
                    Debug.Log(string.Format("the components for {0} have been added! ", go));
                }

                //arrIndexObj = assembly.CreateInstance(elementType.FullName, true);
                //object myTypeobject = assembly.CreateInstance("Switch", false);
                //object myTypeOfobject = Activator.CreateInstance(typeOf,false);
                //object mysWitch = Activator.CreateInstance(getType,false);

                //var manager = GameObject.Find("quadro electtrico manager");
                //manager.AddComponent(mytype);

                //char[] chars = new char[26];
                //for (int ctr = 0; ctr < 26; ctr++)
                //    chars[ctr] = (char)(ctr + 0x0061);

                //object obj = Activator.CreateInstance(typeof(Switch), false /*new object[] { chars, 13, 10 }*/);

                //Debug.Log("MSDN test " + obj);
            }
        }

    }

    public void ConfigureHingeJoints()
    {
        var names = GetDataFromTable(dt, "NAME", false);
        var x = GetDataFromTable(dt, "ROTATING AXIS x", false);
        var y = GetDataFromTable(dt, "ROTATING AXIS y", false);
        var z = GetDataFromTable(dt, "ROTATING AXIS z", false);

        for (int i = 0; i < names.Count; i++)
        {
            int.TryParse(x[i], out int a);
            int.TryParse(y[i], out int b); //parse the values read from .CSV 
            int.TryParse(z[i], out int c);
            

            var go = GameObject.Find(names[i]);
            if (go != null) 
            {
                if (go.TryGetComponent(out HingeJoint hingeJoint))
                {
                    hingeJoint.axis = new Vector3(a,b,c);  //assign the value read in .CSV to the axis within hinge joints
                    Debug.Log(string.Format("hingeJoint: {0}'s axis is updated as: {1}. ", hingeJoint, new Vector3(a, b, c)));
                }
            }
            else
            {
                Debug.Log("there is a gameobject that can not be found in the scene: " + names[i]);
            }

        }
    }


    public void ConfigureActuatorClass()
    {
        var names = GetDataFromTable(dt, "NAME", false);
        var statusNumber = GetDataFromTable(dt, "TOTAL STATUS", false);

        for (int i = 0; i < names.Count; i++)
        {
            int.TryParse(statusNumber[i], out int a);

            var go = GameObject.Find(names[i]);
            if (go != null)
            {
                if (go.TryGetComponent(out IGeneratorComponent objectClass))
                {
                    //if (objectClass is IRotatableComponent)
                    //{
                    //    //Debug.Log("this is a rotatable component: "+ objectClass.GetType().ToString());
                    //}

                    switch (objectClass.GetType().ToString())
                    {
                        case "Switch":
                            Debug.Log("this is a switch component: " + objectClass.GetType().ToString());
                            var generatorSwitch = objectClass as Switch; //the object class is a switch class

                            if (generatorSwitch!=null)
                            {
                                generatorSwitch.IsAdsorbent = true;

                                //initialize the size of adsorbent angles
                                if (generatorSwitch._adsorbableAngles.Length == 0) 
                                {
                                    float[] angleSize = new float[a];
                                    generatorSwitch._adsorbableAngles = angleSize;
                                }


                                //the hinge joint's limits are set by using the switch.cs 
                                var hingeJoint = generatorSwitch.gameObject.GetComponent<HingeJoint>();
                                hingeJoint.useLimits = true;
                                JointLimits hingeLimits=new JointLimits();
                                hingeLimits.min = generatorSwitch.angleRange.min;
                                hingeLimits.max = generatorSwitch.angleRange.max;
                                hingeJoint.limits = hingeLimits;

                                //initialize the number of materials
                                if (generatorSwitch.SwitchStatusMaterials.MaterialSet.Count == 0) 
                                {
                                    for (int j = 0; j < a; j++)
                                    {
                                        var material = new SwitchPartsMaterialSets();
                                        material.FontName = string.Format("Status {0} materials ", j);
                                        generatorSwitch.SwitchStatusMaterials.MaterialSet.Add(material);
                                    }
                                }

                                //fill in the properties in rigidbody
                                var rigidbody = generatorSwitch.gameObject.GetComponent<Rigidbody>();
                                rigidbody.useGravity = false;
                                rigidbody.isKinematic = true;

                                //fill in the properties in interaction behaviour
                                var interactionBehaviour = generatorSwitch.gameObject.GetComponent<InteractionBehaviour>();
                                interactionBehaviour.graspedMovementType = InteractionBehaviour.GraspedMovementType.Nonkinematic;

                            }
                            break;

                        default:
                            Debug.Log("this is an another component: "+ objectClass.GetType().ToString());
                            break;
                    }

                    //hingeJoint.axis = new Vector3(a, b, c);  //assign the value read in .CSV to the axis within hinge joints
                    //Debug.Log(string.Format("hingeJoint: {0}'s axis is updated as: {1}. ", hingeJoint, new Vector3(a, b, c)));
                }
            }
            else
            {
                Debug.Log("there is a gameobject that can not be found in the scene: " + names[i]);
            }

        }
    }

    public void SetUpColliders()
    {
        var names = GetDataFromTable(dt,"NAME",false);

        foreach (var name in names)
        {
            var go = GameObject.Find(name);
            if (go.transform.childCount == 0 && go.GetComponent<MeshCollider>() == null) 
            {
                var col = go.AddComponent<MeshCollider>();
                col.convex = true;
                Debug.Log(string.Format("{0}'s collider has been added.",name) );
            }
            else
            {
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    var child = go.transform.GetChild(i);

                    if (child.gameObject.GetComponent<MeshCollider>() == null) 
                    {
                        var col = child.gameObject.AddComponent<MeshCollider>();
                        col.convex = true;
                        Debug.Log(string.Format("the part{0} of {1}'s collider has been added.", i + 1, name));
                    }
                }
            }
        }
    }
}




 
