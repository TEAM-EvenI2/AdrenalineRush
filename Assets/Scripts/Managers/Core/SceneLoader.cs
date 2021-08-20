using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneLoader : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private Image loadingImage;
    private TextMeshProUGUI loadingText;

    private string loadSceneName;

    public static SceneLoader Create()
    {
        var SceneLoaderPrefab = Resources.Load<SceneLoader>("Prefabs/SceneLoader");
        return Instantiate(SceneLoaderPrefab);
    }

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        loadingImage = transform.Find("LoadingImage").GetComponent<Image>();
        loadingText = transform.Find("LoadingMessage").GetComponent<TextMeshProUGUI>();
    }

    public void LoadScene(string sceneName, System.Func<bool> condition, System.Action action)
    {
        gameObject.SetActive(true);
        SceneManager.sceneLoaded += LoadSceneEnd;
        loadSceneName = sceneName;
        StartCoroutine(CoLoad(sceneName));

        if(condition != null)
        {
            StartCoroutine(CoSceneChangeAction(condition, action));
        }
    }

    private IEnumerator CoSceneChangeAction(System.Func<bool> condition, System.Action action)
    {
        while (!condition.Invoke())
            yield return null;

        action.Invoke();
    }

    private IEnumerator CoLoad(string sceneName)
    {
        loadingText.text = "0%";
        yield return StartCoroutine(CoFade(true));
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false; 

        while (!op.isDone)
        {
            yield return null;
            loadingText.text = op.progress * 100 + "%";
            if (op.progress >= 0.9f)
            {
                loadingText.text = "100%";
                op.allowSceneActivation = true;
                yield break;
            }
        }
    }

    private void LoadSceneEnd(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name.Equals(loadSceneName))
        {
            StartCoroutine(CoFade(false));
            SceneManager.sceneLoaded -= LoadSceneEnd;
        }
    }

    private IEnumerator CoFade(bool isFadeIn)
    {
        float timer = 0f;
        while (timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * 2f;
            canvasGroup.alpha = Mathf.Lerp(isFadeIn ? 0 : 1, isFadeIn ? 1 : 0, timer);
        }
        if (!isFadeIn)
        {
            gameObject.SetActive(false);
        }
    }
}
