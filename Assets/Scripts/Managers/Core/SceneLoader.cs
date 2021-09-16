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
    private TextMeshProUGUI touchText;

    private string loadSceneName;

    private bool hasCondition;

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
        touchText = transform.Find("TouchText").GetComponent<TextMeshProUGUI>();
    }

    public void LoadScene(string sceneName, System.Func<bool> condition, System.Action action, bool diplayLoadScene=false)
    {
        gameObject.SetActive(true);
        if (diplayLoadScene)
        {
            SceneManager.sceneLoaded += LoadSceneEnd;
        }
        loadSceneName = sceneName;

        hasCondition = condition != null;
        StartCoroutine(CoLoad(sceneName, diplayLoadScene));

        if (condition != null && action != null)
        {
            StartCoroutine(CoSceneChangeAction(condition, action));
        }
    }

    private IEnumerator CoSceneChangeAction(System.Func<bool> condition, System.Action action)
    {
        while (!condition.Invoke())
            yield return null;

        action.Invoke();


        gameObject.SetActive(false);
    }

    private IEnumerator CoLoad(string sceneName, bool diplayLoadScene=false)
    {
        if (diplayLoadScene)
        {
            touchText.text = "로딩중...";
            loadingText.text = "0%";
            yield return StartCoroutine(CoFade(true)); // 스크린 fade out + screenLoader 프리팹의 박스 알파값 변경

            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName); // 엔진단에서 실제로 씬이 로딩되는 부분

            op.allowSceneActivation = false; 

            while (!op.isDone)
            {
                yield return null;
                loadingText.text = op.progress * 100 + "%";
                if (op.progress >= 0.9f)
                {
                    loadingText.text = "100%";
                    touchText.text = "화면을 터치해주세요";

                    bool tourched = false;
                    #if UNITY_EDITOR
                        tourched = Input.GetMouseButtonDown(0);
                    #else
                        tourched = Input.touchCount > 0;
                    #endif
                    if (tourched)
                    {
                        op.allowSceneActivation = true;
                        yield break;
                    }
                }
            }
        }
        else
        {
            FadeOutImmediate();
            SceneManager.LoadSceneAsync(sceneName); // 로딩창 안뜨더라도 씬은 비동기로 불러옴
        }        
    }

    private void LoadSceneEnd(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name.Equals(loadSceneName))
        {
            StartCoroutine(CoFade(false)); // fade in + screenLoader 프리팹의 박스 알파값 변경
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

    private void FadeOutImmediate()
    {
        canvasGroup.alpha = 0;
        if(!hasCondition)
            gameObject.SetActive(false);
    }
}
