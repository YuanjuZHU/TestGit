using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

//just a comment for test
public class ActuatorSettings : MonoBehaviour {
    //public int experience;
    //public int Level {
    //    get { return experience / 750; }
    //}
    public string TestSeriazlization = "This is a serialization test";

    [SerializeField]
    public DataTable myDatatable = new DataTable();
    public DataTable MyDatatable 
    {
        get { return myDatatable; }
        set { myDatatable = value; }
    }

    void Awake()
    {
        MyDatatable = ReadDataFromCsv("Assets/Yuanju/elements of Tirreno Power models.csv");
        MyDatatable.TableName = "Generator elements pre-setup";
    }

    void Update()
    {
        var types1 = new List<string>();
        var types2 = new List<string>();

        types1 = GetComponentInfo(myDatatable, "TYPE");
        types2 = GetComponentInfo(MyDatatable, "TYPE");

        foreach (var type in types1)
        {
            Debug.Log("componentTags type 1: " + type);
        }
        foreach (var type in types2)
        {
            Debug.Log("componentTags type 2: " + type);
        }
        Debug.Log("componentTags type 3: " + TestSeriazlization);
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


    /// <summary>
    /// read csv to dt
    /// </summary>
    /// <param name="file">csv</param>
    /// <returns>dt</returns>
    public static DataTable ReadDataFromCsv(string file)
    {
        DataTable dt = null;

        if (File.Exists(file))
        {
            dt = new DataTable();
            FileStream fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Read);

            StreamReader sr = new StreamReader(fs, Encoding.Default);

            string head = sr.ReadLine();
            string[] headNames = head.Split('&');
            for (int i = 0; i < headNames.Length; i++)
            {
                dt.Columns.Add(headNames[i], typeof(string));
            }
            while (!sr.EndOfStream)
            {
                #region ==read file in a loop==
                string lineStr = sr.ReadLine();
                if (lineStr == null || lineStr.Length == 0)
                    continue;
                string[] values = lineStr.Split('&');
                #region ==add row data==
                DataRow dr = dt.NewRow();
                for (int i = 0; i < values.Length; i++)
                {
                    dr[i] = values[i];
                }
                dt.Rows.Add(dr);
                #endregion
                #endregion
            }
            fs.Close();
            sr.Close();

        }
        return dt;
    }
}
