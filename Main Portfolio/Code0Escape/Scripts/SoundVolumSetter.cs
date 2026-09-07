using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundVolumSetter : MonoBehaviour
{
    private enum SoundKind {
        BGM,
        SE,
    };
    [SerializeField] SoundKind kind;
    [SerializeField] float VoleumRank = 1;
    private SystemSetting settings;
    private AudioSource sourse;
    private float voleum;
    // Start is called before the first frame update
    void Start()
    {
        settings = GameObject.Find("GameManager").GetComponent<SystemSetting>();
        sourse = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        float v = 0;
        switch (kind)
        {
            case SoundKind.BGM:
                v = settings.GetBGMVoleum();
                break;
            case SoundKind.SE:
                v = settings.GetSEVoleum();
                break;
        }

        sourse.volume = v * settings.GetVoleum() * VoleumRank;
    }
}
