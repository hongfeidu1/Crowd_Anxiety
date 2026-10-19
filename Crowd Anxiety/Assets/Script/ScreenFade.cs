using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//黑屏，淡入淡出
//场景重载会销毁场景内所有物体，因为要淡入，所以淡入必须存在静态
public class ScreenFade : MonoBehaviour
{
    //单例实例
    public static ScreenFade Instance {  get; private set; }

    [Header("引用")]
    [Tooltip("Image")]
    public Image image;

    [Header("淡入时间")]
    public float defaultFadeTime = 1f;

    //静态字段，场景重载后保留，用来实现场景淡入
    private static bool startBlackOnLoad;

    private void Awake()
    {
        Instance = this;
        if (image == null) return;

        image.raycastTarget = false;//不阻挡鼠标

        if (startBlackOnLoad)
        {
            startBlackOnLoad = false;
            SetAlpha(1f);//一开始全黑
            StartCoroutine(FadeRoutine(0f, defaultFadeTime));//淡入
        }
        else
        {
            SetAlpha(0f);//透明
        }
    }

    public void FadeOutAndLoadScene(int sceneIndex, float duration)
    {
        StartCoroutine(FadeOutAndLoadRoutine(sceneIndex, duration));
    }

    public void FadeOutAndLoadScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoadRoutine(sceneName));
    }

    private IEnumerator FadeOutAndLoadRoutine(int sceneIndex, float duration)
    {
        yield return FadeRoutine(1f, duration);//变黑

        startBlackOnLoad = true;//淡入标识
        SceneManager.LoadScene(sceneIndex);//重载场景
    }

    private IEnumerator FadeOutAndLoadRoutine(string sceneName)
    {
        yield return FadeRoutine(1f, defaultFadeTime);

        startBlackOnLoad = true;
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeRoutine(float targetAlpha, float duration)
    {
        if(image == null) yield break;

        float startAlpha = image.color.a;
        float d = Mathf.Max(0.001f, duration);
        float t = 0f;
        while (t < d)
        {
            t += Time.unscaledDeltaTime;
            SetAlpha(Mathf.Lerp(startAlpha, targetAlpha, Mathf.Clamp01(t / d)));
            yield return null;//循环太快了，停帧让淡入淡出效果实现
        }
        SetAlpha(targetAlpha);//兜底黑屏/透明
    }

    private void SetAlpha(float v)
    {
        if(image == null) return;

        Color color = image.color;
        color.a = v;
        image.color = color;
    }
}
