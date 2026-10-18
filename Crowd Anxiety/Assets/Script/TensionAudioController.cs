using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//压力值接上音效
//心跳随着压力值逐渐增大
//耳鸣，三阶段淡入
public class TensionAudioController : MonoBehaviour
{
    [Header("数据源")]
    [Tooltip("PressureSystem")]
    public PressureSystem pressureSystem;

    [Header("音频片段")]
    public AudioClip heartbeatClip;
    public AudioClip tinnitusClip;

    [Header("心跳音量")]
    [Tooltip("初始值")]
    [Range(0f, 1f)] public float heartbeatCalm = 0.05f;
    [Tooltip("满压力")]
    [Range(0f, 1f)] public float heartbeatTense = 0.5f;

    [Header("耳鸣（三阶段）")]
    [Range(0f, 1f)] public float tinnitusVolume = 0.3f;
    [Tooltip("淡入/s")]
    public float tinnitusFadeIn = 0.5f;
    [Tooltip("淡出/s")]
    public float tinnitusFadeOut = 1f;

    private AudioSource heartbeatSource;
    private AudioSource tinnitusSource;

    private void Awake()
    {
        heartbeatSource = CreateSource("Heartbeat", heartbeatClip);
        tinnitusSource = CreateSource("Tinnitus", tinnitusClip);
    }

    //子物体上创建循环播放的2D音源
    private AudioSource CreateSource(string name, AudioClip clip)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform, false);

        var src = go.AddComponent<AudioSource>();
        src.clip = clip;
        src.loop = true;
        src.playOnAwake = false;
        src.spatialBlend = 0f;//0-2D音效
        src.volume = 0f;

        if(clip != null)
            src.Play();

        return src;
    }

    private void Update()
    {
        if (pressureSystem == null) return;

        //心跳：线性增大
        if(heartbeatSource != null)
        {
            float t = Mathf.Clamp01(pressureSystem.Pressure);
            heartbeatSource.volume = Mathf.Lerp(heartbeatCalm, heartbeatTense, t);
        }

        //耳鸣：三阶段淡入
        if(tinnitusSource != null)
        {
            float target = pressureSystem.Stage >= 3 ? tinnitusVolume : 0f;

            float speed = (target > tinnitusVolume)
                ? tinnitusVolume / Mathf.Max(0.01f, tinnitusFadeIn)
                : tinnitusVolume / Mathf.Max(0.01f, tinnitusFadeOut);

            tinnitusSource.volume = Mathf.MoveTowards(tinnitusSource.volume, target, speed * Time.deltaTime);
        }
    }
}
