using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemEquipManager : MonoBehaviour
{
    public GameObject MagnetEquipUI;
    public GameObject BoostEquipUI;
    public GameObject ShrinkEquipUI;
    public GameObject SlowEquipUI;
    public GameObject SlotEquipUI;
    private DataManager dataManager;
    private Color EquippedColor;
    private Color UnequippedColor;

    // Update is called once per frame
    void Start()
    {
        EquippedColor = new Color(0,255,0,255);
        UnequippedColor = new Color(255,255,255,255);
        dataManager = FindObjectOfType<DataManager>();
        UpdateUI();
    }

    public void EquipOrUnequipSelectedItem()
    {
        EquipOrUnequipItemToSlot(FindObjectOfType<StoreScene>().selected);
        UpdateUI();
    }

    public void EquipOrUnequipItemToSlot(string itemId)
    {
        if (dataManager.gameData.getItem(itemId).EquipSlot != -1 || itemId == "slot" || !dataManager.gameData.getItem(itemId).HasItem) { // 슬롯아이템은 장착불가
            dataManager.gameData.getItem(itemId).EquipSlot = -1;
            dataManager.SaveGameData();
            Debug.LogError("이미 장착하고 있거나, 장착할 수 없는 아이템이거나, 보유하고 있지 않은 아이템입니다.");
            UpdateUI();
            return;
        }

        // slotNum = -1이면 알아서 장착
        for (int i = 0; i < dataManager.gameData.SlotCount; ++i)
        {
            if (dataManager.gameData.EquippedItem[i] == null)
            {
                dataManager.gameData.getItem(itemId).EquipSlot = i;
                dataManager.SaveGameData();
                UpdateUI();
                return;
            }
        }
        Debug.LogError("장착할 수 있는 칸이 없습니다."); // TODO
        UpdateUI();
        return;
    }

    void Update()
    {
        
    }

    void UpdateUI()
    {
        foreach (ItemData itemData in dataManager.gameData.purchasedItems)
        {
            if (itemData == null) continue;
            bool equipped = itemData.EquipSlot != -1;
            switch (itemData.ItemId)
            {
                case "magnet":
                    ChangeMagnetEquipUI(equipped);
                    break;
                case "boost":
                    ChangeBoostEquipUI(equipped);
                    break;
                case "shrink":
                    ChangeShrinkEquipUI(equipped);
                    break;
                case "slow":
                    ChangeSlowEquipUI(equipped);
                    break;
                case "slot":
                    ChangeSlotEquipUI(equipped);
                    break;
            }
        }
    }

    void ChangeMagnetEquipUI(bool equipped)
    {
        MagnetEquipUI.SetActive(equipped);
    }

    void ChangeBoostEquipUI(bool equipped)
    {
        BoostEquipUI.SetActive(equipped);
    }

    void ChangeShrinkEquipUI(bool equipped)
    {
        ShrinkEquipUI.SetActive(equipped);
    }

    void ChangeSlowEquipUI(bool equipped)
    {
        SlowEquipUI.SetActive(equipped);
    }

    void ChangeSlotEquipUI(bool equipped)
    {
        SlotEquipUI.SetActive(equipped);
    }
}
