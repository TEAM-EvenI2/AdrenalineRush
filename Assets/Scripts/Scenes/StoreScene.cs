using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class StoreScene : BaseScene
{    
    private Animator animator;
    public GameObject SoftCurrUI;
    public GameObject HardCurrUI;
    private DataManager dataManager;
    void Start()
    {
        dataManager = FindObjectOfType<DataManager>();
        if (dataManager)
        {
            dataManager.LoadGameData();
            SoftCurrUI.GetComponent<TextMeshProUGUI>().text = dataManager.gameData.SoftCurr.ToString();
            HardCurrUI.GetComponent<TextMeshProUGUI>().text = dataManager.gameData.HardCurr.ToString();
        }
        else
        {
            Debug.LogError("Cannot load datamanager.");
        }        
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
        FindObjectOfType<AudioManager>().Play("UIClick");
        Managers.Instance.Scene.LoadScene("Lobby", null, null, false);
    }
}
