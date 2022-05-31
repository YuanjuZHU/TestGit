using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// with this class, we are able to store the tables, each instance represents a sequence matrix(table)
/// </summary>
public class Sequence : MonoBehaviour
{
    public string SubTask;
    public List<ActuatorConditions> ActuatorToCheck = new List<ActuatorConditions>();

    public Sequence()
    {

    }
}
