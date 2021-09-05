using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUIManager : UIManager
{
    public TextMeshProUGUI scoreText;

    public GameObject re;
    public CanvasGroup cg;

    public RectTransform healthBar;

    public void ActiveRe()
    {
        re.SetActive(true);
    }

    private void Update()
    {
        scoreText.text = Managers.Instance.GetScene<GameScene>().GetScore().ToString();

        if(cg.alpha > 0)
        {
            cg.alpha -= Time.deltaTime;
            if (cg.alpha <= 0)
                cg.gameObject.SetActive(false);
        }

        float percent = Managers.Instance.GetScene<GameScene>().player.health / 100;
        healthBar.sizeDelta = new Vector2(800 * percent, healthBar.sizeDelta.y);
    }

    public void DoMagnet()
    {
        Managers.Instance.GetScene<GameScene>().player.GetComponent<PlayerBuffManager>().AddMagnetBuff(20,.7f, 7);
    }

    public void DoSize()
    {
        Managers.Instance.GetScene<GameScene>().player.GetComponent<PlayerBuffManager>().AddSizeBuff(5, .5f);
    }
}
