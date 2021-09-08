using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerBuffManager : MonoBehaviour
{
    private List<BuffStruct> buffList = new List<BuffStruct>();

    public Player player;
    public LayerMask itemLayer;

    private Dictionary<BuffType, System.Action<BuffStruct>> handleBuffDict = new Dictionary<BuffType, System.Action<BuffStruct>>();
    private Dictionary<BuffType, System.Action<BuffStruct>> handleEndBuffDict = new Dictionary<BuffType, System.Action<BuffStruct>>();

    public float sizeChangeTime = 0.4f;
    public float speedChangeTime = 1f;

    private void Awake()
    {
        handleBuffDict.Add(BuffType.Magnet, MagnetBuff);
        handleBuffDict.Add(BuffType.Size, SizeBuff);
        handleBuffDict.Add(BuffType.Speed, SpeedBuff);

        handleEndBuffDict.Add(BuffType.Magnet, EndMagnetBuff);
        handleEndBuffDict.Add(BuffType.Size, EndSizeBuff);
        handleEndBuffDict.Add(BuffType.Speed, EndSpeedBuff);
    }

    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        if (Managers.Instance.GetScene<GameScene>().isPause)
            return;

        for (int i = buffList.Count - 1; i >= 0 ; i--)
        {
            if(!HandleBuff(buffList[i]))
            {
                buffList.RemoveAt(i);
            }
        }
    }

    public void AddSizeBuff(int id, float time, float sizeFactor)
    {
        for (int i = 0; i < buffList.Count; i++)
        {
            if (buffList[i].id == id)
            {
                SizeBuffStruct sbs = buffList[i] as SizeBuffStruct;
                sbs.time = time;
                sbs.sizeFactor = sizeFactor;
                return;
            }
        }
        Transform avatar = player.transform.GetChild(0).GetChild(0);
        buffList.Add(new SizeBuffStruct(id, time, sizeFactor, avatar.localScale));
    }

    public void AddSpeedBuff(int id, float time, float distance)
    {
        for (int i = 0; i < buffList.Count; i++)
        {
            if (buffList[i].id == id)
            {
                SpeedBuffStruct sbs = buffList[i] as SpeedBuffStruct;
                sbs.time = time;
                sbs.distance = distance;
                return;
            }
        }
        buffList.Add(new SpeedBuffStruct(id, time, distance, Managers.Instance.Config.playerInfo.velocity));
    }

    public void AddMagnetBuff(int id, float time, float range, float power)
    {

        for (int i = 0; i <buffList.Count; i++)
        {
            if (buffList[i].id == id)
            {
                MagnetBuffStruct mbs = buffList[i] as MagnetBuffStruct;
                mbs.time = time;
                mbs.range = range;
                mbs.power = power;
                return;
            }
        }
        buffList.Add(new MagnetBuffStruct(id, time, range, power));
    }

    private bool HandleBuff(BuffStruct bs)
    {
        System.Action<BuffStruct> action = null;
        if (handleBuffDict.TryGetValue(bs.type, out action))
        {
            action.Invoke(bs);
        }

        bs.time -= Time.deltaTime;
        if (bs.time <= 0)
        {
            System.Action<BuffStruct> endAction = null;
            if(handleEndBuffDict.TryGetValue(bs.type, out endAction))
            {
                endAction.Invoke(bs);
            }
            return false;
        }
        return true;
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

        if (sbs.originTime - sbs.time <= sizeChangeTime)
        {
            avatar.localScale = Vector3.Lerp(sbs.originalSize, sbs.originalSize * sbs.sizeFactor, Utils.Easing.Exponential.Out((sbs.originTime - sbs.time)/ sizeChangeTime));
        }
        else if( sbs.time <= sizeChangeTime)
        {
            avatar.localScale = Vector3.Lerp(sbs.originalSize, sbs.originalSize * sbs.sizeFactor, Utils.Easing.Exponential.Out((sbs.time) / sizeChangeTime));
        }
        else
        {
            avatar.localScale = sbs.originalSize * sbs.sizeFactor;
        }

    }

    private void SpeedBuff(BuffStruct bs)
    {
        SpeedBuffStruct sbs = bs as SpeedBuffStruct;
        player.invincible = true;

        float vel = sbs.distance / sbs.originTime;
        float _vel;

        if (sbs.originTime - sbs.time <= speedChangeTime)
        {
            _vel = Mathf.Lerp(sbs.originSpeed, vel, Utils.Easing.Exponential.Out((sbs.originTime - sbs.time) / speedChangeTime));
        }
        else if (sbs.time <= speedChangeTime)
        {
            _vel = Mathf.Lerp(sbs.originSpeed, vel , Utils.Easing.Exponential.In((sbs.time) / speedChangeTime));
        }
        else
        {
            _vel = vel;
        }
        player.maxVelocity = player.curVelocity = _vel;
    }

    private void EndMagnetBuff(BuffStruct bs)
    {

    }

    private void EndSizeBuff(BuffStruct bs)
    {
        SizeBuffStruct sbs = bs as SizeBuffStruct;
        Transform avatar = player.transform.GetChild(0).GetChild(0);

        avatar.localScale = sbs.originalSize;
    }

    private void EndSpeedBuff(BuffStruct bs)
    {
        SpeedBuffStruct sbs = bs as SpeedBuffStruct;

        player.invincible = false;
        player.maxVelocity = player.curVelocity = sbs.originSpeed;
    }
}
