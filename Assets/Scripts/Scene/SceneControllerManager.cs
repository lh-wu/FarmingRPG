using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneControllerManager : SingletonMonobehavior<SceneControllerManager>
{
    private bool isFading;
    [SerializeField] private float fadeDuration = 1f;               // ����fadeʱ��
    [SerializeField] private CanvasGroup faderCanvasGroup = null;
    [SerializeField] private Image faderImage = null;
    public SceneName startingSceneName;


    private IEnumerator Start()
    {
        faderImage.color = new Color(0f, 0f, 0f, 1f);
        faderCanvasGroup.alpha = 1f;

        yield return StartCoroutine(LoadSceneAndSetActive(startingSceneName.ToString()));   // ���س�ʼ������������Ϊ�
        EventHandler.CallAfterSceneLoadEvent();
        StartCoroutine(Fade(0f));                                                           // ʹ�����ɼ�
    }

    /// <summary>
    /// sceneNameΪĿ��Scene��spawnPositionΪ����Ŀ��Sceneʱ�����λ��
    /// </summary>
    /// <param name="sceneName">Ŀ��Scene</param>
    /// <param name="spawnPosition">����Ŀ��Sceneʱ�����λ��</param>
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

        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);   //ж�ص�ǰ����
        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));                          //�����³���

        EventHandler.CallAfterSceneLoadEvent();

        yield return StartCoroutine(Fade(0f));

        EventHandler.CallAfterSceneLoadFadeInEvent();
    }


    /// <summary>
    /// ��������������<sceneName>�����ظó���(additive)��������Ϊ�����
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        // �����س���
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        // ����Ϊ�����
        Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);  //���¼��صĳ�����idΪSceneManager.sceneCount - 1
        SceneManager.SetActiveScene(newlyLoadedScene);
    }


    /// <summary>
    /// �������л��ĵ���/����ͼƬ���õ�finalAlphaֵ
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
