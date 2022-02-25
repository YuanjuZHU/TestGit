using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NPOI;
using NPOI.SS.UserModel;
using UnityEngine;

class TestReadExcel : MonoBehaviour
{
    void Awake()
    {
        MyFunc();
    }


    public static void MyFunc()
    {
        string importExcelPath = "D:\\import.xlsx";
        string exportExcelPath = "D:\\export.xlsx";
        IWorkbook workbook = WorkbookFactory.Create(importExcelPath);
        Console.WriteLine("workbook: " + workbook);
        ISheet sheet = workbook.GetSheetAt(0);//获取第一个工作薄
        Console.WriteLine("sheet.name: " + sheet.SheetName);
        IRow row = (IRow)sheet.GetRow(0);//获取第一行

        //设置第一行第一列值,更多方法请参考源官方Demo
        row.CreateCell(0).SetCellValue("test");//设置第一行第一列值

        //导出excel
        FileStream fs = new FileStream(exportExcelPath, FileMode.Create, FileAccess.ReadWrite);
        workbook.Write(fs);
        fs.Close();
        Console.ReadKey();
    }


}

