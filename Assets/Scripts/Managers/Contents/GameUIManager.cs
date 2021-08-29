using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUIManager : UIManager
{
    public TextMeshProUGUI scoreText;

    public GameObject re;
    public CanvasGroup cg;

    public float a = 0;
    public void ActiveRe()
    {
        re.SetActive(true);
    }

    private void Update()
    {
        scoreText.text = Managers.Instance.GetScene<GameScene>().GetScore().ToString();

        if(a > 0)
        {
            a -= Time.deltaTime;
            cg.alpha = a;
        }
    }


}
