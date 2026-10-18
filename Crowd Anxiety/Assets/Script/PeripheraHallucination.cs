using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//无来源的幻听：屏幕边缘随机浮现文字
//与HallucinationSpawner并行
public class PeripheraHallucination : MonoBehaviour
{
    [Header("引用")]
    [Tooltip("TextLayer挂载(FloatingTextPool)")]
    public FloatingTextPool pool;
    [Tooltip("读取压力值，太低时不出字，留空忽略压力条件")]
    public PressureSystem pressureSystem;

    [Header("节奏/s")]
    public float intervalMin = 1.5f;
    public float intervalMax = 3.5f;

    [Header("触发条件")]
    [Tooltip("压力低于这个值不生成，0 = 不吃压力")]
    [Range(0.0f, 1.0f)] public float minPressure = 0.3f;

    [Header("出现在屏幕的哪一圈")]
    [Tooltip("0 = 屏幕中心， 1 = 边缘")]
    [Range(0.0f, 1.0f)] public float radiusMin = 0.5f;
    [Range(0.0f, 1.0f)] public float radiusMax = 0.95f;

    [Header("相机")]
    [Tooltip("正交相机固定填10就行")]
    public float cameraDistance = 10f;

    [Header("文字膨胀系数")]
    public float growMultiper = 2f;

    [Header("台词，一条一行")]
    public List<string> lines = new List<string>();

    private float timer;
    private string lastline;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        timer = Random.Range(intervalMin, intervalMax);
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;

        timer = Random.Range(intervalMin, intervalMax);

        if (pool == null || cam == null) return;

        //压力太小不出现
        if (pressureSystem != null && pressureSystem.Pressure < minPressure)
            return;

        FloatingTextItem item = pool.Get();
        if (item == null) return;//同屏上限了，池子给了个滚木

        item.Show(PickLine(), RandomEdgeWorldPosition(), growMultiper);
    }

    private Vector3 RandomEdgeWorldPosition()
    {
        float angle = Random.Range(0f, Mathf.PI * 2);
        float radius = Random.Range(radiusMin, radiusMax);

        //0.5是屏幕中心
        float vx = 0.5f + Mathf.Cos(angle) * radius * 0.5f;
        float vy = 0.5f + Mathf.Sin(angle) * radius * 0.5f;

        Vector3 world = cam.ViewportToWorldPoint(new Vector3(vx, vy, cameraDistance));
        world.z = 0f;
        return world;
    }

    private string PickLine()
    {
        if (lines.Count == 0) return"......";
        if(lines.Count == 1) return lines[0];

        string s;
        int guard = 0;//记录重复抽数并跳出避免死循环
        do
        {
            s = lines[Random.Range(0, lines.Count)];
        } while (s == lastline && ++guard < 10);

        lastline = s;
        return s;

    }
}
