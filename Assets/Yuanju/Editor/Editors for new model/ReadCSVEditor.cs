using System.Collections;
using System.Collections.Generic;
using Leap.Unity.Interaction;
using MGS.UCommon.Generic;
using UnityEditor;
using UnityEngine;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

[CustomEditor(typeof(TestEvaluation))]
public class ReadCSVEditor : Editor
{
    public static List<string> componentTags = new List<string>(); //store all the types of the components and they are the tags of for the game objects 
    public static List<string> componentNames = new List<string>(); //store all the name of the components 
    public static DataTable dt;

    //get the info in Awake
    void Awake() {
        dt = ReadDataFromCsv("Assets/Yuanju/Components in Italian UTF 8 updated.csv");  //read the CSV file and put the data into a data table
        //select the data rows who have the same name as the touched gameobject(eg. Select("name="+touched gameobject.parsedName) )
        componentTags = GetComponentInfo(dt, "TYPE");
        componentNames = GetComponentInfo(dt, "NAME");

        //for (int i = 0; i < dt.Columns.Count; i++)
        //{
        Debug.Log("data columns[i] names: " + dt.Rows[dt.Rows.Count - 1][1]);
        //}
        TestEvaluation myTarget = (TestEvaluation)target;
        dt.TableName = "Generator elements pre-setup";
        myTarget.MyDatatable = dt;

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
    public static DataTable ReadDataFromCsv(string file) {
        DataTable dt = null;

        if(File.Exists(file)) {
            dt = new DataTable();
            FileStream fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Read);

            StreamReader sr = new StreamReader(fs, Encoding.Default);

            string head = sr.ReadLine();
            string[] headNames = head.Split('&');
            for(int i = 0; i < headNames.Length; i++) {
                dt.Columns.Add(headNames[i], typeof(string));
            }
            while(!sr.EndOfStream) {
                #region ==read file in a loop==
                string lineStr = sr.ReadLine();
                if(lineStr == null || lineStr.Length == 0)
                    continue;
                string[] values = lineStr.Split('&');
                #region ==add row data==
                DataRow dr = dt.NewRow();
                for(int i = 0; i < values.Length; i++) {
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
