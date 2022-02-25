using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrialCountAndTaskCount : MonoBehaviour
{
    [HideInInspector] public static int TrialCounter;
    [HideInInspector] public static int TaskCounter;
    [HideInInspector] public static int WrongTaskCounter;
    [HideInInspector] public static int CorrectTaskCounter;
    [HideInInspector] public static GameObject resultsPrompt;
    public GameObject ResultsPrompt;

    public static TaskTrialInfo CurrentTaskTrialInfo;
    public List<TaskTrialInfo> TrainingInfo = new List<TaskTrialInfo>();   



    public string TrialLabel;
    public string CorrectTaskLabel;
    public string WrongTaskLabel;
    public string TotalTaskLabel;

    private static string privousTask;

    [HideInInspector] public static bool IsUpdateResult;

    // Start is called before the first frame update
    void Start()
    {
        ResetCounters();
        resultsPrompt = ResultsPrompt;
        resultsPrompt.transform.parent.gameObject.SetActive(false);
        privousTask = null;

        //CurrentTaskTrialInfo.Initialize();
        //TrainingInfo.Add(CurrentTaskTrialInfo);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("the correct task counts: "+CorrectTaskCounter);
        //Debug.Log("the wrong task counts: "+WrongTaskCounter);
        //Debug.Log("the task counts: "+TaskCounter);
        //Debug.Log("the trial counts: "+TrialCounter);
        if(IsUpdateResult) {
            AssignResult();
            IsUpdateResult = false;
        }

        foreach (var item in TrainingInfo)
        {
            Debug.Log("Item."+item.SubTask);
            Debug.Log("item.CorrectCount" + item.CorrectCount);
            Debug.Log("item.WrongCount" + item.WrongCount);
            Debug.Log("item.OverallTrialCount" + item.OverallTrialCount);
            Debug.Log("======================");
        }

    }

    public void ResetCounters()
    {
        TrialCounter = 0;
        TaskCounter = 0;
        WrongTaskCounter = 0;
        CorrectTaskCounter = 0;
    }

    public void SetTrialAndTotalTask()
    {
        TrialCounter = 0;
        TaskCounter++;
        IsUpdateResult = true;
        privousTask = null; //this is to make sure that when a same task has been selected again, but the wrong task count cannot be updated 
    }

    public static void CountTrialsAndTasks()
    {
        if (Evaluation.isFinalSettingCorrect /*&& TrialCounter == 0*/ ) // and the task has changed
        {
            CorrectTaskCounter++;
            //TrialCounter++;
            CurrentTaskTrialInfo.CorrectCount++;
        }

        if (privousTask != PanelManager.listToggleText[2]) 
        {
            if (!Evaluation.isFinalSettingCorrect )
            {
                //if (TrialCounter == 0)
                //{
                WrongTaskCounter++;//the first time confirm to finish a task with wrong actions, WrongTaskCounter increases by 1
                //}
                CurrentTaskTrialInfo.WrongCount++;
            }

            privousTask = PanelManager.listToggleText[2];
        }
        TrialCounter++; //every time confirm to finish a task, trail increases by 1 
        CurrentTaskTrialInfo.OverallTrialCount++;
        //TaskCounter = WrongTaskCounter + CorrectTaskCounter;
        IsUpdateResult = true;
    }
    /// <summary>
    /// Assign the result to the text
    /// </summary>
    public void AssignResult()
    {
        resultsPrompt.GetComponent<Text>().text = TotalTaskLabel + TaskCounter + "\n" + CorrectTaskLabel + CorrectTaskCounter + "\n" + WrongTaskLabel + WrongTaskCounter + "\n" + TrialLabel + TrialCounter;
    }

    public static void VisualizeResult()
    {
        //SpeechRecognizer.IsUpdateResult = true;
        resultsPrompt.transform.parent.gameObject.SetActive(true);
    }

    public static void HideResult()
    {
        resultsPrompt.transform.parent.gameObject.SetActive(false);
    }

    public void AddTaskTrialInfo()  //this method should be executed when "confirm" button is pressed
    {
        bool isListContainTask = false;


        if (TrainingInfo.Count != 0) 
        {
            foreach (var item in TrainingInfo)
            {
                if (item.SubTask.Contains(PanelManager.listToggleText[2]))
                {
                    CurrentTaskTrialInfo = item;
                    isListContainTask = true;
                    break;
                }
            }
        }

        if (!isListContainTask)
        {
            CurrentTaskTrialInfo = new TaskTrialInfo();
            CurrentTaskTrialInfo.Initialize();
            for (int i = 0; i < NPOIGetDatatable.SequenceDatatables.Count; i++)
            {
                if (NPOIGetDatatable.SequenceDatatables[i].TableName.Contains(PanelManager.listToggleText[2]))
                {
                    CurrentTaskTrialInfo.SubTask = NPOIGetDatatable.SequenceDatatables[i].TableName;
                    Debug.Log("I add a new task: " + CurrentTaskTrialInfo.SubTask);
                    break;
                }

            }

            isListContainTask = true;
        }
        TrainingInfo.Add(CurrentTaskTrialInfo);
    }

    /// <summary>
    /// use a class to record how many times are wrong and correct and the trials for each time to achieve the task
    /// </summary>
    public class TaskTrialInfo
    {
        public string SubTask { get; set; }
        public int CorrectCount { get; set; }
        public int WrongCount { get; set; }
        public int OverallTrialCount { get; set; }

        public void Initialize()
        {
            SubTask = " ";
            CorrectCount = 0;
            WrongCount = 0;
            OverallTrialCount = 0;
        }
    }


}
