using Assets.Yuanju.Interfaces_and_classes.generator_components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SetScenes : MonoBehaviour
{
    public static Dictionary<string, IGeneratorComponent> ScriptComponents= new Dictionary<string, IGeneratorComponent>();
    //public static List<String> GeneratorElements=new List<String>(); //attention here the operable components is the same as the list OperableComponentNames, but the names are in different order
    public static Dictionary<string, GameObject> NonSettableElements = new Dictionary<string, GameObject>(); //the elements like the water tank cover, which don't have object class attached
    void Awake()
    {
        //Debug.Log("ReadCSV.componentNames: "+ ReadCSV.componentNames.Count);
        AddInstancesToDic();
    }


    void OnGUI()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            Debug.Log("+ pressed");

            //PopSettings(1);
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            //PopSettings(0);
        }
    }

    /// <summary>
    /// in this method, the datatables has been replaced
    /// </summary>
    void Update()
    {
        if (!PanelManager.isGeneratorConfigured || (Input.GetKeyDown(KeyCode.A)))
        {
            for (int i = 0; i < SteamGenerator.SubTasks.Count()/*NPOIGetInitialFinalSettings.SubTasks.Count*/; i++)
            {
                //TODO needs to implement for the italian version
                if (SteamGenerator.SubTasks.ElementAt(i)/*NPOIGetInitialFinalSettings.SubTasks[i]*/.Contains(PanelManager.listToggleText[2]))
                {
                    Debug.Log("the task in the excel: " + SteamGenerator.SubTasks.ElementAt(i));
                    Debug.Log("the task in the toggle: " + PanelManager.listToggleText[2]);
                    PopSettings(SteamGenerator.SubTasks.ElementAt(i));
                }
            }
            PanelManager.isGeneratorConfigured = true;
        }
    }

    /// <summary>
    /// Use names as keys to map the script instances 
    /// </summary>
    private void AddInstancesToDic()
    {
        foreach (var Name in ReadCSV.componentNames)
        {
            //Debug.Log("the name off the4 gameobject 0: " + GameObject.Find(Name).ToString());
            if (GameObject.Find(Name).GetComponentInChildren<IGeneratorComponent>()!=null)
            {
                //Debug.Log("the name off the4 gameobject 1: "+ GameObject.Find(Name).ToString());
                ScriptComponents.Add(Name, GameObject.Find(Name).GetComponentInChildren<IGeneratorComponent>());
                //GeneratorElements.Add(Name);
                Debug.Log("GameObject.Find(Name).GetComponentInChildren<IGeneratorComponent>(): " + Name);
                Debug.Log("GameObject.Find(Name).GetComponentInChildren<IGeneratorComponent>(): " + GameObject.Find(Name).GetComponentInChildren<IGeneratorComponent>());

            }
            else
            {
                NonSettableElements.Add(Name, GameObject.Find(Name));
            }
            
            //Debug.Log("the Generator components: "+ ScriptComponents[Name]);
        }
    }
    //TODO implement the method that can use the info in the excel file "initial settings" to pop the generator's settings.
    /// <summary>
    /// use initial settings to set the scene
    /// </summary>
    private void PopSettings(string task)
    {
        var settings = SteamGenerator.InitialSettings.Where(x => x.SubTask == task).ElementAt(0);
        foreach (var Name in SteamGenerator.InitialSettingElements/*GeneratorElements*/)
        {
            //TODO if the name exist in the dictionary's key words
            //Debug.Log("NPOIGetInitialFinalSettings.TaskComponentInitialSettings[task][Name]: "+ NPOIGetInitialFinalSettings.TaskComponentInitialSettings[task][Name]);
            if (settings.ElementStatus[Name].Status!=null)
            {
                ScriptComponents[Name].Status = (int)settings.ElementStatus[Name].Status; /*NPOIGetInitialFinalSettings.TaskComponentInitialSettings[task][Name];*/
            }
            ScriptComponents[Name].UpdateMaterials(); //indispensable, sometimes the previous status is the same as the status to pop in
            //Debug.Log("the actuators names: " + Name);
            //Debug.Log("the actuators supposed status: " + NPOIGetInitialFinalSettings.TaskComponentInitialSettings[task][Name]);

            //TODO should update the "status" of the non operable components, like change the material for the LEDs. 
            //Debug.Log(" NPOIReadExcelOld.TaskComponentInitialSettings[NPOIReadExcelOld.SubTasks[SceneNumber]][Name]: " + NPOIReadExcelOld.TaskComponentInitialSettings[NPOIReadExcelOld.SubTasks[SceneNumber]][Name]);
        }

        //foreach (var VARIABLE in ReadCSV.componentNames)
        //{
        //    if (!NPOIGetInitialFinalSettings.TaskCompoentInitialSettings[NPOIGetInitialFinalSettings.SubTasks[0]].Keys.Contains(VARIABLE))
        //        Debug.Log("!ReadCSV.componentNames.Contains(VARIABLE): " + VARIABLE);

        //}
    }
}
