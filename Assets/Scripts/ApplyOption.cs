using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HeathenEngineering.UX.Samples;

public class ApplyOption : MonoBehaviour
{
    public GameObject VolumeSlider;
    public GameObject SilencedUI;
    public GameObject VibrateOffUI;
    public GameObject HasVibrationUI;
    private DataManager dataManager;
    private float prevVolume;
    private bool prevVibration;

    void Start()
    {
        dataManager = FindObjectOfType<DataManager>();
        prevVolume = dataManager.gameData.masterVolume;
        VolumeSlider.GetComponent<Slider>().value = dataManager.gameData.masterVolume;
        prevVibration = dataManager.gameData.hasVibration;
        HasVibrationUI.GetComponent<ToggleSetAnimatorBoolean>().SetBoolean(prevVibration);
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
        if (prevVibration != HasVibrationUI.GetComponent<ToggleSetAnimatorBoolean>().GetBoolean())
        {
            dataManager.gameData.hasVibration = !prevVibration;
            dataManager.SaveGameData();
            if (!prevVibration) // 진동을 ON한 경우
            {
                VibrateOffUI.SetActive(false);
                FindObjectOfType<AudioManager>().Vibrate(); // 진동으로 알려줌
            } else {
                VibrateOffUI.SetActive(true);
            }
        }
        prevVibration = dataManager.gameData.hasVibration;
    }
}
