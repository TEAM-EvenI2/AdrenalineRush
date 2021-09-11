using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int SoftCurr; // 소프트커런시
    public int HardCurr; // 하드커런시
    public List<ItemData> purchasedItems; // 구매한 아이템들
    public List<CharacterData> purchasedCharacters; // 구매한 캐릭터들

    public GameData()
    {
        SoftCurr = 0;
        HardCurr = 0;
        purchasedItems = new List<ItemData>();
        purchasedCharacters = new List<CharacterData>();
    }
}
