using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTank : MonoBehaviour, IGeneratorComponent
{
    #region Properties
    [SerializeField]
    protected bool isEnabled = true;
    public bool Enabled
    {
        set { isEnabled = value; }
        get { return isEnabled; }
    }

    [SerializeField] protected int status;  //the status here has a physical meaning: the current water level in the water tank
    public int Status
    {
        get { return status; }
        set { status = value; }
    }

    public bool IsNeedCheck { get; set; }

    [SerializeField] private GameObject waterInTank;
    [SerializeField] private float scale; //use this scale to make the status visualization a percentage

    private Vector3 initialWaterLevel;

    private int previousStatus;
    #endregion

    #region Methods
    public void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        initialWaterLevel = waterInTank.transform.localPosition;
        //previousStatus = status;
    }
    public void GetOperatedComponent()
    {
    }

    public void UpdateMaterials() //in this class, update materials is used to update the level of water in the tank 
    {
        waterInTank.transform.localPosition = initialWaterLevel + scale * Vector3.up * status / 100;
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
