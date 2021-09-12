using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class StoreScene : BaseScene
{    
    private Animator animator;
    public string selected; // 선택한 아이템
    public GameObject SoftCurrUI;
    public GameObject HardCurrUI;
    private DataManager dataManager;

    public GameObject CurrentItemNameUI;
    public GameObject CurrentItemPriceUI;
    public GameObject CurrentItemUpgradeUI; // 현재 업그레이드가 얼만큼 되었는지

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
        selected = "";       
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
        // 돈 차감의 경우 음수를 보내야함.
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
        if (selected != "")
            PurchaseItem(selected);
        else
            Debug.LogWarning("아무 아이템도 선택하지 않았습니다.");
    }

    private void PurchaseItem(string itemId)
    {
        foreach (ItemData itemData in dataManager.gameData.purchasedItems)
        {
            if (itemData.ItemId == itemId)
            {
                int upgrade = itemData.Upgrade;
                if (upgrade >= 5) break;

                int price = GetItemPrice(itemId, upgrade);
                if (CanPurchase(price)){
                    SpendMoney(price);
                } else {
                    Debug.LogWarning("돈이 부족합니다.");
                    UpdateUI();
                    return; // TODO : 부족알림?
                }

                // 구매완료
                if (!itemData.HasItem) itemData.HasItem = true;
                itemData.Upgrade += 1;
                UpdateUI(itemId);
                break;
            }
        }
        dataManager.SaveGameData();
    }
    
    private void UpdateUI(string itemId="<없음>")
    {
        SoftCurrUI.GetComponent<TextMeshProUGUI>().text = dataManager.gameData.SoftCurr.ToString();
        HardCurrUI.GetComponent<TextMeshProUGUI>().text = dataManager.gameData.HardCurr.ToString();
        foreach (ItemData itemData in dataManager.gameData.purchasedItems)
        {
            if (itemData.ItemId == itemId)
            {
                switch (itemId)
                {
                    case "magnet":
                        CurrentItemNameUI.GetComponent<TextMeshProUGUI>().text = "자석: ";
                        break;
                    case "boost":
                        CurrentItemNameUI.GetComponent<TextMeshProUGUI>().text = "무적 부스트: ";
                        break;
                    case "shrink":
                        CurrentItemNameUI.GetComponent<TextMeshProUGUI>().text = "크기 축소: ";
                        break;
                    case "slow":
                        CurrentItemNameUI.GetComponent<TextMeshProUGUI>().text = "슬로우: ";
                        break;
                    case "slot":
                        CurrentItemNameUI.GetComponent<TextMeshProUGUI>().text = "슬롯 추가: ";
                        break;
                }
                if (itemData.Upgrade == 0){
                    CurrentItemUpgradeUI.GetComponent<TextMeshProUGUI>().text = "미구매";
                }else{
                    CurrentItemUpgradeUI.GetComponent<TextMeshProUGUI>().text = "현재 " + itemData.Upgrade.ToString() + "단계 업그레이드";
                }
                
                if (itemData.Upgrade >= 5){
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
