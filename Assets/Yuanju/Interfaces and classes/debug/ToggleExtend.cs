using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleExtend : MonoBehaviour
{
    public Toggle toggleChangedValue;
    public bool isToggleChangedValue;
    public string labelBrief;
    public string labelDetailed;
    // Start is called before the first frame update
    void Start()
    {

        isToggleChangedValue = false;
    }

    // Update is called once per frame
    void Update()
    {
        //if (toggle1.isOn)
        //{
        //    toggle2.isOn = false;
        //}
        //if (toggle2.isOn)
        //{
        //    toggle1.isOn = false;
        //}
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Debug.Log("KeyCode.Alpha9 pressed: " );
            //var ToggleStatus = gameObject.GetComponent<Toggle>().isOn;
            //ToggleStatus = !ToggleStatus;
            //gameObject.GetComponent<Toggle>().Select();
            gameObject.GetComponent<Toggle>().SetIsOnWithoutNotify(!gameObject.GetComponent<Toggle>().isOn);
            Debug.Log("ToggleStatus: " + gameObject.GetComponent<Toggle>().isOn);
        }
        //if (Input.GetKeyDown(KeyCode.Alpha8))
        //{
        //    toggle2.isOn = true;
        //}
        //OnValueChange();
    }

    //settings that will done while pressing the toggle
    public void SetToggleValue()
    {
        isToggleChangedValue = true;
        toggleChangedValue = gameObject.GetComponent<Toggle>();
        //set value
        gameObject.GetComponent<Toggle>().SetIsOnWithoutNotify(!gameObject.GetComponent<Toggle>().isOn);
        //set color
        if (gameObject.GetComponent<Toggle>().isOn)
        {
            gameObject.GetComponentInChildren<Image>().color = Color.green;
        }
        else
        {
            gameObject.GetComponentInChildren<Image>().color = Color.white;
        }

    }

    //public void SetButtonValueRed()
    //{
    //    var image = gameObject.GetComponent<Image>();
    //    image.color=Color.red;
    //    Debug.Log("Image Status red: ");
    //}

    //public void SetButtonValueBlue()
    //{
    //    var image = gameObject.GetComponent<Image>();
    //    image.color = Color.blue;
    //    Debug.Log("Image Status blue: ");
    //}

    //subscribe to the event toggle change value
}
