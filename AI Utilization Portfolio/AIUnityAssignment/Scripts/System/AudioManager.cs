using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    private AudioSource bgmSource;
    private AudioSource bgsSource;
    private List<AudioSource> sfxSources = new List<AudioSource>();

    private List<AudioSource> pausedSFXSources = new List<AudioSource>();

    private float lastMasterVol = -1f;
    private float lastBGMVol = -1f;
    private float lastBGSVol = -1f;
    private float lastSFVol = -1f;

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSources()
    {
        GameObject bgmGO = new GameObject("BGM_Source");
        bgmGO.transform.SetParent(transform);
        bgmSource = bgmGO.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;

        GameObject bgsGO = new GameObject("BGS_Source");
        bgsGO.transform.SetParent(transform);
        bgsSource = bgsGO.AddComponent<AudioSource>();
        bgsSource.loop = true;
        bgsSource.playOnAwake = false;

        for (int i = 0; i < 50; i++)
        {
            CreateNewSFXSource();
        }
    }

    private AudioSource CreateNewSFXSource()
    {
        GameObject sfxGO = new GameObject($"SFX_Source_{sfxSources.Count}");
        sfxGO.transform.SetParent(transform);
        AudioSource newSource = sfxGO.AddComponent<AudioSource>();
        newSource.loop = false;
        newSource.playOnAwake = false;
        sfxSources.Add(newSource);
        return newSource;
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;

        ConfigData config = GameManager.Instance.currentConfig;

        if (lastMasterVol != config.masterVolume ||
            lastBGMVol != config.bgmVolume ||
            lastBGSVol != config.bgsVolume ||
            lastSFVol != config.sfVolume)
        {
            ApplyAllVolumes();

            lastMasterVol = config.masterVolume;
            lastBGMVol = config.bgmVolume;
            lastBGSVol = config.bgsVolume;
            lastSFVol = config.sfVolume;
        }

        if (Time.timeScale == 0f && !isPaused)
        {
            PauseAllSFX();
            isPaused = true;
        }
        else if (Time.timeScale > 0f && isPaused)
        {
            ResumeAllSFX();
            isPaused = false;
        }
    }
    private void ApplyAllVolumes()
    {
        if (GameManager.Instance == null) return;
        ConfigData config = GameManager.Instance.currentConfig;

        float master = config.masterVolume;

        if (bgmSource != null) bgmSource.volume = master * config.bgmVolume;
        if (bgsSource != null) bgsSource.volume = master * config.bgsVolume;

        float sfxVol = master * config.sfVolume;
        foreach (var sfx in sfxSources)
        {
            sfx.volume = sfxVol;
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.clip = clip;
        bgmSource.volume = GameManager.Instance.currentConfig.masterVolume * GameManager.Instance.currentConfig.bgmVolume;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PlayBGS(AudioClip clip)
    {
        if (clip == null) return;
        if (bgsSource.clip == clip && bgsSource.isPlaying) return;

        bgsSource.clip = clip;
        bgsSource.volume = GameManager.Instance.currentConfig.masterVolume * GameManager.Instance.currentConfig.bgmVolume;
        bgsSource.Play();
    }

    public void StopBGS()
    {
        bgsSource.Stop();
    }

    public void PlaySFX(AudioClip clip, bool isUI_Sound = false)
    {
        if (clip == null) return;

        if (Time.timeScale == 0f && !isUI_Sound) return;

        AudioSource availableSource = null;

        foreach (var source in sfxSources)
        {
            if (!source.isPlaying)
            {
                if (!pausedSFXSources.Contains(source))
                {
                    availableSource = source;
                    break;
                }
            }
        }

        if (availableSource == null)
        {
            availableSource = CreateNewSFXSource();
        }

        availableSource.clip = clip;
        availableSource.volume = GameManager.Instance.currentConfig.masterVolume * GameManager.Instance.currentConfig.sfVolume;; 

        availableSource.Play();
    }

    private void PauseAllSFX()
    {
        pausedSFXSources.Clear();
        foreach (var source in sfxSources)
        {
            if (source.isPlaying)
            {
                source.Pause();
                pausedSFXSources.Add(source); 
            }
        }
    }

    private void ResumeAllSFX()
    {
        foreach (var source in pausedSFXSources)
        {
            if (source != null)
            {
                source.UnPause();
            }
        }
        pausedSFXSources.Clear();
    }
}