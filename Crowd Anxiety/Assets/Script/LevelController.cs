using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    [Header("数据源")]
    [Tooltip("PressureSystem")]
    public PressureSystem pressureSystem;

    [Header("失败条件")]
    [Tooltip("判定压力阈值")]
    [Range(0f, 1f)] public float failThreshold = 0.95f;
    [Tooltip("抗压时间上限")]
    public float failDuration = 5f;

    [Header("事件")]//死亡效果
    public UnityEvent onLeveCleared;

    private float overThresholdTimer;//记录抗压时间
    private bool cleared;
    private bool reloading;

    private void Update()
    {
        if (cleared || reloading || pressureSystem == null) return;

        if(pressureSystem.Pressure >= failThreshold)
        {
            overThresholdTimer += Time.deltaTime;
            if (overThresholdTimer >= failDuration)
                Fail();
        }
        else
        {
            overThresholdTimer = 0f;//压力低于阈值，计时归零
        }
    }

    public void ClearLevel()
    {
        if(cleared) return;
        cleared = true;
        onLeveCleared?.Invoke();//死亡事件，暂时为空
    }

    private void Fail()
    {
        reloading = true;
        overThresholdTimer = 0f;
        int index = SceneManager.GetActiveScene().buildIndex;//获取场景序列号

        if(index < 0)
        {
            Debug.LogError("场景不在Build Settings");
            //在File - Build Settings - Scene In Build
            return;
        }

        SceneManager.LoadScene(index);
    }
}
