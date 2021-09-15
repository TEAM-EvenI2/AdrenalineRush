
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameScene : BaseScene
{
    public Volume postProcessVolume;
    public Player player;

    public bool isPause = false;

    private List<int> currentSelectedBuffs;
    private Vector2[] currentSelectedBuffPos;

    protected override void Init()
    {
        base.Init();

        Debug.Log("GameScene Loaded");
        SceneType = Define.Scene.Game;
        Managers.Instance.SetScene(this);


        SettingFinish = true;
    }

    public void SettingBuff(List<int> buffs, Vector2[] buttonPos)
    {
        ((GameUIManager)baseUIManager).SettingBuff(buffs, buttonPos);
        currentSelectedBuffs = buffs;
        currentSelectedBuffPos = buttonPos;
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
        SettingFinish = false;

        FindObjectOfType<AudioManager>().Play("UIClick");
        Managers.Instance.Scene.LoadScene(sceneName,
            () => { return Managers.Instance.GetScene<GameScene>() != null && Managers.Instance.GetScene<GameScene>().SettingFinish; },
            () =>
            {
                Managers.Instance.GetScene<GameScene>().SettingBuff(currentSelectedBuffs, currentSelectedBuffPos);
            });
    }

    public void GotoMain()
    {
        FindObjectOfType<AudioManager>().Play("UIClick");
        Managers.Instance.Scene.LoadScene("Lobby", null, null);
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
