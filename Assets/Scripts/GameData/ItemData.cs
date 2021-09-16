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

public static class ItemDescChart
{
    readonly public static string[] ItemDesc = {
        "일정 범위에서 캐릭터가 아이템들을 끌어 당긴다. 장애물을 피하느라 정신이 없을 때 더 쉽게 아이템들을 먹을 수 있다. ", 
        "일정 시간 동안 캐릭터의 크기가 작아진다. 많은 장애물들을 피하기에 안성맞춤!", 
        "일정시간 동안 맵 전체의 속도가 줄어든다. 보다 느리지만 더 정밀한 조작이 가능하다.", 
        "일정 시간동안 장애물들을 무시하고 빠르게 지나갈 수 있다.", 
        "게임 시작 전 적용가능한 버프 아이템 개수를 설정할 수 있다. 버프 아이템을 사기 전 필수!"
        };

    readonly public static string[] MAGNET = {
        "3초간 일정 범위 내 자석효과 발생", 
        "5초간 일정 범위 내 자석효과 발생", 
        "7초간 일정 범위 내 자석효과 발생", 
        "10초간 일정 범위 내 자석효과 발생", 
        "15초간 일정 범위 내 자석효과 발생"
        };
    readonly public static string[] SHRINK = {
        "3초간 캐릭터 축소효과 발생", 
        "5초간 캐릭터 축소효과 발생", 
        "7초간 캐릭터 축소효과 발생", 
        "10초간 캐릭터 축소효과 발생", 
        "15초간 캐릭터 축소효과 발생"
        };
    readonly public static string[] SLOW = {
        "3초간 속도 감소효과 발생", 
        "5초간 속도 감소효과 발생", 
        "7초간 속도 감소효과 발생", 
        "10초간 속도 감소효과 발생", 
        "15초간 속도 감소효과 발생"
        };
    readonly public static string[] BOOST = {
        "3초간 부스트와 함께 무적효과",
        "5초간 부스트와 함께 무적효과",
        "8초간 부스트와 함께 무적효과",
        "12초간 부스트와 함께 무적효과",
        "17초간 부스트와 함께 무적효과"
        };    
    readonly public static string[] SLOT = {
        "1개의 아이템 슬롯 사용 가능", 
        "2개의 아이템 슬롯 사용 가능", 
        "3개의 아이템 슬롯 사용 가능", 
        "4개의 아이템 슬롯 사용 가능", 
        "5개의 아이템 슬롯 사용 가능", 
        };
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
        if (itemId == "slot")
        {
            upgrade = 1;
            hasItem = true;
        }
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
