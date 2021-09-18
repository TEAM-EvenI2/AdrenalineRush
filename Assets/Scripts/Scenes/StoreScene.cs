using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class StoreScene : BaseScene
{    
    private Animator animator;
    [HideInInspector]
    public string selected; // 선택한 아이템
    public GameObject SoftCurrUI;
    public GameObject HardCurrUI;
    private DataManager dataManager;
    public GameObject ItemDescUI;
    public GameObject ItemNextUpgradeDescUI;
    public GameObject ItemCurrUpgradeDescUI;
    public GameObject ItemUpgradeArrow;
    public GameObject CurrentItemNameUI;
    public GameObject CurrentItemPriceUI;
    public GameObject CurrentItemUpgradeUI; // 현재 업그레이드가 얼만큼 되었는지
    public GameObject CurrentSlotLeftUI; // 남은 칸수
    public GameObject ShopWarnUI;
    public GameObject ShopLogUI;
    public GameObject ScrollPanel; // 아이템 버튼들을 저장중
    private AudioManager audioManager;

    void Start()
    {
        dataManager = FindObjectOfType<DataManager>();
        audioManager = FindObjectOfType<AudioManager>();
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
        selected = "";  
        UpdateUI();   
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

    public void MovePlacerScene()
    {
        FindObjectOfType<AudioManager>().Play("UIClick");
        Managers.Instance.Scene.LoadScene("ButtonPlace", null, null, false);
    }
    public void MoveLobbyScene()
    {
        FindObjectOfType<AudioManager>().Play("UIClick");
        Managers.Instance.Scene.LoadScene("Lobby", null, null, true);
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

    private bool CanPurchase(int price)
    {
        if (dataManager.gameData.SoftCurr >= price)
            return true;
        return false;
    }

    public int GetItemPrice(string itemId, int upgradeLvl)
    {
        // 첫 해금의 경우 upgradeLvl = 1
        switch (itemId)
        {
            case "magnet":
                return ItemPriceChart.MAGNET[upgradeLvl];
            case "boost":
                return ItemPriceChart.BOOST[upgradeLvl];
            case "shrink":
                return ItemPriceChart.SHRINK[upgradeLvl];
            case "slow":
                return ItemPriceChart.SLOW[upgradeLvl];
            case "slot":
                return ItemPriceChart.SLOT[upgradeLvl];
        }
        Debug.LogError("잘못된 itemId 입력 - GetItemPrice()");
        return 0;
    }

    private void SpendMoney(int amount)
    {
        if (CanPurchase(amount)) // 혹시 모를 이중 체크
        {
            dataManager.gameData.SoftCurr -= amount;
            dataManager.SaveGameData();
            return;
        }
        Debug.LogError("돈이 부족한 상황에서 SpendMoney가 호출됨.");
    }

    public void PurchaseSelectedItem()
    {
        if (selected != "" && selected != null)
            PurchaseItem(selected);
        else
            Debug.LogError("아무런 아이템도 선택되지 않았습니다.");
    }

    private void PurchaseItem(string itemId)
    {
        foreach (ItemData itemData in dataManager.gameData.purchasedItems)
        {
            if (itemData.ItemId == itemId)
            {
                int upgrade = itemData.Upgrade;
                if ((itemId == "slot" && upgrade >= 4) || upgrade >= 5){
                    WarnUser("더 이상 업그레이드 할 수 없습니다.");
                    break;
                }

                int price = GetItemPrice(itemId, upgrade);
                if (CanPurchase(price)){
                    SpendMoney(price);
                } else {
                    WarnUser("돈이 부족합니다.");
                    UpdateUI();
                    return;
                }

                // 구매완료
                if (!itemData.HasItem) itemData.HasItem = true;
                itemData.Upgrade += 1;
                UpdateUI(itemId);
                FindObjectOfType<AudioManager>().Play("PurchaseItem");
                break;
            }
        }
        dataManager.SaveGameData();
    }
    
    private void UpdateUI(string itemId="<없음>")
    {
        /**
        itemId:
            Optional, 특정 아이템을 입력 시 해당 아이템의 정보를 UI에 표시함.
            만약 공란이거나 알 수 없는 값이 주어지면 UI에 아무런 아이템 정보도 표시하지 않음. (초기화)
        */
        SoftCurrUI.GetComponent<TextMeshProUGUI>().text = dataManager.gameData.SoftCurr.ToString();
        HardCurrUI.GetComponent<TextMeshProUGUI>().text = dataManager.gameData.HardCurr.ToString();

        for (int i = 0; i < ScrollPanel.transform.childCount; ++i)// 개선 가능
        {
            ScrollPanel.transform.GetChild(i).GetComponent<Image>().color = new Color(255,255,255);
        }

        foreach (ItemData itemData in dataManager.gameData.purchasedItems)
        {
            if (itemData.ItemId == "slot")
            {
                CurrentSlotLeftUI.GetComponent<TextMeshProUGUI>().text = "아이템 슬롯 수: " + dataManager.gameData.SlotCount.ToString();
            }

            if (itemData.ItemId == itemId)
            {
                ItemNextUpgradeDescUI.GetComponent<TextMeshProUGUI>().text = ""; // 일단 지움
                switch (itemId)
                {
                    case "magnet":
                        CurrentItemNameUI.GetComponent<TextMeshProUGUI>().text = "자석: ";
                        ItemDescUI.GetComponent<TextMeshProUGUI>().text = ItemDescChart.ItemDesc[0];
                        ItemUpgradeArrow.SetActive(false);
                        ItemNextUpgradeDescUI.GetComponent<TextMeshProUGUI>().text = "";
                        if (itemData.Upgrade > 0) {
                            Debug.Log("SSSSSS");
                            ItemCurrUpgradeDescUI.GetComponent<TextMeshProUGUI>().text = "현재: "+ItemDescChart.MAGNET[itemData.Upgrade - 1];
                            if (itemData.Upgrade < 5) {
                                ItemNextUpgradeDescUI.GetComponent<TextMeshProUGUI>().text = "강화 후: "+ItemDescChart.MAGNET[itemData.Upgrade];
                                ItemUpgradeArrow.SetActive(true);
                            }
                        } else {
                            ItemCurrUpgradeDescUI.GetComponent<TextMeshProUGUI>().text = "아이템 효과: "+ItemDescChart.MAGNET[0];
                        }
                        ScrollPanel.transform.GetChild(0).GetComponent<Image>().color = new Color(0,255,0); // 인덱스로 접근하는게 좋은 방식은 아님
                        break;
                    case "boost":
                        CurrentItemNameUI.GetComponent<TextMeshProUGUI>().text = "무적 부스트: ";
                        ItemDescUI.GetComponent<TextMeshProUGUI>().text = ItemDescChart.ItemDesc[1];
                        ItemUpgradeArrow.SetActive(false);
                        ItemNextUpgradeDescUI.GetComponent<TextMeshProUGUI>().text = "";
                        if (itemData.Upgrade > 0) {
                            ItemCurrUpgradeDescUI.GetComponent<TextMeshProUGUI>().text = "현재: "+ItemDescChart.BOOST[itemData.Upgrade - 1];
                            if (itemData.Upgrade < 5) {
                                ItemNextUpgradeDescUI.GetComponent<TextMeshProUGUI>().text = "강화 후: "+ItemDescChart.BOOST[itemData.Upgrade];
                                ItemUpgradeArrow.SetActive(true);
                            }
                        } else {
                            ItemCurrUpgradeDescUI.GetComponent<TextMeshProUGUI>().text = "아이템 효과: "+ItemDescChart.BOOST[0];
                        }
                        ScrollPanel.transform.GetChild(1).GetComponent<Image>().color = new Color(0,255,0);
                        break;
                    case "shrink":
                        CurrentItemNameUI.GetComponent<TextMeshProUGUI>().text = "크기 축소: ";
                        ItemDescUI.GetComponent<TextMeshProUGUI>().text = ItemDescChart.ItemDesc[2];
                        ItemUpgradeArrow.SetActive(false);
                        ItemNextUpgradeDescUI.GetComponent<TextMeshProUGUI>().text = "";
                        if (itemData.Upgrade > 0) {
                            ItemCurrUpgradeDescUI.GetComponent<TextMeshProUGUI>().text = "현재: "+ItemDescChart.SHRINK[itemData.Upgrade - 1];
                            if (itemData.Upgrade < 5) {
                                ItemNextUpgradeDescUI.GetComponent<TextMeshProUGUI>().text = "강화 후: "+ItemDescChart.SHRINK[itemData.Upgrade];
                                ItemUpgradeArrow.SetActive(true);
                            }
                        } else {
                            ItemCurrUpgradeDescUI.GetComponent<TextMeshProUGUI>().text = "아이템 효과: "+ItemDescChart.SHRINK[0];
                        }
                        ScrollPanel.transform.GetChild(2).GetComponent<Image>().color = new Color(0,255,0);
                        break;
                    case "slow":
                        CurrentItemNameUI.GetComponent<TextMeshProUGUI>().text = "슬로우: ";
                        ItemDescUI.GetComponent<TextMeshProUGUI>().text = ItemDescChart.ItemDesc[3];
                        ItemUpgradeArrow.SetActive(false);
                        ItemNextUpgradeDescUI.GetComponent<TextMeshProUGUI>().text = "";
                        if (itemData.Upgrade > 0) {
                            ItemCurrUpgradeDescUI.GetComponent<TextMeshProUGUI>().text = "현재: "+ItemDescChart.SLOW[itemData.Upgrade - 1];
                            if (itemData.Upgrade < 5) {
                                ItemNextUpgradeDescUI.GetComponent<TextMeshProUGUI>().text = "강화 후: "+ItemDescChart.SLOW[itemData.Upgrade];
                                ItemUpgradeArrow.SetActive(true);
                            }
                        } else {
                            ItemCurrUpgradeDescUI.GetComponent<TextMeshProUGUI>().text = "아이템 효과: "+ItemDescChart.SLOW[0];
                        }
                        ScrollPanel.transform.GetChild(3).GetComponent<Image>().color = new Color(0,255,0);
                        break;
                    case "slot":
                        CurrentItemNameUI.GetComponent<TextMeshProUGUI>().text = "슬롯 추가: ";
                        ItemDescUI.GetComponent<TextMeshProUGUI>().text = ItemDescChart.ItemDesc[4];
                        ItemUpgradeArrow.SetActive(false);
                        ItemNextUpgradeDescUI.GetComponent<TextMeshProUGUI>().text = "";
                        if (itemData.Upgrade > 0) {
                            ItemCurrUpgradeDescUI.GetComponent<TextMeshProUGUI>().text = "현재: "+ItemDescChart.SLOT[itemData.Upgrade - 1];
                            if (itemData.Upgrade < 4) {
                                ItemNextUpgradeDescUI.GetComponent<TextMeshProUGUI>().text = "강화 후: "+ItemDescChart.SLOT[itemData.Upgrade];
                                ItemUpgradeArrow.SetActive(true);
                            }
                        } else {
                            ItemCurrUpgradeDescUI.GetComponent<TextMeshProUGUI>().text = "아이템 효과: "+ItemDescChart.SLOT[0];
                        }
                        ScrollPanel.transform.GetChild(4).GetComponent<Image>().color = new Color(0,255,0);
                        break;
                }
                if (itemData.Upgrade == 0){
                    CurrentItemUpgradeUI.GetComponent<TextMeshProUGUI>().text = "미구매";
                }else{
                    CurrentItemUpgradeUI.GetComponent<TextMeshProUGUI>().text = "현재 " + itemData.Upgrade.ToString() + "단계 업그레이드";
                }
                
                if ((itemId == "slot" && itemData.Upgrade >= 4) || itemData.Upgrade >= 5){
                    CurrentItemPriceUI.GetComponent<TextMeshProUGUI>().text = "업그레이드 최대치 도달";
                } else {
                    CurrentItemPriceUI.GetComponent<TextMeshProUGUI>().text = "가격: " + GetItemPrice(itemId, itemData.Upgrade).ToString() + " 혈액";
                }
                
                return;
            }
        }
        // 아이템 id 없으면 그냥 text 삭제
        CurrentItemNameUI.GetComponent<TextMeshProUGUI>().text = "";
        CurrentItemUpgradeUI.GetComponent<TextMeshProUGUI>().text = "";
        CurrentItemPriceUI.GetComponent<TextMeshProUGUI>().text = "";
    }

    public void SelectMagnet()
    {
        UpdateUI("magnet");
        selected = "magnet";
    }

    public void SelectBoost()
    {
        UpdateUI("boost");
        selected = "boost";
    }

    public void SelectShrink()
    {
        UpdateUI("shrink");
        selected = "shrink";
    }

    public void SelectSlow()
    {
        UpdateUI("slow");
        selected = "slow";
    }

    public void SelectSlot()
    {
        UpdateUI("slot");
        selected = "slot";
    }
}
