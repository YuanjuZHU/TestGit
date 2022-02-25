using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Leap.Unity;
using UnityEngine;
using Newtonsoft.Json;
using info.lundin.Math;
using Leap.Unity.Interaction;
using UnityEngine.UI;

public class Evaluation : MonoBehaviour
{
    private Dictionary<string, IGeneratorComponent> scriptComponents;
    private List<string> componentNames;
    public static List<string> previousOperatedComponents=new List<string>(); //use a list of string to store the modified components
    public static GameObject LastOperatedComponent;
    public static GameObject CurrentOperatedComponent;
    public static bool isFinalSettingCorrect = false;
    public static DataTable finalSettingdt = new DataTable();
    static List<char?> finalSettingGroupIndex = new List<char?>();
    static Dictionary<char, List<GameObject>> finalSettingComponentsGroup = new Dictionary<char, List<GameObject>>();  //eg. group a has resistance switch 1, 2, 3 and 4
    static Dictionary<char, List<GameObject>> sequenceComponentsGroup = new Dictionary<char, List<GameObject>>();
    static List<char?> sequenceGroupIndex = new List<char?>();
    public static bool isSequenceCorrect = false;
    public static bool isSequenceForTestingCorrect = true;
    //[HideInInspector]public bool isConfirmClicked = false;
    public static GameObject alarm;

    [SerializeField] public Material AlarmGreen;
    private static Material alarmGreen;
    [SerializeField] public Material AlarmRed;
    private static Material alarmRed;

    [SerializeField] public AudioSource AlarmAudio;
    [SerializeField] public static AudioSource alarmAudio;

    public static Operations StudentOperations;
    public static AnAction AnActionOnActuator;
    private static GameObject _sequenceContainer;
    private Dictionary<string, List<string>> ActuatorAndRelatedSequencetables;
    private static Dictionary<string, int> actionSequenceActuator;
    public static GameObject feedbackPrompt;
    public GameObject FeedbackPrompt;
    public static Evaluation instance;
    public Evaluation Instance;
    public static string congratulationText;
    public string CongradulationText;
    public static string encourageText;
    public string EncourageText;
    private static bool feedbackTextUpdated;

    //use the name of an actuator to find the data tables(those data tables' sequence is not null for this actuator,
    //and they are arranged in the sequence number).

    // Start is called before the first frame update
    void Awake()
    {
        alarmGreen = AlarmGreen;
        alarmRed = AlarmRed;
        alarmAudio = AlarmAudio;
        alarm = GameObject.Find("alarm");
        StudentOperations = new Operations();
        StudentOperations.Initialize();
        AnActionOnActuator = new AnAction();
        AnActionOnActuator.Initialize();
        Debug.Log("this is a test for the AnActionOnActuator.WrongSequenceActions: " + AnActionOnActuator.WrongSequenceActions.Count);
        WrongAction wrongactuator = new WrongAction();
        AnActionOnActuator.WrongSequenceActions.Add(wrongactuator);
        Debug.Log("this is a test for the AnActionOnActuator.WrongSequenceActions: " + AnActionOnActuator.WrongSequenceActions.Count);
        //Debug.Log("this is a test for the actuator: "+ StudentOperations.ActingAction.Actuator);
        //AnAction addTestActuator1 = new AnAction();
        //AnAction addTestActuator2 = new AnAction();
        //addTestActuator1.Actuator = new GameObject("add test 1");
        //addTestActuator2.Actuator = new GameObject("add test 2");
        //StudentOperations.AllActions.Add(addTestActuator1);
        //StudentOperations.AllActions.Add(addTestActuator1);
        //StudentOperations.AllActions.Add(addTestActuator2);
        instance = this;
        Instance = instance;
    }

    void Start()
    {
        LastOperatedComponent = null;
        CurrentOperatedComponent = null;
        scriptComponents = SetScenes.ScriptComponents;
        componentNames = SetScenes.GeneratorElements;
        _sequenceContainer = new GameObject("Sequence Container");
        CreateSequence();
        actionSequenceActuator = new Dictionary<string, int>();
        feedbackPrompt = FeedbackPrompt;
        encourageText = EncourageText;
        congratulationText = CongradulationText;
        feedbackPrompt.transform.parent.gameObject.SetActive(false);
        feedbackTextUpdated = true;
    }


    void Update()
    {
        //check sequence and blink actuators based on the check in training mode
        foreach (var value in SetScenes.ScriptComponents.Values) //iterate in all the elements that has a script inherit from  "IGeneratorComponent" attached
        {
            if (value.IsNeedCheck) //a property which will be set to true when the element is touched(grasped) and it status has been changed
            {
                CheckSequence();
                if (PanelManager.listToggleText[1] == "Training" || PanelManager.listToggleText[1] == "Apprendimento") //check if the mode selections is training
                {
                    BlinkTrainingActuatorSequence(); //blink wrong prerequisite actuators and the acting actuator when student has not clicking on "task done" button
                    SetTrainingAlarmMatAndAud(); //set alarm lamp and sound
                }

                value.IsNeedCheck = false; //the property is set to false to make sure the check is only done once
            }
        }

        //blink actuator,set alarm lamp's color and sound based on sequence check
        if (PanelManager.listToggleText[1] == "Training" || PanelManager.listToggleText[1] == "Apprendimento")
        {

            if (SpeechRecognizer.HasCommandFinish) //the student confirms that he has finished the task by voice command.
            {
                BlinkActuatorFinalSetting(); //blink actuator that are wrong of final setting
                SetTrainingAlarmMatAndAud();
                SpeechRecognizer.HasCommandFinish = false;
                StudentOperations.WrongFinalActuators.Clear();
            }
        }

        //Deal with testing mode
        if (PanelManager.listToggleText[1] == "Testing" || PanelManager.listToggleText[1] == "Verifica")
        {
            if (SpeechRecognizer.HasCommandFinish)
            {
                BlinkTestingActuatorSequence();  //blink the first error committed 
                BlinkActuatorFinalSetting();  //blink final setting error
                
                SetTestingAlarmMatAndAud(); //set alarm lamp color and sound 
                SpeechRecognizer.HasCommandFinish = false;
                StudentOperations.WrongFinalActuators.Clear();
            }

        }
        //Debug.Log("the task after the checks: " + PanelManager.listToggleText[2]);

        
        if(!feedbackTextUpdated) //check if the feedback text is updated
        {
            float displayTime = 0; // the time to display the feedback
            if(alarmAudio.isPlaying) //check if the alarm sound is on
            {
                FeedbackPrompt.GetComponent<Text>().text = encourageText; //assign encourage text if alarm is on
                displayTime = 10000;
                for (int i = 0; i < FeedbackPrompt.transform.childCount; i++)
                {
                    FeedbackPrompt.transform.GetChild(i).gameObject.SetActive(true); //activate the buttons to correct or restart task
                }
            } 
            else 
            {
                FeedbackPrompt.GetComponent<Text>().text = congratulationText; //assign congratulation text if alarm is not on
                displayTime = 3;
                for (int i = 0; i < FeedbackPrompt.transform.childCount; i++)
                {
                    FeedbackPrompt.transform.GetChild(i).gameObject.SetActive(false); //hide the correct and restart button
                }
            }
            feedbackTextUpdated = true; //feedback text is updated only once
            //ShowMessage();
            instance.StartCoroutine(ShowMessage(displayTime));
            //the feedback panel appears on the screen for 10 seconds then disappears
        }
        

        if (Input.GetKeyDown("space")) //for development tests 
        {
            ReplayOperations(); //to check if the actions performed by the student are correctly recorded
        }

    }

    /// <summary>
    /// preparations for sequence check: find the correct table 
    private void CheckSequence()
    {
        isSequenceCorrect = true;
        //isFinalSettingCorrect = false;
        //get column in the data table of the last modified component
        Debug.Log("NPOIGetDatatable.SequenceDatatables.Count in evaluation: " + NPOIGetDatatable.SequenceDatatables.Count);
        Debug.Log("acting actuator in check sequence: " + StudentOperations.ActingAction.Actuator);
        Debug.Log("acting actuator in check sequence status before: " + StudentOperations.ActingAction.StatusBefore);
        Debug.Log("acting actuator in check sequence status after: " + StudentOperations.ActingAction.StatusAfter);
        if (StudentOperations.ActingAction != null && StudentOperations.ActingAction.StatusBefore != StudentOperations.ActingAction.StatusAfter) //this condition can be removed since we put this condition in the object class
        {
            StudentOperations.AllActions.Add(StudentOperations.ActingAction); //add the acting actuator into the actions list
        }

        DataTable theExacTableForSequence = new DataTable();
        for (int i = 0; i < NPOIGetDatatable.SequenceDatatables.Count; i++)
        {
            if (NPOIGetDatatable.SequenceDatatables[i].TableName.Contains(PanelManager.listToggleText[2])) //find the corresponding data table by comparing the text of the selected task toggle with the task in the sequence data tables
            {
                var dt = NPOIGetDatatable.SequenceDatatables[i];
                var drs = dt.Select();
                var cellValue = (StatusData) drs[0][StudentOperations.ActingAction.Actuator.name];
                Debug.Log("status value group: "+cellValue.Group);
                if (cellValue.Status != null && actionSequenceActuator.Keys.Contains(StudentOperations.ActingAction.Actuator.name))
                {
                    if(actionSequenceActuator[StudentOperations.ActingAction.Actuator.name] <= cellValue.Status) //actionSequenceActuator is a dictionary to know if the actuator is being operated for more rounds, '=' is for the case that the student to correct the error of a step
                    {
                        theExacTableForSequence = dt;
                        if(StudentOperations.ActingAction != null && StudentOperations.ActingAction.StatusBefore != StudentOperations.ActingAction.StatusAfter)
                        {
                            GetComponentSequenceChecked(theExacTableForSequence, StudentOperations.ActingAction.Actuator.name);//check the sequence for the acting actuator
                        }
                        if(isCheckCorrect) break;
                    }
                }
            }
        }

        isSequenceCorrect = StudentOperations.ActingAction.IsModificationCorrect;
    }

    /// <summary>
    /// check final settings
    /// </summary>
    public static void CheckFinalSettings()
    {
        feedbackTextUpdated = false; //be ready to update the feedback text
        SpeechRecognizer.HasCommandFinish = true;
        isFinalSettingCorrect = true;
        for (int i = 0; i < NPOIReadExcel.FinalSettings.Count; i++)
        {
            if (NPOIReadExcel.FinalSettings[i].TableName.Contains(PanelManager.listToggleText[2]))
            {
                finalSettingdt = NPOIReadExcel.FinalSettings[i].Copy();  //find the final setting data table that will be used for the checking. It is not necessary to use the copy of the data table
            }
        }

        var names = NPOIReadExcel.ActuatorNames; //only the actuators will be checked, the control elements will be checked in other way later
        for (int i = 0; i < names.Count; i++)
        {
            var sd = (StatusData) finalSettingdt.Rows[0][names[i]];

            if (sd.Group == null)//check if the excel cell has group value 
            {
                if (SetScenes.ScriptComponents[names[i]].Status != sd.Status)
                {
                    Debug.Log(string.Format("the component: {0} should be switched to: {1}", names[i], sd.Status));

                    #region store the actuators that are wrong

                    isFinalSettingCorrect = false;
                    WrongAction wrongAction = new WrongAction();
                    wrongAction.Initialize();
                    wrongAction.WrongActuator = GameObject.Find(names[i]);
                    wrongAction.Status = SetScenes.ScriptComponents[names[i]].Status;
                    wrongAction.ExpectedStatus = (int)sd.Status;
                    StudentOperations.WrongFinalActuators.Add(wrongAction);

                    #endregion
                }

            }
            else if(!finalSettingGroupIndex.Contains(sd.Group))//grab the group index if it has not been spotted
            {
                finalSettingGroupIndex.Add(sd.Group);
            }

        }
        CheckGroupsStatusesFinalSetting(finalSettingdt); //check the final settings of the groups of actuators


        #region check final settings of the parameters
        Debug.Log("I start to check final parameters");
        int columnFlag = 0;
        bool isSubtaskFound = false;
        for (int j = 0; j < NPOIGetParameters.parameterTable.Rows.Count; j++)
        {
            
            for (int i = 0; i < NPOIGetParameters.parameterTable.Columns.Count; i++)
            {
                Debug.Log("PanelManager.listToggleText[2]:::::" + PanelManager.listToggleText[2]);
                if (NPOIGetParameters.parameterTable.Rows[j][i].ToString().Contains(PanelManager.listToggleText[2]) )
                {
                    isSubtaskFound = true;
                    columnFlag = i;
                    Debug.Log("parameterTable sub task: " + NPOIGetParameters.parameterTable.Rows[j][i]);
                    Debug.Log("NPOIGetParameters.parameterTable.Columns[i].ColumnName.ToString(): " + NPOIGetParameters.parameterTable.Columns[i+1].ColumnName.ToString());
                    //the control elements columns must be after the sub task column in the excel
                    //if (i > columnFlag)
                    //{
                        CheckFinalParameters(NPOIGetParameters.parameterTable.Columns[i+1].ColumnName.ToString());
                        Debug.Log("I checked final parameters");
                    //}

                }
            }

            if (isSubtaskFound)
            {
                break;
            }
        }

        #endregion

        TrialCountAndTaskCount.CountTrialsAndTasks();
        //Disable the actuators if the actions are not correct during the final setting check 
        //if (!isFinalSettingCorrect)
        //{
        //    DisableInteractions();
        //}
    }

    /// <summary>
    /// check final settings for the groups of actuators
    /// </summary>
    /// <param name="dt"></param>
    private static void CheckGroupsStatusesFinalSetting(DataTable dt)
    {
        finalSettingComponentsGroup.Clear();
        Debug.Log("final setting group count: " + finalSettingGroupIndex.Count);
        for (int j = 0; j < finalSettingGroupIndex.Count; j++)
        {
            char? constraintSymbole = new char();
            int? constraintStatus = 0;
            List<GameObject> myList = new List<GameObject>();

            for (int i = 0; i < NPOIReadExcel.ActuatorNames.Count; i++)
            {
                //Debug.Log("the elements of the Red start: " +  (StatusData)dr[columnTitle].Status);
                StatusData sd = (StatusData)finalSettingdt.Rows[0][NPOIReadExcel.ActuatorNames[i]]; //get the elements of a column 

                if (sd.Group == finalSettingGroupIndex[j])
                {
                    myList.Add(GameObject.Find(NPOIReadExcel.ActuatorNames[i]));
                    constraintSymbole = sd.Symbol;
                    constraintStatus = sd.Status;
                }
            }
            finalSettingComponentsGroup.Add((char)finalSettingGroupIndex[j], myList); //sort and conclude the group indexes and the corresponding actuator names 
            var groupSum = 0;
            for (int k = 0; k < finalSettingComponentsGroup[(char)finalSettingGroupIndex[j]].Count; k++)
            {
                var componentStatus = finalSettingComponentsGroup[(char)finalSettingGroupIndex[j]][k].GetComponent<IGeneratorComponent>().Status;
                groupSum += componentStatus; //compute the sum of the group of actuators.
            }
            Debug.Log("groupSum: " + groupSum);
            Debug.Log("constraintStatus: " + constraintStatus);


            #region Deal with  different symbols("+","-" and "=")

            if (constraintSymbole == '+' && groupSum < constraintStatus)
            {
                Debug.Log(string.Format("the component of group: {0} should be at least: {1}", finalSettingGroupIndex[j], constraintStatus));
                for(int i = 0; i < finalSettingComponentsGroup[(char)finalSettingGroupIndex[j]].Count; i++) {
                    WrongAction wrongAction = new WrongAction();
                    wrongAction.WrongActuator = finalSettingComponentsGroup[(char)finalSettingGroupIndex[j]][i];
                    wrongAction.ExpectedStatus = (int)constraintStatus;
                    wrongAction.Status = finalSettingComponentsGroup[(char)finalSettingGroupIndex[j]][i]
                        .GetComponentInChildren<IGeneratorComponent>().Status;
                    wrongAction.Group = (char)finalSettingGroupIndex[j];
                    wrongAction.Symbol = constraintSymbole;
                    StudentOperations.WrongFinalActuators.Add(wrongAction);
                }
                isFinalSettingCorrect = false;
            }

            if (constraintSymbole == '-' && groupSum > constraintStatus)
            {
                Debug.Log(string.Format("the component of group: {0} should be at most: {1}", finalSettingGroupIndex[j], constraintStatus));
                for(int i = 0; i < finalSettingComponentsGroup[(char)finalSettingGroupIndex[j]].Count; i++) {
                    WrongAction wrongAction = new WrongAction();
                    wrongAction.WrongActuator = finalSettingComponentsGroup[(char)finalSettingGroupIndex[j]][i];
                    wrongAction.ExpectedStatus = (int)constraintStatus;
                    wrongAction.Status = finalSettingComponentsGroup[(char)finalSettingGroupIndex[j]][i]
                        .GetComponentInChildren<IGeneratorComponent>().Status;
                    wrongAction.Group = (char)finalSettingGroupIndex[j];
                    wrongAction.Symbol = constraintSymbole;
                    StudentOperations.WrongFinalActuators.Add(wrongAction);
                }
                isFinalSettingCorrect = false;
            }

            if (constraintSymbole == '=' && groupSum != constraintStatus)
            {
                Debug.Log(string.Format("the component of group: {0} should be exactly: {1}", finalSettingGroupIndex[j], constraintStatus));
                for(int i = 0; i < finalSettingComponentsGroup[(char)finalSettingGroupIndex[j]].Count; i++) {
                    WrongAction wrongAction = new WrongAction();
                    wrongAction.WrongActuator = finalSettingComponentsGroup[(char)finalSettingGroupIndex[j]][i];
                    wrongAction.ExpectedStatus = (int)constraintStatus;
                    wrongAction.Status = finalSettingComponentsGroup[(char)finalSettingGroupIndex[j]][i]
                        .GetComponentInChildren<IGeneratorComponent>().Status;
                    wrongAction.Group = (char)finalSettingGroupIndex[j];
                    wrongAction.Symbol = constraintSymbole;
                    StudentOperations.WrongFinalActuators.Add(wrongAction);
                }
                isFinalSettingCorrect = false;
            }

            #endregion
            Debug.Log("final setting group elements count: " + finalSettingComponentsGroup[(char)finalSettingGroupIndex[j]].Count);
            Debug.Log("final setting group name: " + (char)finalSettingGroupIndex[j]);

        }
    }

    /// <summary>
    /// reset the game
    /// </summary>
    public void ResetGame()
    {
        isFinalSettingCorrect = false;
        isSequenceCorrect = false;
        isSequenceForTestingCorrect = true;
        alarmAudio.Stop();
        alarm.GetComponentInChildren<MeshRenderer>().material = alarmGreen;
        var names = NPOIGetDatatable.OperableComponentNames;

        actionSequenceActuator.Clear();
        for(int i = 0; i < names.Count; i++)
        {
            //reset actuators blinking 
            var objectClass = GameObject.Find(NPOIGetDatatable.OperableComponentNames[i]).GetComponent<IGeneratorComponent>();
            objectClass.IsNeedCheck = false;
            if (GameObject.Find(NPOIGetDatatable.OperableComponentNames[i]).GetComponent<BlinkMaterial>()!=null)
            {
                var blink = GameObject.Find(NPOIGetDatatable.OperableComponentNames[i]).GetComponent<BlinkMaterial>();
                blink.IsBlink = false;
            }
            //reset actuators material
            SetScenes.ScriptComponents[names[i]].UpdateMaterials();

            actionSequenceActuator.Add(names[i],0); //initialize the dictionary, every actuator is at sequence 0 at the beginning 

        }

        PanelManager.isGeneratorConfigured = false; //this property informs SetScene.cs to set the status of the generator's elements

        //initialize the operation here
        StudentOperations.Initialize();

        //delete the old sequence in the sequence container
        foreach (var child in _sequenceContainer.transform.GetChildren())
        {
            Destroy(child.gameObject);
        }
        CreateSequence();//create the sequence in the scene


    }

    private static bool isCheckCorrect = false;
    //todo implement 8+
    /// <summary>
    /// check sequence for the elements
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="columnTitle"></param>
    public static void GetComponentSequenceChecked(DataTable dt, string columnTitle)
    {
        StudentOperations.ActingAction.WrongSequenceActions.Clear();
        DataRow[] drs = dt.Select(); //get all the rows of the data table
        StudentOperations.ActingAction.IsModificationCorrect = true; //the modification is correct by default, if there is anything wrong, it turns wrong
        var sequenceInCell = (StatusData)drs[0][columnTitle];
        for (int i = 0; i < NPOIGetDatatable.OperableComponentNames.Count; i++) //skip the "sequence" row, and the parameter rows
        {
            StatusData sd = (StatusData)drs[i+1][columnTitle];//get the elements of a column 

            if(sd.Group == null) {

                if(sd.Status.HasValue) {

                    if(drs[i+1].Table.Columns[columnTitle].Ordinal != i)  // for the non diagonal
                    {
                        if(SetScenes.ScriptComponents[NPOIGetDatatable.OperableComponentNames[i]].Status != (int)sd.Status)  //if the status of an actuator is not correct according to the sequence
                        {
                            Debug.Log(string.Format("the component: {0} should be switched to: {1} before modifying {2}", NPOIGetDatatable.OperableComponentNames[i], sd.Status, columnTitle));
                            Debug.Log(string.Format("the component: {0} has status: {1} ", NPOIGetDatatable.OperableComponentNames[i], SetScenes.ScriptComponents[NPOIGetDatatable.OperableComponentNames[i]].Status));
                            isSequenceCorrect = false;
                            isSequenceForTestingCorrect = false;

                            WrongAction wrongAction = new WrongAction();
                            wrongAction.Initialize();
                            wrongAction.WrongActuator = GameObject.Find(NPOIGetDatatable.OperableComponentNames[i]);
                            wrongAction.ExpectedStatus = (int)sd.Status;
                            wrongAction.Status = SetScenes.ScriptComponents[NPOIGetDatatable.OperableComponentNames[i]].Status;
                            wrongAction.Group = null;
                            wrongAction.Symbol = null;
                            StudentOperations.ActingAction.WrongSequenceActions.Add(wrongAction);
                            StudentOperations.ActingAction.IsModificationCorrect = false;

                        }
                        else //everything is correct for non-diagonal elements
                        {
                            Debug.Log(string.Format("the component: {0} has the correct status: {1} before modifying {2}", NPOIGetDatatable.OperableComponentNames[i], sd.Status, columnTitle));
                        }

                    } 

                    else //check for the diagonal
                    {
                        StudentOperations.ActingAction.ExpectedStatus = sd.Status;
                        if(SetScenes.ScriptComponents[NPOIGetDatatable.OperableComponentNames[i]].Status != (int)sd.Status)
                        {
                            StudentOperations.ActingAction.IsModificationCorrect = false;
                            StudentOperations.ActingAction.Group = null;
                            StudentOperations.ActingAction.Symbol = null;
                        }
                    }

                }

            }

            else //add all the different groups to a list 
            {
                //for the actuators belong to a group, they should also have their expected status
                if (drs[i + 1].Table.Columns[columnTitle].Ordinal == i)
                {
                    StudentOperations.ActingAction.ExpectedStatus = sd.Status;  
                }

                if (!sequenceGroupIndex.Contains(sd.Group))
                {
                    sequenceGroupIndex.Add(sd.Group);
                }
            }

        }


        sequenceComponentsGroup.Clear();
        CheckGroupsStatusesSequence(dt, columnTitle);

        isCheckCorrect = StudentOperations.ActingAction.IsModificationCorrect;
        if(sequenceInCell.Status != null && isCheckCorrect) //change the value for the actuator
        {
            actionSequenceActuator.Remove(StudentOperations.ActingAction.Actuator.name);
            actionSequenceActuator.Add(StudentOperations.ActingAction.Actuator.name, (int)sequenceInCell.Status);
        }
    }
    /// <summary>
    /// check sequence of the elements that belong to a group 
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="columnTitle"></param>
    private static void CheckGroupsStatusesSequence(DataTable dt, string columnTitle)
    {
        DataRow[] drs = dt.Select(); //get all the rows of the data table
        for (int j = 0; j < sequenceGroupIndex.Count; j++)
        {
            char? constraintSymbole = new char();
            int? constraintStatus = 0;
            List<GameObject> myList = new List<GameObject>();

            for (int i = 0; i < NPOIGetDatatable.OperableComponentNames.Count; i++)
            {

                StatusData sd = (StatusData)drs[i+1][columnTitle]; //get the elements of a column 

                if (sd.Group == sequenceGroupIndex[j])
                {
                    myList.Add(GameObject.Find(NPOIGetDatatable.OperableComponentNames[i]));
                    constraintSymbole = sd.Symbol;
                    constraintStatus = sd.Status;
                }
            }

            sequenceComponentsGroup.Add((char)sequenceGroupIndex[j], myList);
            //loop inside a group, because for  the components of a group may have different values in excel's cells
            var groupSum = 0;
            for (int k = 0; k < sequenceComponentsGroup[(char)sequenceGroupIndex[j]].Count; k++)
            {
                var componentStatus = sequenceComponentsGroup[(char)sequenceGroupIndex[j]][k].GetComponent<IGeneratorComponent>().Status;
                groupSum += componentStatus;
            }

            if (constraintSymbole == '+' && groupSum < constraintStatus)
            {
                StudentOperations.ActingAction.IsModificationCorrect = false;
                Debug.Log(string.Format("the component of group: {0} should be at least: {1}", sequenceGroupIndex[j], constraintStatus));

                for (int i = 0; i < sequenceComponentsGroup[(char)sequenceGroupIndex[j]].Count; i++)
                {
                    //add the wrong actuators into the WrongAction,'j' is the index for going through a group
                    for(int k = 0; k < sequenceComponentsGroup[(char)sequenceGroupIndex[j]].Count; k++)
                    {

                        WrongAction wrongAction = new WrongAction();
                        wrongAction.WrongActuator = sequenceComponentsGroup[(char)sequenceGroupIndex[j]][k];
                        wrongAction.ExpectedStatus = (int)constraintStatus;
                        wrongAction.Status = sequenceComponentsGroup[(char)sequenceGroupIndex[j]][k]
                            .GetComponentInChildren<IGeneratorComponent>().Status;
                        wrongAction.Group = (char)sequenceGroupIndex[j];
                        wrongAction.Symbol = constraintSymbole;
                        StudentOperations.ActingAction.WrongSequenceActions.Add(wrongAction);
                    }
                    isSequenceCorrect = false;
                    isSequenceForTestingCorrect = false;
                }
            }


            if (constraintSymbole == '-' && groupSum > constraintStatus)
            {
                StudentOperations.ActingAction.IsModificationCorrect = false;
                Debug.Log(string.Format("the component of group: {0} should be at most: {1}", sequenceGroupIndex[j], constraintStatus));
                //TODO blink the group of the component, red and original color
                for (int i = 0; i < sequenceComponentsGroup[(char)sequenceGroupIndex[j]].Count; i++)
                {
                    //add the wrong actuators into the WrongAction,'j' is the index for going through a group
                    for(int k = 0; k < sequenceComponentsGroup[(char)sequenceGroupIndex[j]].Count; k++)
                    {

                        WrongAction wrongAction = new WrongAction();
                        wrongAction.WrongActuator = sequenceComponentsGroup[(char)sequenceGroupIndex[j]][k];
                        wrongAction.ExpectedStatus = (int)constraintStatus;
                        wrongAction.Status = sequenceComponentsGroup[(char)sequenceGroupIndex[j]][k]
                            .GetComponentInChildren<IGeneratorComponent>().Status;
                        wrongAction.Group = (char)sequenceGroupIndex[j];
                        wrongAction.Symbol = constraintSymbole;
                        StudentOperations.ActingAction.WrongSequenceActions.Add(wrongAction);
                    }
                    isSequenceCorrect = false;
                    isSequenceForTestingCorrect = false;
                }
            }

            if (constraintSymbole == '=' && groupSum != constraintStatus)
            {
                StudentOperations.ActingAction.IsModificationCorrect = false;
                Debug.Log(string.Format("the component of group: {0} should be exactly: {1}", sequenceGroupIndex[j], constraintStatus));

                for (int i = 0; i < sequenceComponentsGroup[(char)sequenceGroupIndex[j]].Count; i++)
                {
                    //add the wrong actuators into the WrongAction,'j' is the index for going through a group
                    for(int k = 0; k < sequenceComponentsGroup[(char)sequenceGroupIndex[j]].Count; k++)
                    {

                        WrongAction wrongAction = new WrongAction();
                        wrongAction.WrongActuator = sequenceComponentsGroup[(char)sequenceGroupIndex[j]][k];
                        wrongAction.ExpectedStatus = (int)constraintStatus;
                        wrongAction.Status = sequenceComponentsGroup[(char)sequenceGroupIndex[j]][k]
                            .GetComponentInChildren<IGeneratorComponent>().Status;
                        wrongAction.Group = (char)sequenceGroupIndex[j];
                        wrongAction.Symbol = constraintSymbole;
                        StudentOperations.ActingAction.WrongSequenceActions.Add(wrongAction);
                    }
                    isSequenceCorrect = false;
                    isSequenceForTestingCorrect = false;
                }
            }

        }
    }
    public static void SetTrainingAlarmMatAndAud()
    {
        alarmAudio.volume = 1;
        alarmAudio.loop = true;
        var mat = GameObject.Find("alarm").GetComponentInChildren<MeshRenderer>().material; // pay attention here can not use mat= alamGreen
        if (isSequenceCorrect) //sequence is correct
        {
            GameObject.Find("alarm").GetComponentInChildren<MeshRenderer>().material = alarmGreen; //set alarm lamp's material
            alarmAudio.Stop(); //stop the alarm sound
        }
        if(!isSequenceCorrect || (!isFinalSettingCorrect && SpeechRecognizer.HasCommandFinish)) //sequence is not correct or final setting is not correct
        {
            GameObject.Find("alarm").GetComponentInChildren<MeshRenderer>().material = alarmRed;
            alarmAudio.Play();
        }
    }

    public static void SetTestingAlarmMatAndAud()
    {
        alarmAudio.volume = 1;
        alarmAudio.loop = true;

        var mat = GameObject.Find("alarm").GetComponentInChildren<MeshRenderer>().material; // pay attention here can not use mat= alamGreen
        if (isSequenceForTestingCorrect && isFinalSettingCorrect) //if everyting is correct
        {
            GameObject.Find("alarm").GetComponentInChildren<MeshRenderer>().material = alarmGreen;
            alarmAudio.Stop();
        }
        else/*if(isSequenceForTestingCorrect && !isFinalSettingCorrect) *///if sequence is correct but the final setting is incorrect
        {
            GameObject.Find("alarm").GetComponentInChildren<MeshRenderer>().material = alarmRed;
            alarmAudio.Play();


            //feedbackPrompt.GetComponent<Text>().text = encourageText;
            //instance.StartCoroutine(ShowMessage(10));
            //ShowMessage();
        }

        
    }
    /// <summary>
    /// a method only for seeing if the operations are correctly stored
    /// </summary>
    public static void ReplayOperations()
    {
        Debug.Log("the count of the operations: " + StudentOperations.AllActions.Count);
        Debug.Log("the operations of the acting actuator: " + StudentOperations.ActingAction.Actuator.name);
        for (int i = 0; i < StudentOperations.AllActions.Count; i++)
        {

            Debug.Log("entering the operations************************************");
            Debug.Log("the operations of the student actuator "+ StudentOperations.AllActions[i].Actuator);
            Debug.Log("the operations of the acted actuator status before" + StudentOperations.AllActions[i].StatusBefore);
            Debug.Log("the operations of the acted actuator status after" + StudentOperations.AllActions[i].StatusAfter);
            Debug.Log("the operations of the acted actuator IsModificationCorrect" + StudentOperations.AllActions[i].IsModificationCorrect);
            Debug.Log("the operations of the acted actuator ExpectedStatus" + StudentOperations.AllActions[i].ExpectedStatus);
            if (!StudentOperations.AllActions[i].IsModificationCorrect)
            {
                Debug.Log("StudentOperations.AllActions[i].WrongSequenceActions another operated actuator: "+ StudentOperations.AllActions[i].Actuator.name);
                Debug.Log("StudentOperations.AllActions[i].WrongSequenceActions another operated actuator: ");
                for (int j = 0; j < StudentOperations.AllActions[i].WrongSequenceActions.Count; j++)
                {
                    Debug.Log("StudentOperations.AllActions[i].WrongSequenceActions.WrongActuator: " +
                              StudentOperations.AllActions[i].WrongSequenceActions[j].WrongActuator);
                    Debug.Log("StudentOperations.AllActions[i].WrongSequenceActions.ExpectedStatus: " +
                              StudentOperations.AllActions[i].WrongSequenceActions[j].ExpectedStatus);
                }
            }
            Debug.Log("entered the operations******************************************");
        }
    }

    //Create the classes to store student's actions
    public class Operations
    {
        public List<AnAction> AllActions; //all the actions
        public AnAction ActingAction;//the current action
        public AnAction LastAction;
        //public List<WrongAction> WrongSequenceActions; //all the wrong actuators, maybe not be put here
        public List<WrongAction> WrongFinalActuators; //all the wrongly operated actuators

        public void Initialize()
        {
            AnAction actingActuator = new AnAction();
            AnAction lastActuator = new AnAction();
            actingActuator.Initialize();
            lastActuator.Initialize();
            List<WrongAction> wrongStatusActuator = new List<WrongAction>();
            List<AnAction> operatedActuators = new List<AnAction>();

            this.ActingAction = actingActuator;
            this.LastAction = lastActuator;
            this.AllActions= operatedActuators;
            //this.AllActions.Add(lastActuator);
            this.WrongFinalActuators= wrongStatusActuator;
        }
    }

    public class AnAction
    {
        public GameObject Actuator { get; set; }   //the actuator
        //public string ActuatorName { get; set; }   //the actuator name
        public int StatusBefore { get; set; }       //the status before the modification
        public int StatusAfter { get; set; }        //the status after the modification
        public int? ExpectedStatus { get; set; }     //the expected status according to the excel file sequence sheet
        public bool IsModificationCorrect { get; set; }     //the assessment of the operation by comparing in the sequence sheet
        public char? Group { get; set; }        //Not sure this prop is needed
        public char? Symbol { get; set; }       //Not sure this prop is needed
        //imp Should I put WrongSequenceActions to here instead of operations?
        public List<WrongAction> WrongSequenceActions; //all the wrong actuators

        public void Initialize() {
            //GameObject actuator = new GameObject("inside the simple action's initialization");
            //ActuatorName = "";
            StatusBefore = 0;     //the status before the modification
            StatusAfter = 0;        //the status after the modification
            ExpectedStatus = -1;     //the expected status according to the excel file sequence sheet
            IsModificationCorrect = true;    //the assessment of the operation by comparing in the sequence sheet
            Group = null;        //Not sure this prop is needed
            Symbol = null;
            List<WrongAction> wrongSequenceActuators = new List<WrongAction>();
            this.WrongSequenceActions = wrongSequenceActuators;
        }
    }
    public class WrongAction
    {
        public GameObject WrongActuator { get; set; }   //the actuator
        public int Status{ get; set; }       //the status of the actuator
        public int ExpectedStatus { get; set; }     //the expected status according to the excel file sequence sheet
        public char? Group { get; set; }
        public char? Symbol { get; set; }

        public void Initialize() {
            //GameObject actuator = new GameObject("inside the simple action's initialization");
            //ActuatorName = "";
            Status = 0;
            ExpectedStatus = 10;     //the expected status according to the excel file sequence sheet
            Group = null;        //Not sure this prop is needed
            Symbol = null;
        }
    }

    public void BlinkTrainingActuatorSequence()
    {
        //blink the other prerequisite actuators according to the acting actuator
        foreach(var actuator in StudentOperations.ActingAction.WrongSequenceActions) {
            actuator.WrongActuator.gameObject.GetComponent<BlinkMaterial>().enabled = true;
            actuator.WrongActuator.gameObject.GetComponent<BlinkMaterial>().BlinkMat = actuator.WrongActuator.gameObject.GetComponent<BlinkMaterial>().ErrorAlarmBlinkMat;
            actuator.WrongActuator.gameObject.GetComponent<BlinkMaterial>().IsBlink = true;
        }
        //blink the acting actuator if it is wrong
        if(!StudentOperations.ActingAction.IsModificationCorrect) {
            StudentOperations.ActingAction.Actuator.GetComponent<BlinkMaterial>().BlinkMat = StudentOperations.ActingAction.Actuator.GetComponent<BlinkMaterial>().FirstErrorActuatorBlinkMat;
            StudentOperations.ActingAction.Actuator.GetComponent<BlinkMaterial>().IsBlink = true;
        }
        //set back the material if it is correct
        else {
            Debug.Log("I update the original material for the acting actuator");
            StudentOperations.ActingAction.Actuator.GetComponent<IGeneratorComponent>().UpdateMaterials();
        }

    }

    public void BlinkActuatorFinalSetting()
    {
        //blink the actuator according to the final setting check
        if (SpeechRecognizer.HasCommandFinish)
        {
            foreach (var actuator in StudentOperations.WrongFinalActuators)
            {
                actuator.WrongActuator.gameObject.GetComponent<BlinkMaterial>().enabled = true;
                actuator.WrongActuator.gameObject.GetComponent<BlinkMaterial>().BlinkMat = actuator.WrongActuator.gameObject.GetComponent<BlinkMaterial>().ErrorAlarmBlinkMat;
                actuator.WrongActuator.gameObject.GetComponent<BlinkMaterial>().IsBlink = true;
            }

        }
    }

    public void BlinkTestingActuatorSequence()
    {
        //find the first error committed
        for (int i = 0; i < StudentOperations.AllActions.Count; i++)
        {
            if (!StudentOperations.AllActions[i].IsModificationCorrect)
            {
                //blink the action actuator that lead to the wrong sequence
                StudentOperations.AllActions[i].Actuator.GetComponent<BlinkMaterial>().enabled = true;
                StudentOperations.AllActions[i].Actuator.GetComponent<BlinkMaterial>().BlinkMat = StudentOperations.AllActions[i].Actuator.GetComponent<BlinkMaterial>().FirstErrorActuatorBlinkMat;
                StudentOperations.AllActions[i].Actuator.GetComponent<BlinkMaterial>().IsBlink = true;
                //blink every wrong actuator
                Debug.Log("color the wrong actuator itself: " + StudentOperations.AllActions[i].Actuator);
                foreach (var wrongStatusActuator in StudentOperations.AllActions[i].WrongSequenceActions)
                {
                    Debug.Log("color the wrong prerequisites actuators: " + wrongStatusActuator.WrongActuator);
                    wrongStatusActuator.WrongActuator.GetComponent<BlinkMaterial>().enabled = true;
                    wrongStatusActuator.WrongActuator.GetComponent<BlinkMaterial>().BlinkMat = wrongStatusActuator.WrongActuator.GetComponent<BlinkMaterial>().ErrorAlarmBlinkMat;
                    wrongStatusActuator.WrongActuator.GetComponent<BlinkMaterial>().IsBlink = true;
                }
                break;
            }            
        }
        
    }

    /// <summary>
    /// create the sequence order numbers on the actuators
    /// </summary>
    private void CreateSequence()
    {
        //get the sequence numbers and create texts.
        for (int i = 0; i < NPOIGetDatatable.SequenceDatatables.Count; i++)
        {

            if (NPOIGetDatatable.SequenceDatatables[i].TableName.Contains(PanelManager.listToggleText[2]))
            {
                _sequenceContainer.transform.position = Vector3.zero;
                var sequenceTable = NPOIGetDatatable.SequenceDatatables[i]; //get the correct sequence table
                var datarows = sequenceTable.Select();
                for (int j = 0; j < sequenceTable.Columns.Count; j++)
                {
                    StatusData value = (StatusData) datarows[0][j]; //the first row in the datatable is the sequence row
                    GameObject sequenceNumber = new GameObject("Sequence "+sequenceTable.Columns[j].ColumnName); //create the text objects
                    sequenceNumber.transform.position = GameObject.Find(sequenceTable.Columns[j].ColumnName).transform.position;//set the text to the same position of the actuator
                    sequenceNumber.transform.localScale = new Vector3(0.003f, 0.003f, 0.003f);
                    sequenceNumber.transform.eulerAngles = new Vector3(0, 180, 0);
                    sequenceNumber.transform.SetParent(_sequenceContainer.transform);
                    sequenceNumber.AddComponent<MeshRenderer>();
                    var textMesh = sequenceNumber.AddComponent<TextMesh>();
                    textMesh.text = value.Status.ToString(); //the sequence row is not status data but we read them from the excel as they are status data
                    textMesh.anchor = TextAnchor.UpperCenter;
                    textMesh.fontSize = 300;
                    textMesh.color = Color.red;
                }
                _sequenceContainer.SetActive(false);
            }
        }
    }

    public static void VisualizeSequence()
    {
        if (PanelManager.listToggleText[1] == "Training" || PanelManager.listToggleText[1] == "Apprendimento")
        {
            _sequenceContainer.SetActive(true);
        }
    }

    public static void HideSequence()
    {
        if (PanelManager.listToggleText[1] == "Training" || PanelManager.listToggleText[1] == "Apprendimento")
        {
            _sequenceContainer.SetActive(false);
        }
    }

    /// <summary>
    /// check the final settings parameters here with the use of "math parser", the plugin is "info.lundin.Math" 
    /// </summary>
    /// <param name="columnTitle"></param>
    public static void CheckFinalParameters(string columnTitle)
    {
        // Instantiate the parser
        ExpressionParser parser = new ExpressionParser();

        // Create a hashtable to hold values
        Hashtable h = new Hashtable();

        // Add variables and values to hashtable
        Debug.Log("GameObject.Find(columnTitle).GetComponent<IGeneratorComponent>().Status: " + GameObject.Find(columnTitle).GetComponent<IGeneratorComponent>().Status);
        Debug.Log("PanelManager.listToggleText[2]: " + PanelManager.listToggleText[2]);
        h.Add("x", GameObject.Find(columnTitle).GetComponent<IGeneratorComponent>().Status.ToString());

        // Parse and write the result
        for (int i = 0; i < NPOIGetParameters.parameterTable.Rows.Count; i++)
        {
            if (NPOIGetParameters.parameterTable.Rows[i]["Sub task"].ToString().Contains(PanelManager.listToggleText[2])) //find the correct row by using the task
            {
                Debug.Log("NPOIGetParameters.parameterTable.Rows[i][columnTitle]: " + NPOIGetParameters.parameterTable.Rows[i][columnTitle]);
                double result = parser.Parse(NPOIGetParameters.parameterTable.Rows[i][columnTitle].ToString(), h);
                if (result == 0)
                {
                    WrongAction wrongAction = new WrongAction();
                    wrongAction.Initialize();
                    wrongAction.WrongActuator = GameObject.Find(columnTitle);
                    wrongAction.Status = SetScenes.ScriptComponents[columnTitle].Status;
                    StudentOperations.WrongFinalActuators.Add(wrongAction);
                    isFinalSettingCorrect = false;
                }
                Debug.Log("Result: PanelManager.listToggleText[2]: " + PanelManager.listToggleText[2]);
                Debug.Log("Result: NPOIGetParameters.parameterTable.Rows[i][Sub task]: " + NPOIGetParameters.parameterTable.Rows[i]["Sub task"]);

                Debug.Log("Result: " + result);
            }
        }

    }

    //display and hide the feedback panel
    public static /*void*/ IEnumerator ShowMessage(float delay)
    {
        feedbackPrompt.transform.parent.gameObject.SetActive(true);
        yield return new WaitForSeconds(delay);
        feedbackPrompt.transform.parent.gameObject.SetActive(false);
    }

    public static void StartCorrection() //once the system detects an error. all the actions done after the error will be cancelled if the student start correction
    {
        //todo set back the alarm(stop sound, set color to green)
        //todo set the wrong actions back(status and materials), also the actions after the wrong action

        bool ErrorStartFlag=false;
        for (int i = 0; i < StudentOperations.AllActions.Count; i++)
        {
            if (!StudentOperations.AllActions[i].IsModificationCorrect)
            {
                ErrorStartFlag = true;
                
            }

            if (ErrorStartFlag)
            {
                StudentOperations.AllActions[i].Actuator.GetComponentInChildren<IGeneratorComponent>().Status =
                    StudentOperations.AllActions[i].StatusBefore;//cancel all the actions after the wrong action.
            }
        }
        alarm.GetComponentInChildren<MeshRenderer>().material = alarmGreen;
        alarmAudio.Stop();
        StudentOperations = new Operations();
        StudentOperations.Initialize();
        //isFinalSettingCorrect = true;
        isSequenceForTestingCorrect = true;
    }

    /// <summary>
    /// disable the interactions when the student does some wrong actions and confirm to finish the task
    /// </summary>
    static void DisableInteractions()
    {
        for (int i = 0; i < NPOIGetDatatable.OperableComponentNames.Count; i++)
        {

            GameObject.Find(NPOIGetDatatable.OperableComponentNames[i]).GetComponentInChildren<Collider>().enabled= false;

        }
    }

    /// <summary>
    /// enable the interactions when the student choose the restart the task, correct the actions or quit. So this method should be subscribed to these buttons 
    /// </summary>
    public void ResumeInteractions()
    {
        for (int i = 0; i < NPOIGetDatatable.OperableComponentNames.Count; i++)
        {

            GameObject.Find(NPOIGetDatatable.OperableComponentNames[i]).GetComponentInChildren<Collider>().enabled = true;

        }
    }

    /// <summary>
    /// subscribe to the "quit" button so that the student can turn the actuators without triggering the alarm
    /// </summary>
    public void QuitGame()
    {
        ResetGame();
        PanelManager.isGeneratorConfigured = true;
        isFinalSettingCorrect = true;
    }

}
