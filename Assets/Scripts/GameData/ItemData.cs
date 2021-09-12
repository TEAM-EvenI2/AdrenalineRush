using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemPriceChart
{
    readonly public static int[] MAGNET = {3500, 18000, 35000, 53000, 114000};
    readonly public static int[] BOOST = {3500, 18000, 35000, 53000, 114000};
    readonly public static int[] SHRINK = {3500, 18000, 35000, 53000, 114000};
    readonly public static int[] SLOW = {3500, 18000, 35000, 53000, 114000};
    readonly public static int[] SLOT = {3500, 18000, 35000, 53000, 114000};
}

[System.Serializable]
public class ItemData
{
    // 하나의 아이템 데이터는 하나의 아이템 타입을 나타냅니다

    public ItemData(string Id="")
    {
        itemId = Id;
        hasItem = false;
        upgrade = 0;
        equipSlot = -1; // 장착 안됨
    }

    // 아이템 구매여부
    private bool hasItem;
    public bool HasItem
    {
        get { return hasItem; }
        set { hasItem = value; }
    }


    // 아이템 구매여부
    private int equipSlot; // -1일 경우 장착 안됨.
    public int EquipSlot
    {
        get { return equipSlot; }
        set { equipSlot = value; }
    }
    
    
    // 아이템 종류 식별자
    /**
    편의성을 위해 enum대신 string 사용.
    magnet: 자석 아이템
    boost: 스피드 증가 아이템
    shrink: 크기 축소 아이템
    coin: 재화량 증가 아이템
    */
    private string itemId;
    public string ItemId
    {
        get { return itemId; }
        set { itemId = value; }
    }

    // 아이템 강화 수치
    private int upgrade;
    public int Upgrade
    {
        get { return upgrade; }
        set { upgrade = value; }
    }
}
