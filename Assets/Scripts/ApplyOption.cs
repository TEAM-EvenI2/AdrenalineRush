using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplyOption : MonoBehaviour
{
    public GameObject VolumeSlider;
    public GameObject SilencedUI;
    private DataManager dataManager;
    private float prevVolume;

    void Start()
    {
        dataManager = FindObjectOfType<DataManager>();
        prevVolume = dataManager.gameData.masterVolume;
        VolumeSlider.GetComponent<Slider>().value = dataManager.gameData.masterVolume;
    }

    // Update is called once per frame
    void Update()
    {
        AudioListener.volume = dataManager.gameData.masterVolume;
        if (prevVolume != VolumeSlider.GetComponent<Slider>().value)
        {
            AudioListener.volume = VolumeSlider.GetComponent<Slider>().value;
            dataManager.gameData.masterVolume = AudioListener.volume;
            dataManager.SaveGameData();
            if (AudioListener.volume == 0) {
                SilencedUI.gameObject.SetActive(true);
            } else {
                SilencedUI.gameObject.SetActive(false);
            }
        }
        prevVolume = AudioListener.volume;
    }
}
