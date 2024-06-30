using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneControllerManager : SingletonMonobehavior<SceneControllerManager>
{
    private bool isFading;
    [SerializeField] private float fadeDuration = 1f;               // 控制fade时间
    [SerializeField] private CanvasGroup faderCanvasGroup = null;
    [SerializeField] private Image faderImage = null;
    public SceneName startingSceneName;


    private IEnumerator Start()
    {
        faderImage.color = new Color(0f, 0f, 0f, 1f);
        faderCanvasGroup.alpha = 1f;

        yield return StartCoroutine(LoadSceneAndSetActive(startingSceneName.ToString()));   // 加载初始场景，并设置为活动
        EventHandler.CallAfterSceneLoadEvent();
        StartCoroutine(Fade(0f));                                                           // 使场景可见
    }

    /// <summary>
    /// sceneName为目标Scene；spawnPosition为进入目标Scene时的玩家位置
    /// </summary>
    /// <param name="sceneName">目标Scene</param>
    /// <param name="spawnPosition">进入目标Scene时的玩家位置</param>
    public void FadeAndLoadScene(string sceneName, Vector3 spawnPosition)
    {
        if (!isFading)
        {
            StartCoroutine(FadeAndSwitchScenes(sceneName, spawnPosition));
        }
    }

    private IEnumerator FadeAndSwitchScenes(string sceneName, Vector3 spawnPosition)
    {
        EventHandler.CallBeforeSceneUnloadFadeOutEvent();

        yield return StartCoroutine(Fade(1f));
        PlayerController.Instance.gameObject.transform.position = spawnPosition;

        EventHandler.CallBeforeSceneUnloadEvent();

        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);   //卸载当前场景
        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));                          //加载新场景

        EventHandler.CallAfterSceneLoadEvent();

        yield return StartCoroutine(Fade(0f));

        EventHandler.CallAfterSceneLoadFadeInEvent();
    }


    /// <summary>
    /// 给定场景的名称<sceneName>，加载该场景(additive)，并设置为活动场景
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        // 仅加载场景
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        // 设置为活动场景
        Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);  //最新加载的场景的id为SceneManager.sceneCount - 1
        SceneManager.SetActiveScene(newlyLoadedScene);
    }


    /// <summary>
    /// 将场景切换的淡入/淡出图片设置到finalAlpha值
    /// </summary>
    /// <param name="finalAlpha"></param>
    /// <returns></returns>
    private IEnumerator Fade(float finalAlpha)
    {
        isFading = true;
        faderCanvasGroup.blocksRaycasts = true;

        float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / fadeDuration;
        while (!Mathf.Approximately(faderCanvasGroup.alpha, finalAlpha))
        {
            faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha, finalAlpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }

        faderCanvasGroup.blocksRaycasts = false;
        isFading = false;
    }
}
