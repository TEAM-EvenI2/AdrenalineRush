using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyScene : BaseScene
{
    public GameObject GameCam;
    private Animator animator;
    private AudioManager audioManager;
    private DataManager dataManager;
    private int hardCurrGainPerAd = 100; // 광고 1회 클릭시 획득재화량
    private readonly string[] urls = {"https://gamesmith.tistory.com/"};
    public GameObject[] ModelArr; // GameData의 CharaId와 순서가 똑같아야 함
    public GameObject presetGetInBtn;
    public GameObject presetGetOutBtn;
    public GameObject hardCurr;
    public GameObject softCurr;
    public GameObject prevPresetBtn;
    public GameObject nextPresetBtn;
    public GameObject presetName;
    public GameObject presetDesc;
    public GameObject presetPrice;
    public GameObject settingBtn;
    public GameObject gameStartBtn;
    public GameObject achBtn;
    public GameObject shopBtn;
    public GameObject adBtn;
    public GameObject ShopWarnUI; // 캐릭터샵 ui
    public GameObject ShopLogUI; // 캐릭터샵 ui
    public GameObject PurchaseBtn; // 캐릭터샵 ui
    public GameObject CharacterLockUI;
    private bool isBtnHidden = false;
    void Start()
    {
        animator = GameCam.GetComponent<Animator>();
        dataManager = FindObjectOfType<DataManager>();
        audioManager = FindObjectOfType<AudioManager>();
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
        FindObjectOfType<AudioManager>().Play("UIClick");

        List<int> selectedItem = new List<int>();
        ItemData[] equipedItem = DataManager.instance.gameData.EquippedItem;
        for (int i = 0; i < equipedItem.Length; i++)
        {
            if (equipedItem[i] != null)
            {
                int id = -1;
                for (int j = 0; j < GameData.itemIdList.Length; j++)
                {
                    if(equipedItem[i].ItemId == GameData.itemIdList[j])
                    {
                        id = j;
                        break;
                    }
                }

                if (id != -1)
                    selectedItem.Add(id);
            }
            else
                break;
        }



        Managers.Instance.Scene.LoadScene("Game", 
            () =>{ return Managers.Instance.GetScene<GameScene>() != null && Managers.Instance.GetScene<GameScene>().SettingFinish; },
            ()=>
            {
                Managers.Instance.GetScene<GameScene>().SettingBuff(selectedItem);
            }, true);
    }

    public void MoveAchievementScene()
    {
        FindObjectOfType<AudioManager>().Play("UIClick");
        Managers.Instance.Scene.LoadScene("Achievement", null, null);
    }

    public void MoveOptionScene()
    {
        FindObjectOfType<AudioManager>().Play("UIClick");
        Managers.Instance.Scene.LoadScene("Option", null, null);
    }

    public void MoveStoreScene()
    {
        FindObjectOfType<AudioManager>().Play("UIClick");
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
        hardCurr.SetActive(!isBtnHidden);
        softCurr.SetActive(!isBtnHidden);
        PurchaseBtn.SetActive(!isBtnHidden);

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
        UpdatePresetUI();
        presetName.SetActive(true);
        presetDesc.SetActive(true);
        presetPrice.SetActive(true);
        prevPresetBtn.SetActive(true);
        nextPresetBtn.SetActive(true);
        for (float ff = 0.0f; ff <= 1.0f;) {
            ff += 0.05f;
            Color presetNameTempColor = presetName.GetComponent<TextMeshProUGUI>().faceColor;
            Color presetDescTempColor = presetDesc.GetComponent<TextMeshProUGUI>().faceColor;
            Color presetPriceTempColor = presetPrice.GetComponent<TextMeshProUGUI>().faceColor;
            Color prevBtnTempColor = prevPresetBtn.GetComponentInChildren<Image>().color;
            Color nextBtnTempColor = nextPresetBtn.GetComponentInChildren<Image>().color;

            presetNameTempColor.a = ff;
            presetDescTempColor.a = ff;
            presetPriceTempColor.a = ff;
            prevBtnTempColor.a = ff;
            nextBtnTempColor.a = ff;

            presetName.GetComponent<TextMeshProUGUI>().faceColor = presetNameTempColor;
            presetDesc.GetComponent<TextMeshProUGUI>().faceColor = presetDescTempColor;
            presetPrice.GetComponent<TextMeshProUGUI>().faceColor = presetPriceTempColor;
            prevPresetBtn.GetComponent<Image>().color = prevBtnTempColor;
            nextPresetBtn.GetComponent<Image>().color = nextBtnTempColor;
            yield return new WaitForSeconds(0.05f);
        }
    }

    void UpdatePresetUI()
    {
        GameData tmp = dataManager.gameData;
        presetName.GetComponent<TextMeshProUGUI>().text = tmp.CurrentChar.CharacterName;
        presetDesc.GetComponent<TextMeshProUGUI>().text = tmp.CurrentChar.CharacterDesc;
        CharacterLockUI.SetActive(!tmp.CurrentChar.Purchased);
        string price = "";
        if (tmp.CurrentChar.HardCurrPrice > 0) price += $"{tmp.CurrentChar.HardCurrPrice} 다이아 ";
        if (tmp.CurrentChar.SoftCurrPrice > 0) price += $"{tmp.CurrentChar.SoftCurrPrice} 혈액 ";
        presetPrice.GetComponent<TextMeshProUGUI>().text = price;
        softCurr.GetComponent<TextMeshProUGUI>().text = tmp.SoftCurr.ToString();
        hardCurr.GetComponent<TextMeshProUGUI>().text = tmp.HardCurr.ToString();
    }

    private bool CanPurchase(int soft=0, int hard=0)
    {
        if (dataManager.gameData.SoftCurr >= soft && dataManager.gameData.HardCurr >= hard) return true;
        return false;
    }

    private void SpendMoney(int soft=0, int hard=0)
    {
        if (CanPurchase(soft, hard)) // 혹시 모를 이중 체크
        {
            dataManager.gameData.SoftCurr -= soft;
            dataManager.gameData.HardCurr -= hard;
            dataManager.SaveGameData();
            return;
        }
        Debug.LogError("돈이 부족한 상황에서 SpendMoney가 호출됨.");
    }

    public void PurchaseCharacter()
    {
        // 현재 선택된 캐릭터를 구매
        GameData gd = dataManager.gameData;
        int currInd = gd.currentCharaIndex;
        CharacterData currChar = dataManager.gameData.CurrentChar;

        if (currChar.Purchased){
            WarnUser("이미 보유하고 있습니다.");
            return;
        }

        // 구매
        if (CanPurchase(currChar.SoftCurrPrice, currChar.HardCurrPrice)){
            SpendMoney(currChar.SoftCurrPrice, currChar.HardCurrPrice);
        } else {
            WarnUser("돈이 부족합니다.");
            return;
        }

        // 구매완료
        currChar.Purchased = true;
        audioManager.Play("PurchaseItem");
        UpdatePresetUI();
        dataManager.SaveGameData();
    }

    public void WarnUser(string msg)
    {
        //사용자에게 상점 내 알림창을 띄웁니다.
        ShopLogUI.GetComponent<TextMeshProUGUI>().text = msg;
        ShopWarnUI.SetActive(true);
        audioManager.Play("UserWarn");
    }

    public void CloseWarnUI()
    {
        //상점 내 알림창을 닫습니다.
        ShopWarnUI.SetActive(false);
        ShopLogUI.GetComponent<TextMeshProUGUI>().text = "";
        audioManager.Play("UIClick");
    }

    void UpdatePresetModel()
    {   
        string id = dataManager.gameData.CurrentChar.CharacterId;
        int index = 0;
        switch (id)
        {
            case "rbc": // .하드코딩됨. 좋은 방식은 아님.
                index = 0;
                break;
            case "plasma":
                index = 1;
                break;
            case "wbc":
                index = 2;
                break;
            case "platelet":
                index = 3;
                break;
        }
        for (int i = 0 ; i < ModelArr.Length; i++)
        {
            if (i == index) ModelArr[i].SetActive(true);
            else ModelArr[i].SetActive(false); // SetActive 대신 instantiate를 사용해 프래팹을 가지고와도 괜찮지만 아직 굳이 그럴 단계는 아님.
        }
    }

    public void GetOutPresetScene()
    {
        if (!dataManager.gameData.CurrentChar.Purchased){
            WarnUser("구매하지 않은 캐릭터를 사용할 수 없습니다.");
            return;
        }
        animator.SetBool("CameraMoveToPreset", !animator.GetBool("CameraMoveToPreset"));
        presetGetInBtn.SetActive(false);
        presetGetOutBtn.SetActive(true);
        presetName.SetActive(false);
        presetDesc.SetActive(false);
        presetPrice.SetActive(false);
        prevPresetBtn.SetActive(false);
        nextPresetBtn.SetActive(false);
        ToggleUIButtonsForPreset();
    }

    public void NextPreset()
    {
        GameData tmp = dataManager.gameData;
        tmp.currentCharaIndex += 1;
        if (tmp.currentCharaIndex > GameData.charaIdList.Length-1)
        {
            tmp.currentCharaIndex = 0;
        }
        UpdatePresetUI();
        UpdatePresetModel();
    } 

    public void PrevPreset()
    {
        GameData tmp = dataManager.gameData;
        tmp.currentCharaIndex -= 1;
        if (tmp.currentCharaIndex < 0)
        {
            tmp.currentCharaIndex = GameData.charaIdList.Length - 1;
        }
        UpdatePresetUI();
        UpdatePresetModel();
    }

    public void ShowAd()
    {
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            WarnUser("인터넷 연결 상태를 확인해주세요.");
            return;
        }
        Application.OpenURL(urls[Random.Range(0, urls.Length-1)]);
        WarnUser($"{hardCurrGainPerAd} 다이아를 획득하셨습니다!");
        dataManager.gameData.HardCurr += hardCurrGainPerAd;
    }
}
