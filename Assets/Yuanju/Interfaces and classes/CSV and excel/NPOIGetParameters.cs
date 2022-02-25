using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEngine;
//using NPOI.XSSF.UserModel;
//using NPOI.HSSF.UserModel;
//using NPOI.SS.UserModel;

public class NPOIGetParameters : MonoBehaviour
{
    // Start is called before the first frame update
    private IWorkbook wk;
    private ISheet parameterSheet;
    public static DataTable parameterTable;
    void Awake()
    {
        ReadParametersFromSheets("Assets/Yuanju/Values and situations plus Parameters.xlsx");
        for (int j = 0; j < parameterTable.Rows.Count; j++)
        {
            for (int i = 0; i < parameterTable.Columns.Count; i++)
            {
                Debug.Log("parameterTable.Columns[j][i]: "+ parameterTable.Rows[j][i]);
            }
        }

        //for (int j = 0; j < parameterTable.Columns.Count; j++)
        //{
        //    Debug.Log("parameterTable.Columns[j][i] name: " + parameterTable.Columns[j].ColumnName);
        //}
    }

    private void ReadParametersFromSheets(string filePath)
    {
        wk = null;
        string extension = Path.GetExtension(filePath);
        FileStream fs = File.OpenRead(filePath);
        if (extension.Equals(".xls"))
        {
            wk=new HSSFWorkbook(fs);
        }
        if (extension.Equals(".xlsx"))
        {
            wk = new XSSFWorkbook(fs);
        }
        fs.Close();
        Debug.Log("this is the count of the sheets: " + wk.NumberOfSheets);
        //get the sheet whose name contains "parameter"
        for (int i = 0; i < wk.NumberOfSheets; i++)
        {
            if (wk.GetSheetName(i).Contains("final settings parameters"))
            {
                parameterSheet = wk.GetSheet(wk.GetSheetName(i));
                Debug.Log("this is the parameter sheet: " + wk.GetSheetName(i));
            }
        }

        //add the table columns names
        parameterTable = new DataTable();
        for (int j = 0; j < parameterSheet.GetRow(0).LastCellNum; j++)
        {
            Debug.Log("HERE I ADD THE HEADER ROW: " + parameterSheet.GetRow(0).GetCell(j).ToString());
            parameterTable.Columns.Add(parameterSheet.GetRow(0).GetCell(j).ToString());
            parameterTable.Columns[j].DataType = Type.GetType("System.String");
        }

        //add the column values
        Debug.Log("HERE I parameterSheet.LastRowNum: " + parameterSheet.LastRowNum);
        for (int j = 1; j < parameterSheet.LastRowNum + 1; j++)   // it is very strange that row number count starts from 1 while column count starts from 0
        {
            DataRow dr = parameterTable.NewRow();
            Debug.Log("dr column count: " + dr.Table.Columns.Count);
            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                Debug.Log("HERE I ADD THE cell number: " + parameterSheet.GetRow(j).LastCellNum.ToString());
                Debug.Log("HERE I ADD THE cells: " + parameterSheet.GetRow(j).GetCell(i).ToString());
                //Debug.Log("HERE I ADD THE dr[j]: " + dr[j].GetType());
                dr[i] = parameterSheet.GetRow(j).GetCell(i);
            }
            parameterTable.Rows.Add(dr);
        }
    }

}
