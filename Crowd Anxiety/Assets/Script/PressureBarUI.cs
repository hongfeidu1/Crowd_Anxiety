using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressureBarUI : MonoBehaviour
{
    [Header("数据脚本")]
    public PressureSystem pressureSystem;

    [Header("引用")]
    [Tooltip("竖直填充条。Image 的 Type 必须是 Filled，Fill Method = Vertical，Fill Origin = Bottom")]
    public Image fill;

    private void Awake()
    {
        if (fill != null && fill.type != Image.Type.Filled)
            Debug.LogWarning("PressureBarUI：Fill 的 Image Type 不是 Filled，fillAmount 不会生效", fill);
    }

    private void Update()
    {
        if (pressureSystem == null || fill == null)
            return;

        fill.fillAmount = Mathf.Clamp01(pressureSystem.Pressure);
    }
}
