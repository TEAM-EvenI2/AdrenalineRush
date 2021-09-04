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

    IEnumerator blink() {
        for (int i = 0; i < blinkCount; ++i)
        {
            for (float f = 0f; f < 1f; f += fadeSpeed) {
                Color c = img.color;
                c.a = f;
                img.color = c;
                yield return null;
            }
            for (float f = 1f; f > 0; f -= fadeSpeed) {
            Color c = img.color;
            c.a = f;
            img.color = c;
            yield return null;
            }
        }
    }
}
