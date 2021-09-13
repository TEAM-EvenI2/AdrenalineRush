using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    /**
    GameData 내에 저장되는 어떠한 정보라도 변할 경우 DataManager에서 debugging = true로 키고 한번 돌려서 새로 초기화해주어야 합니다
    */
    public int SoftCurr; // 소프트커런시
    public int HardCurr; // 하드커런시
    public float masterVolume;
    public ItemData[] purchasedItems; // 구매한 아이템들
    public CharacterData[] purchasedCharacters; // 구매한 캐릭터들
    public static readonly string[] itemIdList = {"magnet", "shrink", "boost", "slow", "slot"};

    public int SlotCount
    {
        get {
            foreach (ItemData itemData in purchasedItems)
            {
                if (itemData.ItemId == "slot")
                {
                    if (!itemData.HasItem) return 0;
                    else return itemData.Upgrade;
                }
            }
            Debug.LogWarning("SlotCount 제대로 반환하지 못함.");
            return 0;
        }
    }

    public ItemData[] EquippedItem
    {
        get {
            ItemData[] eqItems = new ItemData[5];
            foreach (ItemData itemData in purchasedItems)
            {
                if (itemData.EquipSlot != -1)
                {
                    eqItems[itemData.EquipSlot] = itemData;
                }
            }
            return eqItems;
        }
    }

    public ItemData getItem(string itemId)
    {
        foreach(ItemData itemData in purchasedItems)
        {
            if (itemData.ItemId == itemId) return itemData;
        }
        return null;
    }

    public GameData()
    {
        SoftCurr = 10000000;
        HardCurr = 0;
        masterVolume = 1;
        purchasedItems = new ItemData[5];
        for (int i = 0; i < itemIdList.Length; ++i)
        {
            purchasedItems[i] = new ItemData(itemIdList[i]);
        }
        purchasedCharacters = new CharacterData[5];
    }
}
