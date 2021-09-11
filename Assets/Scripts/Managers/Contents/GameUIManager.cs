using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUIManager : UIManager
{
    public TextMeshProUGUI scoreText;

    public GameObject re;
    public CanvasGroup stageChangeView;

    public RectTransform healthBar;

    [Header("Item Count")]
    public TextMeshProUGUI[] itemCountTexts;

    [Header("Buff Buttons")]
    public BuffButton[] buffButtons;

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
        scoreText.text = ((int)Managers.Instance.GetScene<GameScene>().player.DistanceTraveled).ToString() + " m";
        if (score > 0 && score % 1000 == 0)
        {
            reachedThousandPoints();
        }

        // About earned item count
        for(int i = 0; i < Managers.Instance.GetScene<GameScene>().player.earnedItems.Length; i++)
        {
            itemCountTexts[i].text = Managers.Instance.GetScene<GameScene>().player.earnedItems[i].ToString();
        }

        // About Stage Change
        if(stageChangeView.alpha > 0)
        {
            stageChangeView.alpha -= Time.deltaTime;
            if (stageChangeView.alpha <= 0)
                stageChangeView.gameObject.SetActive(false);
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

    public void SettingBuff(List<int> buffs)
    {
        for (int i = 0; i < buffs.Count && i < buffButtons.Length; i++)
        {
            buffButtons[i].Setting(buffs[i]);
            buffButtons[i].gameObject.SetActive(true);
        }
    }

}
