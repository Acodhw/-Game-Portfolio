using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumSlider : MonoBehaviour
{
    Slider volsli;
    private GameManager gmm;
    private bool settingon = false;
    // Start is called before the first frame update
    void Start()
    {
        gmm = GameObject.Find("GameManager").GetComponent<GameManager>();
        volsli = GetComponent<Slider>();
        volsli.value = gmm.getVolum();
        settingon = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (settingon) {
             gmm.setVolum(volsli.value);
        }
    }
}
