using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using Leap.Unity.Interaction;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CorrectionButton : MonoBehaviour
{
    public GameObject panel;
    //public UnityEvent TestUnityEvent;
    // Start is called before the first frame update
    void Start()
    {
        var interactionButton = gameObject.GetComponent<InteractionButton>();
        interactionButton.OnPress -= Evaluation.StartCorrection;
        interactionButton.OnPress += Evaluation.StartCorrection;

        #region UnityEventTest

        //TestUnityEvent.AddListener(WriteSomething);
        //TestUnityEvent.Invoke();
        //var @event = gameObject.GetComponent<CorrectionButton>().TestUnityEvent;
        //Debug.Log("the class: "+ @event);
        //string method = TestUnityEvent.GetPersistentTarget(0).ToString();
        //Debug.Log("the first listener: "+ method);
        //string method1 = TestUnityEvent.GetPersistentMethodName(0);
        //Debug.Log("the first listener name: "+ method1);
        //int methodCount = TestUnityEvent.GetPersistentEventCount();
        //Debug.Log("the count of the listeners: " + methodCount);

        #endregion
    }


    public void HidePanel()
	{
	    panel.SetActive(false);
	}

    public void WriteSomething()
    {
        Debug.Log("this is a test for event: "+gameObject);
    }
}
