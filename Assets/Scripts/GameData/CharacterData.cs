using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    // 하나의 캐릭터 데이터는 하나의 캐릭터 타입을 나타냅니다

    public CharacterData(string id, string name, string desc, int softPrice, int hardPrice, bool hasPurchased)
    {
        characterId = id;
        characterName = name;
        CharacterDesc = desc;
        softCurrPrice = softPrice;
        hardCurrPrice = hardPrice;
        purchased = hasPurchased;
    }

    // 캐릭터 이름
    private string characterName;
    public string CharacterName
    {
        get { return characterName; }
        set { characterName = value; }
    }

    // 캐릭터 종류 식별자
    private string characterId;
    public string CharacterId
    {
        get { return characterId; }
        set { characterId = value; }
    }

    private bool purchased;
    public bool Purchased
    {   
        get { return purchased; }
        set { purchased = value; }
    }

    private string characterDesc;
    public string CharacterDesc
    {
        get {return characterDesc;}
        set {characterDesc = value;}
    }

    private int softCurrPrice;
    public int SoftCurrPrice
    {
        get {return softCurrPrice;}
        set {softCurrPrice = value;}
    }

    private int hardCurrPrice;
    public int HardCurrPrice
    {
        get {return hardCurrPrice;}
        set {hardCurrPrice = value;}
    }

    // TODO? 캐릭터별 스킨정보 저장 (시간되면)
}
