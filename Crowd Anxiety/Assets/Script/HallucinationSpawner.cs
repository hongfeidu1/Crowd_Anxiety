using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//做范围检测后文字生成
public class HallucinationSpawner : MonoBehaviour
{
    [Header("检测中心")]
    [Tooltip("player")]
    public PlayerSensor sensor;

    [Header("文字模板")]
    [Tooltip("FloatingText")]
    public FloatingTextItem item;

    [Header("间隔时间")]
    [Tooltip("单个对象文字浮现间隔时间")]
    public float interval = 3.0f;

    [Header("文字文本")]
    [Tooltip("暂时为单一台词，后面做对象池再做随机")]
    public string context = "那边那个人……";

    [Header("坐标偏差")]
    [Tooltip("与对象坐标的偏移量")]
    public float heightOffset = 1.0f;

    private float timer;

    private void Update()
    {
        //涉及到时间间隔，优先做时间判断，没走完上一次不进入下一次
        timer -= Time.deltaTime;
        if (timer > 0.0f)
            return;

        Transform target = FindNearest();//找最近对象，但其实不只是找最近，后面改
        if (target == null)
            return;

        //上一次结束，且找到了对象，进入下一次
        timer = interval;
        //调用文本浮现代码
        item.Show(context, target.position + Vector3.up * heightOffset);
    }

    private Transform FindNearest()
    {
        //前置调节，没有加player没法做寻找
        if(sensor == null)
            return null;

        //int count = Mathf.Min(sensor.peopleList.Count, sensor.distanceList.Count);//这里要改检测文档，两个列表纯傻逼操作

        var people = sensor.People;

        Transform best = null;
        float bestDistance = float.MaxValue;//初始设置为最大，后续判定都为小

        for(int i = 0; i < people.Count; i++)
        {
            PlayerSensor.PersonInfo p = people[i];
            if(p.transform == null) continue;
            //if (sensor.people[i] == null)
            //    continue;
            //避免对象被删除导致为空，后续代码报错

            if(p.distance < bestDistance)
            {
                bestDistance = p.distance;
                best = p.transform;
            }
            //float d = sensor.distanceList[i];
            //if(d < bestDistance)
            //{
                //bestDistance = d;
                //best = sensor.peopleList[i].transform;
            //}
        }
        return best;
    }
}
