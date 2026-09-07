using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMon : MonoBehaviour
{
    BGMManager bm;
    public int code;
    // Start is called before the first frame update
    void Start()
    {
        bm = GameObject.Find("GameManager").GetComponent<BGMManager>();
        if (code == -1) bm.OffBGM();
        else bm.StartBGM(code);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
