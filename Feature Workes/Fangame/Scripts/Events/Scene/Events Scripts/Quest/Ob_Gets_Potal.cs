using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ob_Gets_Potal : MonoBehaviour
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
            if (gmm.getPrograss(1, 0) && gmm.getPrograss(1, 1) && gmm.getPrograss(1, 2) && gmm.getPrograss(1, 3) && gmm.getPrograss(1, 4) && gmm.getPrograss(1, 5) && gmm.getPrograss(1, 6) && gmm.getPrograss(1, 7) && gmm.getPrograss(1, 8) && gmm.getPrograss(1, 9))
                managingPotal.SetActive(true);
        }
        else
        {
            if (gmm.getPrograss(1, EventPotalNum))
            {
                managingPotal.SetActive(false);
            }
        }
    }
}
