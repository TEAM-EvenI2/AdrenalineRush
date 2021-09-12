using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    [HideInInspector]
    public GameData gameData; // 인게임에서 플레이어의 구매기록, 재화정보 등에 접근할 경우 이 오브젝트를 사용합니다

    public static DataManager instance;
    public bool debugging = false; // 배포 시 반드시 false로 둘 것.

    void Awake()
    {
        // 중복 방지
        if (instance == null)
            instance = this;
        else{
            Destroy(gameObject);
        }

        if (gameData == null || debugging)
        {
            Debug.LogError("기존 게임데이터를 찾을 수 없습니다. 데이터를 새로 생성합니다.");
            debugging = false;
            gameData = new GameData();
            SaveGameData();
        }
        gameData = GameDataManager.LoadData();

        DontDestroyOnLoad(gameObject);
    }

    public void SaveGameData()
    {
        // 게임을 저장할 때 사용합니다
        GameDataManager.SaveData(gameData);
    }

    public void LoadGameData()
    {
        // 게임을 로드할 때 사용합니다
        gameData = GameDataManager.LoadData();
    }
}
