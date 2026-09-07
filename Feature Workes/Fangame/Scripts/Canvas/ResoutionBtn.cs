using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResoutionBtn : MonoBehaviour
{
    public bool isSetResulBtn;
    public int setCode;

    private ResolutionChanger rc;
    // Start is called before the first frame update
    void Start()
    {
        rc = GameObject.Find("GameManager").GetComponent<ResolutionChanger>();
        if(!isSetResulBtn)
            GetComponent<Button>().onClick.AddListener(delegate { rc.setfull(); });
        else
            GetComponent<Button>().onClick.AddListener(delegate { rc.setScreenResolution(setCode); });
    }

}
