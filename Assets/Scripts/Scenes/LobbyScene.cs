using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        Debug.Log("LobbyScene Loaded");
        SceneType = Define.Scene.Lobby;
        Managers.Instance.SetScene(this);

    }

    public override void Clear()
    {

    }
    
    public void MoveGameScene()
    {
        Managers.Instance.Scene.LoadScene("Game", null, null);
    }

    public void MoveAchievementScene()
    {
        Managers.Instance.Scene.LoadScene("Achievement", null, null);
    }

    public void MoveOptionScene()
    {
        Managers.Instance.Scene.LoadScene("Option", null, null);
    }
}
