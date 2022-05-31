using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boiler : MonoBehaviour, IGeneratorComponent
{
    //public GameObject EntireGenerator;
    public GameObject PartToBeTransparent;
    public Material TransparentMaterial;
    public GameObject BoilerGO;
    public GameObject Water;
    public GameObject DetectorShort;
    public GameObject DetectorMedium;
    public GameObject DetectorLong;
    public GameObject WaterLevelIndicator;
    public GameObject DisplayWindow;

    public bool Enabled { get; set; }
    [SerializeField]
    private int status;//the water full fill percentage in the boiler
    public int Status
    {
        get { return status; }
        set { status = value; }
    }

    public bool IsNeedCheck { get; set; }
    public int ShortRodLevel;
    public int MidRodLevel;
    public int LongRodLevel;
    public int MaxiLevel;

    private float workingDistance;
    private float AlarmingDistance;
    private float DeltaLZeroToLongRod;
    private float DeltaLLongToMedium;
    private float DeltaLMediumToShort;
    private Vector3 ZeroWaterLevel;

    public static bool IsShowBoilerTriggered = false;
    public static bool IsHideBoilerTriggered = false;

    private Dictionary<MeshRenderer, Material> defaultMaterials = new Dictionary<MeshRenderer, Material>();

    public void Awake()
    {
        Initialize();
    }
    public void Initialize()
    {
        ZeroWaterLevel = Water.transform.localPosition;
        //Status = WaterLevelIndicator.GetComponentInChildren<WaterLevelIndicator>().Status;// the water level in the boiler is calculated(the same as) by the water level in the indicator
        DeltaLZeroToLongRod = ComputeDistanceWaterAndRod(ZeroWaterLevel, DetectorLong);
        //Debug.Log("Deletas DeltaLZeroToLongRod:" + DeltaLZeroToLongRod);
        DeltaLLongToMedium = ComputeDistanceWaterAndRod(ZeroWaterLevel, DetectorMedium) - DeltaLZeroToLongRod;
        //Debug.Log("Deletas DeltaLLongToMedium:" + DeltaLLongToMedium);
        DeltaLMediumToShort = ComputeDistanceWaterAndRod(ZeroWaterLevel, DetectorShort)- DeltaLLongToMedium- DeltaLZeroToLongRod;
        //Debug.Log("Deletas DeltaLMediumToShort:" + DeltaLMediumToShort);


    }

    public void GetOperatedComponent()
    {

    }

    public void UpdateMaterials()
    {

    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            GetMaterialOfPartToHide();
            AssignTransparentMat();
        }
        if (Input.GetKey(KeyCode.W))
        {
            AssignDefaultMaterial();
        }

        //if (Input.GetKey(KeyCode.E))
        //{
            MoveBoilerWaterLevel();
        //Debug.Log("E is pressed");
        //}
        //ShowBoiler();
        //HideBoiler();
    }

    public void ShowBoiler()
    {
        //if (IsShowBoilerTriggered)
        {
            GetMaterialOfPartToHide();
            AssignTransparentMat();
            MoveBoilerWaterLevel();
            IsShowBoilerTriggered = false;
            HelpMenuButtons.IsBoilerInsideButtonClicked = true;
        }
    }

    public void HideBoiler()
    {
        //if (IsHideBoilerTriggered)
        {
            AssignDefaultMaterial();       
            MoveBoilerWaterLevel();
            IsHideBoilerTriggered = false;
            HelpMenuButtons.IsBoilerInsideButtonClicked = false;
        }
    }

    ///// <summary>
    ///// get the renderer and the default materials of the generator
    ///// </summary>
    //public void GetOtherPartsMaterials()
    //{
    //    var generatorRenderer = EntireGenerator.GetComponentsInChildren<MeshRenderer>().ToList();
    //    var boilerRenderer= GetComponentsInChildren<MeshRenderer>().ToList();

    //    var myRenderers = generatorRenderer.Except(boilerRenderer);

    //    foreach (var renderer in myRenderers)
    //    {
    //        if (renderer.material.HasProperty("_Color") && renderer.material.color != TransparentMaterial.color) 
    //        {
    //            defaultMaterials.Remove(renderer);
    //            defaultMaterials.Add(renderer,renderer.material);
    //            Debug.Log("renderer.material and TransparentMaterial: "+ renderer.material+ "===" + TransparentMaterial);
    //            Debug.Log("renderer.material.name: " + renderer.material.name);
    //            Debug.Log("TransparentMaterial.name: " + TransparentMaterial.name);
    //            Debug.Log("renderer.material.HasProperty(\"color\"): " + renderer.material.HasProperty("Color"));
    //        }
    //    }
    //}


    /// <summary>
    /// 
    /// </summary>
    public void GetMaterialOfPartToHide()
    {
        var partRenderer = PartToBeTransparent.GetComponentInChildren<MeshRenderer>();
        if (partRenderer.material.HasProperty("_Color") && partRenderer.material.color != TransparentMaterial.color)
        {
            defaultMaterials.Remove(partRenderer);
            defaultMaterials.Add(partRenderer, partRenderer.material);
        }

    }



    public void AssignTransparentMat()
    {
        var keys = defaultMaterials.Keys;
        foreach (var key in keys)
        {
            key.material = TransparentMaterial;
        }
    }

    public void AssignDefaultMaterial()
    {
        var keys = defaultMaterials.Keys;
        foreach (var key in keys)
        {
            key.material = defaultMaterials[key];
        }
    }

    public float ComputeHeightBetweenRods(Vector3 waterLevel, GameObject detectorShort, GameObject detectorLong)
    {
        
        Vector3 closestPoint1 = detectorShort.GetComponent<Collider>().ClosestPointOnBounds(waterLevel);
        Vector3 closestPoint2 = detectorLong.GetComponent<Collider>().ClosestPointOnBounds(waterLevel);
        float distance = Mathf.Abs(closestPoint1.y - closestPoint2.y)/BoilerGO.transform.localScale.y;
        //Debug.Log("closestPoint short pin: " + closestPoint1);
        //Debug.Log("closestPoint long pin: " + closestPoint2);
        return distance;
    }

    public float ComputeDistanceWaterAndRod(Vector3 waterLevel, GameObject detectorRod)
    {

        Vector3 closestPoint = detectorRod.GetComponent<Collider>().ClosestPointOnBounds(waterLevel);
        float DeltaL = Mathf.Abs(waterLevel.y - closestPoint.y) / BoilerGO.transform.localScale.y;
        return DeltaL;
    }


    private void MoveBoilerWaterLevel()
    {
        //Status = WaterLevelIndicator.GetComponentInChildren<WaterLevelIndicator>().Status;// the water level in the boiler is calculated(the same as) by the water level in the indicator
        //compute per unit water level difference on the water level indicator, the displacement of the water level in the boiler

        //var alarmDistance = ComputeHeightBetweenRods(Water, DetectorMedium, DetectorLong);
        //Debug.Log("the y distance between two metal rods alarm: " + alarmDistance);
        //Debug.Log("the y distance between two metal rods working: " + workingDistance);


        //if (cumulateddistance <= alarmDistance)
        //{
        float cumulatedDispalecementCorrect=0;
        if (Status > ShortRodLevel)
        {
            //cumulatedDispalecementCorrect = (ComputeDistanceWaterAndRod(ZeroWaterLevel, DetectorShort) + a* deltaStatus * ScaleShortToFull);
            //Debug.Log("beyond limit: ");
        }
        else if (Status > MidRodLevel)
        {
            //cumulatedDispalecementCorrect = (ComputeDistanceWaterAndRod(ZeroWaterLevel, DetectorMedium) + a* deltaStatus * ScaleMediumToShort);
            //cumulatedDispalecementCorrect = (DeltaLZeroToLongRod+DeltaLLongToMedium)+ (DeltaLZeroToLongRod + DeltaLLongToMedium + DeltaLMediumToShort) * ((Status - MidRodLevel) / ShortRodLevel);
            //Debug.Log("ComputeDistanceWaterAndRod(ZeroWaterLevel, DetectorMedium): " + ComputeDistanceWaterAndRod(ZeroWaterLevel, DetectorMedium));
            //Debug.Log("sum of delta zerotolong and delta longtomid: " + (DeltaLZeroToLongRod + DeltaLLongToMedium).ToString());
            //Debug.Log("cumulatedDispalecementCorrect old: " + cumulatedDispalecementCorrect);

            cumulatedDispalecementCorrect = (DeltaLZeroToLongRod + DeltaLLongToMedium) + (Status - MidRodLevel) * (DeltaLMediumToShort / (ShortRodLevel - MidRodLevel));
            //Debug.Log("cumulatedDispalecementCorrect new: " + cumulatedDispalecementCorrect);

        }
        else if (Status > LongRodLevel)
        {
            //cumulatedDispalecementCorrect = DeltaLZeroToLongRod + (DeltaLZeroToLongRod + DeltaLLongToMedium) * (Status - LongRodLevel) / MidRodLevel;

            cumulatedDispalecementCorrect = DeltaLZeroToLongRod + (Status - LongRodLevel) * DeltaLLongToMedium / (MidRodLevel - LongRodLevel);
            //Debug.Log("cumulatedDispalecementCorrect MidRodLevel > Status > LongRodLevel: " + cumulatedDispalecementCorrect);
        }
        else
        {
            cumulatedDispalecementCorrect = DeltaLZeroToLongRod * Status / LongRodLevel;
            //Debug.Log("cumulatedDispalecementCorrect LongRodLevel > Status: " + cumulatedDispalecementCorrect);
            //Debug.Log("DeltaLZeroToLongRod c: " + DeltaLZeroToLongRod);
        
        }




        Vector3 displacementVec = new Vector3(0, cumulatedDispalecementCorrect, 0);
        Vector3 newPos = displacementVec + ZeroWaterLevel;
        Water.transform.localPosition = newPos;

    }


}

