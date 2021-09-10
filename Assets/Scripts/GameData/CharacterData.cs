using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    // 하나의 캐릭터 데이터는 하나의 캐릭터 타입을 나타냅니다

    // 캐릭터 종류 식별자
    private string characterId;
    public string CharacterId
    {
        get { return characterId; }
        set { characterId = value; }
    }

    // TODO? 캐릭터별 스킨정보 저장 (시간되면)
}
