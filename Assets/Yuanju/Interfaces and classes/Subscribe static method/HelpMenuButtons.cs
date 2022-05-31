using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;
using System;

public class HelpMenuButtons : MonoBehaviour
{
    public GameObject SequenceButton;
    public static bool IsSequenceButtonClicked;
    public GameObject ProblemButton;
    public static bool IsProblemButtonClicked;
    public GameObject ResultsButton;
    public static bool IsResultsButtonClicked;
    public GameObject BoilerInsideButton;
    public static bool IsBoilerInsideButtonClicked;
    public GameObject AnimationButton;
    public static bool IsAnimationButtonClicked;

    //public delegate void MyDelegate(); // This defines what type of method you're going to call.
    //public MyDelegate m_methodToCall; // This is the variable holding the method you're going to call.


    //void Start()
    //{
    //    AddButtonMethod(SequenceButton, InvokeButtonMethod(Evaluation.VisualizeSequence, IsSequenceButtonClicked), IsSequenceButtonClicked);
    //    //AddButtonMethod(ProblemButton, Evaluation.VisualizeSequence, IsSequenceButtonClicked);
    //    //AddButtonMethod(ResultsButton, Evaluation.VisualizeSequence, IsSequenceButtonClicked);
    //    //AddButtonMethod(BoilerInsideButton, Evaluation.VisualizeSequence, IsSequenceButtonClicked);
    //    //AddButtonMethod(AnimationButton, Evaluation.VisualizeSequence, IsSequenceButtonClicked);
    //}

    //private Action InvokeButtonMethod(Action method, bool isButtonClicked)
    //{
    //    if (IsSequenceButtonClicked)
    //    {
    //        method();//Evaluation.VisualizeSequence();
    //    }
    //    else
    //    {
    //        method();//Evaluation.HideSequence();
    //    }
    //    isButtonClicked = !isButtonClicked;
    //    return null;
    //}

    //private void AddButtonMethod(GameObject Button, Action methodToInvoke, bool isButtonClicked)
    //{
    //    var button = SequenceButton.GetComponentInChildren<InteractionButton>();
    //    button.OnPress -= InvokeButtonMethod(methodToInvoke, isButtonClicked);
    //    button.OnPress += InvokeButtonMethod(methodToInvoke, isButtonClicked);
    //}
    void Update()
    {
        var sequenceButton = SequenceButton.GetComponentInChildren<InteractionButton>();
        var problemButton = ProblemButton.GetComponentInChildren<InteractionButton>();
        var resultsButton = ResultsButton.GetComponentInChildren<InteractionButton>();
        var boilerInsideButton = BoilerInsideButton.GetComponentInChildren<InteractionButton>();
        var animationButton = AnimationButton.GetComponentInChildren<InteractionButton>();



        sequenceButton.OnPress -= ClickSequenceButton;
        sequenceButton.OnPress += ClickSequenceButton;

        problemButton.OnPress -= ClickProblemButton;
        problemButton.OnPress += ClickProblemButton;

        resultsButton.OnPress -= ClickResultsButton;
        resultsButton.OnPress += ClickResultsButton;

        boilerInsideButton.OnPress -= ClickBoilerInsideButton;
        boilerInsideButton.OnPress += ClickBoilerInsideButton;

        animationButton.OnPress -= ClickAnimationButton;
        animationButton.OnPress += ClickAnimationButton;

        if (IsSequenceButtonClicked)
        {
            Evaluation.VisualizeSequence();
        }
        else
        {
            Evaluation.HideSequence();
        }

        if (IsProblemButtonClicked)
        {
            ToggleLabelManager.DisplayProblem();
        }
        else
        {
            ToggleLabelManager.HideProblem();
        }

        if (IsResultsButtonClicked)
        {
            TrialCountAndTaskCount.VisualizeResult();
        }
        else
        {
            TrialCountAndTaskCount.HideResult();
        }

        if (IsBoilerInsideButtonClicked)
        {
            GameObject.Find("Boiler").GetComponent<Boiler>().ShowBoiler(); 
        }
        else
        {
            GameObject.Find("Boiler").GetComponent<Boiler>().HideBoiler(); 
        }

        if (IsAnimationButtonClicked)
        {
            GameObject.Find("Managers").GetComponent<AnimationManager>().PlayAnimations(); 
        }
        else
        {
            GameObject.Find("Managers").GetComponent<AnimationManager>().StopAnimations(); 
        }
    }

    private void ClickSequenceButton()
    {
        IsSequenceButtonClicked = !IsSequenceButtonClicked;
    }

    private void ClickProblemButton()
    {
        IsProblemButtonClicked = !IsProblemButtonClicked;
    }

    private void ClickResultsButton()
    {
        IsResultsButtonClicked = !IsResultsButtonClicked;
    }

    private void ClickBoilerInsideButton()
    {
        IsBoilerInsideButtonClicked = !IsBoilerInsideButtonClicked;
    }

    private void ClickAnimationButton()
    {
        IsAnimationButtonClicked = !IsAnimationButtonClicked;
    }

}
