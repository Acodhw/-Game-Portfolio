using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemSetting : MonoBehaviour
{
    private float Sensitivity = 2.0f;

    private float mainVoleum = 1.0f;

    private float bgmVoleum = 1.0f;
    private float seVoleum = 1.0f;


    public void SetSensitvity(float s) { Sensitivity = s; }
    public void SetVoleum(float v) { mainVoleum = v; }
    public void SetBGMVoleum(float v) { bgmVoleum = v; }
    public void SetSEVoleum(float v) { seVoleum = v; }

    public float GetSensitvity() { return Sensitivity; }
    public float GetVoleum() { return mainVoleum; }
    public float GetBGMVoleum() { return bgmVoleum; }
    public float GetSEVoleum() { return seVoleum; }
}
