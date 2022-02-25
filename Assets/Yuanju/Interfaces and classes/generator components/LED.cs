using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class for the LEDs on the control panel of the steam generator
/// </summary>
public class LED : MonoBehaviour, IGeneratorComponent   
{
    #region Properties
    [SerializeField]
    protected bool isEnabled = true;
    public bool Enabled
    {
        set { isEnabled = value; }
        get { return isEnabled; }
    }

    private int status = 0;
    public int Status
    {
        get { return status; }
        set { status = value; }
    }

    public bool IsNeedCheck { get; set; }

    [SerializeField] private Material OffMaterial; //the material for the LED when it is off
    [SerializeField] private Material OnMaterial; //the material for the LED when it is on
    private bool runTimeException;
    private int previousStatus;
    #endregion

    #region Methods
    public void Awake()
    {

    }

    public void Initialize()
    {
        previousStatus = status;
    }
    public void GetOperatedComponent()
    {
    }

    public void UpdateMaterials()
    {
        if (status == 0 )
        {
            gameObject.GetComponentInChildren<MeshRenderer>().material = OffMaterial;
        }
        else
        {
            gameObject.GetComponentInChildren<MeshRenderer>().material = OnMaterial;
        }
    }

    void Update()
    {
        if (previousStatus!=status)
        {
            UpdateMaterials();
            previousStatus = status;
        }
    }

    #endregion


}
