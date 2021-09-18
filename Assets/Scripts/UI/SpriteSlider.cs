using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSlider : MonoBehaviour
{
    public float remainTime = 3f;
    public float slideTime = 1f;
    public Sprite[] sprites;
    private int spriteIndex;

    private Image[] imageA;
    private CanvasGroup[] imageCg;
    private int curIndex;

    private void Awake()
    {
        curIndex = 1;
        imageA = new Image[2];
        imageCg = new CanvasGroup[2];
        for(int i = 0; i < 2; i++)
        {
            imageA[i] = transform.GetChild(i).GetComponent<Image>();
            imageCg[i] = transform.GetChild(i).GetComponent<CanvasGroup>();
        }
    }

    private void OnEnable()
    {
        if(sprites != null && sprites.Length > 0)
        {
            StartCoroutine("ImageSlide");
        }
    }

    IEnumerator ImageSlide()
    {
        float percent = 0;
        float speed = 1 / slideTime;
        imageA[curIndex].sprite = sprites[spriteIndex];
        imageCg[(curIndex + 1) % 2].alpha = 0;
        while (percent < 1)
        {
            percent += speed * Time.deltaTime;
            imageCg[curIndex].alpha = percent;
            yield return null;
        }


        while (true)
        {
            yield return new WaitForSeconds(remainTime);

            percent = 0;
            int _curIndex = (curIndex + 1) % 2;
            spriteIndex = (spriteIndex + 1) % sprites.Length;
            imageA[_curIndex].sprite = sprites[spriteIndex];
            imageCg[_curIndex].alpha = 1;

            while (percent < 1)
            {
                percent += speed * Time.deltaTime;
                imageCg[curIndex].alpha = (1 - percent);
                yield return null;
            }
            curIndex = _curIndex;
        }
    }

}
