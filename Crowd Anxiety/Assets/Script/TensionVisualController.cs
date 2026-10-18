using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//视野收缩
//把压力值接到画面后处理上
public class TensionVisualController : MonoBehaviour
{
    [Header("数据源")]
    [Tooltip("PressureSystem")]
    public PressureSystem pressureSystem;

    [Header("Volume")]
    [Tooltip("TensionVolumeProfile")]
    public VolumeProfile profile;

    [Header("Vignette:强度")]
    [Tooltip("初始值")]
    [Range(0f, 1f)] public float instensityClam = 0.2f;
    [Tooltip("满压力")]
    [Range(0f, 1f)] public float instensityTense = 0.8f;

    [Header("Vignette:过度硬度")]
    [Tooltip("初始值")]
    [Range(0f, 1f)] public float smoothnessClam = 0.4f;
    [Tooltip("满压力")]
    [Range(0f, 1f)] public float smoothnessTense = 0.6f;

    [Header("阶段跃迁冲击")]
    [Tooltip("越过阈值的瞬时收缩")]
    [Range(0f, 0.5f)] public float punchStrength = 0.2f;
    [Tooltip("冲击衰减时间/s")]
    [Range(0.1f, 2f)] public float punchDuration = 0.5f;

    private Vignette vignette;
    private int lastStage = 1;
    private float punchTimer;

    private void Awake()
    {
        if(profile == null)
        {
            Debug.LogError("TensionVisualController:没有指定Volume Profile", this);
            return;
        }
        if(!profile.TryGet(out vignette))
        {
            Debug.LogError("TensionVisualController:Profile 没有添加Vignette override", this);
            return;
        }

        vignette.intensity.overrideState = true;
        vignette.smoothness.overrideState = true;
    }

    private void Update()
    {
        if (pressureSystem == null || vignette == null) return;
        //跃迁
        if(pressureSystem.Stage > lastStage)
        {
            lastStage = pressureSystem.Stage;
            punchTimer = punchDuration;
        }

        float t = Mathf.Clamp01(pressureSystem.Pressure);
        float intensity = Mathf.Lerp(instensityClam, instensityTense, t);
        float smoothness = Mathf.Lerp(smoothnessClam, smoothnessTense, intensity);
        //衰减
        if(punchTimer > 0f)
        {
            punchTimer -= Time.deltaTime;
            intensity += punchStrength * Mathf.Clamp01(punchTimer / punchDuration);
        }

        vignette.intensity.value = Mathf.Clamp01(intensity);
        vignette.smoothness.value = Mathf.Clamp01(smoothness);
    }
}
