using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardControl : MonoBehaviour
{
    public GameObject comment;
    public Text cardName;
    public Text cardInfo;

    public RectTransform[] cardPoint;
    public RectTransform cardCursor;

    public int selectedCardSquare = 0;
    public int selectedCardNum = -1;
    PlayerState_Proto pstate;
    GameDatas gd;
    // Start is called before the first frame update

    private void Start()
    {
        pstate = GameObject.Find("GameManager").GetComponent<PlayerState_Proto>();
        gd = GameObject.Find("GameManager").GetComponent<GameDatas>();
    }

    private void OnEnable()
    {
        selectedCardSquare = 0;
        selectedCardNum = -1;
        comment.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {       
        cardCursor.transform.position = new Vector3(cardPoint[selectedCardSquare].position.x, cardPoint[selectedCardSquare].position.y, 0);

        if (Input.GetButtonDown("SelectCard"))
        {
            if (selectedCardNum != -1)
            {
                if (selectedCardSquare != selectedCardNum)
                {
                    if (selectedCardSquare < pstate.OpenedCard.Count)
                    {
                        int tmp = pstate.OpenedCard[selectedCardNum];
                        pstate.OpenedCard[selectedCardNum] = pstate.OpenedCard[selectedCardSquare];
                        pstate.OpenedCard[selectedCardSquare] = tmp;
                    }
                    else
                    {
                        int tmp = pstate.OpenedCard[selectedCardNum];
                        pstate.OpenedCard.RemoveAt(selectedCardNum);
                        pstate.OpenedCard.Add(tmp);
                    }
                    selectedCardNum = -1;
                }
            }
            else if (selectedCardSquare < pstate.OpenedCard.Count)
                selectedCardNum = selectedCardSquare;
        }
        if (Input.GetButtonDown("Vertical"))
        {
            if (Input.GetAxisRaw("Vertical") == -1)
            {
                if (selectedCardSquare < pstate.OpenedCard.Count)
                    pstate.OpenedCard.RemoveAt(selectedCardSquare);
            }
        }
        if (selectedCardNum != -1) {
            comment.SetActive(true);
            cardName.text = gd.card[pstate.OpenedCard[selectedCardNum]].CardName;
            cardInfo.text = gd.card[pstate.OpenedCard[selectedCardNum]].CardInfo;
            
        }
        if (Input.GetButtonDown("Horizontal"))
        {
            if (Input.GetAxisRaw("Horizontal") == 1)
            {
                if (selectedCardSquare == 3)
                    selectedCardSquare = 0;
                else
                    selectedCardSquare += 1;
            }
            else
            {
                if (selectedCardSquare == 0)
                    selectedCardSquare = 3;
                else
                    selectedCardSquare -= 1;
            }
        }
    }
}
