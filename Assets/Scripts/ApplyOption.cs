using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplyOption : MonoBehaviour
{
    public GameObject VolumeSlider;
    public GameObject SilencedUI;

    void Awake()
    {
        VolumeSlider.GetComponent<Slider>().value = AudioListener.volume;
    }

    // Update is called once per frame
    void Update()
    {
        AudioListener.volume = VolumeSlider.GetComponent<Slider>().value;
        if (AudioListener.volume == 0) {
            SilencedUI.gameObject.SetActive(true);
        } else {
            SilencedUI.gameObject.SetActive(false);
        }
    }
}
