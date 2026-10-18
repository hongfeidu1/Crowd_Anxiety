using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;//TMP

//文本显示：时间、透明度、运动、尺寸、颜色
public class FloatingTextItem : MonoBehaviour
{
    [Header("引用")]
    [Tooltip("留空会自动从子物体中找")]
    public TextMeshProUGUI label;//label:标签

    [Header("时间")]
    [Tooltip("显示时长/s")]
    public float lifeMin = 1.0f;
    public float lifeMax = 2.0f;

    [Header("淡入淡出")]
    [Range(0f, 0.5f)] public float fadeIn = 0.1f;
    [Range(0.05f, 1f)] public float fadeOut = 0.2f;

    [Header("运动速度")]
    [Tooltip("向上移动/秒")]
    public float riseSpeed = 0.5f;

    [Header("水平漂移")]
    public float driftX = 0.2f;

    [Header("大小")]
    public float scaleMin = 1f;
    public float scaleMax = 2f;

    [Header("颜色")]
    public Color colorA = new Color(0.85f, 0.85f, 0.85f, 1f);
    public Color colorB = new Color(0.78f, 0.10f, 0.12f, 1f);

    [HideInInspector] public FloatingTextPool pool;//存储对象池的一个公开引用（不显示在Inspector）

    CanvasGroup group;//控制透明度
    float age;//记录文本生命时间
    float life;//文本生命周期
    float lifeFadeIn;
    float lifeFadeOut;
    Vector2 drift;
    private bool busy;

    //新增，文字浮现效果优化：随压力等级增大
    private float baseScale;//本次显示的基础尺寸
    private float growTo;//最终膨胀倍数

    public bool IsBusy => busy;//外部只读，是否显示

    private void Awake()
    {
        EnsureRefs();
    }

    private void EnsureRefs()
    {
        if (label == null)//懒加载，只有未获取时才执行查找
            label = GetComponentInChildren<TextMeshProUGUI>(true);
        //自身及子物体查找，true保证在gameobject未勾选状态也能找到TMP组件

        if (group == null)
        {
            group = GetComponent<CanvasGroup>();
            if (group == null)
                group = gameObject.AddComponent<CanvasGroup>();//找不到就自己加
        }

        group.interactable = false;//不参与UI交互
        group.blocksRaycasts = false;//不阻挡射线
        group.alpha = 0f;//初始透明
        //label.enabled = false;//组件初始不启用
    }

    //文本显示方法，将外部传入的文本内容和坐标位置转为组件内部对应内容，并启用组件
    public void Show(string content, Vector3 worldPosition, float growToMultiplier)
    {
        EnsureRefs();

        life = UnityEngine.Random.Range(lifeMin, lifeMax);//生命周期随机选值
        lifeFadeIn = Mathf.Min(fadeIn, life * 0.4f);
        lifeFadeOut = Mathf.Min(fadeOut, life * 0.5f);

        label.text = content;
        label.color = Color.Lerp(colorA, colorB, UnityEngine.Random.value);

        //基础尺寸随机，膨胀在该基础上叠加
        baseScale = UnityEngine.Random.Range(scaleMin, scaleMax);
        growTo = Mathf.Max(1f, growToMultiplier);

        transform.position = worldPosition;
        transform.localScale = Vector3.one * baseScale;
        
        drift = new Vector2(UnityEngine.Random.Range(-driftX, driftX), riseSpeed);
        label.enabled = true;
        //timer = lifetime;
        age = 0f;
        group.alpha = 0f;
        busy = true;
    }

    //关闭组件以及对应的启用信息
    public void Hide()
    {
        if (!busy) return;//已经不显示了，不重复隐藏

        label.enabled = false;
        busy = false;

        if (pool != null)
            pool.Release(this);
    }

    private void Update()
    {
        if (!busy)
            return;

        age += Time.deltaTime;
        transform.position += (Vector3)(drift * Time.deltaTime);

        //新增，随存活时间增大
        float progress = Mathf.Clamp01(age / Mathf.Max(0.0001f, life));
        float scle = baseScale * Mathf.Lerp(1f, growTo, progress);
        transform.localScale = Vector3.one * scle;

        float a;
        if (age < lifeFadeIn)
            a = age / Mathf.Max(0.0001f, lifeFadeIn);//淡入
        else if (age > life - lifeFadeOut)
            a = Mathf.Clamp01((life - age) / Mathf.Max(0.0001f, lifeFadeOut));//淡出
        else
            a = 1f;//满

        group.alpha = a;
        if (age >= life)
            Hide();
    }
}
