using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    // 하나의 아이템 데이터는 하나의 아이템 타입을 나타냅니다

    // 아이템 종류 식별자
    private string itemId;
    public string ItemId
    {
        get { return itemId; }
        set { itemId = value; }
    }

    // 아이템 강화 수치
    private int upgrade = 0;
    public int Upgrade
    {
        get { return upgrade; }
        set { upgrade = value; }
    }
}
