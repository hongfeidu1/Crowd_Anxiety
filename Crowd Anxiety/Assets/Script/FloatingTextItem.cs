using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;//TMP

public class FloatingTextItem : MonoBehaviour
{
    [Header("引用")]
    [Tooltip("留空会自动从子物体中找")]
    public TextMeshProUGUI label;//label:标签

    [Header("时间")]
    [Tooltip("显示时长/s")]
    public float lifetime = 2.0f;

    private float timer;
    private bool busy;

    public bool IsBusy => busy;//外部只读

    private void Awake()
    {
        if(label == null)//懒加载，只有未获取时才执行查找
            label = GetComponentInChildren<TextMeshProUGUI>(true);//自身及子物体查找，true保证在gameobject未勾选状态也能找到TMP组件
        label.enabled = false;//组件初始不启用
    }

    //文本显示方法，将外部传入的文本内容和坐标位置转为组件内部对应内容，并启用组件
    public void Show(string content, Vector3 worldPosition)
    {
        if (label == null)
            label = GetComponentInChildren<TextMeshProUGUI>(true);//因为是对象池

        label.text = content;
        transform.position = worldPosition;
        label.enabled = true;

        timer = lifetime;
        busy = true;
    }

    //关闭组件以及对应的启用信息
    public void Hide()
    {
        label.enabled = false;
        busy = false;
    }

    private void Update()
    {
        if (!busy)
            return;

        timer -= Time.deltaTime;
        if(timer < 0f )
            Hide();
    }
}
