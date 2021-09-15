using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPlacerScene : BaseScene
{

    public BuffButtonPlacer bbp;

    protected override void Init()
    {
        base.Init();

        Debug.Log("ButtonPlacerScene Loaded");
        SceneType = Define.Scene.Lobby;
        Managers.Instance.SetScene(this);

        Invoke("SettingButtonByData", 0.1f);
    }

    public override void Clear()
    {

    }

    private void SettingButtonByData()
    {
        bbp.SetButtonsPositions(DataManager.instance.gameData.buttonsPos);
    }

    public void MoveLobbyScene()
    {
        Vector2[] buttons = bbp.GetButtonPositions();
        for(int i = 0; i < DataManager.instance.gameData.buttonsPos.Length; i++)
        {
            DataManager.instance.gameData.buttonsPos[i] = Utils.Vector2Ser(buttons[i]);
        }
        DataManager.instance.SaveGameData();

        FindObjectOfType<AudioManager>().Play("UIClick");
        Managers.Instance.Scene.LoadScene("Lobby", null, null, false);
    }
}
