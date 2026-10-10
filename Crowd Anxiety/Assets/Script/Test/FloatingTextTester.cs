using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//新建空白对象，加入脚本，添加item
public class FloatingTextTester : MonoBehaviour
{
    public FloatingTextItem item;
    public KeyCode testKey = KeyCode.Space;
    public string testContent = "你看那个人";

    private void Update()
    {
        if (!Input.GetKeyDown(testKey))
            return;

        Vector3 test = Camera.main.ScreenToWorldPoint(Input.mousePosition);//鼠标屏幕像素坐标转换为世界坐标
        test.z = 0f;
        
        item.Show(testContent, test);
    }
}
