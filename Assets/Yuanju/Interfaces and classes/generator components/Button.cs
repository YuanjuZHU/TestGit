using System.Collections.Generic;
using System.Linq;
using Assets.Yuanju.Interfaces_and_classes.generator_components;
using Leap.Unity.Interaction;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// class for button.
/// </summary>
//[RequireComponent(typeof(Collider))]
public class Button :MonoBehaviour, IPressableComponent
{
    #region Field and Property
    /// <summary>
    /// the button is enable to control or not
    /// </summary>
    [Tooltip("turn on or off the script")]
    [SerializeField]
    protected bool isEnabled = true;
    public bool Enabled
    {
        set { isEnabled = value; }
        get { return isEnabled; }
    }
    /// <summary>
    /// Button down offset: downOffset = Max(position of button) - Min(position of button)
    /// </summary>
    //! max and min are the parameters in the interaction button
    [Tooltip("Button down offset.")]
    [SerializeField]
    protected float downOffset = 0.0045f;

    /// <summary>
    /// is the button pressed down or not
    /// </summary>
    [Tooltip("is the button pressed down already or not(the status of the button)")]
    [SerializeField]
    protected bool isPressedDown;

    /// <summary>
    /// Self lock offset percent: new DownOffset = lockPercent*DownOffset
    /// </summary>
    [Tooltip("Self lock offset percent.")]
    [Range(0, 1)]
    [SerializeField]
    protected float lockPercent = 0.5f;


    /// <summary>
    /// Button down offset.
    /// </summary>
    public float DownOffset
    {
        set { downOffset = value; }
        get { return downOffset; }
    }

    /// <summary>
    /// Self lock on button down
    /// </summary>
    public bool IsPressedDown
    {
        set { isPressedDown = value; }
        get { return isPressedDown; }
    }

    /// <summary>
    /// Self lock offset percent.
    /// </summary>
    public float LockPercent
    {
        set { lockPercent = value; }
        get { return lockPercent; }
    }

    /// <summary>
    /// the materials for the button
    /// </summary>
    [SerializeField] public Material relaxDefaultMaterial;
    [SerializeField] public Material relaxHighlightMaterial;
    [SerializeField] public Material depressedDefaultMaterial;
    [SerializeField] public Material depresseddHighlightMaterial;

    public int _status;
    public int Status
    {
        get => _status;
        set => _status = value;
    }
    private int previousStatus;

    private Dictionary<bool, int> ButtonUpdownStatus = new Dictionary<bool, int>();
    private Dictionary<int, bool> ButtonStatusUpdown = new Dictionary<int, bool>();
    public bool IsNeedCheck { get; set; }
    private GameObject iterateGameObject;
    #endregion

    #region Method
    /// <summary>
    /// Initialize button component.
    /// </summary>
    public void Initialize()
    {
        //****************************************************Should be other axis, maybe not x axis// 
        transform.GetComponent<InteractionButton>().minMaxHeight =
            new Vector2(transform.localPosition.y, transform.localPosition.y + DownOffset);
        transform.GetComponentInChildren<MeshRenderer>().material = relaxDefaultMaterial;
        AddMethodsToEvents();
        MapStatusesUpdown();
        //TODO get start status up or down? seems the IsPressedDown = false is!
        IsPressedDown = false;
        Status = ButtonUpdownStatus[IsPressedDown];
        previousStatus = Status;
        IsNeedCheck = false;
    }

    /// <summary>
    /// initialize the component in the awake
    /// </summary>
    public void Awake()
    {
        Initialize();
        //todo find the pressure display game object in Awake
        iterateGameObject = gameObject;
        while (!SteamGenerator.InitialSettingElements/*SetScenes.GeneratorElements*/.Contains(iterateGameObject.name))
        {
            iterateGameObject = iterateGameObject.transform.parent.gameObject;
        }
    }

    void Update()
    {
        if (previousStatus != Status)
        {
            //TODO remember to turn the component to the correct status
            //Rotate(SwitchStatusAngle[status] - StartAngle);
            UpdateMaterials();
            previousStatus = Status;
        }
    }

    public void UpdateMaterials()
    {
        if (!ButtonStatusUpdown[Status])
        {
            transform.GetComponent<InteractionButton>().minMaxHeight =
                new Vector2(transform.localPosition.y, transform.localPosition.y + DownOffset);
            transform.GetComponentInChildren<MeshRenderer>().material = relaxDefaultMaterial;
        }
        else
        {
            transform.GetComponent<InteractionButton>().minMaxHeight =
                new Vector2(transform.localPosition.y, transform.localPosition.y + DownOffset * LockPercent);
            transform.GetComponentInChildren<MeshRenderer>().material = depressedDefaultMaterial;
        }
    }

    /// <summary>
    /// at the moment that the leap hand press down the button
    /// </summary>
    //! "press down" is the moment when the button reaches the extreme position.
    public void OnPress()
    {
        if (!isEnabled)
        {
            return;
        }
        //todo: else trigger something

        //if check the state of the button, if it is pressed down(or not) already, set the new movement range for the button 
        if (IsPressedDown)
        {
            transform.GetComponent<InteractionButton>().minMaxHeight =
                new Vector2(transform.localPosition.y, transform.localPosition.y + DownOffset );
        }
        else
        {
            transform.GetComponent<InteractionButton>().minMaxHeight =
                new Vector2(transform.localPosition.y, transform.localPosition.y + DownOffset * LockPercent);

        }
        //every time the button is released, the value of the flag should be updated
        //Debug.Log("IsPressedDown before change its state: " + IsPressedDown);
        IsPressedDown = !IsPressedDown;
        //Debug.Log("IsPressedDown after change its state: " + IsPressedDown);
    }

    public void OnContactBegin()
    {
        Evaluation.StudentOperations.ActingAction.StatusBefore = iterateGameObject.GetComponent<IGeneratorComponent>().Status;
        Debug.Log("iterateGameObject.GetComponent<IGeneratorComponent>().Status before: "+ iterateGameObject.GetComponent<IGeneratorComponent>().Status);
    }

    /// <summary>
    /// at the moment when the button is released
    /// </summary>
    //! the instant when the button is released
    public void OnUnpress()
    {
        Status = ButtonUpdownStatus[IsPressedDown];
        //if the not enabled, do nothing
        if (!isEnabled)
        {
            return;
        }

    }
    /// <summary>
    /// the material of the moment when the button is contacted and "grasped"
    /// </summary>
    //! even if the "grasp" is ignored in the interaction button, the grasp still works as events.
    //! "grasp" ignored only means that the physics is ignored(which means the gameobject can not "stick" to a leap hand)
    public void OnContactBeginStay()
    {
        if (gameObject.GetComponent<BlinkMaterial>()!=null)
        {
            gameObject.GetComponent<BlinkMaterial>().IsBlink = false;

        }

        if (!Enabled)
        {
            return;
        }
        //set the material depends on the status of the button(if it is pressed down or not)
        if (!IsPressedDown)
        {
            transform.GetComponentInChildren<MeshRenderer>().material = relaxHighlightMaterial;
        }
        else
        {
            transform.GetComponentInChildren<MeshRenderer>().material = depresseddHighlightMaterial;
        }


    }

    /// <summary>
    /// also set the materials depends on the status of the button
    /// </summary>
    public void OnContactEnd()
    {
        if (!IsPressedDown)
        {
            transform.GetComponentInChildren<MeshRenderer>().material = relaxDefaultMaterial;
        }
        else
        {
            transform.GetComponentInChildren<MeshRenderer>().material = depressedDefaultMaterial;
        }

        Evaluation.StudentOperations.ActingAction.StatusAfter = iterateGameObject.GetComponent<IGeneratorComponent>().Status/*Status*/;
        Debug.Log("iterateGameObject.GetComponent<IGeneratorComponent>().Status after: " + iterateGameObject.GetComponent<IGeneratorComponent>().Status); 
        iterateGameObject.GetComponent<IGeneratorComponent>().IsNeedCheck = true;
    }


    private void AddMethodsToEvents()
    {
        var interactionButton = gameObject.GetComponent<InteractionButton>();
        interactionButton.OnPress -= OnPress;
        interactionButton.OnPress += OnPress;
        interactionButton.OnUnpress -= OnUnpress;
        interactionButton.OnUnpress += OnUnpress;

        interactionButton.OnContactBegin -= OnContactBegin;
        interactionButton.OnContactBegin += OnContactBegin;

        interactionButton.OnContactStay -= OnContactBeginStay;
        interactionButton.OnContactStay += OnContactBeginStay;
        interactionButton.OnContactEnd -= OnContactEnd;
        interactionButton.OnContactEnd += OnContactEnd;


    }

    /// <summary>
    /// map status and angles
    /// </summary>
    private void MapStatusesUpdown()
    {
        ButtonUpdownStatus.Clear();
        //add the status to the switchActions dictionary
        ButtonUpdownStatus.Add(true,1);
        ButtonUpdownStatus.Add(false,0);

        ButtonStatusUpdown = ButtonUpdownStatus.ToDictionary((i) => i.Value, (i) => i.Key);
    }

    public void GetOperatedComponent()
    {
        //if grasp a new component and its status is modified by leap hand 
        //Debug.Log("Evaluation.StudentOperations.ActingAction.Actuator.name: " + Evaluation.StudentOperations.ActingAction.Actuator.name);

        if (true/*gameObject != Evaluation.CurrentOperatedComponent *//*&& previousStatus != Status*/)
        {
            Evaluation.LastOperatedComponent = Evaluation.CurrentOperatedComponent;
            if (true/*!Evaluation.previousOperatedComponents.Contains(gameObject.name)*/)
            {
                Evaluation.previousOperatedComponents.Add(iterateGameObject.name);
                Debug.Log("the status of the actuator while releasing: " + iterateGameObject.GetComponentInChildren<IGeneratorComponent>().Status);
            }
        }
        Evaluation.CurrentOperatedComponent = iterateGameObject;
        //Evaluation.StudentOperations.ActingAction.StatusAfter = Status;
        //actingModifyActuator.StatusAfter = Evaluation.StudentOperations.ActingAction.StatusAfter;

        if (Evaluation.StudentOperations.ActingAction.StatusBefore != Status && !Evaluation.isFinalSettingCorrect) //only when the status is different, we check its sequence
        {
            IsNeedCheck = true;
        }

        //GameObject generator = GameObject.Find("Generator V3");
        //SteamGenerator steamGenerator = new SteamGenerator(generator);
        //Debug.Log("Serializzo");
        //var compList = steamGenerator.SerializeGeneratorStatus();
        //Debug.Log("Produco json");
        //var json = steamGenerator.GetJson(compList);
        //Debug.Log("Produco json " + json);

        //UDP_Connection.senderUdpClient(json);
    }

    public void LowlightComponentsWhileContacting() //subscribe to interaction OnGrasp(Contact)Begin, lowlight here means stop blinking
    {
        Evaluation.StudentOperations.ActingAction = new Evaluation.AnAction(); // the creation of the acting actuator should be put in the "on press" event
        Evaluation.StudentOperations.ActingAction.Initialize();
        Evaluation.StudentOperations.ActingAction.IsModificationCorrect = true;
        //Debug.Log("the status of the actuator while contacting: " + iterateGameObject.GetComponentInChildren<IGeneratorComponent>().Status);
        Evaluation.StudentOperations.LastAction.Actuator = Evaluation.StudentOperations.ActingAction.Actuator;
        //Evaluation.StudentOperations.ActingAction.Actuator = null; //get the actuator
        //todo here should be a logic to be able to find control elements(the pressure display)

        //iterateGameObject.GetComponent<IGeneratorComponent>().IsNeedCheck = true;//need to sequence check the pressure display
        //Debug.Log("the iterate gameobject class: "+ iterateGameObject.GetComponent<IGeneratorComponent>());
        //Debug.Log("the iterate gameobject class.status "+ iterateGameObject.GetComponent<IGeneratorComponent>().Status);
        //Debug.Log("the iterate gameobject class.isneedcheck: "+ iterateGameObject.GetComponent<IGeneratorComponent>().IsNeedCheck);
        Evaluation.StudentOperations.ActingAction.Actuator = iterateGameObject; //get the actuator

        //Debug.Log("here I get the acting actuator gameobject: " + Evaluation.StudentOperations.ActingAction.Actuator.name);
        //Evaluation.StudentOperations.ActingAction.StatusBefore = gameObject.GetComponentInChildren<IGeneratorComponent>().Status; //get the status before
        foreach (var name in SteamGenerator.InitialSettingElements)
        {
            if(GameObject.Find(name).GetComponent<BlinkMaterial>() != null) 
            {
                GameObject.Find(name).GetComponent<BlinkMaterial>().IsBlink = false;
            }
            GameObject.Find(name).GetComponent<IGeneratorComponent>().UpdateMaterials();

        }
    }

    #endregion
}


