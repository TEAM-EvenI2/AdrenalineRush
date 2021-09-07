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
        re.SetActive(!re.gameObject.activeSelf);
        Managers.Instance.GetScene<GameScene>().Pause();
    }

    private void reachedThousandPoints()
    {
        // TODO : 부스트?
        Debug.Log("1000");
    }

    private void Update()
    {
        float score = Managers.Instance.GetScene<GameScene>().GetScore();
        scoreText.text = score.ToString();
        if (score > 0 && score % 1000 == 0)
        {
            reachedThousandPoints();
        }

        if(cg.alpha > 0)
        {
            cg.alpha -= Time.deltaTime;
            if (cg.alpha <= 0)
                cg.gameObject.SetActive(false);
        }

        float percent = Managers.Instance.GetScene<GameScene>().player.health / 100;
        healthBar.sizeDelta = new Vector2(800 * percent, healthBar.sizeDelta.y);

        if (Application.platform == RuntimePlatform.Android)
        {

            if (Input.GetKey(KeyCode.Escape))
            {
                ActiveRe();
            }
        }
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
