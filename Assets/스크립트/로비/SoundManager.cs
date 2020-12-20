using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [HideInInspector] public float bgmVolume;
    [HideInInspector] public float sfxVolume;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        if (PlayerPrefs.HasKey("bgmVolume"))
        {
            bgmVolume = PlayerPrefs.GetFloat("bgmVolume");
        }
        else
        {
            bgmVolume = 1;
        }
        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
        }
        else
        {
            sfxVolume = 1;
        }
    }

    [Header("bgm")]
    public AudioClip lobbyBgm;
    public AudioClip ingameBgm;
    [Header("오디오 소스")]
    public AudioSource bgmAudioSource;
    public AudioSource buttonAudioSource;

    private void Start()
    {
        LobbyBgmStart();
    }

    public void LobbyBgmStart()
    {
        bgmAudioSource.volume = bgmVolume;
        if (bgmAudioSource.clip != lobbyBgm)
        {
            bgmAudioSource.clip = lobbyBgm;
            bgmAudioSource.Play();
        }
    }
    public void IngameBgmStart()
    {
        bgmAudioSource.volume = bgmVolume;
        if (bgmAudioSource.clip != ingameBgm)
        {
            bgmAudioSource.clip = ingameBgm;
            bgmAudioSource.Play();
        }
    }
    public void BgmVolumeSet(float volume)
    {
        PlayerPrefs.SetFloat("bgmVolume", volume);
        this.bgmVolume = volume;
        bgmAudioSource.volume = bgmVolume;
    }
    bool clickFlag = false;
    public void SfxPlay(AudioSource audioSource)
    {
        audioSource.volume = sfxVolume;
        audioSource.Play();
        clickFlag = true;
    }
    public void SfxVolumeSet(float volume)
    {
        PlayerPrefs.SetFloat("sfxVolume", volume);
        sfxVolume = volume;
    }


    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (!clickFlag)
            {
                buttonAudioSource.volume = sfxVolume;
                buttonAudioSource.Play();
            }
            clickFlag = false;
        }
    }
}
