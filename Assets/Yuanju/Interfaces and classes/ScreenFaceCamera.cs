using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFaceCamera : MonoBehaviour
{
    private Camera cam;
    private Vector3 PreviousAngle;
    private Vector3 PreviousPos;
    // Update is called once per frame
    void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        PreviousAngle = cam.transform.forward;
        //PreviousPos = cam.transform.position;
    }

    void Update()
    {

        //gameObject.transform.rotation = cam.transform.rotation;


        //update the rotation around the camera
        var crossProduct= Vector3.Cross(PreviousAngle, cam.transform.forward);
        //gameObject.transform.RotateAround((Vector3)cam.transform.position, crossProduct, 180*Vector3.AngleBetween(PreviousAngle, cam.transform.forward)/ Mathf.PI);
        
        //the local rotation of the slide
        Quaternion Rotation = gameObject.transform.rotation; //but why "w", not "y"
        Rotation.w = -cam.transform.rotation.w;
        gameObject.transform.rotation = Rotation;


        //the local translation of the slide
        //var offset = cam.transform.position - PreviousPos;
        //gameObject.transform.Translate(-offset);


        PreviousAngle = cam.transform.forward; //renew the previousAngle
        //PreviousPos = cam.transform.position;//renew the previousPos
    }
}
