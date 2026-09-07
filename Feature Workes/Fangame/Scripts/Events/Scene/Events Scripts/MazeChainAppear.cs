using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeChainAppear : MonoBehaviour
{
    private GameManager gmm;
    private PlayerControl pc;
    public int eventNum;
    public GameObject pl_inter;
    public GameObject BossInter;
    // Start is called before the first frame update
    void Start()
    {
        gmm = GameObject.Find("GameManager").GetComponent<GameManager>();
        pc = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
    }

    IEnumerator EventStart()
    {
        pc.ispause = true;
        if (!gmm.getSEvent(eventNum))
        {
            pl_inter.SetActive(false);
            yield return new WaitForSeconds(5f);
            pl_inter.SetActive(true);
            BossInter.SetActive(true);
            gmm.setSEvent(eventNum, true);
        }
        pc.ispause = false;
    }
}
