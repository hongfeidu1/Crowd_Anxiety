using System;
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

    [Header("刻度线")]
    [Tooltip("阶段二")]
    public RectTransform stage2Maker;
    [Tooltip("阶段三")]
    public RectTransform stage3Maker;

    private void Awake()
    {
        if (fill != null && fill.type != Image.Type.Filled)
            Debug.LogWarning("PressureBarUI：Fill 的 Image Type 不是 Filled，fillAmount 不会生效", fill);

        PlaceMakers();
    }

    private void Update()
    {
        if (pressureSystem == null || fill == null)
            return;

        fill.fillAmount = Mathf.Clamp01(pressureSystem.Pressure);
    }

    private void OnValidate()
    {
        PlaceMakers();
    }

    private void PlaceMakers()
    {
        PlaceMaker(stage2Maker, pressureSystem.stage2Threshold);
        PlaceMaker(stage3Maker, pressureSystem.stage3Threshold);
    }

    private void PlaceMaker(RectTransform maker, float t)
    {
        if(maker == null) return;

        maker.anchorMin = new Vector2(0.5f, t);
        maker.anchorMax = new Vector2(0.5f, t);
        maker.anchoredPosition = Vector2.zero;
    }
}
