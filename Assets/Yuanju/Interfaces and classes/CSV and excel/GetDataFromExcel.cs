using System;
using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using UnityEngine;
using System.Linq;

public class GetDataFromExcel : MonoBehaviour
{

    //public List<ElementSequenceTable> SequenceTables = new List<ElementSequenceTable>();




    void Awake()
    {

        
        //TODO all the above sentences are the body of the function "SetSettingParameters"

        //Tasks = InitialSettings.Select(x=>x.Task);
        //SubTasks = InitialSettings.Select(x=>x.SubTask);
        //InitialSettingElements = new List<string>(InitialSettings[0].ElementStatus.Keys);
        //FinalSettingElements = new List<string>(FinalSettings[0].ElementStatus.Keys);
        //FinalParamterElements = new List<string>(FinalParametersSetting[0].ParameterSetting.Keys);
        //for (int i = 0; i < FinalParametersSetting.Count; i++)
        //{
        //    Debug.Log("FinalParametersSetting subtask: " + FinalParametersSetting[i].SubTask);
        //    Debug.Log("FinalParametersSetting string condition: " + FinalParametersSetting[i].ParameterSetting[FinalParamterElements[0]]);

        //}
    }

    /// <summary>
    /// get the wk(workbook) by reading the .xlsx file
    /// </summary>
    /// <param name="filePath"></param>
    public static IWorkbook OpenCloseExcelFile(string filePath)
    {
        IWorkbook wk = null;
        string extension = System.IO.Path.GetExtension(filePath);
        try
        {
            FileStream fs = File.OpenRead(filePath);
            if (extension.Equals(".xls"))
            {
                //write the xls data into wk
                wk = new HSSFWorkbook(fs);
            }
            else
            {
                //write the xlsx data into wk
                wk = new XSSFWorkbook(fs);
            }

            fs.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        return wk;
    }

    /// <summary>
    /// read a sheet(initial and final settings) and return a list of ElementSettings instances
    /// </summary>
    /// <param name="sheetName"></param>
    /// <returns></returns>
    //Todo [SetSettingParameters(string sheetName, ref List<ElementSettings> settingList)]
    public static List<ElementSettings> ReadInitialFinalSetting(IWorkbook wk, string sheetName)
    {
        //Debug.Log("the name of the sheet: "+ sheetName);
        List<ElementSettings> settings = new List<ElementSettings>();

        //get the sheet
        ISheet sheet = wk.GetSheet(sheetName);
        IRow ElementRow = sheet.GetRow(0);  //get the first row

        //Debug.Log("the first cell in the first row: "+ row.GetCell(0));
        //Debug.Log("sheet.LastRowNum: " + sheet.LastRowNum);
        for (int i = 1; i <= sheet.LastRowNum; i++) //from 0 to last row number
        {
            ElementSettings elementSetting = new ElementSettings();
            IRow row = sheet.GetRow(i);  //get the current row
            if (row != null)
            {
                //LastCellNum the column number of the current row
                for (int j = 0; j < row.LastCellNum; j++)
                {
                    //data of row i column j
                    if (i > 0)
                    {
                        elementSetting.GeneratorStatus = row.GetCell(1).ToString();
                        elementSetting.Task = row.GetCell(2).ToString();
                        elementSetting.SubTask = row.GetCell(3).ToString();
                        //Debug.Log("elementSetting.SubTask" + elementSetting.SubTask);

                        if (j > 3) //skip the first four columns
                        {

                            if (!sheetName.Contains("parameter"))
                            {
                                StatusData sd = new StatusData();
                                sd = ParseValue(row.GetCell(j));
                                elementSetting.ElementStatus.Add(ElementRow.GetCell(j).ToString(), sd);

                            }
                            else 
                            {
                                elementSetting.ParameterSetting.Add(ElementRow.GetCell(j).ToString(), row.GetCell(j).ToString());
                            }
                        }
                    }
                }
            }
            settings.Add(elementSetting);
        }
        return settings;
    }

    /// <summary>
    /// this method is designed to read the sequence sheets and store the data with the instances of "sequence" class
    /// </summary>
    /// <param name="wk"></param>
    /// <returns></returns>
    public static List<Sequence> ReadSequenceSheets(IWorkbook wk)
    {
        List<Sequence> sequenceMatrixList = new List<Sequence>();

        for (int i = 0; i < wk.NumberOfSheets; i++)
        {
            if (wk.GetSheetName(i).Contains("sequence")) //this is on doubt a sequence sheet
            {
                ISheet sheet = wk.GetSheetAt(i); //get the sheet
                IRow firstRow = sheet.GetRow(0);  //get the first row
                IRow components = sheet.GetRow(2); //get the third row(the second row is the row of the numbers of the elements)
                int ColumnNum = components.LastCellNum;
                int paramterNumber = (int.Parse)(firstRow.GetCell(3).ToString());
                //Debug.Log("wk.GetSheetName(i) parameter number: " + wk.GetSheetName(i) + " " + paramterNumber);
                Debug.Log("paramterNumber: " + paramterNumber);
                //divide the data into several tables
                int TableNum = (sheet.LastRowNum + 1) / ((ColumnNum + 1/*sequence order row*/+ 1/*number row*/+ 1/*task row*/) + paramterNumber/*ParaNum*/);
                Debug.Log("the TableNum sheet.LastRowNum + 1: " + (sheet.LastRowNum + 1));
                Debug.Log("ColumnNum + 1 + paramterNumber: " + (ColumnNum + 1 + paramterNumber));
                Debug.Log("TableNum: " + TableNum);
                Debug.Log("1/1: " + (int)1/1);
                TableNum = 1;
                for(int k = 0; k < TableNum; k++) //loop for the different tables within a task sequence sheet
                {
                    Sequence sequence = new Sequence();
                    ActuatorConditions actuatorConditions;
                    // write the header row
                    var headerRow = sheet.GetRow(2); // the header row is always the same for the tables
                    //we know that we get the task at the starting of a table
                    var subTask = sheet.GetRow(k * ((ColumnNum + 1/*order row*/+ 1/*nunmber row*/+ 1/*task row*/) + paramterNumber/*ParaNum*/)).GetCell(1);//get the ith table's task 
                    sequence.SubTask = subTask.ToString();
                    Debug.Log("the the table: ");
                    for(int l = 1; l < ColumnNum; l++) //starts from "Modeaccensione pompa" and ends at "Pressostato setting" it seems that .lastCellNum sometimes take some empty cells
                    {
                        if (sheet.GetRow(k * (ColumnNum + 3 + paramterNumber) + 3).GetCell(l).ToString() != "NULL") //when the sequence order's value is "NULL", the column is not stored
                        {
                            actuatorConditions = new ActuatorConditions(); //a column in the table
                            actuatorConditions.Name = headerRow.GetCell(l).ToString(); //the column represent an element to be checked 
                            actuatorConditions.SequenceOrder = int.Parse(sheet.GetRow(k * (ColumnNum + 3 + paramterNumber) + 3).GetCell(l).ToString());
                            Debug.Log("columns in the matrices: " + actuatorConditions.Name);
                            Debug.Log("actuatorConditions.SequenceOrder: " + actuatorConditions.SequenceOrder);
                            for (int j = k * (ColumnNum + 3 + paramterNumber) + 3 + 1; j < (k + 1) * (ColumnNum + 3 + paramterNumber/*ParaNum*/); j++) //jth row, skip the task number and components
                            {
                                StatusData sd = new StatusData();
                                var cell = sheet.GetRow(j).GetCell(l);
                                sd = ParseValue(cell);
                                sd.Name = sheet.GetRow(j).GetCell(0).ToString();
                                //Debug.Log("Jth row, first element, should be the names of the elements in the sequence tables: " + sd.Name);
                                actuatorConditions.Prerequisites.Add(sd);
                            }
                            sequence.ActuatorToCheck.Add(actuatorConditions);
                        }
                    }
                    sequenceMatrixList.Add(sequence);
                }
            }
        } 
        return sequenceMatrixList;
    }


    /// <summary>
    /// A method to parse the values read in the excel file, can be used for all the different sheets
    /// </summary>
    /// <param name="cellValue"></param>
    /// <returns></returns>
    public static StatusData ParseValue(ICell cellValue)
    {
        StatusData sd = new StatusData();
        var data = cellValue.ToString();
        var dataLength = data.Length;

        //A new method to parse the data
        if (data == "NULL")
        {
            sd.Status = null;
        }
        else
        {
            int statusPure;
            bool success = Int32.TryParse(data, out statusPure);
            if (success) //if the cell contains a "pure" number
            {
                sd.Group = null;
                sd.Status = statusPure;
                sd.Symbol = null;
            }
            if (!success)
            {

                //Debug.Log("statusWithoutGroup data: " + data);
                int statusWithoutGroup;
                string numberWithoutGroup = data.Remove(0, 1);
                success = Int32.TryParse(numberWithoutGroup, out statusWithoutGroup);
                if (success) //if the cell contains group + number
                {
                    sd.Group = data[0];
                    sd.Status = statusWithoutGroup;
                    sd.Symbol = null;
                }
                if (!success)
                {
                    int statusWithoutSymbol;
                    var numberWithoutSymbol = data.Remove(data.Length - 1, 1);

                    success = Int32.TryParse(numberWithoutSymbol, out statusWithoutSymbol);
                    if (success) //if the cell contains number + symbol
                    {
                        sd.Group = null;
                        sd.Status = statusWithoutSymbol;
                        sd.Symbol = data[data.Length - 1];
                    }
                    if (!success) //the cell must contains group + number + symbol
                    {
                        int statusWithoutSymbolAndGroup;
                        var numberWithoutSymbolAndGroup = data.Remove(data.Length - 1, 1);
                        numberWithoutSymbolAndGroup = numberWithoutSymbolAndGroup.Remove(0, 1);
                        success = Int32.TryParse(numberWithoutSymbolAndGroup, out statusWithoutSymbolAndGroup);
                        sd.Group = data[0];
                        sd.Status = statusWithoutSymbolAndGroup;
                        sd.Symbol = data[data.Length - 1];
                    }
                }
            }
        }
        return sd;
    }
}
