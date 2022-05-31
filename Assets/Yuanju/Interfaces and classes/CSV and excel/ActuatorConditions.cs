using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// this class is dedicated to describe the stucture of the sequence tables, it contains properties that represent the information in the sequence table
/// </summary>
public class ActuatorConditions : MonoBehaviour
{
    public string Name; //the name of the actuators in the first column of sequence sheets
    public int? SequenceOrder; //the sequence order row in the sequence sheets
    public List<StatusData> Prerequisites=new List<StatusData>(); // the columns of the sequence table

    public ActuatorConditions()
    {

    }
}
