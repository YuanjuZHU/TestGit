using System.Collections;
using System.Collections.Generic;using UnityEditor;
using UnityEngine;


[InitializeOnLoad]
public static class ProjectOpenEvent
{
    private const string k_ProjectOpened = "ProjectOpened";
    static ProjectOpenEvent()
    {
        if (!SessionState.GetBool(k_ProjectOpened, false))
        {
            SessionState.SetBool(k_ProjectOpened, true);
            //Debug.Log("[Debug]: project opened");

            // DO WHAT YOU WANT
        }
    }
}



