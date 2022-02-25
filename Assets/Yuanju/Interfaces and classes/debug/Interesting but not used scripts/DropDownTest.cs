using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Chinar例子脚本，用以输出
/// </summary>
public class DropDownTest : MonoBehaviour
{
    private int testflag;
    void Start()
    {
        //贴心的 Chinar 为新手提供了 代码动态绑定的方法，如果通过代码添加监听事件，外部就无需再做添加
        //GameObject.Find("Dropdown").GetComponent<TMPro.TMP_Dropdown>().onValueChanged.AddListener(ConsoleResult);
    }

    //void Update()
    //{
    //    Debug.Log("value: " + testflag);
    //}

    /// <summary>
    /// 输出结果 —— 添加监听事件时要注意，需要绑定动态方法
    /// </summary>
    public void ConsoleResult(int value)
    {
        //这里用 if else if也可，看自己喜欢
        //分别对应：第一项、第二项....以此类推
        Debug.Log("to see when this func is excuted: "  );
        testflag = value;
        switch (value)
        {
                
            case 0:
                Debug.Log("第1页");
                break;
            case 1:
                Debug.Log("第2页");
                break;
            case 2:
                Debug.Log("第3页");
                break;
            case 3:
                Debug.Log("第4页");
                break;
            //如果只设置的了4项，而代码中有第五个，是永远调用不到的
            //需要对应在 Dropdown组件中的 Options属性 中增加选择项即可
            case 4:
                print("第5页");
                break;
        }

    }
}