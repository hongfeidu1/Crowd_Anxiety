using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//测试使用lineRender制作虚线圆环效果，本质是创建了许多的点，思考性能消耗问题？游戏逻辑并不复杂理论可以忽略不计
//注意到，在LineRenderer组件中，loop决定了是否点首尾相连
//line.useWorldSpace决定了物体是否使用局部坐标，跟随物体移动
public class LineTest : MonoBehaviour
{
    [Header("圆环参数")]
    public float radius = 5f;
    public int pointCount = 10;

    private LineRenderer line;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        GenerateCircle();
    }

    private void GenerateCircle()
    {
        line.positionCount = pointCount;

        //生成点坐标
        Vector3[] points = new Vector3[pointCount];
        for (int i = 0; i < pointCount; i++)
        {
            //做圆本质还是三角函数，多个点
            float rad = 2 * Mathf.PI * i / pointCount;//计算点对应的角大小
            float x = radius * Mathf.Cos(rad);
            float y = radius * Mathf.Sin(rad);

            points[i] = new Vector3(x, y, 0);
        }

        line.SetPositions(points);//一次性设置全部点坐标
        Debug.Log(line.colorGradient);
    }
}
