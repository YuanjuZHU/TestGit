using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCameraLocation : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            transform.localPosition = new Vector3(-0.22f, 1.1f, 41.98f);
            transform.localEulerAngles = new Vector3(0f, 90f, 0f);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            transform.localPosition = new Vector3(0.72f, 1.1f, 42.82f);
            transform.localEulerAngles = new Vector3(0f, 180f, 0f);
        }
    }
}
