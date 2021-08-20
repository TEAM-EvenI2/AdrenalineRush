
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{

    public Player player;

    protected override void Init()
    {
        base.Init();

        Debug.Log("GameScene Loaded");
        SceneType = Define.Scene.Game;
        Managers.Instance.SetScene(this);

    }

    public override void Clear()
    {

    }

    public void Re()
    {
        Managers.Instance.Scene.LoadScene("Game", null, null);
    }

    public void Exit()
    {
        Application.Quit(0);
    }

    public int GetScore()
    {

        return (int)player.DistanceTraveled + player.earnedScore;
    }

}
