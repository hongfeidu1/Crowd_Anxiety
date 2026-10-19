using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//开始游戏按钮，黑屏后进入第一关
public class StartButton : MonoBehaviour
{
    [Tooltip("要加载的游戏场景（第一关，没做选关界面）")]
    public string gameSceneName = "SampleScene";

    //挂载在Button的OnClick
    public void OnClick()
    {
        if (ScreenFade.Instance != null)
            ScreenFade.Instance.FadeOutAndLoadScene(gameSceneName);
        else
            SceneManager.LoadScene(gameSceneName);
    }
}
