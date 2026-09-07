using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuyEvent : MonoBehaviour
{
    TalkSystem _talkSystem;
    public int Itemkind;
    bool canTalk = false;
    bool talkCool = true;
    private PlayerState state;
    // Start is called before the first frame update
    void Start()
    {
        _talkSystem = GameObject.Find("PlayerPrefeb").GetComponent<TalkSystem>();
        state = GameObject.Find("GameManager").GetComponent<PlayerState>();
    }

    void OnTriggerEnter(Collider hit)
    {
        if (hit.tag.Equals("Player"))
        {
            canTalk = true;
        }
    }

    void OnTriggerExit(Collider hit)
    {
        if (hit.tag.Equals("Player"))
        {
            canTalk = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        _talkSystem.InteractionMessage(canTalk);
        int money = state.GetMoney();
        if (canTalk && Input.GetKeyDown(KeyCode.Z))
        {
            if (Itemkind == 1)
            {
                if (money >= 10) {
                    state.SetHpitem(state.GetHpitem() + 1);
                    state.SetMoney(money - 10);
                    }
            }

            if (Itemkind == 2)
            {
                if (money >= 20)
                {
                    state.SetBuffitem(state.GetBuffitem() + 1);
                    state.SetMoney(money - 20);
                }
            }
        }
    }
}
