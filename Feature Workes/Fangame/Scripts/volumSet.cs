using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class volumSet : MonoBehaviour
{
    private GameManager gmm;
    AudioSource ause;
    void Start()
    {
        gmm = GameObject.Find("GameManager").GetComponent<GameManager>();
        ause = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        ause.volume = gmm.getVolum();
    }
}
