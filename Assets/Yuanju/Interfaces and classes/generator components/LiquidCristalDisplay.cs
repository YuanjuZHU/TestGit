using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using UnityEngine;
using Text = UnityEngine.UI.Text;
using System.Linq;

public class LiquidCristalDisplay : MonoBehaviour, IGeneratorComponent
{
    #region Properties
    [SerializeField]
    protected bool isEnabled = true;
    public bool Enabled
    {
        set { isEnabled = value; }
        get { return isEnabled; }
    }

    public int status = 0;  //status here has a physical meaning: the current pressure(bar)
    public int Status
    {
        get { return status;}
        set { status = value; }
    }

    public bool IsNeedCheck { get; set; }



    public Text textPressureOnLCD;
    private int previousStatus;
    public Dictionary<Transform, Material> PartMaterials = new Dictionary<Transform, Material>();
    //[SerializeField] public Material defualtMaterialButtons;
    //[SerializeField] public Material defualtMaterialScreen;
    //[SerializeField] public Material defualtMaterialPanel;
    #endregion
    // Start is called before the first frame update
    public void Awake()
    {
        //get the pressure text on the LCD
        textPressureOnLCD = GameObject.Find("pressure text").GetComponent<Text>();
        Initialize();
    }

    void Start()
    {
        GetMaterials();
    }

    // Update is called once per frame
    void Update()
    {
        //if we detect that the status does not equal to the text on the LCD
        if (status!=int.Parse(textPressureOnLCD.text) )
        {
            //there can be two possibilities: the button(s) has been pressed or the status is changed externally from the simulator
            if (previousStatus!=status) //the status has been changed externally
            {
                textPressureOnLCD.text = "\n"+ status.ToString();
                previousStatus = status;
            }

            else //the button(s) has been pressed, a button has been pressed can also be detected in the "GetOperatedComponent" method in this class, but we are not using it. 
            {
                status = int.Parse(textPressureOnLCD.text);
            }
        }
    }

    public void Initialize()
    {
        textPressureOnLCD.text = "\n"+ status.ToString();
        previousStatus = status;
        
    }

    public void GetOperatedComponent()
    {
    }

    //todo send Katia the list of actuators and list of actuators + control elements
    public void GetMaterials()
    {
        var AllLCDParts = gameObject.transform.GetSelfAndAllChildren();
        for (int i = 0; i < AllLCDParts.Count; i++)
        {
            if (AllLCDParts[i].GetComponent<MeshRenderer>() != null)
            {
                PartMaterials.Add(AllLCDParts[i], AllLCDParts[i].GetComponent<MeshRenderer>().material);
            }
        }
    }

    /// <summary>
    /// use this method to assign the LCD with the default materials
    /// </summary>
    public void UpdateMaterials()
    {
        var transforms = new List<Transform>();
        transforms = PartMaterials.Keys.ToList();
        for (int i = 0; i < transforms.Count; i++)
        {
            transforms[i].GetComponent<MeshRenderer>().material = PartMaterials[transforms[i]];
        }
    }

}
