using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkImage : MonoBehaviour
{
    private float fadeSpeed = 0.005f;

    private int blinkCount = 3;
    private Image img;
    // Start is called before the first frame update
    void Start()
    {
        img = this.GetComponent<Image>();
        StartCoroutine("blink");
    }

    IEnumerator blink()
    {
        float percent = 0;
        float speed = 1 / 0.7f ;
        Color c = img.color;
        for (int i = 0; i < blinkCount; ++i)
        {
            while(percent < 1)
            {
                percent += speed * Time.deltaTime;
                c.a = percent;
                img.color = c;
                yield return null;
            }
            percent = 0;
            while(percent < 1)
            {
                percent += speed * Time.deltaTime;
                c.a = (1 - percent);
                img.color = c;
                yield return null;
            }
            
        }
    }
}
