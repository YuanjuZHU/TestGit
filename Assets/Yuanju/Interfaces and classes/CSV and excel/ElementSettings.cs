using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// this class is used to define the row of initial settings and final settings, each instance of this class can be regard as a row
/// </summary>
public class ElementSettings : MonoBehaviour
{
    public string GeneratorStatus; //the "Status" in the initial and final settings sheets
    public string Task; //the "Task" in the initial and final settings sheets
    public string SubTask; //the "Sub Task" in the initial and final settings
    public Dictionary<string, StatusData> ElementStatus = new Dictionary<string, StatusData>(); //a dictionary using the name of element to retrive the status of the corresponding element
    public Dictionary<string, string> ParameterSetting = new Dictionary<string, string>(); //a dictionary using the name of element to retrive the status of the corresponding parameter, the element has complex conditions to meet
                                                                                           //TODO add two constructors(can be empty), one is empty, the other is with all the properties
    public ElementSettings()
    {
    }

    public ElementSettings(string generatorStatus, string task, string subTask, Dictionary<string, StatusData> elementStatus)
    {
        GeneratorStatus = generatorStatus;
        Task = task;
        SubTask = subTask;
        ElementStatus = elementStatus;
    }

    public ElementSettings(string generatorStatus, string task, string subTask, Dictionary<string, string> parameterSetting)
    {
        GeneratorStatus = generatorStatus;
        Task = task;
        SubTask = subTask;
        ParameterSetting = parameterSetting;
    }
}
