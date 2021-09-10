using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplyOption : MonoBehaviour
{
    public GameObject VolumeSlider;

    void Awake()
    {
        VolumeSlider.GetComponent<Slider>().value = AudioListener.volume;
    }

    // Update is called once per frame
    void Update()
    {
        AudioListener.volume = VolumeSlider.GetComponent<Slider>().value;
    }
}
