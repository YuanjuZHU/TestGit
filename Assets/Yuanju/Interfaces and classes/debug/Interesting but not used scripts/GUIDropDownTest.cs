using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

public class GUIDropDownTest : MonoBehaviour
{
    private bool m_bolIsOpen = false;
    private float m_floSpeed = 10;
    private float[] m_floAllBtnY;
    public Rect DropDownInitState;//这个是下拉菜单的初始化位置、大小，可以在Inspector面板中调节。
    public List<string> m_iMenuRowName;
    public List<TimeEventTrigger> allList;//所有菜单选项在Inspector面板上的动态添加方法
    private delegate void TestMethods();
    TestMethods testMethods;
    void Awake()
    {
        m_floAllBtnY = new float[m_iMenuRowName.Count];
    }
    void OnGUI()
    {
        if (GUI.Button(DropDownInitState, "下拉菜单"))
        {
            m_bolIsOpen = true;
        }
        if (m_bolIsOpen)
        {
            for (int i = 0; i < m_iMenuRowName.Count; i++)
            {
                if (GUI.Button(new Rect(DropDownInitState.x, m_floAllBtnY[i], DropDownInitState.width, DropDownInitState.height), m_iMenuRowName[i]))
                {
                    m_bolIsOpen = false;
                    for (int j = 0; j < m_iMenuRowName.Count; j++)
                    {
                        m_floAllBtnY[j] = 20;
                    }
                    allList[i].m_TimeEvent.Invoke();
                }
            }
        }
    }
    //四个Test方法是我随便写的，用来放到Inspector面板的函数入口上，进行Debug测试，实际使用中大家奇异删掉这部分。
    public void Test1()
    {
        Debug.Log("我是番茄1");
    }
    public void Test2()
    {
        Debug.Log("我是番茄2");
    }
    public void Test3()
    {
        Debug.Log("我是番茄3");
    }
    public void Test4()
    {
        Debug.Log("我是番茄4");
    }
    void FixedUpdate()
    {
        if (m_bolIsOpen)
        {
            for (int i = 0; i < m_iMenuRowName.Count; i++)
            {
                if (m_floAllBtnY[i] < 20 + DropDownInitState.height * (i + 1))
                {
                    m_floAllBtnY[i] += m_floSpeed;
                }
            }
        }

    }

}

//一下是在Inspector面板上动态添加方法函数的代码
[Serializable]
public class TimeEventTrigger
{
    [Serializable]
    public class TimeEvent : UnityEvent { }
    [SerializeField]
    public TimeEvent m_TimeEvent = new TimeEvent();
}
