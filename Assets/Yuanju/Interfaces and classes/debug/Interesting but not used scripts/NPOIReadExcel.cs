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

public class NPOIReadExcel : MonoBehaviour
{
    public static List<string> SubTasks = new List<string>();
    public static List<string> Tasks = new List<string>();
    public static List<string> GeneratorStatus = new List<string>();
    private List<string> columnName = new List<string>(); //all the components's names, operable or not plus the status and task.
    public static List<string> ActuatorNames = new List<string>(); //all the actuators' names
    public static Dictionary<string, Dictionary<string, int>> TaskComponentInitialSettings;
    public static List<DataTable> InitialSettings;
    public static Dictionary<string, Dictionary<string, int>> TaskComponentNecessity;
    public static List<DataTable> Necessity;
    
    public static List<DataTable> FinalSettings;

    private List<List<int>> ComponentsListsToCheck=new List<List<int>>();
    private IWorkbook wk; //"Iworkbook" is provided by NPOI
    private bool ComponentsFullyAdded;
    private bool FirstRead;

    void Awake()
    {
        ComponentsFullyAdded = false;
        FirstRead = true;
        Debug.Log("TaskComponentInitialSettings.Keys.count: ");
        ReadFromExcelFile("Assets/Yuanju/Values and situations plus Parameters.xlsx");

        for(int i = 0; i < FinalSettings.Count; i++) {
            //Debug.Log("final settings in awake: " + FinalSettings[i].TableName);
        }

        for (int i = 0; i < FinalSettings[0].Rows.Count; i++)
        {
            //Debug.Log("final settings in read excel: " + FinalSettings[0].Rows[0][1]);
        }
        Debug.Log("TaskComponentInitialSettings.Keys.count: ");
        Debug.Log("TaskComponentInitialSettings.Keys.count: " + TaskComponentInitialSettings.Keys.Count);
        foreach (var key in TaskComponentInitialSettings.Keys)
        {
            Debug.Log("keys: " + key);
            //for (int i = 0; i < TaskComponentInitialSettings[key].Count; i++)
            //{
                
            //}
        }

        for (int i = 0 + 3/*skip the number,generator status and task */  ; i < FinalSettings[0].Columns.Count; i++)
        {
            //Debug.Log("final settings in read excel: " + FinalSettings[0].Rows[0][1]);
            ActuatorNames.Add(FinalSettings[0].Columns[i].ColumnName);
            Debug.Log("the name of the actuators: "+ ActuatorNames[i-3]);
        }

        for (int i = 0; i < SubTasks.Count; i++)
        {

            //Debug.Log("TaskComponentInitialSettings POMPA MAN-0-AUT: " + TaskComponentInitialSettings[SubTasks[i]]["POMPA MAN-0-AUT"]  );
            //Debug.Log("TaskComponentInitialSettings Liquid crystal display: " + TaskComponentInitialSettings[SubTasks[i]]["Liquid crystal display"]  );
        }

    }

    public void ReadFromExcelFile(string filePath)
    {
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
            TaskComponentInitialSettings = ReadSetting("initial settings");
            //TaskComponentNecessity = ReadSetting("necessity");
            //TaskComponentFinalSettings = ReadSetting("final settings");
        FinalSettings = ReadSettingToDataTable("final settings");
        //InitialSettings = ReadSettingToDataTable("initial settings");
        //Debug.Log("InitialSettings[0].Columns[i].count: " + InitialSettings[0].Columns.Count);

        for (int i = 0; i < FinalSettings.Count; i++)
        {
            Debug.Log("final setting data table: " + FinalSettings[i].TableName);
            Debug.Log("FinalSettings[i].Rows[0][0]: " + FinalSettings[i].Rows[0][0]);
            Debug.Log("FinalSettings[i].Rows[0][1]: " + FinalSettings[i].Rows[0][1]);
            Debug.Log("FinalSettings[i].Rows[0][2]: " + FinalSettings[i].Rows[0][2]);
            StatusData value1 = (StatusData)FinalSettings[i].Rows[0][3];
            StatusData value2 = (StatusData)FinalSettings[i].Rows[0][4];
            Debug.Log("FinalSettings[i].Rows[0][3]: " + value1.Status);
            Debug.Log("FinalSettings[i].Rows[0][4]: " + value2.Status);
            StatusData value3 = (StatusData)FinalSettings[i].Rows[0]["Valvola intercettazione acqua"];
            Debug.Log("FinalSettings[i].Rows[0][Valvola intercettazione acqua]: " + value3.Status);
        }

        for (int i = 0; i < FinalSettings[0].Columns.Count; i++)
        {
            Debug.Log("final setting data table in the first table: " + FinalSettings[0].Columns[i].ColumnName);
        }
        //}
        //catch (Exception e)
        //{
        //    Debug.Log(e.Message);
        //}
    }

    private Dictionary<string, Dictionary<string, int>> ReadSetting(string sheetName)
    {
        Debug.Log("the name of the sheet: "+ sheetName);
        var myDictionary = new Dictionary<string, Dictionary<string, int>>();

        //get the sheet
        ISheet sheet = wk.GetSheet(sheetName);
        IRow row = sheet.GetRow(0);  //get the first row
        //Debug.Log("the first cell in the first row: "+ row.GetCell(0));
        Debug.Log("sheet.LastRowNum: " + sheet.LastRowNum);
        for (int i = 0; i <= sheet.LastRowNum; i++) //from 0 to last row number
        {
            Dictionary<string, int> ComponentSettings = new Dictionary<string, int>();
            ComponentSettings.Clear();
            row = sheet.GetRow(i);  //get the current row
            if (row != null)
            {
                //LastCellNum the column number of the current row
                for (int j = 0; j < row.LastCellNum; j++)
                {
                    //data of row i column j
                    string value = row.GetCell(j).ToString();

                    if (j > 3) //skip the first four columns
                    {
                        Debug.Log("the cell's value for status: " + value);
                        if (i == 0 && FirstRead/*!ComponentsFullyAdded*/) //the first row
                        {
                            columnName.Add(value); // add the components names into a list
                            
                        }
                        if(i > 0)
                        {
                            ComponentSettings.Add(columnName[j - 4], Int32.Parse(value)); //map the components with status
                        }
                    }
                }

                if (i > 0)
                {
                    if (FirstRead)
                    {
                        Debug.Log("the cell's value for task: " + row.GetCell(2).ToString());
                        SubTasks.Add(row.GetCell(3).ToString()); //add all the sub tasks
                        Tasks.Add(row.GetCell(2).ToString()); //add all the tasks
                        GeneratorStatus.Add(row.GetCell(1).ToString()); //add the starting status of the generator
                    }
                    
                    myDictionary.Add(SubTasks[i - 1], ComponentSettings);
                }//the logic should be when the system read the first row, parse each cell's meaning, get the task's column number.
                //Debug.Log("row.GetCell(2): " + row.GetCell(2).ToString());
            }
            //Debug.Log("\n");
            //ComponentsFullyAdded = true;
            
        }
        FirstRead = false;
        return myDictionary;
    }


    /// <summary>
    /// TASK(string)->list of dictionary, key: components(string)->value: list of Data(the statuses)
    /// </summary>
    /// <returns></returns>

    private Dictionary<string, List<Dictionary<string,List<int>>>> ReadSequence()
    {
        var myDictionary = new Dictionary<string, List<Dictionary<string, List<int>>>>();

        //get the sheet
        ISheet sheet = wk.GetSheet("sequence");
        IRow row = sheet.GetRow(0);  //get the first row
        //var FirstTask = row.GetCell(1);//get the task N1
        var components= sheet.GetRow(1);
        int ColumnNum = components.LastCellNum;
        //Debug.Log("ColumnNum: " + ColumnNum);

        //divide the data into several tables
        int TableNum = (sheet.LastRowNum + 1) / (ColumnNum + 1);
        Debug.Log("TableNum: " + TableNum);
        for (int i = 0; i < TableNum; i++) //loop for the different tables
        {

            //we know that we get the task at the starting of a table
            var Task = sheet.GetRow(i * (ColumnNum +1) ).GetCell(1);//get the ith table's task 
            //Debug.Log("ColumnNum: " + ColumnNum);
            //loop for the rows of a table, note that the rows numbers are the amount of the components + 1
            for (int j = i * (ColumnNum + 1) + 2; j < (i + 1) * (ColumnNum + 1); j++)  //skip the task and the components lines
            {
                //Debug.Log("J's values: " + j);
                //loop for the  columns in a row
                for (int k = 0; k < ColumnNum/*sheet.GetRow(j).LastCellNum*/; k++) //using .lastCellNum will cause problem, it seems that .lastCellNum take some empty cells
                {
                    //if (sheet.GetRow(j).LastCellNum == 25)
                    //{
                    //var range = "E3:H8";
                    //var cellRange = CellRangeAddress.ValueOf(range);


                    Debug.Log("every element: " + sheet.GetRow(j).GetCell(k));
                    
                        //}

                        //Debug.Log("columns= sheet.GetRow(j).LastCellNum: " + sheet.GetRow(j).LastCellNum);
                        //Debug.Log("every element: "+ sheet.GetRow(j).GetCell(k));
                }
            }

        }

        //for (int i = 0; i < 40; i++)
        //{
        //    Debug.Log("sheet.GetRow(i).LastCellNum: "+ sheet.GetRow(i).LastCellNum);
        //}

        //var cr = new CellReference("D5");
        //var rowTest = sheet.GetRow(cr.Row);
        //var cell = rowTest.GetCell(cr.Col);
        //Debug.Log("get column cell test: "+ cell);
        //Debug.Log("get cr.Row test : " + cr.Row);
        //Debug.Log("get cr.Col test : " + cr.Row);
        

        return myDictionary;
    }


    private List<DataTable> ReadSettingToDataTable(string sheetName)
    {
        List<DataTable> myDataTableLst = new List<DataTable>();

        //get the sheet
        ISheet sheet = wk.GetSheet(sheetName);
        IRow row = sheet.GetRow(0);  //get the first row
        var columnNum = row.LastCellNum; //imp lastCellNum can never be iterated
        for (int i = 1; i <= sheet.LastRowNum; i++)  //loop for the rows (is also the tables)
        {
            row = sheet.GetRow(i);  //get the current row
            
            DataTable ComponentSettingsdt = new DataTable();
            var Task = row.GetCell(2);
            //LastCellNum the column number of the current row

            for (int j = 0; j < columnNum; j++) //must configurate the data table and then fill in the table
            {
                ComponentSettingsdt.Columns.Add(sheet.GetRow(0).GetCell(j).ToString()); //get the header row
                if (j <3 || j >= ReadCSV.componentNames.Count+3)  //should be more generic here
                {
                    ComponentSettingsdt.Columns[j].DataType = Type.GetType("System.String");
                }
                else if (j > 2)
                {
                    ComponentSettingsdt.Columns[j].DataType = Type.GetType("StatusData");
                }
            }

            DataRow dr = ComponentSettingsdt.NewRow();
            for (int j = 0; j < columnNum; j++) //fill in the data table, loop for the tables(rows)
            {
                //string value = row.GetCell(j).ToString();
                StatusData cellData = new StatusData();


                var data = sheet.GetRow(i).GetCell(j).ToString();
                var dataLength = data.Length;

                if (2 < j && j < ReadCSV.componentNames.Count + 3)
                {
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
                                var numberWithoutSymbol = data.Remove(data.Length - 1, 1);

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
                    dr[j] = cellData;
                }
                else
                {
                    dr[j] = data; //the task or status of the generator
                }
            }
            ComponentSettingsdt.Rows.Add(dr);
            ComponentSettingsdt.TableName = Task.ToString();
            myDataTableLst.Add(ComponentSettingsdt);
            //Debug.Log("row.GetCell(2): " + row.GetCell(2).ToString());

            //Debug.Log("\n");
            //ComponentsFullyAdded = true;



        }
        return myDataTableLst;
    }
}
