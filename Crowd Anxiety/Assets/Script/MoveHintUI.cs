using UnityEngine;

/// 第一关的操作提示：出现后，玩家第一次点击就淡出消失。
[RequireComponent(typeof(CanvasGroup))]
public class MoveHintUI : MonoBehaviour
{
    [Tooltip("停留多久后自动淡出（秒）。设成 0 = 只靠点击消失")]
    public float autoHideAfter = 10f;

    [Tooltip("淡出时长（秒）")]
    public float fadeOutTime = 0.6f;

    private CanvasGroup group;
    private float timer;
    private bool hiding;
    private float hideTimer;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        group.alpha = 1f;
        timer = autoHideAfter;
    }

    private void Update()
    {
        if (!hiding)
        {
            // 第一次点击（或触摸）就消失——这正是玩家开始移动的瞬间
            bool clicked = Input.GetMouseButtonDown(0) || Input.touchCount > 0;

            if (autoHideAfter > 0f)
                timer -= Time.deltaTime;

            if (clicked || (autoHideAfter > 0f && timer <= 0f))
            {
                hiding = true;
                hideTimer = fadeOutTime;
            }

            return;
        }

        hideTimer -= Time.deltaTime;
        group.alpha = Mathf.Clamp01(hideTimer / Mathf.Max(0.0001f, fadeOutTime));

        if (hideTimer <= 0f)
            gameObject.SetActive(false);
    }
}