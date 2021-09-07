using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyScene : BaseScene
{
    public GameObject GameCam;
    private Animator animator;
    public GameObject presetGetInBtn;
    public GameObject presetGetOutBtn;
    public GameObject prevPresetBtn;
    public GameObject nextPresetBtn;
    public GameObject presetName;
    public GameObject presetDesc;
    public GameObject settingBtn;
    public GameObject gameStartBtn;
    public GameObject achBtn;
    public GameObject shopBtn;
    public GameObject adBtn;
    private bool isBtnHidden = false;
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

    public void MoveStoreScene()
    {
        Managers.Instance.Scene.LoadScene("Store", null, null);
    }

    private void ToggleUIButtonsForPreset()
    {
        // preset 씬 진입 시 필요 없는 버튼들 disable, 필요한 요소들 enable
        // 다시 실행 시 반대 작업.
        settingBtn.SetActive(isBtnHidden);
        gameStartBtn.SetActive(isBtnHidden);
        achBtn.SetActive(isBtnHidden);
        shopBtn.SetActive(isBtnHidden);
        adBtn.SetActive(isBtnHidden);

        isBtnHidden = !isBtnHidden;
    }

    public void GetInPresetScene()
    {
        // 로비 씬 내에서 처리
        animator.SetBool("CameraMoveToPreset", !animator.GetBool("CameraMoveToPreset"));
        presetGetInBtn.SetActive(true);
        presetGetOutBtn.SetActive(false);
        ToggleUIButtonsForPreset();
        StartCoroutine("CoDisplayPresetUI"); // 카메라 이동 도중 UI 생기지 않게 코루틴 사용
    }

    public IEnumerator CoDisplayPresetUI()
    {
        presetName.SetActive(true);
        presetDesc.SetActive(true);
        prevPresetBtn.SetActive(true);
        nextPresetBtn.SetActive(true);
        for (float ff = 0.0f; ff <= 1.0f;) {
            ff += 0.05f;
            Color presetNameTempColor = presetName.GetComponent<TextMeshProUGUI>().faceColor;
            Color presetDescTempColor = presetDesc.GetComponent<TextMeshProUGUI>().faceColor;
            Color prevBtnTempColor = prevPresetBtn.GetComponentInChildren<Image>().color;
            Color nextBtnTempColor = nextPresetBtn.GetComponentInChildren<Image>().color;

            presetNameTempColor.a = ff;
            presetDescTempColor.a = ff;
            prevBtnTempColor.a = ff;
            nextBtnTempColor.a = ff;

            presetName.GetComponent<TextMeshProUGUI>().faceColor = presetNameTempColor;
            presetDesc.GetComponent<TextMeshProUGUI>().faceColor = presetDescTempColor;
            prevPresetBtn.GetComponent<Image>().color = prevBtnTempColor;
            nextPresetBtn.GetComponent<Image>().color = nextBtnTempColor;
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void GetOutPresetScene()
    {
        animator.SetBool("CameraMoveToPreset", !animator.GetBool("CameraMoveToPreset"));
        presetGetInBtn.SetActive(false);
        presetGetOutBtn.SetActive(true);
        presetName.SetActive(false);
        presetDesc.SetActive(false);
        prevPresetBtn.SetActive(false);
        nextPresetBtn.SetActive(false);
        ToggleUIButtonsForPreset();
    }
}
