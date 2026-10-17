using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

//做压力值计算：对象数目与距离转换为非线性压力曲线
public class PressureSystem : MonoBehaviour
{
    [Header("数据来源脚本")]
    public PlayerSensor sensor;

    [Header("距离曲线")]
    [Tooltip("X轴：距离/探测半径；Y轴：压力贡献值")]//归一化
    //AnimationCurve:Unity内置动画曲线类型，存储一组[时间-值]关键点，可自定义曲线形状
    public AnimationCurve distanceCurve = new AnimationCurve(
        new Keyframe(0f, 1f),
        new Keyframe(0.4f, 0.45f),
        new Keyframe(0.75f, 0.12f),
        new Keyframe(1f, 0f));

    [Header("数量曲线")]
    [Tooltip("权重总和上限")]
    [Min(0.01f)] public float crowdScale = 5f;
    [Tooltip("X轴：权值总和/上限；Y轴：瞬时压力")]
    public AnimationCurve crowdCurve = new AnimationCurve(
        new Keyframe(0f, 0f),
        new Keyframe(0.5f, 0.3f),
        new Keyframe(1f, 1f));

    [Header("积累/消退")]
    [Tooltip("每秒上升：1/riseSpeed = 0到1的时间s")]
    [Min(0f)] public float riseSpeed = 0.5f;
    [Tooltip("每秒下降")]
    [Min(0f)] public float fallSpeed = 0.2f;

    [Header("心理阶段阈值一")]
    [Tooltip("棘轮逻辑，数值只允许单向变化无法回落")]
    [Range(0f, 1f)] public float stage2Threshold = 0.5f;//Threshold:门槛、入口、阈值、临界线
    [Range(0f, 1f)] public float stage3Threshold = 0.8f;

    [Header("运行读数")]//测试
    [SerializeField] private float targetPressure;//瞬时压力
    [SerializeField] private float pressure;//积累后的输出
    [SerializeField] private int peopleCount;
    [SerializeField] private int stage = 1;
    [SerializeField] private float pressureFloor;

    public float TargetPressure => targetPressure;
    public float Pressure => pressure;
    public int PeopleCount => peopleCount;
    public int Stage => stage;
    public float PressureFloor => pressureFloor;

    private void Update()
    {
        ComputeTargetPressure();//计算瞬时压力
        ApplyAccumulation();//累积压力
    }

    private void ComputeTargetPressure()
    {
        //由人数与距离计算瞬时压力值

        //初始化
        targetPressure = 0f;
        peopleCount = 0;

        if (sensor == null) return;

        float radius = Mathf.Max(0.001f, sensor.detectionRadius);//检测范围半径
        var people = sensor.People;//将PlayerSensor中的列表读过来

        float raw = 0f;

        for(int i = 0; i < people.Count; i++)
        {
            float a = people[i].distance;

            float x = Mathf.Clamp01(a / radius);
            raw += distanceCurve.Evaluate(x);//函数计算,单人的贡献
        }

        peopleCount = people.Count;

        if(raw <= 0f)
            return;

        float t = Mathf.Clamp01(raw / Mathf.Max(0.001f, crowdScale));
        targetPressure = crowdCurve.Evaluate(t);
    }

    private void ApplyAccumulation()
    {
        float goal = Mathf.Max(targetPressure, pressureFloor);
        float speed = (goal > pressure) ? riseSpeed : fallSpeed;

        if (speed <= 0f)
            pressure = goal;//判断为最小值0时，保持不变
        else
            pressure = Mathf.MoveTowards(pressure, goal, speed * Time.deltaTime);

        UpdateStage();//判断是否进入下一阶段
        
        //兜底，防止意外
        if(pressure < pressureFloor)
            pressure = pressureFloor;
    }

    private void UpdateStage()
    {
        if(stage < 3 && pressure >= stage3Threshold)
        {
            stage = 3;
            pressureFloor = stage3Threshold;
        }
        else if(stage < 2 && pressure >= stage2Threshold)
        {
            stage = 2;
            pressureFloor = stage2Threshold;
        }
    }
}
