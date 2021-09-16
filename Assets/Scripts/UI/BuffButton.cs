using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Define;

public class BuffButton : MonoBehaviour
{
    public float remainCooltime;
    public int id;

    public Transform sizeTransform;
    public GameObject blackCool;

    public Transform testImageParent;

    private bool decreaseCool = false;


    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            ButtonInvoke();
        });
    }

    void Update()
    {
        if(remainCooltime > 0 )
        {
            if (decreaseCool)
            {
                remainCooltime -= Time.deltaTime;
                sizeTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.2f, remainCooltime / (GetBuffSturct().coolTime));
            }
        }
        else
        {
            if (blackCool.activeSelf)
            {
                sizeTransform.localScale = Vector3.one;
                blackCool.SetActive(false);
                decreaseCool = false;
            }
        }
    }

    public void ButtonInvoke()
    {
        if(remainCooltime <= 0)
        {
            Managers.Instance.GetScene<GameScene>().player.GetComponent<PlayerBuffManager>().AddBuff(GetBuffSturct());
            remainCooltime = GetBuffSturct().coolTime;
            decreaseCool = false;

            sizeTransform.localScale = Vector3.one;
            blackCool.SetActive(true);
        }
    }

    public void StartDecreaseCool()
    {
        decreaseCool = true;
    }

    private BuffStruct GetBuffSturct()
    {
        int upgrade = DataManager.instance.gameData.purchasedItems[id].Upgrade - 1;
        if (upgrade < 0)
            upgrade = 0;
        else if (upgrade >= Managers.Instance.Config.buffInfos[id].Count)
            upgrade = Managers.Instance.Config.buffInfos[id].Count - 1;

        return Managers.Instance.Config.buffInfos[id][upgrade];
    }

    public void Setting(int buffId, Vector2 pos)
    {
        id = buffId;

        for(int i = 0; i < testImageParent.childCount; i++)
        {
            testImageParent.GetChild(i).gameObject.SetActive(false);
        }
        testImageParent.GetChild(buffId).gameObject.SetActive(true);

        GetComponent<RectTransform>().anchoredPosition = pos;

    }
}
