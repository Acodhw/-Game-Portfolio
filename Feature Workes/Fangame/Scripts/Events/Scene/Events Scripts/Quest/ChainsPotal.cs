using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainsPotal : MonoBehaviour
{
    public int EventPotalNum;
    public bool isMainPotal;
    public GameObject managingPotal;
    private GameManager gmm;
    // Start is called before the first frame update
    void Start()
    {
        gmm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMainPotal)
        {
            if (gmm.getPrograss(0, 0) && gmm.getPrograss(0, 1) && gmm.getPrograss(0, 2) && gmm.getPrograss(0, 3))
                managingPotal.SetActive(true);
        }
        else
        {
            if (gmm.getPrograss(0, EventPotalNum)) 
            {
                managingPotal.SetActive(false);
            }
        }
    }
}
