using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameDataManager
{
    static string versionNum = "1_0_4";

    // 게임 데이터의 세이브/로드를 관리
    public static void SaveData(GameData gameData)
    {
        /**
        DataManager 이외의 오브젝트에서 이 함수를 직접 호출하지 않는 것이 좋습니다
        */
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/gamedata"+versionNum+".eveni";
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, gameData);
        stream.Close();
    }


    public static GameData LoadData()
    {
        /**
        DataManager 이외의 오브젝트에서 이 함수를 직접 호출하지 않는 것이 좋습니다
        */
        string path = Application.persistentDataPath + "/gamedata"+versionNum+".eveni";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData gameData = formatter.Deserialize(stream) as GameData;
            stream.Close();
            return gameData;
        } else
        {
            Debug.LogWarning("Cannot find any gamedata from path: " + path);
            return null;
        }
    }
}
