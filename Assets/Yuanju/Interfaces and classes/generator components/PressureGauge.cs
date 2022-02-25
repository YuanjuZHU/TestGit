using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureGauge : MonoBehaviour, IGeneratorComponent
{
    #region Properties
    [SerializeField]
    protected bool isEnabled = true;
    public bool Enabled
    {
        set { isEnabled = value; }
        get { return isEnabled; }
    }

    [SerializeField] protected int status = 0;  //the status here has a physical meaning: the current pressure in the generator
    public int Status
    {
        get { return status; }
        set { status = value; }
    }

    public bool IsNeedCheck { get; set; }

    [SerializeField] private GameObject pressureNeedle;

    private int previousStatus;
    private Vector3 originalRotation;
    #endregion

    #region Methods
    public void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        previousStatus = status;
        originalRotation = pressureNeedle.transform.localEulerAngles;
    }
    public void GetOperatedComponent()
    {
    }

    public void UpdateMaterials() //in this class, update materials is used to update the level of water in the indicator 
    {
        pressureNeedle.transform.localEulerAngles = new Vector3(originalRotation.x, originalRotation.y,  status*18); //18 = 90 / 5 which is got from the picture in the slides. 
    }

    void Update()
    {
        if (previousStatus != status)
        {
            UpdateMaterials();
            previousStatus = status;
        }
    }

    #endregion
}

