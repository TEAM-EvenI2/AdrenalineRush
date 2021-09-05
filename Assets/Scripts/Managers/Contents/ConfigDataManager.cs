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


    public void Init()
    {
        List<Dictionary<string, object>> playercsv = CSVReader.Read("Config/PlayerInfo");

        playerInfo = new PlayerInfo();
        playerInfo.velocity = float.Parse(playercsv[0]["PlayerSpeed"].ToString());
        playerInfo.rotateVelocity = float.Parse(playercsv[0]["PlayerRotateSpeed"].ToString());

        List<Dictionary<string, object>> buffMagnetcsv = CSVReader.Read("Config/BuffMagnetInfo");
        List<Dictionary<string, object>> buffSpeedcsv = CSVReader.Read("Config/BuffSpeedInfo");
        List<Dictionary<string, object>> buffSizecsv = CSVReader.Read("Config/BuffSizeInfo");
    }
}
