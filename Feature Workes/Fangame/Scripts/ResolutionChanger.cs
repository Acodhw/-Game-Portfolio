using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionChanger : MonoBehaviour
{
    public int[] xset;
    private bool isfullscreen;
    // Start is called before the first frame update

    public void setfull()
    {
        isfullscreen = !isfullscreen;
        Screen.fullScreen = isfullscreen;
    }

    public void setScreenResolution(int setting)
    {
        Screen.SetResolution(xset[setting], xset[setting] * 9 / 16, isfullscreen);
    }
}
