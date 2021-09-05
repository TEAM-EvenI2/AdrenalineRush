using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerBuffManager : MonoBehaviour
{
    private List<BuffStruct> buffList = new List<BuffStruct>();

    public Player player;
    public LayerMask itemLayer;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        
        for(int i = buffList.Count - 1; i >= 0 ; i--)
        {
            HandleBuff(buffList[i]);

            buffList[i].time -= Time.deltaTime;

            if(buffList[i].time <= 0)
            {
                buffList.RemoveAt(i);
            }
        }
    }

    public void AddSizeBuff(float time, float sizeFactor)
    {
        for (int i = 0; i < buffList.Count; i++)
        {
            if (buffList[i].type == BuffType.Size)
            {
                SizeBuffStruct sbs = buffList[i] as SizeBuffStruct;
                sbs.time = time;
                sbs.sizeFactor = sizeFactor;
                return;
            }
        }
        Transform avatar = player.transform.GetChild(0).GetChild(0);
        buffList.Add(new SizeBuffStruct(time, sizeFactor, avatar.localScale));
    }
    public void AddSpeedBuff(float time, float distance)
    {
        for (int i = 0; i < buffList.Count; i++)
        {
            if (buffList[i].type == BuffType.Speed)
            {
                SpeedBuffStruct sbs = buffList[i] as SpeedBuffStruct;
                sbs.time = time;
                sbs.distance = distance;
                return;
            }
        }
        buffList.Add(new SpeedBuffStruct(time, distance));
    }
    public void AddMagnetBuff(float time, float range, float power)
    {

        for (int i = 0; i <buffList.Count; i++){
            if(buffList[i].type == BuffType.Magnet)
            {
                MagnetBuffStruct mbs = buffList[i] as MagnetBuffStruct;
                mbs.time = time;
                mbs.range = range;
                mbs.power = power;
                return;
            }
        }
        buffList.Add(new MagnetBuffStruct(time, range, power));
    }

    private void HandleBuff(BuffStruct bs)
    {
        switch (bs.type)
        {
            case BuffType.Magnet:
                MagnetBuff(bs);
                break;
            case BuffType.Size:
                SizeBuff(bs);
                break;
            case BuffType.Speed:
                SpeedBuff(bs);
                break;
        }
    }

    private void MagnetBuff(BuffStruct bs)
    {
        MagnetBuffStruct mbs = bs as MagnetBuffStruct;
        Transform avatar = player.transform.GetChild(0).GetChild(0);
        Collider[] items = Physics.OverlapSphere(avatar.position + avatar.right * mbs.range / 2, mbs.range, itemLayer);
        print(items.Length);
        

        for (int i = 0; i < items.Length; i++)
        {
            Vector3 dir = (avatar.position - items[i].transform.position).normalized;
            

            if(Vector3.Distance(avatar.position, items[i].transform.position) < mbs.power * Time.deltaTime)
            {
                items[i].transform.position = avatar.position;
            }
            else
                items[i].transform.position += dir * mbs.power * Time.deltaTime;
        }
    }

    private void SizeBuff(BuffStruct bs)
    {
        SizeBuffStruct sbs = bs as SizeBuffStruct;
        Transform avatar = player.transform.GetChild(0).GetChild(0);


        if (sbs.time - Time.deltaTime > 0)
            avatar.localScale = sbs.originalSize * sbs.sizeFactor;
        else
            avatar.localScale = sbs.originalSize;

    }
    private void SpeedBuff(BuffStruct bs)
    {
        SpeedBuffStruct sbs = bs as SpeedBuffStruct;
    }
}
