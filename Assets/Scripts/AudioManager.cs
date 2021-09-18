using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    // 게임 내 모든 오디오 및 진동을 관리합니다.
    public Sound[] sounds;
    public static AudioManager instance;
    private DataManager dataManager;
    void Awake()
    {  
        // 중복 방지
        if (instance == null)
            instance = this;
        else{
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    void Start()
    {
        dataManager = FindObjectOfType<DataManager>();
        AudioListener.volume = FindObjectOfType<DataManager>().gameData.masterVolume;
        Play("LobbyTheme");
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null) s.source.Play();
    }

    public void Vibrate()
    {
        if (dataManager.gameData.hasVibration)
        {
            Debug.Log("(Vibrating Device)");
            Handheld.Vibrate(); // 진동패턴이나 진동시간 조절하려면 이 함수 말고 다른 함수를 제작해 써야함.
        }
    }
}
