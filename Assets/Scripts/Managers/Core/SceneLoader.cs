using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneLoader : MonoBehaviour
{
    private Image loadingImage;
    private TextMeshProUGUI loadingText;
    private SpriteSlider storyToon;
    private TextMeshProUGUI touchText;

    private string loadSceneName;

    private bool _diplayLoadScene;
    private bool _hasCondition;

    private int fadeState = 0;
    private bool doFadeAnim = false;

    private Animator animator;

    public static SceneLoader Create()
    {
        var SceneLoaderPrefab = Resources.Load<SceneLoader>("Prefabs/SceneLoader");
        return Instantiate(SceneLoaderPrefab);
    }

    private void Awake()
    {
        loadingImage = transform.Find("LoadingImage").GetComponent<Image>();
        loadingText = transform.Find("LoadingMessage").GetComponent<TextMeshProUGUI>();
        storyToon = transform.Find("StoryToon").GetComponent<SpriteSlider>();
        touchText = transform.Find("TouchText").GetComponent<TextMeshProUGUI>();

        animator = GetComponent<Animator>();
    }

    public void LoadScene(string sceneName, System.Func<bool> condition, System.Action action, bool diplayLoadScene=false, bool showToon=false)
    {
        _diplayLoadScene = diplayLoadScene;
        gameObject.SetActive(true);
        if (diplayLoadScene)
        {
            SceneManager.sceneLoaded += LoadSceneEnd;
        }
        loadSceneName = sceneName;

        _hasCondition = condition != null;
        StartCoroutine(CoLoad(sceneName, diplayLoadScene, showToon));

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

        if(!_diplayLoadScene)
            gameObject.SetActive(false);
    }

    private IEnumerator CoLoad(string sceneName, bool diplayLoadScene, bool showToon)
    {
        if (diplayLoadScene)
        {
            touchText.text = "?????????...";
            //loadingText.text = "0%";
            yield return StartCoroutine(CoFade(true)); // ????????? fade out + screenLoader ???????????? ?????? ????????? ??????

            storyToon.gameObject.SetActive(showToon);

            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName); // ??????????????? ????????? ?????? ???????????? ??????

            op.allowSceneActivation = false; 

            while (!op.isDone)
            {
                yield return null;
                //loadingText.text = op.progress * 100 + "%";
                if (op.progress >= 0.9f)
                {
                    bool tourched = true;
                    if (showToon)
                    {
                        tourched = false;
                        //loadingText.text = "100%";
                        //touchText.text = "????????? ???????????? ???????????????";
                        touchText.text = "";

                        tourched = storyToon.finished;
                    }
                    if (tourched)
                    {
                        op.allowSceneActivation = true;
                        storyToon.gameObject.SetActive(false);
                        yield break;
                    }
                }
            }
        }
        else
        {
            FadeOutImmediate();
            SceneManager.LoadSceneAsync(sceneName); // ????????? ??????????????? ?????? ???????????? ?????????
        }        
    }

    private void LoadSceneEnd(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name.Equals(loadSceneName))
        {
            StartCoroutine(CoFade(false)); // fade in + screenLoader ???????????? ?????? ????????? ??????
            SceneManager.sceneLoaded -= LoadSceneEnd;
        }
    }

    private IEnumerator CoFade(bool isFadeIn)
    {
        //float timer = 0f;
        //while (timer <= 1f)
        //{
        //    yield return null;
        //    timer += Time.unscaledDeltaTime * 2f;
        //    canvasGroup.alpha = Mathf.Lerp(isFadeIn ? 0 : 1, isFadeIn ? 1 : 0, timer);
        //}

        if (isFadeIn) { 
        animator.Play("SceneLoading_FadeIn");
    }
        else
        {
            animator.Play("SceneLoading_FadeOut");
        }

        doFadeAnim = true;

        while (doFadeAnim)
        {
            yield return null;
        }

        if (!isFadeIn)
        {
            gameObject.SetActive(false);
            print("FadeOut");
        }
    }

    private void FadeOutImmediate()
    {
        if(!_hasCondition)
            gameObject.SetActive(false);
    }

    public void FinishedFadeAnim()
    {
        doFadeAnim = false;
    }
}
