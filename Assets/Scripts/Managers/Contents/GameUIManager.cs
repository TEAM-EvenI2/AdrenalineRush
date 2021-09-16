using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Define;

public class GameUIManager : UIManager
{

    public FinishWindow finishWindow;
    public GameObject pauseWindow;
    public CanvasGroup stageChangeView;
    public Color hitColor;

    public RectTransform healthBar;

    [Header("Main Game UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI[] itemCountTexts;
    public BuffButton[] buffButtons;

    public Color[] earnedItemTextColors;

    private void Start()
    {
        stageChangeView.alpha = 1;
    }

    public void OpenPauseWindow()
    {
        pauseWindow.SetActive(!pauseWindow.gameObject.activeSelf);
        Managers.Instance.GetScene<GameScene>().Pause();
    }

    public void OpenFinishWindow()
    {
        finishWindow.gameObject.SetActive(true);

        finishWindow.Initialized();
        finishWindow.AddAdditionalAnimQueue(Managers.Instance.GetScene<GameScene>().player.earnedItems[0], 5, earnedItemTextColors[0]);
        finishWindow.AddAdditionalAnimQueue(Managers.Instance.GetScene<GameScene>().player.earnedItems[1], 10, earnedItemTextColors[1]);

    }

    private void reachedThousandPoints()
    {
        // TODO : 부스트?
        Debug.Log("1000");
    }

    private void Update()
    {
        SettingScoreText();
        // About earned item count
        SettingEarnedItemCount();

        // About Stage Change
        if (stageChangeView.alpha > 0)
        {
            stageChangeView.alpha -= Time.deltaTime;
        }

        float percent = Managers.Instance.GetScene<GameScene>().player.health / 100;
        healthBar.sizeDelta = new Vector2(800 * percent, healthBar.sizeDelta.y);

        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                OpenPauseWindow();
            }
        }
    }

    public void ChangeScreen()
    {
        stageChangeView.alpha = 1;
        stageChangeView.GetComponent<Image>().color = Color.white;
    }
    public void HitScreen()
    {
        stageChangeView.alpha = 0.4f;
        stageChangeView.GetComponent<Image>().color = hitColor;
    }


    private void SettingScoreText()
    {
        float score = Managers.Instance.GetScene<GameScene>().GetScore();
        scoreText.text = ((int)Managers.Instance.GetScene<GameScene>().player.DistanceTraveled).ToString() + " m";
        if (score > 0 && score % 1000 == 0)
        {
            reachedThousandPoints();
        }

        int totalItem = 0;
        for (int i = 0; i < Managers.Instance.GetScene<GameScene>().player.earnedItems.Length; i++)
        {
            totalItem += Managers.Instance.GetScene<GameScene>().player.earnedItems[i];
        }

        RectTransform stagePercent = scoreText.transform.GetChild(0).GetComponent<RectTransform>();
        MapSystem mapSystem = Managers.Instance.GetScene<GameScene>().player.mapSystem;

        float percent = 0;

        StageInformation cur = mapSystem.GetCurrentStage();
        StageInformation nxt = mapSystem.GetNextStage();

        if(cur != null && nxt != null)
        {
            percent = (totalItem - cur.enterPoint) / (float)(nxt.enterPoint - cur.enterPoint);
        }

        float width = scoreText.rectTransform.sizeDelta.x * (percent - 1);

        stagePercent.sizeDelta = new Vector2(width, 0);
        stagePercent.anchoredPosition = new Vector2(width / 2f, 0);

    }

    private void SettingEarnedItemCount()
    {
        for (int i = 0; i < Managers.Instance.GetScene<GameScene>().player.earnedItems.Length; i++)
        {
            itemCountTexts[i].text = Managers.Instance.GetScene<GameScene>().player.earnedItems[i].ToString();
            itemCountTexts[i].rectTransform.anchoredPosition = new Vector2(
                scoreText.rectTransform.sizeDelta.x / 2 * (i == 0? -1: 1),
                itemCountTexts[i].rectTransform.anchoredPosition.y);
            itemCountTexts[i].rectTransform.localEulerAngles = Vector3.forward * (i == 0 ? -1 : 1) * 25f;
        }
    }

    public void StartBuffCoolTime(int id)
    {
        for (int i = 0; i < buffButtons.Length; i++)
        {
            if(buffButtons[i].id == id)
            {
                buffButtons[i].StartDecreaseCool();
                break;
            }
        }
    }
    public void SetRemainTime(int id, int time)
    {
        for (int i = 0; i < buffButtons.Length; i++)
        {
            if(buffButtons[i].id == id)
            {
                buffButtons[i].SetTime(time);
                break;
            }
        }
    }

    public void SettingBuff(List<int> buffs, SerVector2[] buttonPos)
    {
        for (int i = 0; i < buffs.Count && i < buffButtons.Length; i++)
        {
            buffButtons[i].Setting(buffs[i], Utils.Ser2Vector(buttonPos[i]));
            buffButtons[i].gameObject.SetActive(true);
        }
    }

}
