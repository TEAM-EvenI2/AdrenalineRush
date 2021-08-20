using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUIManager : UIManager
{
    public TextMeshProUGUI scoreText;

    public GameObject re;
    public void ActiveRe()
    {
        re.SetActive(true);
    }

    private void Update()
    {
        scoreText.text = Managers.Instance.GetScene<GameScene>().GetScore().ToString();
    }
}
