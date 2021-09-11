using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Define;

public class BuffButton : MonoBehaviour
{
    public float remainCooltime;
    private int id;

    public Transform sizeTransform;
    public GameObject blackCool;

    public TextMeshProUGUI buffText;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            ButtonInvoke();
        });
    }

    void Update()
    {
        if(remainCooltime > 0)
        {
            remainCooltime -= Time.deltaTime;
            sizeTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.2f, remainCooltime / (GetBuffSturct().coolTime));
        }
        else
        {
            if (blackCool.activeSelf)
            {
                sizeTransform.localScale = Vector3.one;
                blackCool.SetActive(false);
            }
        }
    }

    public void ButtonInvoke()
    {
        if(remainCooltime <= 0)
        {
            Managers.Instance.GetScene<GameScene>().player.GetComponent<PlayerBuffManager>().AddBuff(GetBuffSturct());
            remainCooltime = GetBuffSturct().coolTime;

            blackCool.SetActive(true);
        }
    }

    private BuffStruct GetBuffSturct()
    {
        return Managers.Instance.Config.buffInfos[id][0];
    }

    public void Setting(int buffId)
    {
        id = buffId;

        // TODO
        // image 
        buffText.text = Managers.Instance.Config.buffTextInfos[id].name;
    }
}
