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
    public GameObject ShowLoadSceneUI;
    private DataManager dataManager;
    private float prevVolume;
    private bool prevVibration;
    private bool prevShowLoadingScene; // 항상 로딩씬을 보여줄지 여부 (false면 처음 한번만 보여줌)

    void Start()
    {
        dataManager = FindObjectOfType<DataManager>();
        prevVolume = dataManager.gameData.masterVolume;
        VolumeSlider.GetComponent<Slider>().value = dataManager.gameData.masterVolume;
        prevVibration = dataManager.gameData.hasVibration;
        HasVibrationUI.GetComponent<ToggleSetAnimatorBoolean>().SetBoolean(prevVibration);
        prevShowLoadingScene = dataManager.gameData.alwaysShowLoadingScene;
        ShowLoadSceneUI.GetComponent<ToggleSetAnimatorBoolean>().SetBoolean(prevShowLoadingScene);
        UpdateUI();
    }

    void UpdateUI()
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
        if (prevShowLoadingScene != ShowLoadSceneUI.GetComponent<ToggleSetAnimatorBoolean>().GetBoolean())
        {
            dataManager.gameData.alwaysShowLoadingScene = !prevShowLoadingScene;
            dataManager.SaveGameData();
        }
        prevShowLoadingScene = dataManager.gameData.alwaysShowLoadingScene;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }
}
