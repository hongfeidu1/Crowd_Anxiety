using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextPool : MonoBehaviour
{
    //做文本池，借出、收回、同屏上限
    [Header("预制体")]
    public FloatingTextItem perfab;

    [Header("池容量")]
    [Tooltip("开局预先创建几个")]
    public int inistialSize = 8;

    [Header("同屏上限")]
    public int maxSize = 15;

    //Queue队列，先进先出，存空闲可用文本
    readonly Queue<FloatingTextItem> idle = new Queue<FloatingTextItem>();
    int created;//已经创建了多少给实例，用来做同屏上限判断

    public int ActiveCount { get; private set; }
    //当前显示的数目

    private void Awake()
    {
        //预生成实例
        for(int i = 0; i < inistialSize; i++)
        {
            idle.Enqueue(CreateInstance());//Enqueue队列方法，入队
        }
    }

    //实例化
    private FloatingTextItem CreateInstance()
    {
        FloatingTextItem item = Instantiate(perfab, transform);//实例化
        item.pool = this;
        item.gameObject.SetActive(false);//创建对象但是先不用
        created++;//计数
        return item;
    }

    //取
    public FloatingTextItem Get()
    {
        FloatingTextItem item = null;
        while(item == null && idle.Count > 0)
            item = idle.Dequeue();//队列中取出

        //队列取完还是null，新建一个，达到创建数目达到上限就不生成
        if(item == null)
        {
            if (created >= maxSize)
                return null;
            item = CreateInstance();
        }

        item.gameObject.SetActive(true);//创建即启用
        ActiveCount++;//计数
        return item;
    }

    //还
    public void Release(FloatingTextItem item)
    {
        item.gameObject.SetActive(false);//关闭对象
        idle.Enqueue(item);//回到空闲队列
        ActiveCount = Mathf.Max(0, ActiveCount - 1);//更新计数，保底为0
    }
}
