using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers s_instance; // 유일성이 보장된다
    public static Managers Instance
    {
        get
        {
            if (s_instance == null)
            {
                GameObject go = GameObject.Find("@Managers");
                if (go == null)
                {
                    go = new GameObject { name = "@Managers" };
                    go.AddComponent<Managers>();
                }

                DontDestroyOnLoad(go);
                s_instance = go.GetComponent<Managers>();

                s_instance.Scene.Init();
            }
            return s_instance;
        }
    } // 유일한 매니저를 갖고온다

    #region Contents
    public ResourceManager Resource { get; } = new ResourceManager();
    public CustomSceneManager Scene { get; } = new CustomSceneManager();

    public BaseScene scene { get; private set; }
    #endregion


    void Update()
    {
    }

    public void SetScene(BaseScene scene)
    {
        this.scene = scene;
    }

    public void Clear()
    {
    }

    public T GetUIManager<T>() where T : UIManager
    {
        return scene.baseUIManager as T;
    }

    public T GetScene<T>() where T : BaseScene
    {
        return scene as T;
    }

}
