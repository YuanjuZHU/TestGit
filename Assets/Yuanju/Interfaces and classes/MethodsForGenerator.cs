using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
//using GameObject = UnityEngine.GameObject;

public class MethodsForGenerator : MonoBehaviour
{
    #region Overall methodes
    public void TellMyIdentity(GameObject instance)
    {
        //Debug.Log("I'm inside method, gameobject:" + instance.name);
        //Debug.Log(string.Format("I'm inside method, gameobject:{0} Switch.status:{1}", instance.name, instance.GetComponent<Switch>().Status));
        //status = Switch.status;
        //Debug.Log("I'm a method of switches: "+ status);
    }
    //TODO this method is an overall method, pay attention to the "Membrane.GetComponent<Switch>()"
    #endregion
    #region MethodsForSwitch



    #endregion

    public List<float> MembranePressureLimits;
    public Text LCDPressure;
    private float numPressure;
    private float numPressureIteration;
    public float numberIncrementPerSecond; //the bigger this value is, the faster the number on the LCD changes
    public float HoldOnTime;
    #region MethodsForButton

    //TODO store the starting(current) value of the pressure in a variable(can be also used for the scene setting), record the pressure p(t), 
    public void AdjustPressureTemperatureOnLCD(GameObject membrane)
    {
        //var Membranes = GameObject.FindGameObjectsWithTag("Membrane");
        //LCDPressure = GameObject.Find("pressure text").GetComponent<Text>();
        numPressure = int.Parse(LCDPressure.text);
        if (membrane.name.Contains("+") && numPressure < MembranePressureLimits.Max()/*maxPressure*/ )
        {
            Invoke("DisplayTextOnLCD", HoldOnTime);
            numPressureIteration += numberIncrementPerSecond*Time.deltaTime;
            Debug.Log("numPressure: "+ numPressureIteration);
            Debug.Log("Time.deltaTime: " + Time.deltaTime);
        }
        if (membrane.name.Contains("-") && numPressure > MembranePressureLimits.Min()/*minPressure*/)
        {
            Invoke("DisplayTextOnLCD", HoldOnTime);
            numPressureIteration -= numberIncrementPerSecond * Time.deltaTime;
        }

        //var Text = Membrane.GetComponentInChildren<Text>();

        //status = Switch.status;
        //Debug.Log("I'm a method of switches: "+ status);



    }

    public void InitializePressureButton()
    {
        numPressureIteration = int.Parse(LCDPressure.text);
    }

    public void DisplayTextOnLCD()
	{
        LCDPressure.text = "\n" + ((int)numPressureIteration).ToString(/*"#0.0"*/);

    }
    #endregion
    #region MethodsForValve



    #endregion


}
