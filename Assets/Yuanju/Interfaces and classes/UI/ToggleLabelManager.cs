using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// this class manage the text of label on the toggles, if the student ask for the reason of the problem, the text of the labels will be replaced by the reasons
/// </summary>
public class ToggleLabelManager : MonoBehaviour
{
    public static GameObject problemText;
    public GameObject ProblemText;
    public static GameObject backGroundPanel;
    public GameObject BackGroundPanel;

    void Start()
    {
        InitializeToggleLabels();
        problemText = ProblemText;
        backGroundPanel = BackGroundPanel;
    }


    //initialize the toggles
    void InitializeToggleLabels()
    {
        //the label brief is the text set in Unity's hierarchy window
        for (int i = 0; i < PanelManager.allToggles.Length; i++)
        {
            PanelManager.allToggles[i].labelBrief = PanelManager.allToggles[i].GetComponentInChildren<Text>(true).text;
        }

        for (int i = 0; i < PanelManager.allToggles.Length; i++)
        {

            for (int j = 0; j < NPOIReadExcel.SubTasks.Count; j++)
            {
                if (NPOIReadExcel.SubTasks[j].Contains(PanelManager.allToggles[i].GetComponentInChildren<Text>(true).text) )
                {
                    PanelManager.allToggles[i].labelDetailed = NPOIReadExcel.GeneratorStatus[j];
                }
            }
        }

        for (int i = 0; i < PanelManager.allToggles.Length; i++)
        {
            Debug.Log("PanelManager.allToggles[i].labelBrief: "+ PanelManager.allToggles[i].labelBrief);
            Debug.Log("PanelManager.allToggles[i].labelDetailed: "+ PanelManager.allToggles[i].labelDetailed);
        }

    }

    public static void AssignLabelBrief()
    {
        for (int i = 0; i < PanelManager.allToggles.Length; i++)
        {
            if (PanelManager.allToggles[i].labelBrief.Length != 0) 
            {
                PanelManager.allToggles[i].gameObject.GetComponentInChildren<Text>(true).text = PanelManager.allToggles[i].labelBrief;
            }
        }
    }

    public static void AssignLabelDetailed()
    {
        for (int i = 0; i < PanelManager.allToggles.Length; i++)
        {
            if (PanelManager.allToggles[i].labelDetailed.Length != 0)
            {
                PanelManager.allToggles[i].gameObject.GetComponentInChildren<Text>(true).text = PanelManager.allToggles[i].labelDetailed;
            }
        }
    }

    public static void DisplayProblem()
    {
        problemText.GetComponentInChildren<Text>().text = NPOIReadExcel.GeneratorStatus[NPOIReadExcel.SubTasks.IndexOf(PanelManager.listToggleText[2])];
        backGroundPanel.SetActive(true);
    }

    public static void HideProblem()
    {
        problemText.GetComponentInChildren<Text>().text = null;
        backGroundPanel.SetActive(false);
    }
}
