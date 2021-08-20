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

    public void LoadScene(string sceneName, System.Func<bool> condition, System.Action action)
    {
        sceneLoader.LoadScene(sceneName, condition, action);
    }

    public int GetCurrentSceneBuildIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }
    

}
