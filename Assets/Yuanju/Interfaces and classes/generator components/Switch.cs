using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Assets.Yuanju.Interfaces_and_classes.generator_components;
using Leap;
using Leap.Unity;
using Leap.Unity.Interaction;
using MGS.UCommon.Generic;
using NPOI.OpenXmlFormats.Dml.Diagram;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(HingeJoint))]
[RequireComponent(typeof(InteractionBehaviour))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BlinkMaterial))]
public class Switch :  MonoBehaviour, IRotatableComponent
{
    #region properties
    //the switch component is able to turn on and turn off
    [SerializeField]
    protected bool isEnabled = true;
    public bool Enabled
    {
        set { isEnabled = value; }
        get { return isEnabled; }
    }

    /// <summary>
    /// Limit rotate angle?
    /// </summary>
    [SerializeField]
    protected bool _rotateLimit=true;
    public bool RotateLimit
    {
        set { _rotateLimit = value; }
        get { return _rotateLimit; }
    }

    /// <summary>
    /// Range of rotate angle.
    /// </summary>
    [Tooltip("Range of rotate angle.")]
    [SerializeField]
    public Range angleRange = new Range(-60, 60);
    public Range AngleRange
    {
        set { angleRange = value; }
        get { return angleRange; }
    }


    //the switch is not adsorbent
    [SerializeField]
    protected bool _isAdsorbent;
    public bool IsAdsorbent
    {
        get { return _isAdsorbent; }
        set { _isAdsorbent = value; }
    }


    /// <summary>
    /// Adsorbable angles.
    /// </summary>
    [Tooltip("Adsorbable angles.")]
    [SerializeField]
    public float[] _adsorbableAngles;
    public float[] AdsorbableAngles
    {
        set
        {
            _adsorbableAngles = value;
        }
        get
        {
            return _adsorbableAngles;
        }
    }


    public float Angle { set; get; } //the position of the switch handle

    public float StartAngle { set; get; } //the start position of switch handle

    /// <summary>
    /// switch current rotate percent base range.
    /// </summary>
    private float _openPercentage; //the status of the switch(a percentage to describe the open degree of the switch)
    public float OpenPercentage
    {
        get
        {
            if (_rotateLimit)
            {
                //get the adsorbent angle (and to get the status) 
                Angle = GetAdsorbentAngle(Angle, _adsorbableAngles);
                var range = angleRange.Length;

                return /*_openPercentage =*/ (Angle - StartAngle) / (range == 0 ? 1 : range);
            }
            return 0;
        }
    }





    /// <summary>
    /// Start angles.
    /// </summary>
    public Vector3 StartAngularPosition { protected set; get; }

    //the materials
    [SerializeField]
    public Material DefaultCubeMaterialStatus0;
    public Material DefaultCylinderMaterialStatus0;
    [SerializeField]
    public Material GraspedCubeMaterial;
    public Material GraspedCylinderMaterial;
    [SerializeField]
    public Material DefaultCubeMaterialStatus1;
    public Material DefaultCylinderMaterialStatus1;
    [SerializeField]
    public Material DefaultCubeMaterialStatus2;
    public Material DefaultCylinderMaterialStatus2;

    public AudioClip SwitchAudioClip;
    //public List<Material> MaterialList=new List<Material>();
    [SerializeField]
    public SwitchStatusMaterialSets SwitchStatusMaterials = new SwitchStatusMaterialSets();
    [SerializeField]
    public SwitchPartsMaterialSets SwitchGraspedMaterials = new SwitchPartsMaterialSets();

    private HingeJoint hingeJointSwitch;


    private int _status;
    public int Status  
    {
        get => _status;
        set => _status = value;
    }

    public int previousStatus;


    //A dictionary for mapping the status with the adsorbable angles
    private Dictionary<float, int> SwitchAngleStatus = new Dictionary<float, int>();
    private Dictionary<int, float> SwitchStatusAngle = new Dictionary<int, float>();
    
    //private bool actionsExecuted;

    [SerializeField]
    public bool IsNeedCheck { get; set; }  //if the switch needs to be checked(for an action) 
    public bool IsPowerConnected { get; set; }

#endregion properties


    #region methods
    public void Awake()
    {
        //var assemblyPath = this.GetType().Assembly;
        //Debug.Log("the switch type of the types, can we get the classes?" + assemblyPath);
        Initialize();
    }

    public void Initialize()
    {

        StartAngularPosition = transform.localEulerAngles;

        ////some settings(the limits) in hinge joint is controlled by the properties in this("Switch.cs") class
        hingeJointSwitch = transform.GetComponentInChildren<HingeJoint>();
        //JointLimits limits = hingeJointSwitch.limits;
        //limits.min = angleRange.min;
        //limits.max = angleRange.max;
        //transform.GetComponent<HingeJoint>().limits = limits;

        StartAngle = hingeJointSwitch.axis.x * StartAngularPosition.x + hingeJointSwitch.axis.y * StartAngularPosition.y + hingeJointSwitch.axis.z * StartAngularPosition.z;
        StartAngle = (StartAngle < 180) ? StartAngle : StartAngle - 360;


        AddMethodsToInteractionBehaviour();
        MapStatusesAngles();//fill in the dictionary and the reverse dictionary
        Angle = (StartAngle < 180) ? StartAngle : StartAngle - 360;
        Status = SwitchAngleStatus[GetAdsorbentAngle(Angle,AdsorbableAngles)];
        previousStatus = Status;
        //actionsExecuted = false;//the actions are not executed
        IsNeedCheck = false;
        IsPowerConnected = false;
    }


    void Update()
    {
        //if the status is modified in the inspector
        if (previousStatus!= Status)
        {
            //turn the component to the correct status
            Rotate(SwitchStatusAngle[Status] - StartAngle); //update the position
            UpdateMaterials(); //update the materials
            previousStatus = Status;
            if (gameObject.name.Contains("start handle"))
            {
                var switches = GameObject.FindGameObjectsWithTag("Switch");
                if (Status==1)
                {
                    foreach (var _switch in switches)
                    {
                        _switch.GetComponent<Switch>().IsPowerConnected = true;
                        _switch.GetComponent<Switch>().UpdateMaterials();
                    }
                }
                else
                {
                    foreach (var _switch in switches)
                    {
                        _switch.GetComponent<Switch>().IsPowerConnected = false;
                        _switch.GetComponent<Switch>().UpdateMaterials();
                    }
                }
            }
        }
    }

    /// <summary>
    /// *******************************************************************************
    /// Response leap hand grasp: limit the rotate range, get the open percentage and change material of the switch.
    /// </summary>
    public virtual void OnGraspMaintain()
    {
        gameObject.GetComponent<BlinkMaterial>().IsBlink = false;  //stop blinking the actuator if it is grasped
        if (!isEnabled)
        {
            return;
        }

        float angle = hingeJointSwitch.axis.x * transform.localEulerAngles.x + hingeJointSwitch.axis.y * transform.localEulerAngles.y + hingeJointSwitch.axis.z * transform.localEulerAngles.z;
        Angle = angle;
        Angle = (Angle < 180) ? Angle : Angle - 360;
        //obsolete, because can not limit the range precisely, use the limits in hinge joint instead
        //if (_rotateLimit)
        //{
        //    Angle = Mathf.Clamp(Angle, angleRange.min, angleRange.max);
        //}
        //Rotate(Angle);
        if (_openPercentage != OpenPercentage)
        {
            _openPercentage = OpenPercentage;
            //play audio here
            if (gameObject.GetComponent<AudioSource>() != null)
            {
                gameObject.GetComponent<AudioSource>().Play();
            }
            else
            {
                var audioSource = gameObject.AddComponent<AudioSource>();
                //AudioClip clip = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/Yuanju/Audio/Tik Tak (mp3cut.net).wav", typeof(AudioClip));
                //audioSource.clip = clip;
                gameObject.GetComponent<AudioSource>().Play();
            }

        }

        UpdateHighlightedMaterials();

    }
    /// <summary>
    /// Rotate mobile part to target angle.
    /// </summary>
    /// <param name="rotateAngle">Rotate angle.</param>
    protected virtual void Rotate(float rotateAngle)
    {
        //Debug.Log("StartAngularPosition: " + StartAngularPosition);
        //Debug.Log("Vector3.back * rotateAngle: " + Vector3.back * rotateAngle);
        //transform.gameObject.GetComponent<HingeJoint>().connectedAnchor= StartAnchorPos;
        transform.localRotation = Quaternion.Euler(StartAngularPosition + hingeJointSwitch.axis/*Vector3.back*/ * rotateAngle);

    }

    /// <summary>
    /// *******************************************************************************
    /// Response leap hand release the switch: get the adsorbent angle, place the mobile part to the correct position and set back the default material.
    /// </summary>
    public virtual void OnGraspFinish()
    {
        if (!isEnabled)
        {
            return;
        }

        if (!_isAdsorbent || _adsorbableAngles.Length == 0)
        {
            return;
        }
        Angle = GetAdsorbentAngle(Angle, _adsorbableAngles);
        //Debug.Log("StartAnchor while release: " + StartAnchorPos);
        Rotate(Angle - (hingeJointSwitch.axis.x * StartAngularPosition.x + hingeJointSwitch.axis.y * StartAngularPosition.y +hingeJointSwitch.axis.z * StartAngularPosition.z));
        //Rotate(Angle - StartAngularPosition.y);
        Status = SwitchAngleStatus[Angle];
        //Debug.Log("the status in Switch class: " + Status);

        UpdateMaterials();
        Evaluation.StudentOperations.ActingAction.StatusAfter = gameObject.GetComponentInChildren<IGeneratorComponent>().Status;
    }

    /// <summary>
    /// method to update the materials for the switch
    /// </summary>
    public void UpdateMaterials()
    {
        //if (IsPowerConnected)
        //{
        //    for (int i = 0; i < transform.childCount; i++)
        //    {
        //        var child = transform.GetChild(i).gameObject;
        //        if (child.name.Contains("cube"))
        //        {
        //            if (Status == 0)
        //                child.GetComponentInChildren<MeshRenderer>().material = DefaultCubeMaterialStatus0;
        //            if (Status == 1)
        //                child.GetComponentInChildren<MeshRenderer>().material = DefaultCubeMaterialStatus1;
        //            if (Status == 2)
        //                child.GetComponentInChildren<MeshRenderer>().material = DefaultCubeMaterialStatus2;
        //        }

        //        if (child.name.Contains("cylinder") || child.name.Contains("integrated"))
        //        {
        //            if (Status == 0)
        //                child.GetComponentInChildren<MeshRenderer>().material = DefaultCylinderMaterialStatus0;
        //            if (Status == 1)
        //                child.GetComponentInChildren<MeshRenderer>().material = DefaultCylinderMaterialStatus1;
        //            if (Status == 2)
        //                child.GetComponentInChildren<MeshRenderer>().material = DefaultCylinderMaterialStatus2;
        //        }

        //    }
        //}
        //else
        //{
        //    for (int i = 0; i < transform.childCount; i++)
        //    {
        //        var child = transform.GetChild(i).gameObject;
        //        if (child.name.Contains("cube"))
        //        {
        //            child.GetComponentInChildren<MeshRenderer>().material = DefaultCubeMaterialStatus0;
        //        }

        //        if (child.name.Contains("cylinder") || child.name.Contains("integrated"))
        //        {
        //            child.GetComponentInChildren<MeshRenderer>().material = DefaultCylinderMaterialStatus0;
        //        }

        //    }
        //}

        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i).gameObject;
            if (IsPowerConnected)
            {
                child.GetComponentInChildren<MeshRenderer>().material = SwitchStatusMaterials.MaterialSet[Status].MaterialsOfDifferentParts[i];
            }
            else
            {
                child.GetComponentInChildren<MeshRenderer>().material = SwitchStatusMaterials.MaterialSet[0].MaterialsOfDifferentParts[i];
            }
        }


        //Debug.Log("Materials updated: " + gameObject);
    }

    public void UpdateHighlightedMaterials()
	{
	    for(int i = 0; i < transform.childCount; i++) {
	        var child = transform.GetChild(i).gameObject;
            //if(child.name.Contains("cube"))
            //    child.GetComponentInChildren<MeshRenderer>().material = GraspedCubeMaterial;
            //if(child.name.Contains("cylinder") || child.name.Contains("integrated"))
            //    child.GetComponentInChildren<MeshRenderer>().material = GraspedCylinderMaterial;
            child.GetComponentInChildren<MeshRenderer>().material = SwitchGraspedMaterials.MaterialsOfDifferentParts[i];
        }
    }


    /// <summary>
    /// Get the adsorbent angle base on knob current angle.
    /// </summary>
    /// <param name="currentAngle">Current angle of knob.</param>
    /// <param name="adsorbableAngles">Adsorbable angles of knob.</param>
    /// <returns>Target adsorbent angle of knob.</returns>
    protected float GetAdsorbentAngle(float currentAngle, float[] adsorbableAngles)
    {
        var nearAngle = 0f;
        var deltaAngle = 0f;
        var nearDelta = float.PositiveInfinity;
        foreach (var adsorbentAngle in adsorbableAngles)
        {
            deltaAngle = Mathf.Abs(currentAngle - adsorbentAngle);
            if (deltaAngle < nearDelta)
            {
                nearDelta = deltaAngle;
                nearAngle = adsorbentAngle;
            }
        }
        return nearAngle;
    }

    //add methods for the events
    public void AddMethodsToInteractionBehaviour()
    {
        var interactionBehaviour = gameObject.GetComponent<InteractionBehaviour>();
        interactionBehaviour.graspedMovementType = InteractionBehaviour.GraspedMovementType.Nonkinematic;
        interactionBehaviour.OnGraspStay -= OnGraspMaintain;
        interactionBehaviour.OnGraspStay += OnGraspMaintain;
        interactionBehaviour.OnGraspEnd -= OnGraspFinish;
        interactionBehaviour.OnGraspEnd += OnGraspFinish;
    }

    /// <summary>
    /// map status and angles
    /// </summary>
    private void MapStatusesAngles()
    {
        //sort the adsorbable angles
        //AdsorbableAngles = AdsorbableAngles.OrderByDescending(c => c).ToArray();
        SwitchAngleStatus.Clear();
        //add the status to the switchActions dictionary
        for (int i = 0; i < AdsorbableAngles.Length; i++)
        {
            SwitchAngleStatus.Add(AdsorbableAngles[i], i);
        }

        SwitchStatusAngle = SwitchAngleStatus.ToDictionary((i) => i.Value, (i) => i.Key);
    }




    //void Update()
    //{
    //    //should not execute all the status but the current status

    //    if (!actionsExecuted)
    //    {
    //        var action = switchActions[Angle.ToString()];
    //        action();
    //        actionsExecuted = true;

    //    }
    //}
    //public static Evaluation.AnAction actingModifyActuator;
    public void GetOperatedComponent()
    {
        //if grasp a new component and its status is modified by leap hand 
        //Debug.Log("Evaluation.StudentOperations.ActingAction.Actuator.name: " + Evaluation.StudentOperations.ActingAction.Actuator.name);

        if(true/*gameObject != Evaluation.CurrentOperatedComponent *//*&& previousStatus != Status*/) {
            Evaluation.LastOperatedComponent = Evaluation.CurrentOperatedComponent;
            if(true/*!Evaluation.previousOperatedComponents.Contains(gameObject.name)*/) {
                Evaluation.previousOperatedComponents.Add(gameObject.name);
                Debug.Log("the status of the actuator while releasing: " + gameObject.GetComponentInChildren<IGeneratorComponent>().Status);
            }
        }
        Evaluation.CurrentOperatedComponent = gameObject;
        Evaluation.StudentOperations.ActingAction.StatusAfter = Status;
        //actingModifyActuator.StatusAfter = Evaluation.StudentOperations.ActingAction.StatusAfter;
        if (Evaluation.StudentOperations.ActingAction.StatusBefore!= Status && !Evaluation.isFinalSettingCorrect) //only when the status is different, we check its sequence
        {
            IsNeedCheck = true;
        }

        
        //GameObject generator = GameObject.Find("Generator V3");
        //SteamGenerator steamGenerator  = new SteamGenerator(generator);
        //var compList = steamGenerator.SerializeGeneratorStatus();
        //var json = steamGenerator.GetJson(compList);
        //UDP_Connection.senderUdpClient(json);
    }

    public void LowlightComponentsWhileContacting() //subscribe to interaction OnGrasp(Contact)Begin, lowlight here means stop blinking
    {
        Evaluation.StudentOperations.ActingAction = new Evaluation.AnAction();
        Evaluation.StudentOperations.ActingAction.Initialize();
        Evaluation.StudentOperations.ActingAction.IsModificationCorrect = true; //the actuator action should be default false so that the check sequence method can be executed in the evaluation script
        Debug.Log("the status of the actuator while contacting: " + gameObject.GetComponentInChildren<IGeneratorComponent>().Status);
        Evaluation.StudentOperations.LastAction.Actuator = Evaluation.StudentOperations.ActingAction.Actuator;
        //Evaluation.StudentOperations.ActingAction.Actuator = null; //get the actuator
        Evaluation.StudentOperations.ActingAction.Actuator = gameObject; //get the actuator
        //Debug.Log("here I get the acting actuator gameobject: " + Evaluation.StudentOperations.ActingAction.Actuator.name);
        Evaluation.StudentOperations.ActingAction.StatusBefore = gameObject.GetComponentInChildren<IGeneratorComponent>().Status; //get the status before
        foreach (var name in NPOIGetDatatable.OperableComponentNames)
        {
            if(GameObject.Find(name).GetComponent<BlinkMaterial>()!= null)
            {
                GameObject.Find(name).GetComponent<BlinkMaterial>().IsBlink = false;
            }
            GameObject.Find(name).GetComponent<IGeneratorComponent>().UpdateMaterials();

        }
        //actingModifyActuator = new Evaluation.AnAction();
        //actingModifyActuator.Actuator = Evaluation.StudentOperations.ActingAction.Actuator;
        //actingModifyActuator.StatusBefore = Evaluation.StudentOperations.ActingAction.StatusBefore;
    }
    #endregion

    void OnEnable()
    {
        Debug.Log("an on enabled test");
    }

}

/// <summary>
/// a switch have several status, for each status, there should be a set of materials 
/// </summary>
[System.Serializable]
public class SwitchStatusMaterialSets
{
    //[SerializeField]
    public List<SwitchPartsMaterialSets> MaterialSet=new List<SwitchPartsMaterialSets>(); /*{ get; set; }*/

}

/// <summary>
/// a switch may have several parts
/// </summary>
[System.Serializable]
public class SwitchPartsMaterialSets
{
    //[SerializeField]
    [HideInInspector]
    public string FontName;

    public List<Material> MaterialsOfDifferentParts = new List<Material>(); /*{ get; set; }*/
    //public void Initialize()
    //{
    //    FontName = "Status in Initialize"/* + Switch.mySwitchMateri.MaterialSet.Count.ToString()*/;
    //}
}

