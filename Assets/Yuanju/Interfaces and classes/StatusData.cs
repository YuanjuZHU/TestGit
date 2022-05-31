using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class is used to save the data that filled in the excel file, the data in the excel files has more information than just status.
public class StatusData
{
    //the first "digit" should be the group of the component belongs to  
    public string Name;

    private char? group;

    public char? Group
    {
        get { return group; }
        set { group = value; }
    }

    private bool hasGroup;

    public bool HasGroup
    {
        get { return hasGroup = this.Group != null; }
        set { hasGroup = value; }
    }

    //the "status" related to this group
    private int? status;

    public int? Status
    {
        get { return status; }
        set { status = value; }
    }

    private bool hasStatus;

    public bool HasStatus
    {
        get { return hasStatus = this.Status!=null; }
        set { hasStatus = value; }
    }


    private char? symbol;

    public char? Symbol
    {
        get { return symbol; }
        set { symbol = value; }
    }

    private bool hasSymbol;

    public bool HasSymbol
    {
        get { return hasSymbol = ((Symbol == '+')|| (Symbol == '-')|| (Symbol == '=')); }
        set { hasSymbol = value; }
    }


}

public class Test
{
    public StatusData statusDataTest = new StatusData();

    void MyFunc()
    {
        statusDataTest.Symbol = '+';
        Debug.Log("statusDataTest.has symbol: "+ statusDataTest.HasSymbol);
    }
}

