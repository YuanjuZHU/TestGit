using System.Collections;
using System.Collections.Generic;
using Leap.Unity.Interaction;
using TMPro;
using TMPro.Examples;
using UnityEngine;

public class Slider : MonoBehaviour, IGeneratorComponent
{
    #region properties

    public bool Enabled { get; set; }

    private int privousStatus;
    public int status;
    public int Status
    {
        get { return status; }
        set
        {
            status = value;
        }
    }

    public bool IsNeedCheck { get; set; }

    public GameObject PressureDisplayValue; //the game object that contains the text component to display the value of the slider
    private Vector3 textOffset; //the offset between the slider and pressure set value


    public int SliderMaxValue;//the maximum value the slider can reach
    public int SliderMinValue;//the minimum value when slider can reach
    public Vector2 Offset;
    public float Size;
    #endregion

    public void Awake()
    {
        //var slideLimits = gameObject.GetComponent<InteractionSlider>().sliderType == InteractionSlider.SliderType.Horizontal ? gameObject.GetComponent<InteractionSlider>().horizontalSlideLimits : gameObject.GetComponent<InteractionSlider>().verticalSlideLimits;
        //slideDistance = slideLimits.y - slideLimits.x; //L

        if (gameObject.GetComponent<InteractionSlider>().sliderType == InteractionSlider.SliderType.Horizontal)
        {
            gameObject.GetComponent<InteractionSlider>().horizontalSteps = SliderMaxValue - SliderMinValue;
            gameObject.GetComponent<InteractionSlider>().defaultHorizontalValue =
                (float) 1 / (SliderMaxValue - SliderMinValue) * status;
        }
        else
        {
            gameObject.GetComponent<InteractionSlider>().verticalSteps = SliderMaxValue - SliderMinValue;
            gameObject.GetComponent<InteractionSlider>().defaultVerticalValue =
                (float) 1 / (SliderMaxValue - SliderMinValue) * status;
        }

        //Debug.Log("slide limits: "+ slideDistance);
        //Status = 10;
        //initialIncrement = status;
        privousStatus = status;

        PressureDisplayValue.transform.localScale *= Size;
        UpdateText();
        //PressureDisplayValue.transform.localPosition = transform.localPosition + new Vector3(Offset.x, Offset.y, -0.0028f);
    }

    #region methods

    public void Initialize()
    {

    }

    public void GetOperatedComponent()
    {

    }

    public void UpdateMaterials()
    {

    }

    /// <summary>
    /// update the current step of the slider when the status in the inspector is modified
    /// </summary>
    void Update()
    {
        if ( privousStatus != status)
        {
            if (gameObject.GetComponent<InteractionSlider>().sliderType == InteractionSlider.SliderType.Horizontal)
            {
                gameObject.GetComponent<InteractionSlider>().HorizontalSliderPercent = (float)status / gameObject.GetComponent<InteractionSlider>().horizontalSteps;
                
            }
            else
            {
                gameObject.GetComponent<InteractionSlider>().VerticalSliderPercent = (float)status / gameObject.GetComponent<InteractionSlider>().verticalSteps;

            }
            privousStatus = status;
        }
    }

    public void UpdateStatus()
	{
	    status = gameObject.GetComponent<InteractionSlider>().sliderType == InteractionSlider.SliderType.Horizontal
	        ? (int)(GetComponent<InteractionSlider>().HorizontalSliderPercent* GetComponent<InteractionSlider>().horizontalSteps)
            : (int)(GetComponent<InteractionSlider>().VerticalSliderPercent * GetComponent<InteractionSlider>().verticalSteps);
	    
	}

    public void UpdateText()
    {
        PressureDisplayValue.GetComponent<TextMeshPro>().text = status.ToString() + "bar";
        PressureDisplayValue.transform.localPosition = transform.localPosition + new Vector3(Offset.x, Offset.y, -0.0028f);
    }



    #endregion
}
