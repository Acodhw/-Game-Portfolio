using UnityEngine;

public enum AudioType
{
    BGM,
    BGS,
    SFX
}

public class AudioAdd : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("재생할 오디오 클립")]
    public AudioClip audioClip;

    [Tooltip("오디오 종류")]
    public AudioType audioType = AudioType.SFX;

    [Header("Auto Play Options")]
    [Tooltip("오브젝트가 활성화될 때 자동으로 재생할지 여부")]
    public bool playOnEnable = false;

    [Tooltip("Start() 타이밍에 자동으로 재생할지 여부")]
    public bool playOnStart = false;

    [Tooltip("일시정지에도 실행됨")]
    public bool isUISound = false;

    private void Start()
    {
        if (playOnStart)
        {
            PlaySound();
        }
    }

    private void OnEnable()
    {
        if (playOnEnable)
        {
            if (AudioManager.Instance != null)
            {
                PlaySound();
            }
        }
    }

    public void PlaySound()
    {
        if (AudioManager.Instance == null)
        {
            return;
        }
        if (audioClip == null)
        {
            if (audioType == AudioType.BGM)
            {
                AudioManager.Instance.StopBGM();
            }
            else if (audioType == AudioType.BGS)
            {
                AudioManager.Instance.StopBGS();
            }
            else
            {
 
            }
            return;
        }

        switch (audioType)
        {
            case AudioType.BGM:
                AudioManager.Instance.PlayBGM(audioClip);
                break;
            case AudioType.BGS:
                AudioManager.Instance.PlayBGS(audioClip);
                break;
            case AudioType.SFX:
                AudioManager.Instance.PlaySFX(audioClip, isUISound);
                break;
        }
    }

    public void StopSound()
    {
        if (AudioManager.Instance == null) return;

        if (audioType == AudioType.BGM)
        {
            AudioManager.Instance.StopBGM();
        }
        else if (audioType == AudioType.BGS)
        {
            AudioManager.Instance.StopBGS();
        }
    }
}