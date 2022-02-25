using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AnotherMonoClass : MonoBehaviour
{
    void Update() {
        Debug.Log("Awaike");
        var testEvaluation = gameObject.GetComponent<TestEvaluation>();
        //Debug.Log("testclass.level: " + testEvaluation.Level);
        Debug.Log("testclass.MyDatatable: " + testEvaluation.MyDatatable);
    }

}

