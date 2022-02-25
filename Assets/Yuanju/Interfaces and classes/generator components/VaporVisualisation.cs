using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaporVisualisation : MonoBehaviour
{
    public GameObject Vapor;

    void Start()
	{
	    Vapor.SetActive(false);

    }

    void Update()
    {
        if(GetComponent<Valvola>().Status == 0) {
            Vapor.SetActive(false);
        }
        if(GetComponent<Valvola>().Status == 1) {
            Vapor.SetActive(true);
        }
    }

}
