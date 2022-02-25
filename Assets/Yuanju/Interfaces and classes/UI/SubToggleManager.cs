using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using UnityEngine;
using UnityEngine.UI;

public class SubToggleManager : MonoBehaviour
{
    private int index;
    private GameObject gameobjectForIteration;
    public GameObject PanelInEnglish;
    private Dictionary<Transform, bool> stateBeforeEnteringList; //the status of the toggles(but the toggles in the sub task list should be excluded,it is done in "void Start")
    private Toggle[] subToggles;
    private string originalText;
    private bool panelStatusRecorded; //to know if the dictionary(stateBeforeEnteringList) has been filled or not

    private List<Transform> lstTransform;
    // Start is called before the first frame update
    void Start()
    {
        gameobjectForIteration = gameObject;
        stateBeforeEnteringList = new Dictionary<Transform, bool>();
        subToggles = gameObject.GetComponentsInChildren<Toggle>(true);
        lstTransform = new List<Transform>();
        panelStatusRecorded = false;
        //RecordThePanelDisplayStatus();
        //remove the subtoggles

        originalText = gameObject.transform.parent.GetComponentInChildren<Text>().text;
    }

    private void RecordThePanelDisplayStatus()
    {
        lstTransform.Clear();
        stateBeforeEnteringList.Clear(); //clear the dictionary before recording the previous panel status
        PanelManager.menuLanguagesCanvasPanels[PanelManager.languageOption].transform.GetAllChildren(lstTransform, false);

        foreach (var toggle in subToggles)
        {
            stateBeforeEnteringList.Remove(toggle.transform);
            lstTransform.Remove(toggle.transform);
        }
        lstTransform.Remove(gameObject.transform);

        for (int i = 0; i < lstTransform.Count; i++)
        {
            stateBeforeEnteringList.Add(lstTransform[i], lstTransform[i].gameObject.activeSelf);
        }
    }

    // Update is called once per frame
    void Update()
    {
        ////if the parent has been checked, this ensemble of toggles should be visualized
        //if (gameObject.transform.parent.GetComponent<Toggle>().isOn && previousToggleIson != gameObject.transform.parent.GetComponent<Toggle>().isOn)
        //{
        //    Debug.Log("the parent toggle has been checked! ");
        //    //Hide the siblings, the uncles and the granduncles 
        //    for (int i = 0; i < gameObject.transform.parent.childCount; i++)
        //    {
        //        if (i!= gameObject.transform.GetSiblingIndex())
        //        {
        //            gameObject.transform.parent.GetChild(i).gameObject.SetActive(false);
        //        }
        //    }

        //    //set the children visible
        //    for (int i = 0; i < gameObject.transform.childCount; i++)
        //    {
        //        gameObject.transform.GetChild(i).gameObject.SetActive(false);
        //    }

        //    previousToggleIson = gameObject.transform.parent.GetComponent<Toggle>().isOn;
        //}
        ////index = gameObject.transform.GetSiblingIndex();
        if (PanelManager.listToggleText[1] == "Training" || PanelManager.listToggleText[1] == "Apprendimento")
        {
            HideDisplayToggles();
        }

        SetTestingModeLabel();
    }

    void HideDisplayToggles()
    {
        for (int k = 0; k < PanelManager.allToggles.Length; k++)
        {
            if (PanelManager.allToggles[k].isToggleChangedValue/*PanelManager.allToggles[i].toggleChangedValue.gameObject==gameObject*/)
            {
                if (!panelStatusRecorded)
                {
                    RecordThePanelDisplayStatus();
                    panelStatusRecorded = true;
                }
                Debug.Log("PanelManager.allToggles[k].toggleChangedValue.transform: " + PanelManager.allToggles[k].toggleChangedValue.transform);
                Debug.Log("  " + gameObject.transform.parent);
                if (PanelManager.allToggles[k].toggleChangedValue.transform == gameObject.transform.parent)
                {
                    Debug.Log("gameobjectForIteration in hiding the toggles 1: ");
                    Debug.Log("gameObject.transform.parent: " + gameObject.transform.parent.name);
                    if (gameObject.transform.parent.GetComponent<Toggle>().isOn) //if "shut down" is on
                    {
                        Debug.Log("gameobjectForIteration in hiding the toggles 2: ");

                        //hide the use profile, mode, ... on the panel
                        while (gameobjectForIteration != PanelInEnglish)
                        {
                            Debug.Log("gameobjectForIteration in hiding the toggles: " + gameobjectForIteration);
                            //Hide the siblings, the uncles and the granduncles 
                            for (int i = 0; i < gameobjectForIteration.transform.parent.childCount; i++)
                            {
                                if (i != gameobjectForIteration.transform.GetSiblingIndex())
                                {
                                    gameobjectForIteration.transform.parent.GetChild(i).gameObject.SetActive(false);
                                }
                            }
                            gameobjectForIteration = gameobjectForIteration.transform.parent.gameObject;
                        }

                        gameobjectForIteration = gameObject;


                        //set the children visible
                        for (int i = 0; i < gameObject.transform.childCount; i++)
                        {
                            gameObject.transform.GetChild(i).gameObject.SetActive(true);
                        }
                        PanelManager.allToggles[k].isToggleChangedValue = false;
                    }
                    else
                    {
                        gameObject.transform.parent.GetComponentInChildren<Text>().text = originalText;
                        PanelManager.allToggles[k].isToggleChangedValue = false;
                    }




                }

                if (!gameObject.transform.parent.GetComponent<Toggle>().isOn)
                {
                    gameObject.transform.parent.GetComponentInChildren<Text>().text = originalText;
                }

                for (int i = 0; i < subToggles.Length; i++)
                {
                    Debug.Log("this is a sub toggle: " + subToggles[i]);
                    if (subToggles[i].isOn)
                    {
                        Debug.Log("this is the dicitonary: " + stateBeforeEnteringList.Keys.Count);
                        foreach (var key in stateBeforeEnteringList.Keys)
                        {

                            Debug.Log("the keys and the status: " + key + stateBeforeEnteringList[key]);
                            key.gameObject.SetActive(stateBeforeEnteringList[key]);
                        }

                        for (int j = 0; j < gameObject.transform.childCount; j++)
                        {
                            gameObject.transform.GetChild(j).gameObject.SetActive(false);
                            Debug.Log("I'm hiding the prompt content: " + gameObject.transform.GetChild(j).gameObject);
                        }
                        subToggles[i].SetIsOnWithoutNotify(false);
                        gameObject.transform.parent.GetComponentInChildren<Text>().text = subToggles[i].GetComponentInChildren<Text>().text;
                        panelStatusRecorded = false;
                    }
                }
            }


        }

        //ToggleExtend.isToggleChangedValue = false;
        Debug.Log("a toggle changed value: " + gameObject.name);
            ////////////if (gameObject.transform.parent.GetComponent<Toggle>().isOn ) //if "shut down" is on
            ////////////{
            ////////////    //hide the use profile, mode, ... on the panel
            ////////////    while (gameobjectForIteration != PanelInEnglish)
            ////////////    {
            ////////////        Debug.Log("gameobjectForIteration in hiding the toggles: " + gameobjectForIteration);
            ////////////        //Hide the siblings, the uncles and the granduncles 
            ////////////        for (int i = 0; i < gameobjectForIteration.transform.parent.childCount; i++)
            ////////////        {
            ////////////            if (i != gameobjectForIteration.transform.GetSiblingIndex())
            ////////////            {
            ////////////                gameobjectForIteration.transform.parent.GetChild(i).gameObject.SetActive(false);
            ////////////            }
            ////////////        }
            ////////////        gameobjectForIteration = gameobjectForIteration.transform.parent.gameObject;
            ////////////    }

            ////////////    gameobjectForIteration = gameObject;


            ////////////    //set the children visible
            ////////////    for (int i = 0; i < gameObject.transform.childCount; i++)
            ////////////    {
            ////////////        gameObject.transform.GetChild(i).gameObject.SetActive(true);
            ////////////    }

            ////////////}
            ////////////else
            ////////////{
            ////////////    gameObject.transform.parent.GetComponentInChildren<Text>().text = originalText;
            ////////////}
            
            ////////////for (int i = 0; i < subToggles.Length; i++)
            ////////////{
            ////////////    Debug.Log("this is a sub toggle: "+ subToggles[i]);
            ////////////    if (subToggles[i].isOn)
            ////////////    {
            ////////////        foreach (var key in stateBeforeEnteringList.Keys)
            ////////////        {

            ////////////            Debug.Log("the keys and the status: " + key + stateBeforeEnteringList[key]);
            ////////////            key.gameObject.SetActive(stateBeforeEnteringList[key]);
            ////////////        }

            ////////////        for (int j = 0; j < gameObject.transform.childCount; j++)
            ////////////        {
            ////////////            gameObject.transform.GetChild(j).gameObject.SetActive(false);
            ////////////            Debug.Log("I'm hiding the prompt content: " + gameObject.transform.GetChild(j).gameObject);
            ////////////        }
            ////////////        subToggles[i].SetIsOnWithoutNotify(false);
            ////////////        gameObject.transform.parent.GetComponentInChildren<Text>().text = subToggles[i].GetComponentInChildren<Text>().text;
            ////////////    }
            ////////////}
            //if (!gameObject.transform.parent.GetComponent<Toggle>().isOn)
            //{
            //    foreach (var key in stateBeforeEnteringList.Keys)
            //    {
            //        Debug.Log("the keys and the status: "+ key+ stateBeforeEnteringList[key]);
            //        key.gameObject.SetActive(stateBeforeEnteringList[key]);
            //    }

                


            //    //for (int i = 0; i < gameobjectForIteration.transform.childCount; i++)
            //    //{
            //    //    //GameObject iterateGameobject;
            //    //    //iterateGameobject = gameobjectForIteration;
            //    //    gameobjectForIteration.transform.GetChild(i).gameObject.SetActive(true);
            //    //    if (/*iterateGameobject*/ gameobjectForIteration != gameObject && /*iterateGameobject*/gameobjectForIteration.transform.childCount != 0)
            //    //    {
            //    //        for (int k = 0; k < /*iterateGameobject*/gameobjectForIteration.transform.childCount; k++)
            //    //        {
            //    //            for (int j = 0; j < /*iterateGameobject*/gameobjectForIteration.transform.childCount; j++)
            //    //            {
            //    //                /*iterateGameobject*/ gameobjectForIteration.transform.GetChild(j).gameObject.SetActive(true);
            //    //            }
            //    //            /*iterateGameobject*/ gameobjectForIteration = /*iterateGameobject*/gameobjectForIteration.transform.GetChild(k).gameObject;
            //    //        }
            //    //    }

            //    //    Debug.Log("I'm inside the iteration: ");
            //    //}

            //    //for (int i = 0; i < gameObject.transform.childCount; i++)
            //    //{
            //    //    gameObject.transform.GetChild(i).gameObject.SetActive(false);
            //    //    Debug.Log("I'm hiding the prompt content: " + gameObject.transform.GetChild(i).gameObject);
            //    //}

            //    //PanelManager.GetToggleValueWhileItChange();
            //}
            

            ////////for (int i = 0; i < PanelManager.allToggles.Length; i++)
            ////////{
            ////////    if (!PanelManager.allToggles[i].GetComponentInChildren<Toggle>().isOn)
            ////////    {
            ////////        PanelManager.allToggles[i].GetComponentInChildren<Image>().color = Color.white;

            ////////    }
            ////////    Debug.Log("I'm checking the toggles: "+ PanelManager.allToggles[i]);
            ////////}
            //PanelManager.HideDisplayPanels();
        



    }

    public void SetTestingModeLabel()
    {
        if (PanelManager.listToggleText[1] == "Testing" || PanelManager.listToggleText[1] == "Verifica")
        {
            gameObject.transform.parent.GetComponentInChildren<Text>().text = originalText;
        }
    }

    public void GetRandomTask()
    {
        
        if (gameObject.transform.parent.GetComponent<Toggle>().isOn && (PanelManager.listToggleText[1] == "Testing" || PanelManager.listToggleText[1] == "Verifica"))
        {
            var subTaskSet = new List<string>();
            subTaskSet.Clear();
            for (int i = 0; i < NPOIReadExcel.Tasks.Count; i++)
            {
                //Debug.Log("tasks:: " + NPOIReadExcel.Tasks[i]);
                if (gameObject.transform.parent.GetComponentInChildren<Text>().text == NPOIReadExcel.Tasks[i])
                {
                    subTaskSet.Add(NPOIReadExcel.SubTasks[i]);
                    //Debug.Log("sub tasks:: " + NPOIReadExcel.SubTasks[i]);
                }
            }
            int randomTaskIndex = Random.Range(0, subTaskSet.Count-1);
            PanelManager.listToggleText[2] = subTaskSet[randomTaskIndex];
            //Debug.Log("this is a random sub task: "+ PanelManager.listToggleText[2]);
        }
    }

}
