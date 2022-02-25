using System.Collections;
using System.Collections.Generic;
using Leap.Unity.Interaction;
using MGS.UCommon.Generic;
using UnityEditor;
using UnityEngine;

public class SetGeneratorComponents : EditorWindow
{
    [MenuItem("Tools/Set the generator's components")]
    public static void ShowWindow()
    {
        UnityEditor.EditorWindow window = GetWindow<SetGeneratorComponents>("Configurate the project!");
        window.Show();

    }

    void OnGUI()
    {
        if (GUILayout.Button("Set Materials"))
        {
           
            //Material Green = (Material)AssetDatabase.LoadAssetAtPath("Assets/Yuanju/Materials/Green Yuanju.mat", typeof(Material));
            //Material LightGreen = (Material)AssetDatabase.LoadAssetAtPath("Assets/Yuanju/Materials/Green light Yuanju.mat", typeof(Material));
            //Material Red = (Material)AssetDatabase.LoadAssetAtPath("Assets/Yuanju/Materials/Red Yuanju.mat", typeof(Material));
            //Material LightRed = (Material)AssetDatabase.LoadAssetAtPath("Assets/Yuanju/Materials/Red light Yuanju.mat", typeof(Material));
            //Material Blue = (Material)AssetDatabase.LoadAssetAtPath("Assets/Yuanju/Materials/Blue Yuanju.mat", typeof(Material));
            //Material LightBlue = (Material)AssetDatabase.LoadAssetAtPath("Assets/Yuanju/Materials/Blue light Yuanju.mat", typeof(Material));
            //Material Silver = (Material)AssetDatabase.LoadAssetAtPath("Assets/Yuanju/Materials/Silver Yuanju.mat", typeof(Material));
            //Material LightSilver = (Material)AssetDatabase.LoadAssetAtPath("Assets/Yuanju/Materials/Silver light Yuanju.mat", typeof(Material));

            var components = GameObject.FindGameObjectsWithTag("Component");
            foreach (var component in components)
            {
                //add and set the corresponding script components to the generator components
                #region setup the switch
                if (component.name.Contains("switch"))
                {
                    //add the components
                    component.AddComponent<Rigidbody>();
                    component.AddComponent<HingeJoint>();
                    component.AddComponent<Switch>();
                    //component.AddComponent<InteractionBehaviour>();

                    //set the rigidbody
                    var rigidbody = component.GetComponent<Rigidbody>();
                    rigidbody.useGravity = false;
                    rigidbody.isKinematic = true;

                    //set the hingejoint
                    var hingeJoint = component.GetComponent<HingeJoint>();
                    hingeJoint.useLimits = true;
                    //set the rotating axis
                    if (component.name.Contains("front"))
                    {
                        hingeJoint.axis= Vector3.up;
                    }
                    if (component.name.Contains("top"))
                    {
                        hingeJoint.axis = Vector3.forward;
                    }
                    if (component.name.Contains("side"))
                    {
                        hingeJoint.axis = Vector3.right;
                    }
                    
                    //settings in switch class
                    var selecteur = component.GetComponent<Switch>();
                    //selecteur.RotateLimit = true;
                    selecteur.AngleRange = new Range(-30, 30);
                    selecteur.IsAdsorbent = true;

                    //************************************************************************************
                    //here should get the status amounts for the switch
                    //for (int i = 0; i < 3; i++)
                    //{
                    //  selecteur.AdsorbableAngles[1] = -30;
                    //}
                    ////selecteur.SwitchStatusAmounts = 3;
                    ////var myList = selecteur.AdsorbableAngles.ToList();
                    ////for (int i = 0; i < 3; i++)
                    ////{
                    ////    myList.Add(-30 + i * 30);
                    ////}

                    ////selecteur.AdsorbableAngles = myList.ToArray();

                    ////set the materials
                    //for (int i = 0; i < selecteur.transform.childCount; i++)
                    //{
                    //    var child = selecteur.transform.GetChild(i).gameObject;
                    //    if (child.name.Contains("cube"))
                    //        child.GetComponentInChildren<MeshRenderer>().material = LightGreen;
                    //    if (child.name.Contains("cylinder"))
                    //        child.GetComponentInChildren<MeshRenderer>().material = Green;
                    //}

                    //selecteur.DefaultCubeMaterialStatus0 = LightGreen;
                    //selecteur.DefaultCylinderMaterialStatus0 = Green;
                    //selecteur.GraspedCubeMaterial = LightRed;
                    //selecteur.GraspedCylinderMaterial = Red;

                    //set the interactionBhavour
                    var interactionBehaviour = component.GetComponent<InteractionBehaviour>();
                    interactionBehaviour.graspedMovementType = InteractionBehaviour.GraspedMovementType.Nonkinematic;
                    
                }
                #endregion setup for the switch
                #region setup the valve
                if (component.name.Contains("valve"))
                {
                    //add the components
                    component.AddComponent<Rigidbody>();
                    component.AddComponent<HingeJoint>();
                    component.AddComponent<Valvola>();
                    //component.AddComponent<InteractionBehaviour>();

                    //set the rigidbody
                    var rigidbody = component.GetComponent<Rigidbody>();
                    rigidbody.useGravity = false;
                    rigidbody.isKinematic = true;

                    //set the hinge joint
                    var hingeJoint = component.GetComponent<HingeJoint>();
                    hingeJoint.useLimits = true;
                    //set the rotating axis
                    if (component.name.Contains("front"))
                    {
                        hingeJoint.axis = Vector3.up;
                    }
                    if (component.name.Contains("top"))
                    {
                        hingeJoint.axis = Vector3.forward;
                    }
                    if (component.name.Contains("side"))
                    {
                        hingeJoint.axis = Vector3.right;
                    }

                    //settings in switch class
                    var valve = component.GetComponent<Valvola>();
                    //selecteur.RotateLimit = true;
                    valve.AngleRange = new Range(0, 90);
                    valve.IsAdsorbent = false;


                    ////set the materials
                    //for (int i = 0; i < valve.transform.childCount; i++)
                    //{
                    //    var child = valve.transform.GetChild(i).gameObject;
                    //    if (child.name.Contains("cube"))
                    //        child.GetComponentInChildren<MeshRenderer>().material = LightGreen;
                    //    if (child.name.Contains("cylinder"))
                    //        child.GetComponentInChildren<MeshRenderer>().material = Green;
                    //}

                    //valve.DefaultMetalHandleMaterial = Silver;
                    //valve.DefaultPlasticCoverMaterial = Blue;
                    //valve.GraspedMetalHandleMaterial = LightSilver;
                    //valve.GraspedPlasticCoverMaterial = LightBlue;

                    //set the interactionBhavour
                    var interactionBehaviour = component.GetComponent<InteractionBehaviour>();
                    interactionBehaviour.graspedMovementType = InteractionBehaviour.GraspedMovementType.Nonkinematic;


                }
                #endregion setup for the valve
                #region setup the button

                if (component.name.Contains("button"))
                {
                    //add the components
                    var myObj= component.transform.GetChild(0).gameObject;
                    myObj.AddComponent<Rigidbody>();
                    myObj.AddComponent<Button>();
                    myObj.AddComponent<InteractionButton>();

                    //set the rigidbody
                    var rigidbody = myObj.GetComponent<Rigidbody>();
                    rigidbody.useGravity = false;
                    rigidbody.isKinematic = false;
                    rigidbody.drag = 40;
                    rigidbody.freezeRotation = true;

                    //set the Button component
                    var button = myObj.GetComponent<Button>();
                    button.DownOffset = 0.017f;
                    button.LockPercent = 0.223f;
                    //if (component.name.Contains("Emergency"))
                    //{
                    //    button.relaxDefaultMaterial = Red;
                    //    button.relaxHighlightMaterial = LightRed;
                    //    button.depressedDefaultMaterial = Red;
                    //    button.depresseddHighlightMaterial = LightRed;
                    //}
                    //else
                    //{
                    //    button.relaxDefaultMaterial = Blue;
                    //    button.relaxHighlightMaterial = LightBlue;
                    //    button.depressedDefaultMaterial = Red;
                    //    button.depresseddHighlightMaterial = LightRed;
                    //}

                    //set the interactionButton
                    var interactionButton = myObj.GetComponent<InteractionButton>();
                    interactionButton.restingHeight=0.99f;
                    interactionButton.springForce=0.99f;
                }
                #endregion
            }
        }


        if(GUILayout.Button("Setup Switch(Lever)")) {

            var components = GameObject.FindGameObjectsWithTag("Switch");
            foreach(var component in components) {
                //add and set the corresponding script components to the generator components
                #region setup the switch
                if(component.name.Contains("LEVER")) {
                    //add the components
                    component.AddComponent<Rigidbody>();
                    component.AddComponent<HingeJoint>();
                    component.AddComponent<Switch>();
                    //component.AddComponent<InteractionBehaviour>();

                    //set the rigidbody
                    var rigidbody = component.GetComponent<Rigidbody>();
                    rigidbody.useGravity = false;
                    rigidbody.isKinematic = true;

                    //set the hingejoint
                    var hingeJoint = component.GetComponent<HingeJoint>();
                    hingeJoint.useLimits = true;
                    //set the rotating axis
                    if(component.name.Contains("front")) {
                        hingeJoint.axis = Vector3.up;
                    }
                    if(component.name.Contains("top")) {
                        hingeJoint.axis = Vector3.forward;
                    }
                    if(component.name.Contains("side")) {
                        hingeJoint.axis = Vector3.right;
                    }

                    //settings in switch class
                    var selecteur = component.GetComponent<Switch>();
                    //selecteur.RotateLimit = true;
                    selecteur.AngleRange = new Range(-30, 30);
                    selecteur.IsAdsorbent = true;

                    //************************************************************************************
                    //here should get the status amounts for the switch
                    //for (int i = 0; i < 3; i++)
                    //{
                    //  selecteur.AdsorbableAngles[1] = -30;
                    //}
                    ////selecteur.SwitchStatusAmounts = 3;
                    ////var myList = selecteur.AdsorbableAngles.ToList();
                    ////for (int i = 0; i < 3; i++)
                    ////{
                    ////    myList.Add(-30 + i * 30);
                    ////}

                    ////selecteur.AdsorbableAngles = myList.ToArray();

                    ////set the materials
                    //for(int i = 0; i < selecteur.transform.childCount; i++) {
                    //    var child = selecteur.transform.GetChild(i).gameObject;
                    //    if(child.name.Contains("cube"))
                    //        child.GetComponentInChildren<MeshRenderer>().material = LightGreen;
                    //    if(child.name.Contains("cylinder"))
                    //        child.GetComponentInChildren<MeshRenderer>().material = Green;
                    //}

                    //selecteur.DefaultCubeMaterialStatus0 = LightGreen;
                    //selecteur.DefaultCylinderMaterialStatus0 = Green;
                    //selecteur.GraspedCubeMaterial = LightRed;
                    //selecteur.GraspedCylinderMaterial = Red;

                    //set the interactionBhavour
                    var interactionBehaviour = component.GetComponent<InteractionBehaviour>();
                    interactionBehaviour.graspedMovementType = InteractionBehaviour.GraspedMovementType.Nonkinematic;

                }
                #endregion setup for the switch

            }
        }



        //delete the components
        if (GUILayout.Button("Remove components"))
        {
            var components = GameObject.FindGameObjectsWithTag("Component");
            foreach (var component in components)
            {
                //delete the components here, use tryget to avoid memory allocation if there is no component
                if (component.name.Contains("switch"))
                {

                    if (component.TryGetComponent<Switch>(out var selecteur))
                    {
                        DestroyImmediate(selecteur);
                    }

                    if (component.TryGetComponent<InteractionBehaviour>(out var interactionBehaviour))
                    {
                        DestroyImmediate(interactionBehaviour);
                    }

                    if (component.TryGetComponent<HingeJoint>(out var hingeJoint))
                    {
                        DestroyImmediate(hingeJoint);
                    }

                    if (component.TryGetComponent<Rigidbody>(out var rigidbody))
                    {
                        DestroyImmediate(rigidbody);
                    }
                }

                if (component.name.Contains("valve"))
                {

                    if (component.TryGetComponent<Valvola>(out var valve))
                    {
                        DestroyImmediate(valve);
                    }

                    if (component.TryGetComponent<InteractionBehaviour>(out var interactionBehaviour))
                    {
                        DestroyImmediate(interactionBehaviour);
                    }

                    if (component.TryGetComponent<HingeJoint>(out var hingeJoint))
                    {
                        DestroyImmediate(hingeJoint);
                    }

                    if (component.TryGetComponent<Rigidbody>(out var rigidbody))
                    {
                        DestroyImmediate(rigidbody);
                    }
                }

                if (component.name.Contains("button"))
                {
                    Debug.Log("I found button: "+ component.name);

                    if (component.transform.GetChild(0).TryGetComponent<Button>(out var button))
                    {
                        DestroyImmediate(button);
                    }

                    if (component.transform.GetChild(0).TryGetComponent<InteractionButton>(out var interactionButton))
                    {
                        DestroyImmediate(interactionButton);
                    }

                    if (component.transform.GetChild(0).TryGetComponent<Rigidbody>(out var rigidbody))
                    {
                        DestroyImmediate(rigidbody);
                    }
                }
            }
        }
    }
}
