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
    public ItemData[] purchasedItems; // 게임 내 존재하는 모든 아이템들 (구매여부는 개별적으로 저장)
    public CharacterData[] purchasedCharacters; // 게임 내 존재하는 모든 캐릭터들 (구매여부는 개별적으로 저장)
    public static readonly string[] charaIdList = {"rbc", "plasma", "wbc", "platelet"};
    public static readonly string[] charaNameList = {"적혈구", "혈장", "백혈구", "혈소판"};
    public static readonly string[] charaDescList = {"적혈구입니다.", "혈장입니다.", "백혈구입니다.", "혈소판입니다."};
    public static readonly int[] charaSoftCurrPriceList = {500, 500, 0, 0};
    public static readonly int[] charaHardCurrPriceList = {0, 0, 1, 1};
    // 가격 = sofrCurrPrice + HardCurrPrice
    public static readonly bool[] charaStartCond = {true, false, false, false};
    public static readonly string[] itemIdList = {"magnet", "shrink", "boost", "slow", "slot"};
    public int currentCharaIndex = 0; // 실제로 장착한 캐릭터 인덱스가 아님.
    public int equippedCharaIndex = 0; // 장착한 인덱스

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

    public CharacterData EquippedCharacter
    {
        get {return purchasedCharacters[currentCharaIndex];}
    }

    public void EquipCurrentCharacter()
    {
        // 현재 선택(장착 아님)한 캐릭터를 장착함.
        equippedCharaIndex = currentCharaIndex;
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
        purchasedCharacters = new CharacterData[5];
        for (int i = 0; i < charaIdList.Length; ++i)
        {
            purchasedCharacters[i] = new CharacterData(charaIdList[i], charaNameList[i], charaDescList[i], charaSoftCurrPriceList[i], charaHardCurrPriceList[i], charaStartCond[i]);
        }
        for (int i = 0; i < itemIdList.Length; ++i)
        {
            purchasedItems[i] = new ItemData(itemIdList[i]);
        }
    }
}
