using System;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{

    public GameObject menuCanvasPanelEnglish;
    public GameObject OperatorPanelEn;
    public GameObject CertifierPanelEn;
    public GameObject menuCanvasPanelItalian;
    public GameObject OperatorPanelIt;
    public GameObject CertifierPanelIt;
    public GameObject languagePanel;
    public static Dictionary<string, GameObject> menuLanguagesCanvasPanels=new Dictionary<string, GameObject>();

    public static ToggleExtend[] allToggles;//except language toggles
    private ToggleExtend[] languageToggles;//language toggles
    public Text WarningText;
    public static bool isGeneratorConfigured;
    
    //use tags, text and ison to know if at least one toggle of a tag is selected(ison) 
    private List<string> toggleTags;
    public static List<string> listToggleText;
    List<bool> listToggleIson;

    private Dictionary<string, List<string>> userProfileObjectives = new Dictionary<string, List<string>>();
    public List<string> CertifierObjectives;

    //[SerializeField] List<GroupToggle> listToggleGroups;
    [SerializeField] List<ToggleGroup> listGroupsToggle;
    private bool isOptionChanged = false;
    private string previousUserProfileToggleText= "Operator";

    public static string languageOption = "En";
    // Start is called before the first frame update
    void Awake()
    {
        listToggleText = new List<string>()
        {
            "Operator",
            "Training",
            "Activation"
        }; 
        listToggleIson = new List<bool>()
        {
            true,
            true,
            true
        }; 
        toggleTags = new List<string>()
        {
            "User profile toggle",
            "Mode toggle",
            "Objective toggle"
        };
        //listToggle.Add(userProfile,mode,objective);
        //use a dictionary to hold the know corresponding tasks of 
        //TODO needs to add the task in Italian too, it would better that userProfileObjectives has the keys like "OperatorEn"?
        userProfileObjectives.Add("Operator", NPOIReadExcel.SubTasks);
        //TODO should add the certifier objectives manually in the inspector

        userProfileObjectives.Add("Certifier", CertifierObjectives);
        menuLanguagesCanvasPanels.Add("En", menuCanvasPanelEnglish);
        menuLanguagesCanvasPanels.Add("It", menuCanvasPanelItalian);

        languageToggles = languagePanel.GetComponentsInChildren<ToggleExtend>();
        allToggles = menuCanvasPanelEnglish.GetComponentsInChildren<ToggleExtend>(true);
        OperatorPanelEn.SetActive(true);
        CertifierPanelEn.SetActive(false);
        OperatorPanelIt.SetActive(false);
        CertifierPanelIt.SetActive(false);
        languagePanel.SetActive(false);
        isGeneratorConfigured = true; //here we can initalize the generator if we want.
        //initialize the interface
        foreach (GameObject value in menuLanguagesCanvasPanels.Values)
        {
            value.SetActive(false);
        }

    }

    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        //for(int i = 0; i < listToggleText.Count; i++) {
        //    Debug.Log("I'm checking the toggless text: " + listToggleText[i]);
        //}

        //detect if the language languageOption is changed
        if (isOptionChanged || listToggleText[0]!= previousUserProfileToggleText)
        {
            allToggles = menuLanguagesCanvasPanels[languageOption].GetComponentsInChildren<ToggleExtend>();
            //menuLanguagesCanvasPanels[languageOption].transform.Find("OperatorObjectivePanel").gameObject.SetActive(false);
            //menuLanguagesCanvasPanels[languageOption].transform.Find("CertifierOjectivePanel").SetActive(false);
            //hide all the panels
            foreach (GameObject value in menuLanguagesCanvasPanels.Values)
            {
                value.SetActive(false);
            }
            //display the selected language panels
            menuLanguagesCanvasPanels[languageOption].SetActive(true);
            languagePanel.SetActive(true);
            isGeneratorConfigured = true;
            isOptionChanged = false;
        }

    }


    public void DisplayMenu()
    {
        menuLanguagesCanvasPanels[languageOption].SetActive(true);
        languagePanel.SetActive(true);
    }

    public void HideMenu() //subscribe to the confirm button
    {
        menuLanguagesCanvasPanels[languageOption].SetActive(false);
        languagePanel.SetActive(false);
    }

    public void CheckSelectionsHideMenu() //subscribe to the confirm button
    {
        int CountToggleOn = 0;
        //Get Current Selected Toggles
        Toggle[] allActiveToggles = menuLanguagesCanvasPanels[languageOption].GetComponentsInChildren<Toggle>();
        foreach (var toggle in allActiveToggles)
        {
            if (toggle.isOn)
            {
                CountToggleOn++;
            }
        }

        if (CountToggleOn < toggleTags.Count)
        {
            WarningText.text = "Please select all the settings";
            return;
        }
        WarningText.text = null;
        HideMenu();
        //Debug.Log("hide the menu");
        isGeneratorConfigured = false;
    }


    public void GetToggleValueWhileItChange()// for the all the toggles(except for the language toggles),subscribe to the toggles
    {
        //get the toggle that changed value
        //TODO imp there must be some problem here.
        for (int i = 0; i < allToggles.Length; i++)
        {
            //find the toggle is currently changing ison value
            if (allToggles[i].toggleChangedValue!=null )
            {
                //TODO use while instead
                //loop for the toggle tags
                for (int j = 0; j < toggleTags.Count; j++)
                {
                    //loop for the different tags
                    if (allToggles[i].toggleChangedValue.tag == toggleTags[j] )
                    {
                        //fill in the list of toggle text and ison in order, color the check box
                        if (allToggles[i].toggleChangedValue.isOn)
                        {
                            //listToggle[j] = allToggles[i].toggleChangedValue;
                            if(allToggles[i].toggleChangedValue.tag == "User profile toggle")
                            {
                                previousUserProfileToggleText = listToggleText[j];
                            }                           
                            listToggleText[j] = allToggles[i].toggleChangedValue.GetComponentInChildren<Text>().text;
                            listToggleIson[j] = allToggles[i].toggleChangedValue.isOn;
                            //allToggles[i].GetComponentInChildren<Image>().color = Color.green;
                        }
                    }
                }

            }
            //set the check boxes to white if they are not on
            if (!allToggles[i].GetComponentInChildren<Toggle>().isOn)
            {
                allToggles[i].GetComponentInChildren<Image>().color = Color.white;
            }

            Debug.Log("the toogle is pressed");
        }


        HideDisplayPanels();


        //to store the correct toggle in the selection list's objective 
        var activeToggles= menuLanguagesCanvasPanels[languageOption].GetComponentsInChildren<Toggle>();
        for (int i = 0; i < activeToggles.Length; i++)
        {
            if (userProfileObjectives.ContainsKey(listToggleText[0]))
            {
                if (userProfileObjectives[listToggleText[0]]
                    .Contains(activeToggles[i].GetComponentInChildren<Text>().text) && activeToggles[i].isOn)
                {
                    listToggleText[2] = activeToggles[i].GetComponentInChildren<Text>().text;
                }

            }
        }
        
    }

    public void HideDisplayPanels()
    {
        //display and hide the panels according to the selection of the user profile
        if (listToggleText[0] != null)
        {
            if (listToggleText[0] == "Operator" && listToggleIson[0])
            {
                OperatorPanelEn.SetActive(true);
                CertifierPanelEn.SetActive(false);
                Debug.Log("Operator");
            }

            if (listToggleText[0] == "Certifier" && listToggleIson[0])
            {
                OperatorPanelEn.SetActive(false);
                CertifierPanelEn.SetActive(true);
                Debug.Log("Certifier");
            }

            if (listToggleText[0] == "Conduttore" && listToggleIson[0])
            {
                OperatorPanelIt.SetActive(true);
                CertifierPanelIt.SetActive(false);
                Debug.Log("conduttore");
                Debug.Log("OperatorPanelIt: " + OperatorPanelIt.name);
            }

            if (listToggleText[0] == "Verificatore" && listToggleIson[0])
            {
                OperatorPanelIt.SetActive(false);
                CertifierPanelIt.SetActive(true);
                Debug.Log("verificatore");
                Debug.Log("CertifierPanelIt: " + CertifierPanelIt.name);
            }
        }
    }


    public void GetLanguageToggleValueWhileItChange()// for the all the language, subscribe to the language toggles
    {
        //get the toggle that changed value
        //TODO imp there must be some problem here.
        for (int i = 0; i < languageToggles.Length; i++)
        {
            //set the check boxes to white if they are not on, it is interesting here that languageToogles are compoenents instead of gameobjects
            if (!languageToggles[i].GetComponentInChildren<Toggle>().isOn)
            {
                languageToggles[i].GetComponentInChildren<Image>().color = Color.white;
            }

            if (languageToggles[i].toggleChangedValue!= null && languageToggles[i].toggleChangedValue.isOn) //the toggle changedValvue null means it has not been preesed
            {
                languageOption = languageToggles[i].GetComponentInChildren<Text>().text;
                Debug.Log("language option: " + languageOption);
                isOptionChanged = true;
            }

        }

        //display properly the panels of the language option
        foreach (var panel in menuLanguagesCanvasPanels.Values)
        {
            panel.SetActive(false);
        }
        menuLanguagesCanvasPanels[languageOption].SetActive(true);

    }

}


