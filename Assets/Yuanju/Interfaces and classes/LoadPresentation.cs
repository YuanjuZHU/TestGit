//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using UnityEditor;
//using UnityEngine;
//using UnityEngine.UI;

//public class LoadPresentation : MonoBehaviour
//{
//    static List<Sprite> PPTs = new List<Sprite>();
//    static Dictionary<string,List<Sprite>> PPTLib = new Dictionary<string, List<Sprite>>();
//    public GameObject CurrentSlideOnCanvas;
//    public GameObject AttachementUICanvas;
//    private static GameObject TextNext;
//    private static GameObject TextPrevious;
//    private bool  SlidesVisible;


//    void Start()
//    {
//        PPTs = Resources.LoadAll("manuale_generatore_vapore_unige_SV", typeof(Sprite)).Cast<Sprite>().ToList();
//        CurrentSlideOnCanvas.SetActive(false);
//        AttachementUICanvas.SetActive(false);
//        TextNext = GameObject.Find("Text Next");
//        TextPrevious = GameObject.Find("Text Previous");
//        //string generatorPath = Application.DataPath + "\World"Assets/Generators/Generator x";
//        //FileInfo[] fileInfo = generatorPath.GetFiles("*.*", SearchOption.AllDirectories);
//        string generatorsPath = "Assets/Resources/Generators";
//        //todo Assetdatabase can not be built!!!!
//        var generatorFolders = AssetDatabase.GetSubFolders(generatorsPath); //the name of the folders
//        Debug.Log("generatorFolders[0]: " + generatorFolders[0]);
//        foreach (var generator in generatorFolders)
//        {
//            Debug.Log("generator: "+ generator);
//            foreach (var documentFolder in AssetDatabase.GetSubFolders(generator)) 
//            {
//                Debug.Log("documentFolder: " + documentFolder);
//                var folderName = documentFolder.Replace(generator + "/", "");
//                Debug.Log("folderName: " + folderName);
//                PPTLib.Add(folderName, Resources.LoadAll(folderName, typeof(Sprite)).Cast<Sprite>().ToList());
//            }
//            //Debug.Log("the test folder: " + PPTLib[folder][0].name);
//        }

//        CurrentSlideOnCanvas.GetComponent<Image>().sprite = PPTs[0];

//        foreach (var key in PPTLib.Keys)
//        {
//            Debug.Log("PPTLib: " + key);
//            Debug.Log("PPTLib count: " + PPTLib[key].Count);
            
//        }
//        SlidesVisible = true;
//        //foreach (var VARIABLE in PPTLib)
//        //{
//        //    Debug.Log("PPTLib.count: "+ VARIABLE.Value.Count);
//        //}
//    }

//    //void Update()
//    //{
//    //    DisplayNextSlide();
//    //    DisplayPreviousSlide();
//    //}
//    public void DisplayNextSlide() //imp the difference between OnGUI(FixedUpdate) and Update
//    {
//        int index = PPTs.IndexOf(CurrentSlideOnCanvas.GetComponent<Image>().sprite);
//        TextPrevious.GetComponent<TextMesh>().color = Color.blue;
//        if (index != PPTs.Count - 1)
//        {
//            TextNext.GetComponent<TextMesh>().color = Color.blue;
//            CurrentSlideOnCanvas.GetComponent<Image>().sprite = PPTs[index + 1];
//        }
//        else
//        {
//            TextNext.GetComponent<TextMesh>().color = Color.gray;
//        }

//    }

//    public void DisplayPreviousSlide() //imp the difference between OnGUI(FixedUpdate) and Update
//    {
//        int index = PPTs.IndexOf(CurrentSlideOnCanvas.GetComponent<Image>().sprite);
//        TextNext.GetComponent<TextMesh>().color = Color.blue;

//        if (index != 0)
//        {
//            TextPrevious.GetComponent<TextMesh>().color = Color.blue;
//            CurrentSlideOnCanvas.GetComponent<Image>().sprite = PPTs[index - 1];
//        }
//        else
//        {
//            TextPrevious.GetComponent<TextMesh>().color = Color.gray;
//        }


//    }

//    public void HideDisplayPresentation() //imp the difference between OnGUI(FixedUpdate) and Update
//    {
//        if (SlidesVisible)
//        {
//            CurrentSlideOnCanvas.SetActive(false);
//        }
//        else
//        {
//            CurrentSlideOnCanvas.SetActive(true);
//        }
//        SlidesVisible = !SlidesVisible;
//        LeapControl.AttachmentHasBeenSelected = false;
//    }
//    /// <summary>
//    /// for selecting a presentation, these methods will need to be replaced with a method that take the label of the button and use it for retrieving the presentation
//    /// </summary>
//    public void Automatismi() //imp the difference between OnGUI(FixedUpdate) and Update
//    {
//        PPTs = PPTLib["automatismi"]; //assign new values to the play list
//        CurrentSlideOnCanvas.GetComponent<Image>().sprite = PPTs[0];
//        CurrentSlideOnCanvas.SetActive(true);
//        LeapControl.AttachmentHasBeenSelected = false;
//    }

//    public void AutomatismiMOD() //imp the difference between OnGUI(FixedUpdate) and Update
//    {
//        PPTs = PPTLib["automatismiMOD"]; //assign new values to the play list
//        CurrentSlideOnCanvas.GetComponent<Image>().sprite = PPTs[0];
//        CurrentSlideOnCanvas.SetActive(true);
//        LeapControl.AttachmentHasBeenSelected = false;
//    }

//    public void Manuale() //imp the difference between OnGUI(FixedUpdate) and Update
//    {
//        PPTs = PPTLib["manuale_generatore_vapore_unige_SV"]; //assign new values to the play list
//        CurrentSlideOnCanvas.GetComponent<Image>().sprite = PPTs[0];
//        CurrentSlideOnCanvas.SetActive(true);
//        LeapControl.AttachmentHasBeenSelected = false;
//    }

//    public void DisplayPresentation(GameObject text) //imp the difference between OnGUI(FixedUpdate) and Update
//    {
//        foreach (var key in PPTLib.Keys)
//        {
//            if (text.GetComponentInChildren<Text>().text == key)
//            {
//                PPTs = PPTLib[key]; //assign new values to the play list
//                CurrentSlideOnCanvas.GetComponent<Image>().sprite = PPTs[0];
//                CurrentSlideOnCanvas.SetActive(true);
//                LeapControl.AttachmentHasBeenSelected = false;
//            }
//        }

        
//    }


//    public void HideAttachementCanvas() 
//    {
//        AttachementUICanvas.SetActive(false);
//    }
//    public void DispalyAttachementCanvas()
//    {
//        //if (!GameObject.Find("MainMenuPanel").activeSelf)
//        //{
//        AttachementUICanvas.SetActive(true);
//        //}
//    }

//    //TODO an user interface to swap the presentation source
//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Alpha1)) //if press a button
//        {
//            PPTs = PPTLib["automatismiMOD"]; //assign new values to the play list
//            CurrentSlideOnCanvas.GetComponent<Image>().sprite = PPTs[0]; //set the screen to the first slide of the presentation
//        }
//    }

//}
