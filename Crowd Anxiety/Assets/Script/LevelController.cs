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
    [Tooltip("重载场景前的黑屏淡出时间")]
    public float failFadeTime = 1f;

    [Header("通关")]
    [Tooltip("通关后黑屏时长")]
    public float clearFadeTime = 2f;
    [Tooltip("最后一关返回哪个场景，Build Settings序号")]
    public int mainMenuBuildIndex = 0;

    [Header("事件")]//死亡效果
    public UnityEvent onLevelCleared;

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
        onLevelCleared?.Invoke();//死亡事件，暂时为空

        LoadNextLevel();
    }

    private void LoadNextLevel()
    {
        int current = SceneManager.GetActiveScene().buildIndex;
        int next = current + 1;

        if(next < SceneManager.sceneCountInBuildSettings)
        {
            if (ScreenFade.Instance != null)
                ScreenFade.Instance.FadeOutAndLoadScene(next, clearFadeTime);
            else
                SceneManager.LoadScene(next);
        }
        else
        {
            //最后一关回到主菜单
            if (ScreenFade.Instance != null)
                ScreenFade.Instance.FadeOutAndLoadScene(mainMenuBuildIndex, clearFadeTime);
            else
                SceneManager.LoadScene(mainMenuBuildIndex);
        }
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

        if (ScreenFade.Instance != null)
            ScreenFade.Instance.FadeOutAndLoadScene(index, failFadeTime);
        else
            SceneManager.LoadScene(index);
    }
}
