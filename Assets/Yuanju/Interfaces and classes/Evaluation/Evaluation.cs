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
using Assets.Yuanju.Interfaces_and_classes.generator_components;

public class Evaluation : MonoBehaviour
{
    private Dictionary<string, IGeneratorComponent> scriptComponents;
    public static List<string> previousOperatedComponents=new List<string>(); //use a list of string to store the modified components
    public static GameObject LastOperatedComponent;
    public static GameObject CurrentOperatedComponent;
    public static bool isFinalSettingCorrect = false;
    //public static DataTable finalSettingdt = new DataTable();
    private static ElementSettings settingToBeChecked;
    static List<char?> finalSettingGroupIndex = new List<char?>();
    static Dictionary<char, List<GameObject>> finalSettingComponentsGroup = new Dictionary<char, List<GameObject>>();  //eg. group a has resistance switch 1, 2, 3 and 4
    static Dictionary<char, List<GameObject>> sequenceComponentsGroup = new Dictionary<char, List<GameObject>>();
    static List<char?> sequenceGroupIndex = new List<char?>(); //a list to hold the group indentifier of the elements, one group has on identifier
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
    //private static Dictionary<string, int> actionSequenceActuator;
    public static GameObject feedbackPrompt;
    public GameObject FeedbackPrompt;
    public static Evaluation instance;
    public Evaluation Instance;
    public static string congratulationText;
    public string CongradulationText;
    public static string encourageText;
    public string EncourageText;
    private static bool feedbackTextUpdated;

    public float OffsetSequenceNumber;
    private static bool isCheckCorrect = false;

    List<Sequence> matricesToCheck = new List<Sequence>();

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
        _sequenceContainer = new GameObject("Sequence Container");
        //CreateSequence();
        //actionSequenceActuator = new Dictionary<string, int>();
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

    //private DataTable theCheckingTable;
    //private int sequenceTableIndex;
    //private bool IsActedTwice = false;
    /// <summary>
    /// preparations for sequence check: find the correct table 
    private void CheckSequence()
    {
        isSequenceCorrect = true;
        //isFinalSettingCorrect = false;
        //get column in the data table of the last modified component
        if (StudentOperations.ActingAction != null && StudentOperations.ActingAction.StatusBefore != StudentOperations.ActingAction.StatusAfter) //this condition can be removed since we put this condition in the object class
        {
            StudentOperations.AllActions.Add(StudentOperations.ActingAction); //add the acting actuator into the actions list
        }

        //DataTable theExacTableForSequence = new DataTable();
        ActuatorConditions theExactMatrixForSequence = new ActuatorConditions();

        //for (int i = 0; i < SteamGenerator.SequenceMatrices.Count /*NPOIGetSequenceTable.SequenceDatatables.Count*/; i++) 
        //{
        //if(SteamGenerator.SequenceMatrices[i].SubTask/*NPOIGetSequenceTable.SequenceDatatables[i].TableName*/.Contains(PanelManager.listToggleText[2])) //find the corresponding data table by comparing the text of the selected task toggle with the task in the sequence data tables
        //{
        for (int i = 0; i < matricesToCheck.Count; i++)
        {
            //var actuatorCondition = matricesToCheck[i];
            //var drs = dt.Select();
            //var cellValue = (StatusData)drs[0][StudentOperations.ActingAction.Actuator.name];
            StatusData sd = new StatusData();
            for (int j = 0; j < matricesToCheck[i].ActuatorToCheck.Count; j++)
            {
                if (matricesToCheck[i].ActuatorToCheck[j].Name == StudentOperations.ActingAction.Actuator.name)
                {
                    int count = 0; //the time of the actuator appears discretely in the action list
                    //check the action list
                    int index = StudentOperations.AllActions.Count - 1;

                    for (int k = StudentOperations.AllActions.Count - 1; k > 0; k--)
                    {
                        if (StudentOperations.AllActions[k].Actuator.name == StudentOperations.AllActions[index].Actuator.name)
                        {
                            if (index - k > 1)
                            {
                                count++;
                            }
                            index = k;
                        }
                    }
                    Debug.Log("checking the current matrix: " + matricesToCheck[count].SubTask);
                    GetComponentSequenceChecked(matricesToCheck[count], StudentOperations.ActingAction.Actuator.name);//check the sequence for the acting actuator
                    if(!StudentOperations.ActingAction.IsModificationCorrect && matricesToCheck.Count - 1 > count ) 
                    {
                        Debug.Log("checking the next matrix: " + matricesToCheck[count + 1].SubTask);
                        for (int l = 0; l < matricesToCheck[count + 1].ActuatorToCheck.Count; l++)
                        {
                            if (matricesToCheck[count + 1].ActuatorToCheck[l].Name == StudentOperations.ActingAction.Actuator.name)
                            {
                                if (matricesToCheck[count + 1].ActuatorToCheck[l].SequenceOrder != null) 
                                {
                                    GetComponentSequenceChecked(matricesToCheck[count + 1], StudentOperations.ActingAction.Actuator.name);
                                }
                            }
                        }

                    }
                    if(!StudentOperations.ActingAction.IsModificationCorrect) {
                        Debug.Log("checking the previous matrix: " + matricesToCheck[count].SubTask);
                        for (int l = 0; l < matricesToCheck[count + 1].ActuatorToCheck.Count; l++)
                        {
                            if (matricesToCheck[count + 1].ActuatorToCheck[l].Name == StudentOperations.ActingAction.Actuator.name)
                            {
                                if (matricesToCheck[count + 1].ActuatorToCheck[l].SequenceOrder != null)
                                {
                                    GetComponentSequenceChecked(matricesToCheck[count + 1], StudentOperations.ActingAction.Actuator.name);
                                }
                            }
                        }
                    }
                }
            }
        }

                     
            //}
        //}

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
        for (int i = 0; i < SteamGenerator.FinalSettings.Count; i++)
        {
            if (SteamGenerator.FinalSettings[i].SubTask.Contains(PanelManager.listToggleText[2]))
            {
                settingToBeChecked = SteamGenerator.FinalSettings[i];  //find the final setting data table that will be used for the checking. It is not necessary to use the copy of the data table
            }
        }

        var names = SteamGenerator.FinalSettingElements; //only the actuators will be checked, the control elements will be checked in other way later
        for (int i = 0; i < names.Count; i++)
        {
            var sd = settingToBeChecked.ElementStatus[names[i]];


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
        CheckGroupsStatusesFinalSetting(); //check the final settings of the groups of actuators


        #region check final settings of the parameters
        Debug.Log("I start to check final parameters");
        int columnFlag = 0;
        bool isSubtaskFound = false;
        for (int j = 0; j < SteamGenerator.FinalParametersSetting.Count; j++)
        {
            if (SteamGenerator.FinalParametersSetting[j].SubTask.Contains(PanelManager.listToggleText[2]))
            {
                isSubtaskFound = true;
                //foreach (var key in SteamGenerator.FinalParametersSetting[j].ParameterSetting.Keys)
                //{
                    CheckFinalParameters(SteamGenerator.FinalParametersSetting[j]/*SteamGenerator.FinalParametersSetting[j].ParameterSetting[key]*/);
                //}

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
    private static void CheckGroupsStatusesFinalSetting()
    {
        finalSettingComponentsGroup.Clear();
        for (int j = 0; j < finalSettingGroupIndex.Count; j++)
        {
            char? constraintSymbole = new char();
            int? constraintStatus = 0;
            List<GameObject> myList = new List<GameObject>();

            for (int i = 0; i < SteamGenerator.FinalSettingElements.Count; i++)
            {
                //Debug.Log("the elements of the Red start: " +  (StatusData)dr[columnTitle].Status);
                StatusData sd = (StatusData)settingToBeChecked.ElementStatus[SteamGenerator.FinalSettingElements[i]]; //get the elements of a column 

                if (sd.Group == finalSettingGroupIndex[j])
                {
                    myList.Add(GameObject.Find(SteamGenerator.FinalSettingElements[i]));
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

            #region Deal with  different symbols("+","-" and "=")

            if (constraintSymbole == '+' && groupSum < constraintStatus)
            {
                //Debug.Log(string.Format("the component of group: {0} should be at least: {1}", finalSettingGroupIndex[j], constraintStatus));
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
                //Debug.Log(string.Format("the component of group: {0} should be at most: {1}", finalSettingGroupIndex[j], constraintStatus));
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
            //Debug.Log("final setting group elements count: " + finalSettingComponentsGroup[(char)finalSettingGroupIndex[j]].Count);
            //Debug.Log("final setting group name: " + (char)finalSettingGroupIndex[j]);

        }
    }

    //List<DataTable> sequenceTableToCheck = new List<DataTable>(); //the sequence tables that will be checked for a specific task
    /// <summary>
    /// reset the game
    /// </summary>
    public void ResetGame()
    {
        if(PanelManager.listToggleText[2] == "") {
            PanelManager.listToggleText[2] = "Activation";
        }

        isFinalSettingCorrect = false;
        isSequenceCorrect = false;
        isSequenceForTestingCorrect = true;
        alarmAudio.Stop();
        alarm.GetComponentInChildren<MeshRenderer>().material = alarmGreen;
        //var names = NPOIGetSequenceTable.OperableComponentNames;
        var elementNames = SteamGenerator.SequenceElements;

        //actionSequenceActuator.Clear();
        for(int i = 0; i < elementNames.Count()/*names.Count*/; i++)
        {
            //reset actuators blinking 
            var objectClass = GameObject.Find(elementNames.ElementAt(i)/*NPOIGetSequenceTable.OperableComponentNames[i]*/).GetComponent<IGeneratorComponent>();
            objectClass.IsNeedCheck = false;
            if (GameObject.Find(elementNames.ElementAt(i)/*NPOIGetSequenceTable.OperableComponentNames[i]*/).GetComponent<BlinkMaterial>()!=null)
            {
                var blink = GameObject.Find(elementNames.ElementAt(i)/*NPOIGetSequenceTable.OperableComponentNames[i]*/).GetComponent<BlinkMaterial>();
                blink.IsBlink = false;
            }
            //reset actuators material
            SetScenes.ScriptComponents[elementNames.ElementAt(i)/*names[i]*/].UpdateMaterials();

            //actionSequenceActuator.Add(elementNames.ElementAt(i)/*names[i]*/, 0); //initialize the dictionary, every actuator is at sequence 0 at the beginning 

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

        //sequenceTableToCheck.Clear();
        //get the matrices of the task to be checked
        for (int i = 0; i < SteamGenerator.SequenceMatrices.Count /*NPOIGetSequenceTable.SequenceDatatables.Count*/; i++)
        {
            if (SteamGenerator.SequenceMatrices[i].SubTask/*NPOIGetSequenceTable.SequenceDatatables[i].TableName*/.Contains(PanelManager.listToggleText[2])) //find the corresponding data table by comparing the text of the selected task toggle with the task in the sequence data tables
            {
                matricesToCheck.Add(SteamGenerator.SequenceMatrices[i]);
            }
        }

        //theCheckingTable = null;
        //sequenceTableIndex = 0;
        //IsActedTwice = false;
    }

    /// <summary>
    /// check sequence for the elements
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="columnTitle"></param>
    public static void GetComponentSequenceChecked(Sequence sequence, string columnTitle)
    {
        StudentOperations.ActingAction.WrongSequenceActions.Clear();
        //DataRow[] drs = dt.Select(); //get all the rows of the data table
        StudentOperations.ActingAction.IsModificationCorrect = true; //the modification is correct by default, if there is anything wrong, it turns wrong
        for (int i = 0; i < sequence.ActuatorToCheck.Count()/*NPOIGetSequenceTable.OperableComponentNames.Count*/; i++) //skip the "sequence" row, and the parameter rows
        {
            StatusData sd;
            //StatusData sd = (StatusData)drs[i+1][columnTitle];//get the elements of a column 
            if (sequence.ActuatorToCheck[i].Name == columnTitle)
            {

                for (int j = 0; j < sequence.ActuatorToCheck[i].Prerequisites.Count; j++)
                {
                    sd = sequence.ActuatorToCheck[i].Prerequisites[j];
                    Debug.Log("sequence.SubTask: " + sequence.SubTask);

                    if(sd.Group == null)
                    {
                        if(sd.Status.HasValue)
                        {
                            if(sd.Symbol == null)
                            {
                                if(sequence.ActuatorToCheck[i].Prerequisites[j].Name != columnTitle/*drs[i + 1].Table.Columns[columnTitle].Ordinal != i*/)  // for the non diagonal
                                {
                                    if(SetScenes.ScriptComponents[sequence.ActuatorToCheck[i].Prerequisites[j].Name/*NPOIGetSequenceTable.OperableComponentNames[i]*/].Status != (int)sd.Status)  //if the status of an actuator is not correct according to the sequence
                                    {
                                        //Debug.Log(string.Format("the component: {0} should be switched to: {1} before modifying {2}", NPOIGetSequenceTable.OperableComponentNames[i], sd.Status, columnTitle));
                                        //Debug.Log(string.Format("the component: {0} has status: {1} ", NPOIGetSequenceTable.OperableComponentNames[i], SetScenes.ScriptComponents[NPOIGetSequenceTable.OperableComponentNames[i]].Status));
                                        isSequenceCorrect = false;
                                        isSequenceForTestingCorrect = false;

                                        WrongAction wrongAction = new WrongAction();
                                        wrongAction.Initialize();
                                        wrongAction.WrongActuator = GameObject.Find(sequence.ActuatorToCheck[i].Prerequisites[j].Name)/*GameObject.Find(NPOIGetSequenceTable.OperableComponentNames[i])*/;
                                        wrongAction.ExpectedStatus = (int)sd.Status;
                                        wrongAction.Status = SetScenes.ScriptComponents[sequence.ActuatorToCheck[i].Prerequisites[j].Name].Status;/*SetScenes.ScriptComponents[NPOIGetSequenceTable.OperableComponentNames[i]].Status;*/
                                        wrongAction.Group = null;
                                        wrongAction.Symbol = null;
                                        StudentOperations.ActingAction.WrongSequenceActions.Add(wrongAction);
                                        StudentOperations.ActingAction.IsModificationCorrect = false;

                                    }
                                    else //everything is correct for non-diagonal elements
                                    {
                                        //Debug.Log(string.Format("the component: {0} has the correct status: {1} before modifying {2}", NPOIGetSequenceTable.OperableComponentNames[i], sd.Status, columnTitle));
                                    }

                                }

                                else //check for the diagonal
                                {
                                    StudentOperations.ActingAction.ExpectedStatus = sd.Status;
                                    if(SetScenes.ScriptComponents[columnTitle/*SteamGenerator.SequenceElements.ElementAt(j)*//*NPOIGetSequenceTable.OperableComponentNames[i]*/].Status != (int)sd.Status)
                                    {
                                        StudentOperations.ActingAction.IsModificationCorrect = false;
                                        StudentOperations.ActingAction.Group = null;
                                        StudentOperations.ActingAction.Symbol = null;
                                        //Debug.Log("I find a wrong action: " + sequence.ActuatorToCheck[i].Prerequisites[j].Name);

                                    }
                                }

                            }
                            else
                            {
                                if (sequence.ActuatorToCheck[i].Prerequisites[j].Name != columnTitle/*drs[i + 1].Table.Columns[columnTitle].Ordinal != i*/)  // for the non diagonal
                                {

                                    WrongAction wrongAction = new WrongAction();
                                    wrongAction.Initialize();
                                    wrongAction.WrongActuator = GameObject.Find(sequence.ActuatorToCheck[i].Prerequisites[j].Name/*NPOIGetSequenceTable.OperableComponentNames[i]*/);
                                    wrongAction.ExpectedStatus = (int)sd.Status;
                                    wrongAction.Status = SetScenes.ScriptComponents[sequence.ActuatorToCheck[i].Prerequisites[j].Name/*NPOIGetSequenceTable.OperableComponentNames[i]*/].Status;
                                    wrongAction.Group = null;
                                    if (sd.Symbol == '+') //case "+"
                                    {
                                        if (SetScenes.ScriptComponents[sequence.ActuatorToCheck[i].Prerequisites[j].Name/*NPOIGetSequenceTable.OperableComponentNames[i]*/].Status <= (int)sd.Status)   //if the status of an actuator is not correct according to the sequence
                                        {
                                            //Debug.Log(string.Format("the component: {0} should be switched to: {1} before modifying {2}", NPOIGetSequenceTable.OperableComponentNames[i], sd.Status, columnTitle));
                                            //Debug.Log(string.Format("the component: {0} has status: {1} ", NPOIGetSequenceTable.OperableComponentNames[i], SetScenes.ScriptComponents[NPOIGetSequenceTable.OperableComponentNames[i]].Status));
                                            isSequenceCorrect = false;
                                            isSequenceForTestingCorrect = false;
                                            wrongAction.Symbol = '+';
                                            StudentOperations.ActingAction.WrongSequenceActions.Add(wrongAction);
                                            StudentOperations.ActingAction.IsModificationCorrect = false;
                                        }
                                    }
                                    if (sd.Symbol == '-') //case "-"
                                    {
                                        if (SetScenes.ScriptComponents[sequence.ActuatorToCheck[i].Prerequisites[j].Name/*NPOIGetSequenceTable.OperableComponentNames[i]*/].Status >= (int)sd.Status)  //if the status of an actuator is not correct according to the sequence
                                        {
                                            isSequenceCorrect = false;
                                            isSequenceForTestingCorrect = false;
                                            wrongAction.Symbol = '-';
                                            wrongAction.ExpectedStatus = (int)sd.Status;
                                            StudentOperations.ActingAction.WrongSequenceActions.Add(wrongAction);
                                            StudentOperations.ActingAction.IsModificationCorrect = false;
                                        }
                                    }

                                    else //everything is correct for non-diagonal elements
                                    {
                                        //Debug.Log(string.Format("the component: {0} has the correct status: {1} before modifying {2}", NPOIGetSequenceTable.OperableComponentNames[i], sd.Status, columnTitle));
                                    }

                                }

                                else //check for the diagonal
                                {
                                    StudentOperations.ActingAction.ExpectedStatus = sd.Status;
                                    StudentOperations.ActingAction.Group = null;

                                    //if (sd.Symbol == null)
                                    //{
                                    //    if (SetScenes.ScriptComponents[NPOIGetSequenceTable.OperableComponentNames[i]].Status != (int)sd.Status)
                                    //    {
                                    //        StudentOperations.ActingAction.IsModificationCorrect = false;
                                    //        StudentOperations.ActingAction.Symbol = null;
                                    //    }
                                    //}


                                    if (sd.Symbol == '+') //case "+"
                                    {
                                        if (SetScenes.ScriptComponents[sequence.ActuatorToCheck[i].Prerequisites[j].Name/*NPOIGetSequenceTable.OperableComponentNames[i]*/].Status <= (int)sd.Status)   //if the status of an actuator is not correct according to the sequence
                                        {
                                            //Debug.Log(string.Format("the component: {0} should be switched to: {1} before modifying {2}", NPOIGetSequenceTable.OperableComponentNames[i], sd.Status, columnTitle));
                                            //Debug.Log(string.Format("the component: {0} has status: {1} ", NPOIGetSequenceTable.OperableComponentNames[i], SetScenes.ScriptComponents[NPOIGetSequenceTable.OperableComponentNames[i]].Status));
                                            isSequenceCorrect = false;
                                            isSequenceForTestingCorrect = false;
                                            StudentOperations.ActingAction.Symbol = '+';
                                            StudentOperations.ActingAction.IsModificationCorrect = false;
                                        }
                                    }
                                    if (sd.Symbol == '-') //case "-"
                                    {
                                        if (SetScenes.ScriptComponents[sequence.ActuatorToCheck[i].Prerequisites[j].Name/*NPOIGetSequenceTable.OperableComponentNames[i]*/].Status >= (int)sd.Status)  //if the status of an actuator is not correct according to the sequence
                                        {
                                            isSequenceCorrect = false;
                                            isSequenceForTestingCorrect = false;
                                            StudentOperations.ActingAction.Symbol = '-';
                                            StudentOperations.ActingAction.IsModificationCorrect = false;
                                        }
                                    }


                                }



                            }


                        }
                        //Debug.Log("group is null: " + sd.Group);
                        //Debug.Log("group is null, name: " + sd.Name);
                    }

                    else //add all the different groups to a list 
                    {
                        //Debug.Log("group is not null: " +sd.Group);
                        //for the actuators belong to a group, they should also have their expected status
                        if (sequence.ActuatorToCheck[i].Prerequisites[j].Name == columnTitle/*drs[i + 1].Table.Columns[columnTitle].Ordinal == i*/)
                        {
                            StudentOperations.ActingAction.ExpectedStatus = sd.Status;
                        }

                        if (!sequenceGroupIndex.Contains(sd.Group))
                        {
                            sequenceGroupIndex.Add(sd.Group);
                            //Debug.Log("Adding group: " + sequenceGroupIndex[0]);
                        }
                    }

                }
            }
            //StatusData sd = (StatusData)sequence[columnTitle];//get the elements of a column 
        }


        sequenceComponentsGroup.Clear();
        CheckGroupsStatusesSequence(sequence, columnTitle);



        isCheckCorrect = StudentOperations.ActingAction.IsModificationCorrect;


        //for (int i = 0; i < sequence.ActuatorToCheck.Count()/*NPOIGetSequenceTable.OperableComponentNames.Count*/; i++) //skip the "sequence" row, and the parameter rows
        //{
        //    if (sequence.ActuatorToCheck[i].Name == columnTitle)
        //    {
        //        if (isCheckCorrect)
        //        {
        //            actionSequenceActuator.Remove(StudentOperations.ActingAction.Actuator.name);
        //            actionSequenceActuator.Add(StudentOperations.ActingAction.Actuator.name, sequence.ActuatorToCheck[i].SequenceOrder);
        //        }
        //    }
        //}
    }
    /// <summary>
    /// check sequence of the elements that belong to a group 
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="columnTitle"></param>
    private static void CheckGroupsStatusesSequence(Sequence sequenceMatrix/*DataTable dt*/, string columnTitle)
    {
        //DataRow[] drs = dt.Select(); //get all the rows of the data table
        for (int j = 0; j < sequenceGroupIndex.Count; j++)
        {
            char? constraintSymbole = new char();
            int? constraintStatus = 0;
            List<GameObject> myList = new List<GameObject>();

            for (int i = 0; i < sequenceMatrix.ActuatorToCheck.Count; i++)
            {
                StatusData sd = new StatusData();
                if (sequenceMatrix.ActuatorToCheck[i].Name == columnTitle)//find the element to check now
                {
                    for (int k = 0; k < sequenceMatrix.ActuatorToCheck[i].Prerequisites.Count; k++)
                    {
                        sd = (StatusData)sequenceMatrix.ActuatorToCheck[i].Prerequisites[k]; //get the elements of a column 

                        if (sd.Group == sequenceGroupIndex[j]) //in the column, if an element's group is the same as a char stored in list
                        {
                            myList.Add(GameObject.Find(sd.Name/*SteamGenerator.SequenceElements.ElementAt(i)*//*NPOIGetSequenceTable.OperableComponentNames[i]*/));
                            constraintSymbole = sd.Symbol;
                            constraintStatus = sd.Status;
                            //Debug.Log("the group elements: " + sd.Name);
                        }
                    }
                }
                


            }


            sequenceComponentsGroup.Add((char)sequenceGroupIndex[j], myList); //each group indentifier is able to retrieve the elements
            //loop inside a group, because for  the components of a group may have different values in excel's cells
            var groupSum = 0;
            for (int k = 0; k < sequenceComponentsGroup[(char)sequenceGroupIndex[j]].Count; k++)
            {
                var componentStatus = sequenceComponentsGroup[(char)sequenceGroupIndex[j]][k].GetComponent<IGeneratorComponent>().Status;
                groupSum += componentStatus;
            }

            var abc = groupSum < constraintStatus;
            //Debug.Log("groupSum : " + groupSum);
            //Debug.Log("constraintStatus: " + constraintStatus);
            //Debug.Log("groupSum < constraintStatus: " + abc);

            if(constraintSymbole == '+' && groupSum < constraintStatus)
            {
                StudentOperations.ActingAction.IsModificationCorrect = false;
                //Debug.Log(string.Format("the component of group: {0} should be at least: {1}", sequenceGroupIndex[j], constraintStatus));

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
            Debug.Log("isSequenceCorrect in set alarm"+ isSequenceCorrect);
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
        foreach(var actuator in StudentOperations.ActingAction.WrongSequenceActions)
        {
            Debug.Log("actuator.WrongActuator.gameObject: " + actuator.WrongActuator.gameObject);
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
        float offset = 0;
        //get the sequence numbers and create texts.
        for (int i = 0; i < SteamGenerator.SequenceMatrices.Count/*NPOIGetSequenceTable.SequenceDatatables.Count*/; i++)
        {
            if (SteamGenerator.SequenceMatrices[i].SubTask/*NPOIGetSequenceTable.SequenceDatatables[i].TableName*/.Contains(PanelManager.listToggleText[2]))
            {
                _sequenceContainer.transform.position = Vector3.zero;
                //var sequenceTable = NPOIGetSequenceTable.SequenceDatatables[i]; //get the correct sequence table
                for (int k = 0; k < SteamGenerator.SequenceMatrices[i].ActuatorToCheck.Count; k++)
                {
                    GameObject sequenceNumber = new GameObject("Sequence " + SteamGenerator.SequenceMatrices[i].ActuatorToCheck[k].Name); //create the text objects

                    Vector3 position = GameObject.Find(SteamGenerator.SequenceMatrices[i].ActuatorToCheck[k].Name).transform.position + new Vector3(0, -offset, 0);
                    sequenceNumber.transform.position = position;//set the text to the same position of the actuator
                    sequenceNumber.transform.localScale = new Vector3(0.003f, 0.003f, 0.003f);
                    sequenceNumber.transform.eulerAngles = new Vector3(0, 180, 0);
                    sequenceNumber.transform.SetParent(_sequenceContainer.transform);
                    sequenceNumber.AddComponent<MeshRenderer>();
                    var textMesh = sequenceNumber.AddComponent<TextMesh>();
                    textMesh.text = SteamGenerator.SequenceMatrices[i].ActuatorToCheck[k].SequenceOrder.ToString(); //the sequence row is not status data but we read them from the excel as they are status data
                    textMesh.anchor = TextAnchor.UpperCenter;
                    textMesh.fontSize = 300;
                    textMesh.color = Color.red;
                }

                _sequenceContainer.SetActive(false);
                offset += OffsetSequenceNumber;
            }
        }
    }

    public static void VisualizeSequence()
    {
        if (PanelManager.listToggleText[1] == "Training" || PanelManager.listToggleText[1] == "Apprendimento")
        {
            _sequenceContainer.SetActive(true);
        }
        HelpMenuButtons.IsSequenceButtonClicked = true; 
        //this is done because we have to tell the buttons on the help menu that this voice command has been trigged which is equivalent to button has been clicked. 
    }

    public static void HideSequence()
    {
        if (PanelManager.listToggleText[1] == "Training" || PanelManager.listToggleText[1] == "Apprendimento")
        {
            _sequenceContainer.SetActive(false);
        }
        HelpMenuButtons.IsSequenceButtonClicked = false;
        //this is done because we have to tell the buttons on the help menu that this voice command has been trigged which is equivalent to button has been clicked. 
    }

    /// <summary>
    /// check the final settings parameters here with the use of "math parser", the plugin is "info.lundin.Math" 
    /// </summary>
    /// <param name="columnTitle"></param>
    public static void CheckFinalParameters(ElementSettings es/*string columnTitle*/)
    {
        // Instantiate the parser
        ExpressionParser parser = new ExpressionParser();

        // Create a hashtable to hold values
        Hashtable h = new Hashtable();

        foreach (var key in es.ParameterSetting.Keys)
        {
            // Add variables and values to hashtable
            h.Add("x", GameObject.Find(key.ToString()).GetComponent<IGeneratorComponent>().Status.ToString());
            double newResult = parser.Parse(es.ParameterSetting[key], h);
            if (newResult == 0)
            {
                WrongAction wrongAction = new WrongAction();
                wrongAction.Initialize();
                wrongAction.WrongActuator = GameObject.Find(key);
                wrongAction.Status = SetScenes.ScriptComponents[key].Status;
                StudentOperations.WrongFinalActuators.Add(wrongAction);
                isFinalSettingCorrect = false;
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
                Debug.Log("!StudentOperations.AllActions[i].IsModificationCorrect: " + !StudentOperations.AllActions[i].IsModificationCorrect);
                ErrorStartFlag = true;
                for(int j = i + 1; j < StudentOperations.AllActions.Count; j++)
                {
                    if ((StudentOperations.AllActions[i].Actuator == StudentOperations.AllActions[j].Actuator))
                    {
                        if( StudentOperations.AllActions[j].IsModificationCorrect)
                        {
                            //if (isSequenceForTestingCorrect)
                            //{
                                ErrorStartFlag = false;
                                Debug.Log("ErrorStartFlag 0: " + ErrorStartFlag);
                            //}
                        }

                    }
                }
                
                Debug.Log("ErrorStartFlag 1: " + ErrorStartFlag);
            }

            if (ErrorStartFlag)
            {
                Debug.Log("ErrorStartFlag 2: " + ErrorStartFlag);
                StudentOperations.AllActions[i].Actuator.GetComponentInChildren<IGeneratorComponent>().Status =
                    StudentOperations.AllActions[i].StatusBefore;//cancel all the actions after the wrong action.
                Debug.Log("the actions cancelled: " + StudentOperations.AllActions[i].Actuator);
            }
        }
        alarm.GetComponentInChildren<MeshRenderer>().material = alarmGreen;
        alarmAudio.Stop();
        StudentOperations = new Operations();
        StudentOperations.Initialize();
        //isFinalSettingCorrect = true;
        isSequenceForTestingCorrect = true;
    }

    ///// <summary>
    ///// disable the interactions when the student does some wrong actions and confirm to finish the task
    ///// </summary>
    //static void DisableInteractions()
    //{
    //    for (int i = 0; i < NPOIGetSequenceTable.OperableComponentNames.Count; i++)
    //    {

    //        GameObject.Find(NPOIGetSequenceTable.OperableComponentNames[i]).GetComponentInChildren<Collider>().enabled= false;

    //    }
    //}

    ///// <summary>
    ///// enable the interactions when the student choose the restart the task, correct the actions or quit. So this method should be subscribed to these buttons 
    ///// </summary>
    //public void ResumeInteractions()
    //{
    //    for (int i = 0; i < NPOIGetSequenceTable.OperableComponentNames.Count; i++)
    //    {

    //        GameObject.Find(NPOIGetSequenceTable.OperableComponentNames[i]).GetComponentInChildren<Collider>().enabled = true;

    //    }
    //}

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
