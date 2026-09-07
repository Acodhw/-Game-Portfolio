using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public AudioClip[] bgms;

    AudioSource audioSource;
    int nowCode = -1;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void StartBGM(int bgmCode)
    {
        if (nowCode != bgmCode) {
            audioSource.clip = bgms[bgmCode];
            audioSource.Play();
            nowCode = bgmCode;
        }
    }
    public void OffBGM() {
        audioSource.Stop();
        nowCode = -1;
    }
}
