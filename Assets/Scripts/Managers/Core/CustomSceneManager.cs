using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSceneManager
{

    private SceneLoader sceneLoader;

    public void Init()
    {
        if(sceneLoader == null)
        {
            sceneLoader = SceneLoader.Create();
            sceneLoader.transform.SetParent(Managers.Instance.transform);
            sceneLoader.transform.localScale = Vector3.one;
        }
    }

    /// <param name="displayLoadScene">boolean, 참일 경우 로드 씬을 화면 상에 출력하지 않음.</param>
    public void LoadScene(string sceneName, System.Func<bool> condition, System.Action action, bool diplayLoadScene=false)
    {
        sceneLoader.LoadScene(sceneName, condition, action, diplayLoadScene);
    }

    public int GetCurrentSceneBuildIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }
    

}
