using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class TestDelay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SetBlue", 5, 1f);//在三秒后执行Print方法,并且每隔3秒重复执行一次
        InvokeRepeating("SetRed", 6f, 1f);

    }

    void SetRed()
    {
        Debug.Log("do red");
        gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
    }

    void SetBlue()
    {
        Debug.Log("do blue");
        gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
    }
}
