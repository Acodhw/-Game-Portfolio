using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardMenu : MonoBehaviour
{
    public Transform haveCardContants;
    public Transform SetCardContants;
    public GameObject cardButton;

    public Image CardImage;
    public Text CardName;
    public Text CardText;
    public Text HaveCardCount;
    public Text SetCardCount;

    public Text CardSetButton;

    bool isonDeck = false;
    [SerializeField]
    int CardNumber = -1;

    PlayerState_Proto pstate;
    GameDatas gd;

    int[] diffcheck;
    int[] diffcheckDeck;

    // Start is called before the first frame update
    void Start()
    {
        gd = GameObject.Find("GameManager").GetComponent<GameDatas>();
        pstate = GameObject.Find("GameManager").GetComponent<PlayerState_Proto>();
        diffcheck = new int[pstate.HaveCardKind.Length];
        diffcheckDeck = new int[pstate.SettingCardKind.Length];
    }

    // Update is called once per frame
    void Update()
    {
        int result = 0;
        for (int i = 0; i < pstate.HaveCardKind.Length; i++)
        {
            result += pstate.HaveCardKind[i] - pstate.SettingCardKind[i];
        }
        HaveCardCount.text = result + " / 2000";
        result = 0;
        foreach (int i in pstate.SettingCardKind)
        {
            result += i;
        }
        SetCardCount.text = result + " / 40";
        if (isonDeck)
        {
            CardSetButton.text = "»©±â";
        }
        else
        {
            CardSetButton.text = "³Ö±â";
        }
        if (CardNumber >= 0 && CardNumber < pstate.HaveCardKind.Length)
        {
            CardImage.sprite = gd.card[CardNumber].CardImage;
            CardName.text = gd.card[CardNumber].CardName;
            CardText.text = gd.card[CardNumber].CardInfo;
        }
        bool x = CheckArrayIsSame(diffcheck, pstate.HaveCardKind), y = CheckArrayIsSame(diffcheckDeck, pstate.SettingCardKind);
        
        if (!x || !y)
        {           
            for (int i = 0; i < haveCardContants.childCount; i++)
            {
                Destroy(haveCardContants.GetChild(i).gameObject);
            }
            for (int i = 0; i < SetCardContants.childCount; i++)
            {
                Destroy(SetCardContants.GetChild(i).gameObject);
            }

            for (int i = 0; i < pstate.HaveCardKind.Length; i++)
            {
                if (pstate.HaveCardKind[i] - pstate.SettingCardKind[i] > 0)
                {
                    Button b = Instantiate(cardButton, haveCardContants).GetComponent<Button>();
                    b.onClick.AddListener(delegate { SelectedCardisonDeck(false); });
                    int a = i;
                    b.onClick.AddListener(delegate { SelectedCardNumber(a); });
                    b.transform.GetChild(0).GetComponent<Text>().text = gd.card[i].CardName + " x " + (pstate.HaveCardKind[i] - pstate.SettingCardKind[i]);
                    b.transform.GetChild(1).GetComponent<Image>().sprite = gd.card[i].CardImage;
                }
            }

            for (int i = 0; i < pstate.HaveCardKind.Length; i++)
            {
                if (pstate.SettingCardKind[i] > 0)
                {
                    Button b = Instantiate(cardButton, SetCardContants).GetComponent<Button>();
                    b.onClick.AddListener(delegate { SelectedCardisonDeck(true); });
                    int a = i;
                    b.onClick.AddListener(delegate { SelectedCardNumber(a); });
                    b.transform.GetChild(0).GetComponent<Text>().text = gd.card[i].CardName + " x " + pstate.SettingCardKind[i];
                    b.transform.GetChild(1).GetComponent<Image>().sprite = gd.card[i].CardImage;
                }
            }
            for (int i = 0; i < diffcheck.Length; i++)
            {
                diffcheck[i] = pstate.HaveCardKind[i];
                diffcheckDeck[i] = pstate.SettingCardKind[i];
            }

        }
    }

    public void SelectedCardisonDeck(bool isonDeck)
    {
        this.isonDeck = isonDeck;
    }

    public void SelectedCardNumber(int num)
    {
        CardNumber = num;
    }

    public void PutINOUT()
    {
        if (CardNumber != -1)
        {
            if (isonDeck)
            {
                if (pstate.SettingCardKind[CardNumber] > 0)
                {
                    pstate.SettingCardKind[CardNumber] -= 1;
                    pstate.SettingCard.RemoveAt(pstate.SettingCard.IndexOf(CardNumber));
                    pstate.DacMix();
                }

            }
            else
            {
                int result = 0;
                foreach (int i in pstate.SettingCardKind)
                {
                    result += i;
                }
                if (result < 40)
                {
                    if (pstate.SettingCardKind[CardNumber] < pstate.HaveCardKind[CardNumber])
                    {
                        pstate.SettingCardKind[CardNumber] += 1;
                        pstate.SettingCard.Add(CardNumber);
                        pstate.DacMix();
                    }
                }
            }
        }
    }

    public void Throwout()
    {
        if (CardNumber != -1)
        {
            if (pstate.HaveCardKind[CardNumber] > 0)
            {
                if (isonDeck)
                {
                    if (pstate.SettingCardKind[CardNumber] > 0)
                    {
                        pstate.SettingCardKind[CardNumber] -= 1;
                        pstate.HaveCardKind[CardNumber] -= 1;
                        pstate.SettingCard.RemoveAt(pstate.SettingCard.IndexOf(CardNumber));
                        pstate.DacMix();
                    }
                }
                else if(pstate.HaveCardKind[CardNumber] > 0)
                {
                    pstate.HaveCardKind[CardNumber] -= 1;
                }
            }
        }
    }


    bool CheckArrayIsSame(int[] a, int[] b)
    {
        if (a.Length != b.Length)
            return false;
        else
        {
            if (a.Length == 0 && b.Length == 0)
                return true;
            else
            {
                bool tmp = true;
                for (int i = 0; i < a.Length; i++)
                {
                    if (a[i] != b[i])
                    {
                        tmp = false;
                        break;
                    }
                }
                return tmp;
            }
        }
    }


}
