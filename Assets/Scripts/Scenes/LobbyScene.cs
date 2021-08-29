using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScene : BaseScene
{
    public GameObject GameCam;
    private Animator animator;
    void Start()
    {
        animator = GameCam.GetComponent<Animator>();
    }

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
        Managers.Instance.Scene.LoadScene("Game", null, null, true);
    }

    public void MoveAchievementScene()
    {
        Managers.Instance.Scene.LoadScene("Achievement", null, null);
    }

    public void MoveOptionScene()
    {
        Managers.Instance.Scene.LoadScene("Option", null, null);
    }

    public void PresetScene()
    {
        // 로비 씬 내에서 처리
        animator.SetBool("CameraMoveToPreset", !animator.GetBool("CameraMoveToPreset"));

        // TODO: 이동이 끝나면 캔버스 UI 변경.
    }
}
