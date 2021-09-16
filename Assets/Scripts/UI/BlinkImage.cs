using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkImage : MonoBehaviour
{
    public int blinkCount = 3;
    private int _blinkedCount = 0;
    private CanvasGroup cg;

    // Start is called before the first frame update
    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        if (blinkCount == -1 || _blinkedCount < blinkCount)
        {
            StartCoroutine("blink");
        }
    }

    IEnumerator blink()
    {
        float percent = 0;
        float speed = 1 / 0.7f ;
        while(_blinkedCount < blinkCount || blinkCount == -1)
        {
            percent = 0;
            while (percent < 1)
            {
                percent += speed * Time.deltaTime;
                cg.alpha = percent;
                yield return null;
            }
            yield return new WaitForSeconds(0.3f);
            percent = 0;
            while(percent < 1)
            {
                percent += speed * Time.deltaTime;
                cg.alpha = (1 - percent);
                yield return null;
            }
            _blinkedCount++;
        }
    }
}
