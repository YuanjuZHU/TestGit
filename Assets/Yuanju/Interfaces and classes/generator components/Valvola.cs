using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Yuanju.Interfaces_and_classes.generator_components;
using Leap;
using Leap.Unity;
using Leap.Unity.Interaction;
using MGS.UCommon.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HingeJoint))]
[RequireComponent(typeof(InteractionBehaviour))]
[RequireComponent(typeof(BlinkMaterial))]
[RequireComponent(typeof(AudioSource))]
//[RequireComponent(typeof(InteractionBehaviour))]
public class Valvola :  MonoBehaviour, IRotatableComponent
{
    #region properties
    //the switch component is able to turn on and turn off
    [SerializeField]
    protected bool isEnabled = true;
    //the steam(water) possibleFluxRate that will be sent to(from) the generator m3/s
    [SerializeField]
    private float nominalFluxRate;


    [SerializeField]
    protected bool _rotateLimit=true;

    [Tooltip("Range of rotate angle.")]
    [SerializeField]
    public Range angleRange /*= new Range(-60, 60)*/;
    public Range AngleRange
    {
        set { angleRange = value; }
        get { return angleRange; }
    }


    public bool Enabled
    {
        set { isEnabled = value; }
        get { return isEnabled; }
    }

    //the switch is not adsorbent
    [SerializeField]
    protected bool _isAdsorbent = false;
    public bool IsAdsorbent
    {
        get { return _isAdsorbent; }
        set { _isAdsorbent = value; }
    }

    [Tooltip("Adsorbable angles.")]
    [SerializeField]
    public float[] _adsorbableAngles;
    /// <summary>
    /// Adsorbable angles.
    /// </summary>
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
    //the position of the switch handle
    /// <summary>
    /// switch current angle.
    /// </summary>
    public float Angle { set; get; }

    private float StartAngle { set; get; }
    //the status of the switch(a percentage to describe the open degree of the switch)
    [SerializeField]
    private float _openPercentage;
    /// <summary>
    /// switch current rotate percent base range.
    /// </summary>
    public float OpenPercentage
    {
        get
        {
            if (_rotateLimit)
            {
                //get the adsorbent angle (and to get the status) 
                var range = angleRange.Length;

                return /*_openPercentage =*/ (Angle - StartAngle) / (range == 0 ? 1 : range);
            }
            return 0;
        }
    }
    /// <summary>
    /// Limit rotate angle?
    /// </summary>
    public bool RotateLimit
    {
        set { _rotateLimit = value; }
        get { return _rotateLimit; }
    }

    [SerializeField]
    protected float possibleFluxRate;
    public float PossibleFluxRate
    {
        get { return possibleFluxRate; }
        set { possibleFluxRate = value; }
    }


    [SerializeField]
    protected float possibleTotalFlux;
    public float PossibleTotalFlux
    {
        get { return possibleTotalFlux; }
        set { possibleTotalFlux = value; }
    }

    /// <summary>
    /// Start angles.
    /// </summary>
    public Vector3 StartAngularPosition { protected set; get; }
    //the materials
    [SerializeField]
    public Material DefaultMetalHandleMaterial;
    public Material DefaultPlasticCoverMaterial;
    public Material GraspedMetalHandleMaterial;
    public Material GraspedPlasticCoverMaterial;

    private HingeJoint hingeJointValve;

    public int _status;
    public int Status
    {
        get => _status;
        set => _status = value;
    }
    private int previousStatus;

    private Dictionary<float, int> ValveAngleStatus = new Dictionary<float, int>();
    private Dictionary<int, float> ValveStatusAngle = new Dictionary<int, float>();
    private readonly float[] valveRange =new float[2]; //the real angle range of the valve
    public bool IsNeedCheck { get; set; }
    public AudioSource audioSourceFluid = new AudioSource();
    private AudioSource audioSourceValve;
    public float OpenValveThreshold = 0.99f;


    #endregion

    #region methods

    public void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        StartAngularPosition = transform.localEulerAngles;
        //set the angle range with the setting in the hinge joint
        hingeJointValve = transform.GetComponent<HingeJoint>();
        JointLimits limits = hingeJointValve.limits;
        limits.min = angleRange.min;
        limits.max = angleRange.max;
        transform.GetComponent<HingeJoint>().limits = limits;
        UpdateMaterials();
        AddMethodsToEvents();

        StartAngle = hingeJointValve.axis.x * StartAngularPosition.x + hingeJointValve.axis.y * StartAngularPosition.y + hingeJointValve.axis.z * StartAngularPosition.z;
        StartAngle = (StartAngle < 180) ? StartAngle : StartAngle - 360;
        Angle = StartAngle;


        MapStatusesAngles();
        Status = ValveAngleStatus[StartAngle];//initialization for the valve's status
        previousStatus = Status;//initialization for the valve's previous status
        _openPercentage = OpenPercentage;//initialization for the valve's open percentage

        PossibleFluxRate = nominalFluxRate * OpenPercentage; //assign the possibleFluxRate
        //StartAnchorPos = transform.gameObject.GetComponent<HingeJoint>().connectedAnchor;
        IsNeedCheck = false;

    }

    void Update()
    {
        //update the position of the valve while we set the status in the inspector, also update the audio
        if (previousStatus != Status)
        {
            //TODO remember to turn the component to the correct status
            Rotate(ValveStatusAngle[Status] - StartAngle);
            previousStatus = Status;
            ComputeAngle();
            _openPercentage = (Angle - StartAngle) / (angleRange.Length == 0 ? 1 : angleRange.Length);
            audioSourceFluid.volume = OpenPercentage*0.2f;
            audioSourceFluid.loop=true;
            audioSourceFluid.Play();
        }
    }


    /// <summary>
    /// Responses when leap hands grasp: limit the rotate range, get the open percentage and change material of the switch.
    /// </summary>


    public virtual void OnGraspMaintain()
    {
        gameObject.GetComponent<BlinkMaterial>().IsBlink = false;
        if (!isEnabled)
        {
            return;
        }

        ComputeAngle();
        ////float angle= hingeJointValve.axis.x * transform.localEulerAngles.x+ hingeJointValve.axis.y * transform.localEulerAngles.y+ hingeJointValve.axis.z * transform.localEulerAngles.z;
        ////Angle = angle;
        //////Angle = transform.localEulerAngles.x;
        ////Angle = (Angle < 180) ? Angle : Angle - 360;

        //obsolete, because can not limit the range precisely, use the limits in hinge joint instead
        //if (_rotateLimit)
        //{
        //    Angle = Mathf.Clamp(Angle, angleRange.min, angleRange.max);
        //}
        //Rotate(Angle);

        ////////if (gameObject.GetComponents<AudioSource>().Length == 0)
        ////////{
        ////////    audioSourceFluid = gameObject.AddComponent<AudioSource>();
        ////////    audioSourceFluid.clip = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/Yuanju/Audio/waterpipe_15s.wav", typeof(AudioClip));

        ////////    audioSourceFluid.loop = true;
        ////////}

        if (_openPercentage != OpenPercentage)
        {
            PossibleFluxRate = nominalFluxRate * OpenPercentage;
            _openPercentage = OpenPercentage;
            //change the volume of the audio here, the open percentage is relative to the volume of the water ///waterpipe_15s.wav
            //audio for the valve
            //////if (audioSourceValve != null)
            //////{
            //////    audioSourceValve.Play();
            //////}
            //////else if(gameObject.GetComponents<AudioSource>().Length == 1)
            //////{
            //////    audioSourceValve = gameObject.AddComponent<AudioSource>();
            //////    AudioClip clip = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/Yuanju/Audio/XXXXXXXX.wav", typeof(AudioClip));
            //////    audioSourceValve.clip = clip;
            //////    if (!audioSourceValve.isPlaying)
            //////    {
            //////        gameObject.GetComponent<AudioSource>().Play();

            //////    }

            //////}

        }

        audioSourceFluid.volume =  Mathf.Abs(_openPercentage)*0.2f ;
        if (!audioSourceFluid.isPlaying)
        {
            audioSourceFluid.Play();
            audioSourceFluid.loop = true;
        }



        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i).gameObject;
            if (child.name.Contains("metal handle"))
                child.GetComponentInChildren<MeshRenderer>().material = GraspedMetalHandleMaterial;
            if (child.name.Contains("plastic cover"))
                child.GetComponentInChildren<MeshRenderer>().material = GraspedPlasticCoverMaterial;
        }


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
        transform.localRotation = Quaternion.Euler(StartAngularPosition + hingeJointValve.axis/*Vector3.back*/ * rotateAngle);

    }

    /// <summary>
    /// Response leap hand release the switch: get the adsorbent angle, place the mobile part to the correct position and set back the default material.
    /// </summary>
    public virtual void OnGraspFinish()
    {      
        if (!isEnabled)
        {
            return;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i).gameObject;
            if (child.name.Contains("metal handle"))
            {
                child.GetComponentInChildren<MeshRenderer>().material = DefaultMetalHandleMaterial;
            }
            if (child.name.Contains("plastic cover"))
                child.GetComponentInChildren<MeshRenderer>().material = DefaultPlasticCoverMaterial;
        }

        if(OpenPercentage > OpenValveThreshold) 
        {
            Status = 1;
        } 
        else 
        {
            Status = 0;
        }

        if (!_isAdsorbent || _adsorbableAngles.Length == 0)
        {
            return;
        }

        Angle = GetAdsorbentAngle(Angle, _adsorbableAngles);
        Rotate(Angle);
        //Debug.Log("StartAnchor while release: " + StartAnchorPos);
        //Evaluation.StudentOperations.ActingAction.StatusAfter = Status;
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

    ///// <summary>
    ///// Initialize the materials when hit play
    ///// </summary>
    //private void InitializeMaterialForValve()
    //{
    //    for (int i = 0; i < transform.childCount; i++)
    //    {
    //        var child = transform.GetChild(i).gameObject;
    //        if (child.name.Contains("metal handle"))
    //        {
    //            child.GetComponentInChildren<MeshRenderer>().material = DefaultMetalHandleMaterial;
    //        }
    //        if (child.name.Contains("plastic cover"))
    //            child.GetComponentInChildren<MeshRenderer>().material = DefaultPlasticCoverMaterial;
    //    }
    //}

    //add methods for the events
    public void AddMethodsToEvents()
    {
        var interactionBehaviour = gameObject.GetComponent<InteractionBehaviour>();
        if (interactionBehaviour!=null)
        {
            interactionBehaviour.graspedMovementType = InteractionBehaviour.GraspedMovementType.Nonkinematic;
            interactionBehaviour.OnGraspStay -= OnGraspMaintain;
            interactionBehaviour.OnGraspStay += OnGraspMaintain;
            interactionBehaviour.OnGraspEnd -= OnGraspFinish;
            interactionBehaviour.OnGraspEnd += OnGraspFinish;
        }

    }

    //map status and angles for the valve
    private void MapStatusesAngles()
    {
        //imp the angle of the valve's angle range
        valveRange[0] = AngleRange.min + StartAngle;
        valveRange[1] = AngleRange.max + StartAngle;
        //Rotate(SwitchStatusAngle[status] - (hingeJointSwitch.axis.x * StartAngularPosition.x + hingeJointSwitch.axis.y * StartAngularPosition.y + hingeJointSwitch.axis.z * StartAngularPosition.z));
        ValveAngleStatus.Clear();
        //add the status to the switchActions dictionary
        for (int i = 0; i < valveRange.Length; i++)
        {
            ValveAngleStatus.Add(valveRange[i], i);
        }

        ValveStatusAngle = ValveAngleStatus.ToDictionary((i) => i.Value, (i) => i.Key);
    }
    //TODO display the threshold for turning on the valve (status 1), should be a value between 0 and 1 and compare it with the current open percentage of the valve


    public void GetOperatedComponent()
    {
        //if grasp a new component and its status is modified by leap hand 
        //Debug.Log("Evaluation.StudentOperations.ActingAction.Actuator.name: " + Evaluation.StudentOperations.ActingAction.Actuator.name);

        if (true/*gameObject != Evaluation.CurrentOperatedComponent *//*&& previousStatus != Status*/)
        {
            Evaluation.LastOperatedComponent = Evaluation.CurrentOperatedComponent;
            if (true/*!Evaluation.previousOperatedComponents.Contains(gameObject.name)*/)
            {
                Evaluation.previousOperatedComponents.Add(gameObject.name);
                Debug.Log("the status of the actuator while releasing: " + gameObject.GetComponentInChildren<IGeneratorComponent>().Status);
            }
        }
        Evaluation.CurrentOperatedComponent = gameObject;
        Evaluation.StudentOperations.ActingAction.StatusAfter = Status;
        //actingModifyActuator.StatusAfter = Evaluation.StudentOperations.ActingAction.StatusAfter;

        if (Evaluation.StudentOperations.ActingAction.StatusBefore != Status && !Evaluation.isFinalSettingCorrect) //only when the status is different, we check its sequence
        {
            IsNeedCheck = true;
        }

        GameObject generator = GameObject.Find("Generator V3");
        SteamGenerator steamGenerator = new SteamGenerator(generator);
        steamGenerator.SerializeGeneratorStatus();
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
            //if (GameObject.Find(name).GetComponentInChildren<MeshRenderer>().material.color != BlinkMaterial.blinkMat.color)
            //{
            if(GameObject.Find(name).GetComponent<BlinkMaterial>()!=null) 
            {
                GameObject.Find(name).GetComponent<BlinkMaterial>().IsBlink = false;
            }

            GameObject.Find(name).GetComponent<IGeneratorComponent>().UpdateMaterials();
            //}

        }
    }


    public void UpdateMaterials() //this method can not be deleted
   {
       for(int i = 0; i < transform.childCount; i++) {
           var child = transform.GetChild(i).gameObject;
           if(child.name.Contains("metal handle")) {
               child.GetComponentInChildren<MeshRenderer>().material = DefaultMetalHandleMaterial;
           }
           if(child.name.Contains("plastic cover"))
               child.GetComponentInChildren<MeshRenderer>().material = DefaultPlasticCoverMaterial;
       }

       //Debug.Log("valve materials updated: " + gameObject);
   }


    private void ComputeAngle()
	{
	    float angle = hingeJointValve.axis.x * transform.localEulerAngles.x + hingeJointValve.axis.y * transform.localEulerAngles.y + hingeJointValve.axis.z * transform.localEulerAngles.z;
	    Angle = angle;
	    //Angle = transform.localEulerAngles.x;
	    Angle = (Angle < 180) ? Angle : Angle - 360;
    }

    public static void SetAlarmMatAndAud()
    {
        var alarm = GameObject.Find("alarm");
        var audio = alarm.GetComponentInChildren<AudioSource>();
        var material = alarm.GetComponentInChildren<MeshRenderer>().material;
        audio.Stop();

        //Debug.Log("the material: " + mat);
        //if (isSequenceCorrect && isFinalSettingCorrect)
        //{
        //    GameObject.Find("alarm").GetComponentInChildren<MeshRenderer>().material = alarmGreen;
        //    Debug.Log("the correct material: " + mat);
        //    alarmAudio.Stop();
        //}
        //else
        //{
        //    GameObject.Find("alarm").GetComponentInChildren<MeshRenderer>().material = alarmRed;
        //    Debug.Log("the incorrect material: " + mat);
        //    alarmAudio.Play();
        //}
    }

    #endregion
}
