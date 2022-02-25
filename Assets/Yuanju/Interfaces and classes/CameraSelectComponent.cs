using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Text;
using UnityEditor;
using UnityEngine.UI;

public class CameraSelectComponent : MonoBehaviour
{
    private bool hitObeject; //if the ray hits a gameobject
    private List<string> components;//the list of the tags for the components
    public Text textBox;//the text box in the description gameobject
    private GameObject crossHairsObject;//the crosshairs is the intersection sphere on the hit point
    public static bool haveCrossHairs;//to know if the crosshairs exists or not
    private bool speechEmpty;//add this bool to know the status if the text description is empty 
    private Vector3 previousHitPoint; //add this bool to avoid update runs continuously when user does not move the camera 
    public GameObject backGroundPanel;

    void Start()
    {
        //crossHairs= GameObject.Find("Canvas").transform.Find("CrossHairs");
        //textBox = GameObject.Find("Canvas").transform.Find("Description").GetComponent<Text>();
        haveCrossHairs = false;
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, 10000 * Camera.main.transform.forward);

        hitObeject = Physics.Raycast(ray, out hit);
        // if the ray hits a component
        if (hitObeject && ReadCSV.componentTags.Contains(hit.transform.tag))
        {
            DataRow[] drs = ReadCSV.dt.Select();
            //use name to find out the hit component
            foreach (var dr in drs)
            {
                if (hit.transform.name == dr["NAME"].ToString() )
                {
                    //Debug.Log("the gameobject's name is the same as the name in the CSV!");
                    //here we can also do something
                    //Debug.Log("DESCRIPTION.ToString(): " + dr["DESCRIPTION"]);
                    //textBox.text = dr["DESCRIPTION"].ToString();
                    if (SpeechRecognizer.IsCrossHairsGot)
                    {
                        textBox.text = dr["DESCRIPTION"].ToString();
                        backGroundPanel.SetActive(true);
                        speechEmpty = false;
                    }
                    break;
                }

            }


            // TODO deal with the ray hit triangle of a component's mesh, impossible! the model's mesh is too rough, create a sphere instead
            if (!haveCrossHairs)
            {
                crossHairsObject= GameObject.CreatePrimitive(type: PrimitiveType.Sphere);
                crossHairsObject.transform.localScale=new Vector3(0.01f,0.01f,0.01f);
                crossHairsObject.tag = hit.transform.tag;
                Destroy(crossHairsObject.GetComponent<Collider>());
                crossHairsObject.transform.localPosition= hit.point;
                crossHairsObject.GetComponent<MeshRenderer>().material.color = Color.red;
                haveCrossHairs = true;
            }

            if (haveCrossHairs && previousHitPoint!=hit.point)
            {
                if (crossHairsObject != null)
                {
                    previousHitPoint = hit.point;
                    crossHairsObject.transform.localPosition= hit.point;
                }
            }
        }

        else
        {

            Destroy(crossHairsObject);
            haveCrossHairs = false;
        }

        if (!SpeechRecognizer.IsCrossHairsGot && !speechEmpty)
        {
            textBox.text = null;
            speechEmpty = true;
            speechEmpty = true;
            backGroundPanel.SetActive(false);
        }
    }
}