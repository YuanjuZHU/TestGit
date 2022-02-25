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

public class TestNPOIGetDatatable : MonoBehaviour
{
    private IWorkbook wk;
    static public List<DataTable> SequenceDatatables= new List<DataTable>();
    static public List<string> OperableComponentNames = new List<string>();
    static Dictionary<char, List<GameObject>> sequenceComponentsGroup = new Dictionary<char, List<GameObject>>();
    static List<char?> sequenceGroupIndex = new List<char?>();
    void Awake()
    {

        SequenceDatatables = ReadFromExcelGetDatatable("Assets/Yuanju/Values and situations.xlsx");
        //Debug.Log("the first table' rows: " + SequenceDatatables[0].Columns.Count);
        for (int i = 0; i < SequenceDatatables[0].Columns.Count; i++)
        {
            //Debug.Log("the name of the columns: "+ SequenceDatatables[0].Columns[i].ColumnName);
            //Debug.Log("the data type of the columns: "+ SequenceDatatables[0].Columns[i].DataType);
        }
        //Debug.Log("the table's last value in columns: " + VARIABLE.Columns[VARIABLE.Columns.Count]);
        //GetComponentSequenceColumns(SequenceDatatables[0], "Red start handle");
        //Debug.Log("dt rows: " + SequenceDatatables[0].Rows.Count);
        foreach (DataColumn column in SequenceDatatables[0].Columns)
        {
            OperableComponentNames.Add(column.ColumnName);
        }
    }

    ////////////public static void GetComponentSequenceColumns(DataTable dt, string columnTitle)
    ////////////{
    ////////////    Debug.Log(" ********************************Sequence check for: "+ columnTitle);
    ////////////    DataRow[] drs = dt.Select(); //get all the rows of the data table
    ////////////    DataTable finalSettingdt=new DataTable();
    ////////////    //DataRow finialSettingRow;
    ////////////    //find out the corresponding final setting
    ////////////    for (int i = 0; i < NPOIReadExcel.FinalSettings.Count; i++)
    ////////////    {
    ////////////        if (NPOIReadExcel.FinalSettings[i].TableName == "start the generator" /*PanelManager.listToggleText[2]*/)
    ////////////        {
    ////////////            finalSettingdt = NPOIReadExcel.FinalSettings[i];
    ////////////        }
    ////////////    }
    ////////////    var lastComponentFinalSetting = (StatusData)finalSettingdt.Rows[0][columnTitle];
        
    ////////////    for (int i=0; i<drs.Length; i++ )
    ////////////    {
    ////////////        //Debug.Log("the elements of the Red start: " +  (StatusData)dr[columnTitle].Status);
    ////////////        StatusData sd = (StatusData)drs[i][columnTitle];//get the elements of a column 
    ////////////        if (sd.Group == null)
    ////////////        {
    ////////////            //////check the status of the component, if the last modified component is not put a correct final setting 
    ////////////            ////if (columnTitle == OperableComponentNames[i] && SetScenes.ScriptComponents[OperableComponentNames[i]].Status != lastComponentFinalSetting.Status)
    ////////////            ////{
    ////////////            ////    Debug.Log(string.Format("the component: {0} should be switched to: {1}", OperableComponentNames[i], sd.Status));
    ////////////            ////    //TODO warning the student in the game at real time
    ////////////            ////}
    ////////////            //if the cell is not "NULL", and the component is not put to a  correct status
    ////////////            if (sd.Status.HasValue)
    ////////////            {
    ////////////                if(SetScenes.ScriptComponents[OperableComponentNames[i]].Status != (int)sd.Status) 
    ////////////                {
    ////////////                    Debug.Log(string.Format("the component: {0} should be switched to: {1} before modifying {2}", OperableComponentNames[i], sd.Status, columnTitle));
    ////////////                    Debug.Log(string.Format("the component: {0} has status: {1} ", OperableComponentNames[i], SetScenes.ScriptComponents[OperableComponentNames[i]].Status));
    ////////////                    //TODO warning the student in the game, blink this component, red and original color
    ////////////                    var blink = GameObject.Find(OperableComponentNames[i]).GetComponent<BlinkMaterial>();
    ////////////                    blink.enabled = true;
    ////////////                    blink.IsBlink = true;
    ////////////                } else
    ////////////                {
    ////////////                    Debug.Log(string.Format("the component: {0} has the correct status: {1} before modifying {2}", OperableComponentNames[i], sd.Status,columnTitle));
    ////////////                }
                    
    ////////////            }

    ////////////        }
    ////////////        //add all the different groups to a list 
    ////////////        else if(!sequenceGroupIndex.Contains(sd.Group))
    ////////////        {         
    ////////////            sequenceGroupIndex.Add(sd.Group);
    ////////////        }

            

    ////////////    }

    ////////////    //use group name to find the components
    ////////////    //loop for the component groups  
    ////////////    Debug.Log("the count of the group: " + sequenceGroupIndex.Count);
    ////////////    sequenceComponentsGroup.Clear();
    ////////////    CheckGroupsStatusesSequence(dt,columnTitle);
    ////////////}

 //////   public static void GetComponentFinalSettingChecked()
	//////{
	//////    var names = TestNPOIGetDatatable.OperableComponentNames;
	//////    for(int i = 0; i < names.Count; i++) {
	//////        Debug.Log("status data names: " + names[i]);
	//////        Debug.Log("status data: " + (StatusData)finalSettingdt.Rows[0][names[i]]);
	//////        var sd = (StatusData)finalSettingdt.Rows[0][names[i]];
	//////        if(SetScenes.ScriptComponents[names[i]].Status != sd.Status) {
	//////            Debug.Log(string.Format("the component: {0} should be switched to: {1}", names[i], sd.Status));
	//////            //TODO warning the student in the game
	//////            Evaluation.isFinalSettingCorrect = false;
	//////        }
	//////    }
 //////   }



 ////////////   private static void CheckGroupsStatusesSequence(DataTable dt, string columnTitle)
	////////////{
	////////////    DataRow[] drs = dt.Select(); //get all the rows of the data table
 ////////////       for(int j = 0; j < sequenceGroupIndex.Count; j++) {
 ////////////           Debug.Log("the group indexes[j]: " + sequenceGroupIndex[j]);
 ////////////           char? constraintSymbole = new char();
 ////////////           int? constraintStatus = 0;
 ////////////           List<GameObject> myList = new List<GameObject>();

 ////////////           for(int i = 0; i < drs.Length; i++) {
 ////////////               //Debug.Log("the elements of the Red start: " +  (StatusData)dr[columnTitle].Status);
 ////////////               StatusData sd = (StatusData)drs[i][columnTitle]; //get the elements of a column 

 ////////////               if(sd.Group == sequenceGroupIndex[j]) {
 ////////////                   myList.Add(GameObject.Find(OperableComponentNames[i]));
 ////////////                   constraintSymbole = sd.Symbol;
 ////////////                   constraintStatus = sd.Status;
 ////////////               }
 ////////////           }

 ////////////           sequenceComponentsGroup.Add((char)sequenceGroupIndex[j], myList);
 ////////////           //loop inside a group, because for  the components of a group may have different values in excel's cells
 ////////////           var groupSum = 0;
 ////////////           for(int k = 0; k < sequenceComponentsGroup[(char)sequenceGroupIndex[j]].Count; k++) {
 ////////////               var componentStatus = sequenceComponentsGroup[(char)sequenceGroupIndex[j]][k].GetComponent<IGeneratorComponent>().Status;
 ////////////               groupSum += componentStatus;
 ////////////           }
 ////////////           Debug.Log("groupSum: " + groupSum);

 ////////////           if(constraintSymbole == '+' && groupSum < constraintStatus) {
 ////////////               Debug.Log(string.Format("the component of group: {0} should be at least: {1}", sequenceGroupIndex[j], constraintStatus));
 ////////////               //TODO blink the group of the component, red and original color
 ////////////               for(int i = 0; i < sequenceComponentsGroup[(char)sequenceGroupIndex[j]].Count; i++) {
 ////////////                   var blink = sequenceComponentsGroup[(char)sequenceGroupIndex[j]][i].GetComponent<BlinkMaterial>();
 ////////////                   blink.enabled = true;
 ////////////                   blink.IsBlink = true;
 ////////////               }
 ////////////           }

 ////////////           if(constraintSymbole == '-' && groupSum > constraintStatus) {
 ////////////               Debug.Log(string.Format("the component of group: {0} should be at most: {1}", sequenceGroupIndex[j], constraintStatus));
 ////////////               //TODO blink the group of the component, red and original color
 ////////////               for(int i = 0; i < sequenceComponentsGroup[(char)sequenceGroupIndex[j]].Count; i++) {
 ////////////                   var blink = sequenceComponentsGroup[(char)sequenceGroupIndex[j]][i].GetComponent<BlinkMaterial>();
 ////////////                   blink.enabled = true;
 ////////////                   blink.IsBlink = true;
 ////////////               }
 ////////////           }

 ////////////           if(constraintSymbole == '=' && groupSum != constraintStatus) {
 ////////////               Debug.Log(string.Format("the component of group: {0} should be exactly: {1}", sequenceGroupIndex[j], constraintStatus));
 ////////////               //TODO blink the group of the component, red and original color
 ////////////               for(int i = 0; i < sequenceComponentsGroup[(char)sequenceGroupIndex[j]].Count; i++) {
 ////////////                   var blink = sequenceComponentsGroup[(char)sequenceGroupIndex[j]][i].GetComponent<BlinkMaterial>();
 ////////////                   blink.enabled = true;
 ////////////                   blink.IsBlink = true;
 ////////////               }
 ////////////           }


 ////////////       }
 ////////////   }



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

        var sheet = wk.GetSheet("sequence"); // zero-based index of your target sheet


        // write the rest
        //for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
        //{
        //    var sheetRow = sheet.GetRow(i);
        //    var dtRow = dataTable.NewRow();
        //    dtRow.ItemArray = dataTable.Columns
        //        .Cast<DataColumn>()
        //        .Select(c => sheetRow.GetCell(c.Ordinal, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString())
        //        .ToArray();
        //    dataTable.Rows.Add(dtRow);
        //}


        //get the sheet
        
        IRow row = sheet.GetRow(0);  //get the first row
        //var FirstTask = row.GetCell(1);//get the task N1
        var components = sheet.GetRow(1);
        int ColumnNum = components.LastCellNum;

        //divide the data into several tables
        int TableNum = (sheet.LastRowNum + 1) / (ColumnNum + 1);
        for (int i = 0; i < TableNum; i++) //loop for the different tables
        {

            var dataTable = new DataTable();

            // write the header row
            var headerRow = sheet.GetRow(1);
            for (int l = 1; l < headerRow.LastCellNum; l++) // imp use ColumnNum instead of the "headerRow.LastCellNum"
            {
                //Debug.Log("headerRow.GetCell(i).ToString(): "+ headerRow.GetCell(i));
                dataTable.Columns.Add(headerRow.GetCell(l).ToString());
                dataTable.Columns[l-1].DataType = Type.GetType("StatusData");
            }
            //Debug.Log("dataTable.Columns.count: " + dataTable.Columns.Count);
            //we know that we get the task at the starting of a table
            var Task = sheet.GetRow(i * (ColumnNum + 1)).GetCell(1);//get the ith table's task 
            //Debug.Log("SubTasks: " + Task);
            //loop for the rows of a table, note that the rows numbers are the amount of the components + 1
            for (int j = i * (ColumnNum + 1) + 2; j < (i + 1) * (ColumnNum + 1); j++)  //skip the task and the components lines
            {
                //Debug.Log("J's values: " + j);
                //loop for the  columns in a row

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
                    var data= sheet.GetRow(j).GetCell(k + 1).ToString();
                    var dataLength= data.Length;
                    if (data == "NULL")
                    {
                        cellData.Status = null;
                    }
                    else if (dataLength == 1)
                    {
                        cellData.Status = Int32.Parse(data);
                    }
                    else if (dataLength == 2)
                    {
                        cellData.Status= Int32.Parse(data[0].ToString());
                        //cellData.Status= Int32.Parse(data[0].ToString());  //TODO should be Int32.Parse
                        cellData.Symbol= data[1];
                    }
                    else if (dataLength == 3)
                    {
                        cellData.Group = data[0];
                        cellData.Status = Int32.Parse(data[1].ToString()); 
                        cellData.Symbol = data[2];
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
            dataTable.TableName = Task.ToString();
            dtLst.Add(dataTable);
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
