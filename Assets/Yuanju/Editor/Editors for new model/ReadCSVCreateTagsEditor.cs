using System.Collections;
using System.Collections.Generic;
using Leap.Unity.Interaction;
using UnityEditor;
using UnityEngine;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using NPOI.OpenXmlFormats.Dml.Diagram;

[CustomEditor(typeof(TestEvaluation))]
public class ReadCSVCreateTagsEditor : Editor
{
    public static DataTable dt;

    //get the info in Awake
    void Awake()
    {
        dt = ReadDataFromCsv("Assets/Yuanju/Components in Italian UTF 8 updated.csv");  //read the CSV file and put the data into a data table
        //select the data rows who have the same name as the touched gameobject(eg. Select("name="+touched gameobject.parsedName) )
        //for (int i = 0; i < dt.Columns.Count; i++)
        //{
        //  Debug.Log("data columns[i] names: " + dt.Rows[dt.Rows.Count - 1][1]);
        //}
        TestEvaluation myTarget = (TestEvaluation)target;
        dt.TableName = "Generator elements pre-setup";
        myTarget.MyDatatable = dt;

        var elementTypes = GetDataFromTable(dt, "TYPE");
        foreach (var type in elementTypes)
        {
            //Debug.Log("componentTags type: " + type);
            CreateTag(type);
        }

        AttachAllTags();
    }

    #region Deal with data table
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="ColumnTitle"></param>
    /// <returns></returns>
    private List<string> GetDataFromTable(DataTable dt, string ColumnTitle)
    {
        DataRow[] drs = dt.Select(); //get all the rows of the data table
        List<string> myList = new List<string>();
        //find the types of the components
        foreach (var dr in drs)
        {
            //if (!myList.Contains(dr[ColumnTitle].ToString())) / this is a filter to make sure an element is only added once if there are duplications in the column
            //{
                myList.Add(dr[ColumnTitle].ToString());
            //}
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

    #endregion


    #region Create tags
    public static void CreateTag(string tag)
    {
        var asset = AssetDatabase.LoadMainAssetAtPath("ProjectSettings/TagManager.asset");
        if (asset != null)
        { // sanity checking
            var so = new SerializedObject(asset);
            var tags = so.FindProperty("tags");

            var numTags = tags.arraySize;
            // do not create duplicates
            for (int i = 0; i < numTags; i++)
            {
                var existingTag = tags.GetArrayElementAtIndex(i);
                if (existingTag.stringValue == tag) return;
            }

            tags.InsertArrayElementAtIndex(numTags);
            tags.GetArrayElementAtIndex(numTags).stringValue = tag;
            so.ApplyModifiedProperties();
            so.Update();
        }
    }
    #endregion

    #region Attach tags to corresponding gameobjects

    public void AttachAllTags()
    {
        var names= GetDataFromTable(dt, "NAME");
        var tags= GetDataFromTable(dt, "TYPE");
        Debug.Log("I check the tags count: " + tags.Count);

        for (int i = 0; i < names.Count; i++)
        {
            var go = GameObject.Find(names[i]);
            if (go!=null)
            {
                Debug.Log("I found the GO: "+ go);
                Debug.Log("I check the tags: "+ tags[i]);
                go.tag = tags[i];
                Debug.Log("I attached tags to the GO: " + go);
            }
            //GameObject.Find(names[i]).tag = tags[i];
        }

    }


    #endregion



}


