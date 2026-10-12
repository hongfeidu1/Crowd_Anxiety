using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//做范围检测后文字生成
//有一轮大修改，旧版本在github中找
public class HallucinationSpawner : MonoBehaviour
{
    [Header("检测中心")]
    [Tooltip("player")]
    public PlayerSensor sensor;

    [Header("文字模板")]
    [Tooltip("FloatingText")]
    public FloatingTextPool pool;

    [Header("总时间间隔")]
    [Tooltip("多久出现一次")]
    public float baseInterval = 0.8f;

    [Header("间隔浮动值")]
    [Tooltip("间隔上下浮动，总不能死板的固定时间吧")]
    public float intervalJitter = 0.4f;

    [Header("对象间隔时间")]
    [Tooltip("单个对象在这段时间不会再次被选中")]
    public float interval = 2.0f;

    [Header("距离权值")]
    [Tooltip("越近越大")]
    public float nearBias = 1.0f;

    [Header("距离下限")]
    [Tooltip("防止贴脸权值爆了")]
    public float minDistance = 0.6f;

    [Header("文字文本")]
    [Tooltip("一行一条")]
    public List<string> lines = new List<string>();

    [Header("坐标偏差")]
    [Tooltip("与对象坐标的偏移量")]
    public float heightOffset = 1.0f;

    private float timer;
    private string lastline;//记录上一句文本，避免重复
    private float nextPrune;//update()

    readonly Dictionary<Transform, float> lastSpawnAt = new Dictionary<Transform, float>();
    //一个只读的字典，键类型Transform，存值类型float;
    //存储对象与时间，记录每个人发言的时间，做发言冷却
    readonly List<Transform> deadBuffer = new List<Transform>();
    //一个只读的列表，临时缓冲列表清理字典中被销毁的对象，遍历Dictionary时直接Remove会报错

    private void Update()
    {
        if(Time.time >= nextPrune)//
        {
            nextPrune = Time.time + 5f;
            PruneDeadPeople();//5秒触发一次，这个方法用来清理数据
        }

        timer -= Time.deltaTime;//倒计时
        if (timer > 0f)//发言间隔
            return;

        Transform target = PickPerson();//选发言人
        if (target == null)//如果没人可选，跳出本帧下帧继续
            return;

        //添加对象池
        if (pool == null) return;
        FloatingTextItem item = pool.Get();
        if(item == null) return;//取出来为空，说明同屏上限了，跳出这次，不显示了

        //动态发言间隔，设置最小值保底
        timer = Mathf.Max(0.15f, UnityEngine.Random.Range(baseInterval - intervalJitter,
                                              baseInterval + intervalJitter));

        //调用FloatingItem中的文本显示脚本（文本内容，显示坐标）
        item.Show(PickLine(), target.position + Vector3.up * heightOffset);
        //记录该对象的发言时间用于冷却判定
        lastSpawnAt[target] = Time.time;
    }

    //抽人，根据权值随机抽
    private Transform PickPerson()
    {
        if(sensor == null)
            return null;

        var people = sensor.People;//将范围脚本中的只读列表在该脚本使用

        float total = 0f;//权值
        for (int i = 0; i < people.Count; i++)//循环只读列表计算权值总值
        {
            total += WeightOf(people[i]);
        }

        if(total <= 0f)//权值为0，所有对象均在冷却/没有对象
            return null;

        //轮盘赌，取某一对象的概率为100% * (w / total)
        float pick = UnityEngine.Random.value * total; //[0,total]取权值
        Transform fallback = null;//定义返回的Transform初始为空

        for (int i = 0;i < people.Count; i++)//循环整个只读列表
        {
            float w = WeightOf(people[i]);//获取当前对象权值
            if (w <= 0f) continue;//当对象权值小于0，跳过本回合，这个有必要吗？

            fallback = people[i].transform;//当对象权值大于0，选定对象
            pick -= w;//核心
            if (pick <= 0f)//判定
                return fallback;//返回对象
        }
        return fallback;//返回对象，最后一位权值有效对象
    }

    //计算对象权值
    private float WeightOf(PlayerSensor.PersonInfo personInfo)
    {
        if(personInfo.transform == null) return 0f;//对象已经被删除，返回权值为0

        if (lastSpawnAt.TryGetValue(personInfo.transform, out float last))
            if (Time.time - last < interval)//做冷却判定，将处于冷却的对象权值设为0
                return 0f;
        float d = Mathf.Max(personInfo.distance, minDistance);
        return 1f / Mathf.Pow(d, nearBias);
    }

    //清理已经删除的对象
    private void PruneDeadPeople()
    {
        //如果已经为空，不用清理直接返回
        if(lastSpawnAt.Count == 0) return;

        //清空无效对象临时存储列表，为后面腾位置
        deadBuffer.Clear();
        //遍历字典，判断选出无效对象，存入无效对象临时列表
        foreach(var a in lastSpawnAt)
        {
            if (a.Key == null)
                deadBuffer.Add(a.Key);
        }
        //遍历无效对象列表，删除字典中的无效对象，遍历字典时不能删除字典对象，所以用了辅助列表
        foreach(var b in deadBuffer)
        {
            lastSpawnAt.Remove(b);
        }
    }

    //内容文本
    private string PickLine()
    {
        //当文本列表为空，范围设定文本“……”
        //当文本列表数目为1，返回lines[0]
        if (lines.Count == 0) return "......";
        if (lines.Count == 1) return lines[0];

        string s;
        int guard = 0;//守卫
        do { s = lines[UnityEngine.Random.Range(0, lines.Count)]; }
        while (s == lastline && ++guard < 10);

        lastline = s;
        return s;
    }
}
