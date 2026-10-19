using UnityEngine;

public class QuitOnEscape : MonoBehaviour
{
    [Tooltip("触发按键")]
    public KeyCode quitKey = KeyCode.Escape;

    //静态标记：跨场景只保留一个实例
    private static bool created;

    private void Awake()
    {
        if (created)
        {
            Destroy(gameObject);//已经有实例了，自毁
            return;
        }

        created = true;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(quitKey)) return;

#if UNITY_EDITOR
        //编辑器里Application.Quit()失效
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
