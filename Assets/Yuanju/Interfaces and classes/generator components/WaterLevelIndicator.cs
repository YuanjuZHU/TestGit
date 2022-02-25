using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLevelIndicator : MonoBehaviour, IGeneratorComponent
{
    #region Properties
    [SerializeField]
    protected bool isEnabled = true;
    public bool Enabled
    {
        set { isEnabled = value; }
        get { return isEnabled; }
    }

    [SerializeField] protected int status = 0;  //the status here has a physical meaning: the current water level in indicator
    public int Status
    {
        get { return status; }
        set { status = value; }
    }

    public bool IsNeedCheck { get; set; }

    [SerializeField] private GameObject waterLevel;
    [SerializeField] private float scale; //use this scale to make the status visualization a percentage

    private int previousStatus;
    #endregion

    #region Methods
    public void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        //previousStatus = status;
    }
    public void GetOperatedComponent()
    {
    }

    public void UpdateMaterials() //in this class, update materials is used to update the level of water in the indicator 
    {
        var originalLocalScale = waterLevel.transform.localScale;
        waterLevel.transform.localScale = new Vector3(originalLocalScale.x, originalLocalScale.y, (scale * status/100f));
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
