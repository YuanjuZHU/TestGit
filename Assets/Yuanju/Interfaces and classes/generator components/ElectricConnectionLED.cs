using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// this script is created for the white LED
/// </summary>
public class ElectricConnectionLED : MonoBehaviour
{
    public GameObject ElectrcityConnectionHandle; 

    // Update is called once per frame
    void Update()
    {
        if (ElectrcityConnectionHandle.GetComponent<Switch>().previousStatus != ElectrcityConnectionHandle.GetComponent<Switch>().Status)
        {
            TurnOnOffLED();
        }
    }
    /// <summary>
    /// this method is dedicated to the control of the white LED, when red hand is on, the white LED should be turned on
    /// </summary>
    private void TurnOnOffLED()
    {
        GetComponent<LED>().Status = ElectrcityConnectionHandle.GetComponent<Switch>().IsPowerConnected ? 0 : 1;
        GetComponent<LED>().UpdateMaterials();
    }
}
