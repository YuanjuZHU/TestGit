using System.Collections;
using System.Collections.Generic;
using Leap.Unity.Interaction;
using UnityEngine;

public class TaskDoneButton : MonoBehaviour
{
    void Start()
    {
        var interactionButton = gameObject.GetComponent<InteractionButton>();
        interactionButton.OnPress -= Evaluation.CheckFinalSettings;
        interactionButton.OnPress += Evaluation.CheckFinalSettings;
    }


}
