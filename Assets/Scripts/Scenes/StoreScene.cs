using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreScene : BaseScene
{    
    private Animator animator;
    void Start()
    {
    }

    protected override void Init()
    {
        base.Init();

        Debug.Log("StoreScene Loaded");
        SceneType = Define.Scene.Lobby;
        Managers.Instance.SetScene(this);

    }

    public override void Clear()
    {

    }
    
    public void MoveLobbyScene()
    {
        Managers.Instance.Scene.LoadScene("Lobby", null, null, false);
    }
}
