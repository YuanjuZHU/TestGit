using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data;

//just a comment for test
public class TestEvaluation : MonoBehaviour {
    //public int experience;
    //public int Level {
    //    get { return experience / 750; }
    //}


    private DataTable myDatatable = new DataTable();
    public DataTable MyDatatable 
    {
        get { return myDatatable; }
        set { myDatatable = value; }
    }
    void Update()
    {
        var types = new List<string>();

        types = GetComponentInfo(MyDatatable, "TYPE");

        foreach (var type in types)
        {
            Debug.Log("componentTags: " + type);
        }
    }

    private List<string> GetComponentInfo(DataTable dt, string ColumnTitle) {
        DataRow[] drs = dt.Select(); //get all the rows of the data table
        List<string> myList = new List<string>();
        //find the types of the components
        foreach(var dr in drs) {
            if(!myList.Contains(dr[ColumnTitle].ToString())) {
                myList.Add(dr[ColumnTitle].ToString());
            }
        }
        return myList;
    }
}
