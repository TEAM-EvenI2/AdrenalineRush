using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ConfigDataManager
{
    public PlayerInfo playerInfo;
    //public List<Dictionary<string, object>> buffMagnetInfo;
    //public List<Dictionary<string, object>> buffSpeedInfo;
    //public List<Dictionary<string, object>> buffSizeInfo;

    public List<List<BuffStruct>> buffInfos = new List<List<BuffStruct>>();
    public List<BuffTextStruct> buffTextInfos = new List<BuffTextStruct>();


    public void Init()
    {
        List<Dictionary<string, object>> playercsv = CSVReader.Read("Config/PlayerInfo");

        playerInfo = new PlayerInfo();
        playerInfo.velocity = float.Parse(playercsv[0]["PlayerSpeed"].ToString());
        playerInfo.rotateVelocity = float.Parse(playercsv[0]["PlayerRotateSpeed"].ToString());

        List<Dictionary<string, object>> buffMagnetcsv = CSVReader.Read("Config/BuffMagnetInfo");
        AddBuffInfo(buffMagnetcsv, BuffType.Magnet);

        List<Dictionary<string, object>> buffSpeedcsv = CSVReader.Read("Config/BuffSpeedInfo");
        AddBuffInfo(buffSpeedcsv, BuffType.Speed);

        List<Dictionary<string, object>> buffSizecsv = CSVReader.Read("Config/BuffSizeInfo");
        AddBuffInfo(buffSizecsv, BuffType.Size);

        List<Dictionary<string, object>> buffcsv = CSVReader.Read("Config/BuffInfo");
        AddBuffTextInfo(buffcsv);
    }

    private void AddBuffTextInfo(List<Dictionary<string, object>> buffcsv)
    {
        for (int i = 0; i < buffcsv.Count; i++)
        {
            buffTextInfos.Add(new BuffTextStruct(buffcsv[i]["Text"].ToString(), buffcsv[i]["Name"].ToString()));
        }
    }

    private void AddBuffInfo(List<Dictionary<string, object>> buffcsv, BuffType buffType)
    {

        for (int i = 0; i < buffcsv.Count; i++)
        {
            int id = int.Parse(buffcsv[i]["Id"].ToString());
            float time = float.Parse(buffcsv[i]["Time"].ToString());
            float cooltime = float.Parse(buffcsv[i]["CoolTime"].ToString());
            while (buffInfos.Count <= id)
                buffInfos.Add(new List<BuffStruct>());

            switch (buffType)
            {
                case BuffType.Magnet:
                    {
                        float range = float.Parse(buffcsv[i]["Range"].ToString());
                        buffInfos[id].Add(new MagnetBuffStruct(id, time, cooltime, range));
                        break;
                    }
                case BuffType.Size:
                    {
                        float sizeFactor = float.Parse(buffcsv[i]["SizeFactor"].ToString());
                        buffInfos[id].Add(new SizeBuffStruct(id, time, cooltime, sizeFactor));
                        break;
                    }
                case BuffType.Speed:
                    {
                        float speed = float.Parse(buffcsv[i]["Speed"].ToString());
                        int invincibility = int.Parse(buffcsv[i]["Invincibility"].ToString());
                        buffInfos[id].Add(new SpeedBuffStruct(id, time, cooltime, speed, invincibility == 1));
                        break;
                    }
            }
        }

    }
}
