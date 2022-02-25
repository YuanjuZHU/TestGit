using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEngine;

public class ReadExcel: MonoBehaviour
{
    void Awake()
    {
        DataTable dt = new DataTable();
        dt = NPExcelToDataTable("Assets/Yuanju/ExcelTest.xlsx", "sheet1");
        //Debug.Log("data table got, the row number: " + dt.Rows.Count);
        //Debug.Log("data table got, the value: " + dt.Rows[2][0]);
        //int memoIndex = dt.Columns.IndexOf("name11");
        //Debug.Log("the index of name11: " + memoIndex);
        DataRow[] drs = dt.Select(); //get all the columns of the data table
        //DataColumn dcs=
        //Debug.Log("drs.count: " + drs.Length);
        //foreach (var dr in drs)
        //{
        //    Debug.Log("data table's one column: "+ dr["first name"]);
        //}
    }


    /// <summary>
    /// NPOI reads Excel to DataTable
    /// </summary>
    /// <param name="fileName">Excel path</param>
    /// <param name="sheetName">worksheet name reads the first worksheet by default</param>
    /// <returns></returns>
    public static DataTable NPExcelToDataTable(string fileName, string sheetName = "")
    {
        FileStream fs = null;
        NPOI.SS.UserModel.IWorkbook workbook = null;
        NPOI.SS.UserModel.ISheet sheet = null;
        DataTable dt = new DataTable();
        try
        {
            fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            if (fileName.IndexOf(".xlsx") > 0) // 2007 version
                workbook = new NPOI.XSSF.UserModel.XSSFWorkbook(fs);
            else if (fileName.IndexOf(".xls") > 0) // 97-2003 version
                workbook = new NPOI.HSSF.UserModel.HSSFWorkbook(fs);
            //workbook = NPOI.SS.UserModel.WorkbookFactory.Create(fs);
            if (sheetName != "") //Is there a sheet name
            {
                sheet = workbook.GetSheet(sheetName);
            }
            else
            {
                sheet = workbook.GetSheetAt(0); //Read the first sheet
            }
            int startRow = 0; //Start reading the number of rows
            if (sheet == null)
                throw new Exception("Worksheet not found");
            dt.TableName = sheet.SheetName;
            NPOI.SS.UserModel.IRow firstRow = sheet.GetRow(startRow); //The first row 
            //foreach (var cell in firstRow)
            //{
            //    Debug.Log("cells in first row: "+ cell);
            //}
            int cellCount = firstRow.LastCellNum; //The number of the last cell in a row is the total number of columns


            // imp it seems that the first cell[0] can not have duplicate column names
            //loop for first row
            for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
            {
                NPOI.SS.UserModel.ICell cell = firstRow.GetCell(i);
                if (cell != null)
                {
                    //first row ir recognized as a string row
                    string cellValue = cell.StringCellValue;
                    //Debug.Log("first row cells: " + cellValue);
                    if (cellValue != null)
                    {
                        ////////Debug.Log("dt.Columns.IndexOf(cellValue): " + dt.Columns.IndexOf(cellValue));
                        //////if (dt.Columns.IndexOf(cellValue) > 0) //Are there duplicate column names,if there are ,rename the column with an index
                        //////{
                        //////    Debug.Log("dt.Columns.IndexOf(cellValue)>0: " + cellValue);
                        //////    DataColumn column = new DataColumn(Convert.ToString("duplicate column name" + cellValue + i));
                        //////    dt.Columns.Add(column);
                        //////}
                        //////else
                        //////{
                        //////    Debug.Log("dt.Columns.IndexOf(cellValue)<=0: " + cellValue);
                        //////    DataColumn column = new DataColumn(cellValue);
                        //////    dt.Columns.Add(column);
                        //////}
                        DataColumn column = new DataColumn(cellValue);
                        dt.Columns.Add(column);
                    }
                }
            }
            //loop for the second row
            startRow = startRow + 1;

            //imp that is to say, the number of the last row num in a column is the total number of columns
            int rowCount = sheet.LastRowNum; //Total number of rows
                                             //Fill line
            for (int i = startRow; i <= rowCount; ++i)
            {
                NPOI.SS.UserModel.IRow row = sheet.GetRow(i);
                if (row == null) continue; //The row without data is null by default

                DataRow dataRow = dt.NewRow();
                for (int j = row.FirstCellNum; j < cellCount; ++j)
                {
                    NPOI.SS.UserModel.ICell cell = row.GetCell(j);

                    if (cell != null) //Similarly, cells without data are null by default
                    {
                        if (row.GetCell(j).CellType == NPOI.SS.UserModel.CellType.Formula) //Is it a formula
                        {
                            try
                            {
                                dataRow[j] = cell.StringCellValue;
                            }
                            catch
                            {
                                if (NPOI.SS.UserModel.DateUtil.IsCellDateFormatted(cell)) //Is it a date
                                {
                                    dataRow[j] = cell.DateCellValue;
                                }
                                else { dataRow[j] = cell.NumericCellValue; }
                            }
                        }
                        else { dataRow[j] = row.GetCell(j).ToString(); }
                    }
                }
                dt.Rows.Add(dataRow);
            }
            fs.Close();
            return dt;
        }
        catch (Exception ex)
        {
            if (fs != null)
            {
                fs.Close();
            }
            throw new Exception(ex.Message);
        }
    }
}

