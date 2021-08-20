using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class LoadingWindow : MonoBehaviour
{
    private Image loadingImage;
    private TextMeshProUGUI loadingMessage;

    private int textHeight = 0;

    private void Awake()
    {
        loadingImage = transform.Find("LoadingImage").GetComponent<Image>();
        loadingMessage = transform.Find("LoadingMessage").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        loadingImage.transform.eulerAngles += Vector3.forward * 360 * Time.deltaTime;
    }

    public void Setting(string message, bool clear)
    {
        if (clear)
        {
            loadingMessage.text = "";
            textHeight = 0;
        }

        if(loadingMessage.text.Length != 0)
            loadingMessage.text += "\n";

        loadingMessage.text += message;
        textHeight++;
    }
}
