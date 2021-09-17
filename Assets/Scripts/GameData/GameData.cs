using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[System.Serializable]
public class GameData
{
    /**
    GameData 내에 저장되는 어떠한 정보라도 변할 경우 DataManager에서 debugging = true로 키고 한번 돌려서 새로 초기화해주어야 합니다
    */
    public int SoftCurr; // 소프트커런시
    public int HardCurr; // 하드커런시
    public float masterVolume;
    public bool hasVibration = true;
    public ItemData[] purchasedItems; // 게임 내 존재하는 모든 아이템들 (구매여부는 개별적으로 저장)
    public CharacterData[] purchasedCharacters; // 게임 내 존재하는 모든 캐릭터들 (구매여부는 개별적으로 저장)
    public static readonly string[] charaIdList = {"rbc", "wbc", "platelet", "plasma"};
    public static readonly string[] charaNameList = {"적혈구", "백혈구", "혈소판", "혈장"};
    public static readonly string[] charaDescList = {
        "기본 캐릭터인 적혈구는 혈관을 빠르게 이동할 수 있습니다.", 
        "작고 귀여운 백혈구는 세균을 막아주는 역할을 합니다.\n(회복속도 1.2배)", 
        "울긋불긋하게 생긴 혈소판은 생긴건 이래도 우리 혈액을 응고시켜주는 중요한 역할을 합니다.\n(회복속도 1.5배)",
        "혈장이는 혈액을 구성하는 액체성분입니다.\n(회복속도 2배)",
        };
    public static readonly int[] charaSoftCurrPriceList = {0, 0, 0, 0};
    public static readonly int[] charaHardCurrPriceList = {0, 800, 2000, 2800};
    // 가격 = sofrCurrPrice + HardCurrPrice
    public static readonly bool[] charaStartCond = {true, false, false, false};
    public static readonly string[] itemIdList = {"magnet", "shrink", "boost", "slow", "slot"};
    public SerVector2[] buttonsPos;
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

    public CharacterData CurrentChar
    {
        get {return purchasedCharacters[currentCharaIndex];}
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
        buttonsPos = new SerVector2[4] { new SerVector2(500, -162), new SerVector2(650, -77), new SerVector2(-500, -162), new SerVector2(-650, -77) };
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
