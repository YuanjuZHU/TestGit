using System;
using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using UnityEngine;

/// <summary>
/// the scripts to extract the data from the new version excel file
/// </summary>
public class NPOIGetDatatable : MonoBehaviour
{
    private IWorkbook wk;
    static public List<DataTable> SequenceDatatables= new List<DataTable>();
    static public List<string> OperableComponentNames = new List<string>();
    static Dictionary<char, List<GameObject>> sequenceComponentsGroup = new Dictionary<char, List<GameObject>>();
    static List<char?> sequenceGroupIndex = new List<char?>();
    private ISheet SequenceSheet; 

    void Awake()
    {
        //todo use a public variable to hold the name of the file.
        SequenceDatatables.AddRange(ReadFromExcelGetDatatable("Assets/Yuanju/Values and situations plus Parameters.xlsx"));
        //Debug.Log("the first table' rows: " + SequenceDatatables[0].Columns.Count);
        //Debug.Log("the table's last value in columns: " + VARIABLE.Columns[VARIABLE.Columns.Count]);
        //GetComponentSequenceColumns(SequenceDatatables[0], "Red start handle");
        Debug.Log("SequenceDatatables[0].Columns.Count: " + SequenceDatatables[0].Columns.Count);
        foreach (DataColumn column in SequenceDatatables[0].Columns)
        {
            OperableComponentNames.Add(column.ColumnName);
            //Debug.Log("OperableComponentNames: " + column.ColumnName);
        }
        Debug.Log("table number: " + SequenceDatatables.Count);
        for (int i = 0; i < OperableComponentNames.Count; i++)
        {
            Debug.Log("Operable Component Names: " + OperableComponentNames[i]);
        }

        for (int i = 0; i < SequenceDatatables.Count; i++)
        {
            if (SequenceDatatables[i].TableName == "Activation 1")
            {

                var sd0 = (StatusData)SequenceDatatables[i].Rows[1][0];
                var sd1 = (StatusData)SequenceDatatables[i].Rows[1][1];
                var sd2 = (StatusData)SequenceDatatables[i].Rows[2][0];
                var sd3 = (StatusData)SequenceDatatables[i].Rows[2][1];
                Debug.Log("check the values in the datatable: " + sd0.Status);
                Debug.Log("check the values in the datatable: " + sd1.Status);
                Debug.Log("check the values in the datatable: " + sd2.Status);
                Debug.Log("check the values in the datatable: " + sd3.Status);
            }
        }


    }

    public List<DataTable> ReadFromExcelGetDatatable(string filePath)
    {
        List<DataTable> dtLst = new List<DataTable>();
        wk = null;
        string extension = System.IO.Path.GetExtension(filePath);
        //try
        //{
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

        //get the sequence sheets, here we only want to deal with the sequence sheets
        int SheetCount = wk.NumberOfSheets;//get the number of the sheets
        List<string>  SequenceSheetsName = new List<string>();//store the name of the sequence sheets
        for (int i = 0; i < SheetCount; i++)
        {
            if (wk.GetSheetName(i).Contains("sequence"))
            {
                SequenceSheetsName.Add(wk.GetSheetName(i));
            }
        }

        for (int m = 0; m < SequenceSheetsName.Count; m++) //deal with every sequence sheet, note that there can be several tables within one sheet in the case that an actuator can be operated several times
        {
            SequenceSheet = wk.GetSheet(SequenceSheetsName[m]); 


            //get the sheet

            IRow row = SequenceSheet.GetRow(0);  //get the first row
                                                 //var FirstTask = row.GetCell(1);//get the task N1
            var components = SequenceSheet.GetRow(2);
            int ColumnNum = components.LastCellNum;
            Debug.Log("the column number of the table: "+ ColumnNum);
            //divide the data into several tables
            int TableNum = (SequenceSheet.LastRowNum + 1) / ((ColumnNum + 1/*order row*/+1/*number row*/+1/*task row*/) + 8/*ParaNum*/);
            for (int i = 0; i < TableNum; i++) //loop for the different tables within a task sequence sheet
            {
                var dataTable = new DataTable();
                // write the header row
                var headerRow = SequenceSheet.GetRow(2); // the header row is always the same for the tables
                for (int l = 1; l < headerRow.LastCellNum; l++) // we can use ColumnNum instead of the "headerRow.LastCellNum"
                {
                    //Debug.Log("headerRow.GetCell(i).ToString(): "+ headerRow.GetCell(i));
                    dataTable.Columns.Add(headerRow.GetCell(l).ToString());
                    dataTable.Columns[l - 1].DataType = Type.GetType("StatusData");  //set the data type for the data table's elements
                }
                //Debug.Log("dataTable.Columns.count: " + dataTable.Columns.Count);
                //we know that we get the task at the starting of a table
                var Task = SequenceSheet.GetRow(i * ((ColumnNum + 1/*order row*/+ 1/*nunmber row*/+ 1/*task row*/) + 8/*ParaNum*/)).GetCell(1);//get the ith table's task 
                for (int j = i * (ColumnNum  + 3 + 8)+3 ; j < (i + 1) * (ColumnNum +3 + 8/*ParaNum*/) ; j++) //jth row, skip the task number and components
                {
                    //loop for the columns in a row

                    DataRow dr = dataTable.NewRow();//TODO Set the datatype for the rows: 

                    //TODO column = new DataColumn();
                    //TODO column.DataType = System.Type.GetType("System.Int32");


                    for (int k = 0; k < ColumnNum - 1 /*sheet.GetRow(j).LastCellNum*/; k++) //using .lastCellNum will cause problem, it seems that .lastCellNum take some empty cells
                    {
                        //if (sheet.GetRow(j).LastCellNum == 25)
                        //{
                        //var range = "E3:H8";
                        //var cellRange = CellRangeAddress.ValueOf(range);
                        StatusData cellData = new StatusData();
                        var data = SequenceSheet.GetRow(j).GetCell(k + 1).ToString();
                        var dataLength = data.Length;
                        //////if (data == "NULL")
                        //////{
                        //////    cellData.Status = null;
                        //////}
                        //////else if (dataLength == 1)
                        //////{
                        //////    cellData.Status = Int32.Parse(data);
                        //////}
                        //////else if (dataLength == 2)
                        //////{
                        //////    cellData.Status = Int32.Parse(data[0].ToString());
                        //////    //cellData.Status= Int32.Parse(data[0].ToString());  //TODO should be Int32.Parse
                        //////    cellData.Symbol = data[1];
                        //////}
                        //////else if (dataLength == 3)
                        //////{
                        //////    cellData.Group = data[0];
                        //////    cellData.Status = Int32.Parse(data[1].ToString());
                        //////    cellData.Symbol = data[2];
                        //////}

                        //A new method to parse the data
                        if (data == "NULL")
                        {
                            cellData.Status = null;
                        }
                        else
                        {
                            int statusPure;
                            bool success = Int32.TryParse(data, out statusPure);
                            if (success) //if the cell contains a "pure" number
                            {
                                cellData.Group = null;
                                cellData.Status = statusPure;
                                cellData.Symbol = null;

                            }
                            if (!success)
                            {
                                int statusWithoutGroup;
                                var numberWithoutGroup = data.Remove(0, 1);
                                success = Int32.TryParse(numberWithoutGroup, out statusWithoutGroup);
                                if (success) //if the cell contains group + number
                                {
                                    cellData.Group = data[0];
                                    cellData.Status = statusWithoutGroup;
                                    cellData.Symbol = null;
                                }

                                if (!success)
                                {
                                    int statusWithoutSymbol;
                                    var numberWithoutSymbol = data.Remove(data.Length-1, 1);
                                    
                                    success = Int32.TryParse(numberWithoutSymbol, out statusWithoutSymbol);
                                    if (success) //if the cell contains number + symbol
                                    {
                                        cellData.Group = null;
                                        cellData.Status = statusWithoutSymbol;
                                        cellData.Symbol = data[data.Length - 1];
                                    }
                                    if (!success) //the cell must contains group + number + symbol
                                    {
                                        int statusWithoutSymbolAndGroup;
                                        var numberWithoutSymbolAndGroup = data.Remove(data.Length - 1, 1);
                                        numberWithoutSymbolAndGroup = numberWithoutSymbolAndGroup.Remove(0, 1);
                                        success = Int32.TryParse(numberWithoutSymbolAndGroup, out statusWithoutSymbolAndGroup);
                                        cellData.Group = data[0];
                                        cellData.Status = statusWithoutSymbolAndGroup;
                                        cellData.Symbol = data[data.Length - 1];
                                    }
                                }
                                

                            }
                        }
                        
                        





                        //imp ?????????????????????????????
                        //Type ellDataType = cellData.GetType();
                        //PropertyInfo[] properties = ellDataType.GetProperties();

                        //foreach (var property in properties)
                        //{
                        //    dr[k] = property.GetValue(cellData, null);
                        //}
                        //imp ?????????????????????????????

                        //cellData.Symbol

                        //dr[k] = sheet.GetRow(j).GetCell(k+1);
                        //dr[k].DataType = typeof(StatusData);
                        dr[k] = cellData;


                        //}

                        //Debug.Log("columns= sheet.GetRow(j).LastCellNum: " + sheet.GetRow(j).LastCellNum);
                        //Debug.Log("every element: "+ sheet.GetRow(j).GetCell(k));
                    }
                    dataTable.Rows.Add(dr);
                }
                Debug.Log("Jth row finish reading ");
                dataTable.TableName = Task.ToString();
                dtLst.Add(dataTable);
            }
        }

        




        //get every row, so that we can get every value

        //string[] values = "1";//TODO the value
        
        //DataRow dr = dt.NewRow();
        //for (int i = 0; i < values.Length; i++)
        //{
        //    dr[i] = values[i];
        //}
        //dt.Rows.Add(dr);

        return dtLst;
    }


}
