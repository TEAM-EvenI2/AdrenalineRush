
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameScene : BaseScene
{
    public Volume postProcessVolume;
    public Player player;

    public bool isPause = false;

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

    public void Pause()
    {
        isPause = !isPause;
    }

    public void Re(string sceneName)
    {
        Managers.Instance.Scene.LoadScene(sceneName, null, null);
    }

    public void GotoMain()
    {
        Managers.Instance.Scene.LoadScene("Lobby", null, null);
    }

    public void Exit()
    {
        Application.Quit(0);
    }

    public int GetScore()
    {

        return (int)player.DistanceTraveled + player.earnedItem;
    }

}
