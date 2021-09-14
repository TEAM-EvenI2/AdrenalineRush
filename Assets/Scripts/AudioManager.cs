using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public static AudioManager instance;
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
        AudioListener.volume = FindObjectOfType<DataManager>().gameData.masterVolume;
        Play("LobbyTheme");
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null) s.source.Play();
    }
}
