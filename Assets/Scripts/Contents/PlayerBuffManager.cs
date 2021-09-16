using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerBuffManager : MonoBehaviour
{
    private List<BuffStruct> buffList = new List<BuffStruct>();

    public Player player;
    public Transform playerScalable;
    public LayerMask itemLayer;

    private Dictionary<BuffType, System.Action<BuffStruct>> handleBuffDict = new Dictionary<BuffType, System.Action<BuffStruct>>();
    private Dictionary<BuffType, System.Action<BuffStruct>> handleEndBuffDict = new Dictionary<BuffType, System.Action<BuffStruct>>();

    public float sizeChangeTime = 0.4f;
    public float speedChangeTime = 1f;

    [Header("Effects")]
    public MagnetBuffEffect magnetEffect;
    public RushBuffEffect rushEffect;


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

        if (rushEffect.gameObject.activeSelf)
        {
            rushEffect.transform.position = playerScalable.position;
        }
    }

    public void AddBuff(BuffStruct bs)
    {
        switch (bs.type)
        {
            case BuffType.Magnet:
                AddMagnetBuff(bs.id, bs.time, bs.coolTime, ((MagnetBuffStruct)bs).range);
                break;
            case BuffType.Size:
                AddSizeBuff(bs.id, bs.time, bs.coolTime, ((SizeBuffStruct)bs).sizeFactor);
                break;
            case BuffType.Speed:
                AddSpeedBuff(bs.id, bs.time, bs.coolTime, ((SpeedBuffStruct)bs).speed, ((SpeedBuffStruct)bs).invincibility);
                break;
        }
    }

    public void AddSizeBuff(int id, float time, float coolTime, float sizeFactor)
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
        buffList.Add(new SizeBuffStruct(id, time, coolTime, sizeFactor, playerScalable.localScale, playerScalable.parent.GetComponent<BoxCollider>().size));
    }

    public void AddSpeedBuff(int id, float time, float coolTime, float speed, bool invincibility)
    {
        if(invincibility)
        {
            rushEffect.Setting(speed);
            rushEffect.gameObject.SetActive(true);
        }

        for (int i = 0; i < buffList.Count; i++)
        {
            if (buffList[i].id == id)
            {
                SpeedBuffStruct sbs = buffList[i] as SpeedBuffStruct;
                sbs.time = time;
                sbs.speed = speed;
                return;
            }
        }
        buffList.Add(new SpeedBuffStruct(id, time, coolTime, speed, Managers.Instance.Config.playerInfo.velocity, invincibility));
    }

    public void AddMagnetBuff(int id, float time, float coolTime, float range)
    {

        magnetEffect.Setting(range);
        magnetEffect.gameObject.SetActive(true);

        for (int i = 0; i <buffList.Count; i++)
        {
            if (buffList[i].id == id)
            {
                MagnetBuffStruct mbs = buffList[i] as MagnetBuffStruct;
                mbs.time = time;
                mbs.range = range;
                return;
            }
        }
        buffList.Add(new MagnetBuffStruct(id, time, coolTime, range));
    }

    private bool HandleBuff(BuffStruct bs)
    {
        System.Action<BuffStruct> action = null;
        if (handleBuffDict.TryGetValue(bs.type, out action))
        {
            action.Invoke(bs);
        }

        bs.time -= Time.deltaTime;
        Managers.Instance.GetUIManager<GameUIManager>().SetRemainTime(bs.id, (int)bs.time);
        if (bs.time <= 0)
        {
            System.Action<BuffStruct> endAction = null;
            if(handleEndBuffDict.TryGetValue(bs.type, out endAction))
            {
                endAction.Invoke(bs);
                Managers.Instance.GetUIManager<GameUIManager>().StartBuffCoolTime(bs.id);
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
        
        
        magnetEffect.transform.position = avatar.position + avatar.right * mbs.range / 2;

        float power = player.curVelocity * 1.1f;

        for (int i = 0; i < items.Length; i++)
        {
            items[i].GetComponentInParent<ScoreItem>()?.transform.SetParent(null);
            Vector3 dir = (avatar.position - items[i].transform.position).normalized;
            

            if(Vector3.Distance(avatar.position, items[i].transform.position) < power * Time.deltaTime)
            {
                items[i].transform.position = avatar.position;
            }
            else
                items[i].transform.position += dir * power * Time.deltaTime;
        }
    }

    private void SizeBuff(BuffStruct bs)
    {
        SizeBuffStruct sbs = bs as SizeBuffStruct;

        playerScalable.parent.GetComponent<BoxCollider>().size = new Vector3(sbs.collisionOriginalSize.x * sbs.sizeFactor, sbs.collisionOriginalSize.y, sbs.collisionOriginalSize.z * sbs.sizeFactor);

        if (sbs.originTime - sbs.time <= sizeChangeTime)
        {
            playerScalable.localScale = Vector3.Lerp(sbs.originalSize, sbs.originalSize * sbs.sizeFactor, Utils.Easing.Exponential.Out((sbs.originTime - sbs.time)/ sizeChangeTime));
        }
        else if( sbs.time <= sizeChangeTime)
        {
            playerScalable.localScale = Vector3.Lerp(sbs.originalSize, sbs.originalSize * sbs.sizeFactor, Utils.Easing.Exponential.Out((sbs.time) / sizeChangeTime));
        }
        else
        {
            playerScalable.localScale = sbs.originalSize * sbs.sizeFactor;
        }

    }

    private void SpeedBuff(BuffStruct bs)
    {
        SpeedBuffStruct sbs = bs as SpeedBuffStruct;
        player.invincible = sbs.invincibility;

        float distance = sbs.speed * sbs.originTime;

        float vel = distance / (sbs.originTime - speedChangeTime);
        float _vel;

        if (sbs.originTime - sbs.time <= speedChangeTime)
        {
            _vel = Mathf.Lerp(sbs.originSpeed, vel, Utils.Easing.Exponential.Out((sbs.originTime - sbs.time) / speedChangeTime));
        }
        else if (sbs.time <= speedChangeTime)
        {
            _vel = Mathf.Lerp(sbs.originSpeed, vel , Utils.Easing.Exponential.Out((sbs.time) / speedChangeTime));
        }
        else
        {
            _vel = vel;
        }
        player.maxVelocity = player.curVelocity = _vel;
    }

    private void EndMagnetBuff(BuffStruct bs)
    {
        magnetEffect.Stop();
        //magnetEffect.gameObject.SetActive(false);

        player.currentPipe.DestoryChildNoParent();
    }

    private void EndSizeBuff(BuffStruct bs)
    {
        SizeBuffStruct sbs = bs as SizeBuffStruct;
        Transform avatar = player.transform.GetChild(0).GetChild(0);

        avatar.localScale = sbs.originalSize;
        playerScalable.parent.GetComponent<BoxCollider>().size =sbs.collisionOriginalSize;
    }

    private void EndSpeedBuff(BuffStruct bs)
    {
        SpeedBuffStruct sbs = bs as SpeedBuffStruct;

        player.maxVelocity = player.curVelocity = sbs.originSpeed;

        rushEffect.Stop();

        Invoke("RemoveInvincible", 1.5f);
    }

    private void RemoveInvincible()
    {

        player.invincible = false;
    }
}
