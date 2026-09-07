using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMark : MonoBehaviour
{
    public int CardOutNumber;
    public Sprite DefaultSprite;
    PlayerState_Proto pstate;
    GameDatas gd;
    // Start is called before the first frame update
    void Start()
    {
        pstate = GameObject.Find("GameManager").GetComponent<PlayerState_Proto>();
        gd = GameObject.Find("GameManager").GetComponent<GameDatas>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pstate.OpenedCard.Count >= CardOutNumber + 1)
        {
            GetComponent<UnityEngine.UI.Image>().sprite = gd.card[pstate.OpenedCard[CardOutNumber]].CardImage;
        }
        else 
        {
            GetComponent<UnityEngine.UI.Image>().sprite = DefaultSprite;
        }
    }
}
