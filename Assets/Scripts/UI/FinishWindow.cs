using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FinishWindow : MonoBehaviour
{
    public class AnimQueueContent
    {
        public int addPointCount;
        public int multiply;
        public Color color;
    }

    public TextMeshProUGUI mainScoreText;
    public TextMeshProUGUI additiionalScoreText;
    public TextMeshProUGUI multiplyScoreText;

    public GameObject lastObject;

    private int _mainScore;

    private int _addPointCount;
    private int _multiply;
    private bool finished = true;
    private bool anim = false;


    private Queue<AnimQueueContent> animQueueContent = new Queue<AnimQueueContent>();

    private Animator animator;

    private float passedTime;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!finished)
        {
            passedTime += Time.deltaTime;
            if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
            {
                if (passedTime > 0.5f)
                {
                    Finish();
                }
            }
            else {
                if (!anim) {
                    if (animQueueContent.Count > 0)
                    {
                        AnimQueueContent aqc = animQueueContent.Dequeue();
                        ActiveAddtionalText(aqc.addPointCount, aqc.multiply, aqc.color);
                        animator.SetBool("Additional", anim);
                    }
                }

            }
        }
    }
    public void Initialized()
    {
        _mainScore = (int)Managers.Instance.GetScene<GameScene>().player.DistanceTraveled;
        mainScoreText.text = _mainScore.ToString();
        anim = false;
        finished = false;
        passedTime = 0;
    }

    public void Finish()
    {
        finished = true;
        anim = false;
        animator.SetBool("Additional", anim);

        mainScoreText.text = Managers.Instance.GetScene<GameScene>().GetScore().ToString();
        lastObject.SetActive(true);

        animQueueContent.Clear();

    }

    public void AddAdditionalAnimQueue(int addPointCount, int multiply, Color textColor)
    {
        animQueueContent.Enqueue(new AnimQueueContent() { addPointCount = addPointCount, multiply = multiply, color = textColor });
    }

    public void ActiveAddtionalText(int addPointCount, int multiply, Color textColor)
    {
        anim = true;
        _addPointCount = addPointCount;
        _multiply = multiply;
        additiionalScoreText.text = (_addPointCount).ToString();
        multiplyScoreText.text = "x" + _multiply.ToString();

        additiionalScoreText.color = textColor;
        multiplyScoreText.color = textColor;
    }

    public void FinishAdditionalAnim()
    {
        anim = false;

        animator.SetBool("Additional", anim);
                if (animQueueContent.Count == 0)
                {
                    Finish();
                }
    }

    public void SettingAdditionalText()
    {
        additiionalScoreText.text = (_addPointCount * _multiply).ToString();
    }

    public void SettingMainScoreAdd()
    {
        _mainScore += _addPointCount * _multiply;
        mainScoreText.text = _mainScore.ToString();
    }
}
