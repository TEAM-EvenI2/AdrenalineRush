using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionScene : BaseScene
{    
    private Animator animator;
    void Start()
    {
    }

    protected override void Init()
    {
        base.Init();

        Debug.Log("OptionScene Loaded");
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
