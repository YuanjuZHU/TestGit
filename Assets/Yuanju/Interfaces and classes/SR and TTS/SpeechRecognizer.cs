using System;
using System.Collections.Generic;
using System.Linq;
using Leap.Unity.Interaction;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class SpeechRecognizer : MonoBehaviour
{
    private readonly Dictionary<string, Action> actions = new Dictionary<string, Action>();
    private KeywordRecognizer keywordRecognizer;
    public static bool IsCrossHairsGot;
    public GameObject VoiceCommandText;
    public static bool HasCommandFinish = false;
    public InteractionButton CorrectionButton;
    public InteractionButton RestartButton;
    public InteractionButton QuitButton;
    private void Start()
    {
        IsCrossHairsGot = false;
        //dragModel = GameObject.Find("hand").GetComponent<DragModel>();

        actions.Add("help", SelectComponents);
        actions.Add("aiuto", SelectComponents);
        actions.Add("ok", DeselectComponents);
        //evaluate the final settings
        actions.Add("finito", Evaluation.CheckFinalSettings);
        actions.Add("finish", Evaluation.CheckFinalSettings);
        //actions.Add("finito", TrialCountAndTaskCount.trialCountAndTaskCount.CountTrialsAndTasks);


        actions.Add("show sequence", Evaluation.VisualizeSequence);
        actions.Add("hide sequence", Evaluation.HideSequence);
        //actions.Add("redo", Evaluation.StartCorrection);
        actions.Add("visualizza sequenza", Evaluation.VisualizeSequence);
        actions.Add("nascondi sequenza", Evaluation.HideSequence);

        actions.Add("correggi", CorrectWrongActions);
        actions.Add("correct", CorrectWrongActions);

        actions.Add("restart", RestartTask);
        actions.Add("quit", QuitTask);

        actions.Add("mostra problema", ToggleLabelManager.DisplayProblem);
        //actions.Add("detailed problem", ToggleLabelManager.AssignLabelDetailed);
        actions.Add("show problem", ToggleLabelManager.DisplayProblem);
        actions.Add("hide problem", ToggleLabelManager.HideProblem);
        actions.Add("nascondi problema", ToggleLabelManager.HideProblem);

        //actions.Add("nascondere problema", ToggleLabelManager.AssignLabelBrief);
        //actions.Add("brief problem", ToggleLabelManager.AssignLabelBrief);
        //actions.Add("replay", Evaluation.ReplayOperations);
        actions.Add("show results",TrialCountAndTaskCount.VisualizeResult);
        actions.Add("mostra risultati", TrialCountAndTaskCount.VisualizeResult);
        actions.Add("hide results",TrialCountAndTaskCount.HideResult);
        actions.Add("nascondi risultati", TrialCountAndTaskCount.HideResult);

        

        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        keywordRecognizer.Start();
    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log(speech.text);
        VoiceCommandText.GetComponentInChildren<Text>().text = speech.text;
        actions[speech.text].Invoke();
        Invoke("HideVoiceCommand", 2);//HideVoiceCommand will be exceuted after 2 seconds
    }


    //Methods to trigger if the speech is made(change the value of the bool variable "IsCrossHairsGot")
    public void SelectComponents()
    {
        if (CameraSelectComponent.haveCrossHairs)
        {
            IsCrossHairsGot = true;
        }
    }

    public void DeselectComponents()
    {
        IsCrossHairsGot = false;
    }

    //private void DisplayVoiceCommand()
    //{

    //}

    private void HideVoiceCommand()
    {
        VoiceCommandText.GetComponentInChildren<Text>().text = null;
    }

    private void CorrectWrongActions()
    {
        CorrectionButton.OnPress.Invoke();
    }

    /// <summary>
    /// corresponding to "restart button"
    /// </summary>
    private void RestartTask()
    {
        RestartButton.OnPress.Invoke();
    }

    /// <summary>
    /// corresponding to "quit button"
    /// </summary>
    private void QuitTask()
    {
        QuitButton.OnPress.Invoke();
    }


}