using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class SkillComment {
    public string[] SkillInfo;
}

public class SkillSelectInterface : MonoBehaviour
{
    private PlayerState_Proto pstate;
    private PlayerCharactor_Proto pchar;
    public Text psvStatText;
    public Text attStatText;
    public Text defStatText;
    public Text intStatText;
    public Text mreStatText;
    public Text criStatText;

    public Transform moveSkillcontent;
    public Transform attackSkillcontent;

    public Image moveSkillSelectImg;
    public Image attackSkillSelectImg;

    public Image[] selectedSkills;

    GameObject[] moveSkillBtns;
    GameObject[] attackSkillBtns;

    public SkillComment[] skills;
    public string[] passiveTx;
    
    int moveSelect;
    int attackSelect;

    int defaultcharactor = 0;
    // Start is called before the first frame update
    void Start()
    {
        pchar = GameObject.Find("mainChar_proto").GetComponent<PlayerCharactor_Proto>();
        pstate = GameObject.Find("GameManager").GetComponent<PlayerState_Proto>();
        moveSkillBtns = new GameObject[moveSkillcontent.childCount];
        for (int i = 0; i < moveSkillcontent.childCount; i++)
        {
            moveSkillBtns[i] = moveSkillcontent.GetChild(i).gameObject;
        }
        attackSkillBtns = new GameObject[attackSkillcontent.childCount];
        for (int i = 0; i < attackSkillcontent.childCount; i++)
        {
            attackSkillBtns[i] = attackSkillcontent.GetChild(i).gameObject;
        }
    }
    private void OnEnable()
    {
        moveSkillBtns = new GameObject[moveSkillcontent.childCount];
        for (int i = 0; i < moveSkillcontent.childCount; i++) {
            moveSkillBtns[i] = moveSkillcontent.GetChild(i).gameObject;
        }
        attackSkillBtns = new GameObject[attackSkillcontent.childCount];
        for (int i = 0; i < attackSkillcontent.childCount; i++)
        {
            attackSkillBtns[i] = attackSkillcontent.GetChild(i).gameObject;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        psvStatText.text = "패시브|" + passiveTx[pstate.getCharactorKey()];
        if (defaultcharactor != pstate.getCharactorKey())
        {
            defaultcharactor = pstate.getCharactorKey();
            attackSelect = 0;
        }
        attStatText.text = "공격력 : " + pstate.power;
        defStatText.text = "방어력 : " + pstate.defense;
        intStatText.text = "지능 : " + pstate.intellect;
        mreStatText.text = "마법방어 : " + pstate.magic_resistance;
        criStatText.text = "기본 치명타 계수 : " + pstate.criticalDamageMult;
        if(pstate.MoveSkill1 == -1)
            selectedSkills[0].sprite = pchar.DefaultSkillSprite;
        else
            selectedSkills[0].sprite = pchar.MoveSkills[pstate.MoveSkill1];

        if (pstate.MoveSkill2 == -1)
            selectedSkills[1].sprite = pchar.DefaultSkillSprite;
        else
            selectedSkills[1].sprite = pchar.MoveSkills[pstate.MoveSkill2];

        if (pstate.AttackSkill1[pstate.getCharactorKey()] == -1)
            selectedSkills[2].sprite = pchar.DefaultSkillSprite;
        else
            selectedSkills[2].sprite = pchar.AttackSkills[pstate.getCharactorKey()].AttackSkills[pstate.AttackSkill1[pstate.getCharactorKey()]];
        
        if (pstate.AttackSkill2[pstate.getCharactorKey()] == -1)
            selectedSkills[3].sprite = pchar.DefaultSkillSprite;
        else
            selectedSkills[3].sprite = pchar.AttackSkills[pstate.getCharactorKey()].AttackSkills[pstate.AttackSkill2[pstate.getCharactorKey()]];

        for (int i = 0; i < attackSkillBtns.Length; i++)
        {
            string name = skills[pstate.getCharactorKey()].SkillInfo[i].Split(',')[0];
            string info = skills[pstate.getCharactorKey()].SkillInfo[i].Split(',')[1];

            attackSkillBtns[i].transform.GetChild(0).GetComponent<Text>().text = name;
            attackSkillBtns[i].transform.GetChild(1).GetComponent<Text>().text = info;
            attackSkillBtns[i].transform.GetChild(2).GetComponent<Image>().sprite = pchar.AttackSkills[pstate.getCharactorKey()].AttackSkills[i];
            if (i == pstate.AttackSkill2[pstate.getCharactorKey()] || i == pstate.AttackSkill1[pstate.getCharactorKey()])
                attackSkillBtns[i].transform.GetChild(3).gameObject.SetActive(true);
            else
                attackSkillBtns[i].transform.GetChild(3).gameObject.SetActive(false);
        }

        for (int i = 0; i < moveSkillBtns.Length; i++)
        {
            if (i == pstate.MoveSkill2 || i == pstate.MoveSkill1)
                moveSkillBtns[i].transform.GetChild(3).gameObject.SetActive(true);
            else
                moveSkillBtns[i].transform.GetChild(3).gameObject.SetActive(false);
        }

        moveSkillSelectImg.sprite = pchar.MoveSkills[moveSelect];
        attackSkillSelectImg.sprite = pchar.AttackSkills[pstate.getCharactorKey()].AttackSkills[attackSelect];
    }

    public void moveSkillSelect(int code) {
        moveSelect = code;
    }

    public void attackSkillSelect(int code)
    {
        attackSelect = code;
    }

    public void skillSetting(bool ismoveskill)
    {
        if (ismoveskill)
        {
            if (moveSelect == pstate.MoveSkill2)
            {
                pstate.MoveSkill2 = -1;
            }
            else {
                if (moveSelect == pstate.MoveSkill1)
                {
                    pstate.MoveSkill1 = -1;
                }
                else {
                    if (pstate.MoveSkill1 == -1)
                    {
                        pstate.MoveSkill1 = moveSelect;
                    }
                    else
                    {
                        pstate.MoveSkill2 = moveSelect;
                    }
                }
            }
        }
        else 
        {
            if (attackSelect == pstate.AttackSkill2[pstate.getCharactorKey()])
            {
                pstate.AttackSkill2[pstate.getCharactorKey()] = -1;
            }
            else
            {
                if (attackSelect == pstate.AttackSkill1[pstate.getCharactorKey()])
                {
                    pstate.AttackSkill1[pstate.getCharactorKey()] = -1;
                }
                else
                {
                    if (pstate.AttackSkill1[pstate.getCharactorKey()] == -1)
                    {
                        pstate.AttackSkill1[pstate.getCharactorKey()] = attackSelect;
                    }
                    else
                    {
                        pstate.AttackSkill2[pstate.getCharactorKey()] = attackSelect;
                    }
                }
            }
        }
    }
}
