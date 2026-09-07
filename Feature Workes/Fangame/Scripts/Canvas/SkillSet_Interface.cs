using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSet_Interface : MonoBehaviour
{
    private PlayerControl pc;

    private PlayerState ps;

    private GameObject[] selected;
    
    public Transform contents;

    public Text skill_Info;

    int selectedSkill;
    string info;

    // Start is called before the first frame update
    void Start()
    {
        ps = GameObject.Find("GameManager").GetComponent<PlayerState>();
        pc = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
        selected = new GameObject[contents.childCount + 1];
        for(int i = 1; i <= contents.childCount; i++)
        {
            selected[i] = contents.GetChild(i-1).GetChild(0).gameObject;
        }
    }

    private void Update()
    {
        for (int i = 1; i < selected.Length; i++)
        {
            if (i == pc.skill1code || i == pc.skill2code)
            {
                selected[i].SetActive(true);
            }
            else
            {
                selected[i].SetActive(false);
            }
        }

        for (int i = 0; i < contents.childCount; i++)
        {
            if (ps.getCanSelectSkills(i))
            {
                contents.GetChild(i).gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f);
            }
            else
            {
                contents.GetChild(i).gameObject.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
            }
        }
    }

    public void skillinfo(string s)
    {        
        info = s;
    }

    public void selectSkill(int i)
    {
        if (ps.getCanSelectSkills(i-1))
        {
            selectedSkill = i;
            skill_Info.text = info;
        }
    }

    public void skillset()
    {
        if (selectedSkill == -1)
        {
            skill_Info.text = "고른 스킬이 없습니다.";
        }
        else
        {
            if (selectedSkill == pc.skill2code)
            {
                pc.skill2code = 0;
                skill_Info.text = "";
                selectedSkill = 0;
            }
            else if (selectedSkill == pc.skill1code)
            {
                pc.skill1code = 0;
                skill_Info.text = "";
                selectedSkill = 0;
            }
            else
            {
                if (pc.skill1code == 0)
                {
                    pc.skill1code = selectedSkill;
                    skill_Info.text = "";
                    selectedSkill = 0;
                }
                else if (pc.skill2code == 0)
                {
                    pc.skill2code = selectedSkill;
                    skill_Info.text = "";
                    selectedSkill = 0;
                }
                else
                {
                    skill_Info.text = "스킬을 해제해 주십시오.";
                }
            }
        }
    }
}
