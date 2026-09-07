using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private PlayerManager playerManager;
    private PlayerControl playerControl;
    private GameManager gameManager;
    private LanguageManager languageManager;
    private CharacterBase characterBase;

    private bool HPBarChanged = false;
    private bool MPBarChanged = false;
    private bool BarriorBarChanged = false;
    private bool UTBarChanged = false;

    private float maxBarrior = 0;

    [Header("UI상태")]
    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject menuUI;

    [Header("메인 UI")]

    [Header("표시바")]
    [SerializeField] private Slider HPBar;
    [SerializeField] private Slider HPChangeBar;
    [SerializeField] private Slider MPBar;
    [SerializeField] private Slider MPChangeBar;
    [SerializeField] private Slider BarriorBar;
    [SerializeField] private Slider BarriorChangeBar;
    [SerializeField] private Image EXPGage;
    [SerializeField] private Image UltimatePointGage;
    [SerializeField] private Image UltimatePointGageChangeBar;

    [Header("표시UI")]
    [SerializeField] private Image passive;

    [SerializeField] private TMP_Text HPText;
    [SerializeField] private TMP_Text MPText;
    [SerializeField] private Image UltimateSkill;

    [SerializeField] private Image Skill1;
    [SerializeField] private Image Skill2;
    [SerializeField] private Image Skill3;
    [SerializeField] private Image MoveSkill1;
    [SerializeField] private Image MoveSkill2;


    [Header("메뉴 UI")]

    [Header("표시UI")]
    [SerializeField] private GameObject[] SkillSelects;
    [SerializeField] private GameObject[] MoveSkillSelects;
    [SerializeField] private Image SkillIcon;
    [SerializeField] private TMP_Text SkillName;
    [SerializeField] private TMP_Text SkillInfo;

    [Header("필요 데이터")]
    [SerializeField] private GameObject UltimateEffect;
    [SerializeField] private Sprite[] MainUIDefaults;
    [SerializeField] private Image[] AttackSkillButtons;
    [SerializeField] private Image UltInfoButton;
    [SerializeField] private Image PasInfoButton;
    // Start is called before the first frame update
    void Awake()
    {
        playerControl = GameObject.Find("Player").GetComponent<PlayerControl>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerManager = gameManager.GetComponent<PlayerManager>();
        languageManager = gameManager.GetComponent<LanguageManager>();      
    }

    // Update is called once per frame
    void Update()
    {
        characterBase = gameManager.transform.GetChild(playerManager.GetCharacter()).GetComponent<CharacterBase>();

        for (int i = 0; i < 2; i++)
        {
            if (playerManager.GetMoveSkillCode(i) >= 0)
            {
                MoveSkillSelects[playerManager.GetMoveSkillCode(i)].SetActive(true);
            }
        }

        for (int i = 0; i < 3; i++)
        {
            if (playerManager.GetSettingSkillCode(playerManager.GetCharacter(), i) > 0)
            {
                SkillSelects[playerManager.GetSettingSkillCode(playerManager.GetCharacter(), i) - 1].SetActive(true);

            }
        }

        if (Input.GetButtonDown("Menu")) {
            if (menuUI.activeSelf) { menuUI.SetActive(false); mainUI.SetActive(true); playerControl.SetIsPause(false); }
            else { menuUI.SetActive(true); mainUI.SetActive(false); playerControl.SetIsPause(true); }
        }
        MainUI();
        SkillIconChange();
    }

    private void MainUI() {
        passive.sprite = characterBase.GetPassive().Icon;

        MPText.text = playerManager.GetState("MP").ToString();
        MPBar.maxValue = playerManager.GetState("MaxMP");
        MPBar.value = playerManager.GetState("MP");
        HPText.text = playerManager.GetState("HP").ToString();
        HPBar.maxValue = playerManager.GetState("MaxHP");
        HPBar.value = playerManager.GetState("HP");

        if (maxBarrior < playerManager.GetState("Barrior"))
            maxBarrior = playerManager.GetState("Barrior");      
        else if (playerManager.GetState("Barrior") == 0) 
            maxBarrior = 0;
        
        BarriorBar.maxValue = maxBarrior;
        BarriorBar.value = playerManager.GetState("Barrior");
        BarriorChangeBar.maxValue = maxBarrior;

        MPChangeBar.maxValue = playerManager.GetState("MaxMP");
        HPChangeBar.maxValue = playerManager.GetState("MaxHP");

        if (HPChangeBar.value != HPBar.value && !HPBarChanged)
        {
            if (HPChangeBar.value < HPBar.value) HPChangeBar.value = HPBar.value;
            else
            {
                HPBarChanged = true;
                StartCoroutine("HPBarChange");
            }
        }

        if (maxBarrior <= 0)
        {
            BarriorBar.value = 0;
            BarriorChangeBar.value = 0;
            BarriorBar.gameObject.SetActive(false);
            BarriorChangeBar.gameObject.SetActive(false);
        }
        else
        {
            BarriorBar.gameObject.SetActive(true);
            BarriorChangeBar.gameObject.SetActive(true);
            if (BarriorChangeBar.value != BarriorBar.value && !BarriorBarChanged)
            {
                if (BarriorChangeBar.value < BarriorBar.value) BarriorChangeBar.value = BarriorBar.value;
                else
                {
                    BarriorBarChanged = true;
                    StartCoroutine("BarriorBarChange");
                }
            }
        }

        if (MPChangeBar.value != MPBar.value && !MPBarChanged)
        {
            if (MPChangeBar.value < MPBar.value) MPChangeBar.value = MPBar.value;
            else
            {
                MPBarChanged = true;
                StartCoroutine("MPBarChange");
            }
        }

        if (UltimatePointGageChangeBar.fillAmount != UltimatePointGage.fillAmount && !UTBarChanged)
        {
            if (UltimatePointGageChangeBar.fillAmount < UltimatePointGage.fillAmount) UltimatePointGageChangeBar.fillAmount = UltimatePointGage.fillAmount;
            else
            {
                UTBarChanged = true;
                StartCoroutine("UTBarChange");
            }
        }

        EXPGage.fillAmount = (playerManager.GetState("EXP")) / ((playerManager.GetState("Level") * playerManager.GetState("Level") * 10f) + 100f);
        UltimateSkill.sprite = characterBase.GetSkillwithIndex(0).Icon;
        UltimatePointGage.fillAmount = playerManager.GetState("UltimatePoint") * 0.004f;

        if (playerManager.GetState("UltimatePoint") < 250) UltimateSkill.color = new Color(0.5f, 0.5f, 0.5f);
        else UltimateSkill.color = new Color(1, 1, 1);

        UltimateEffect.SetActive(playerManager.GetState("UltimatePoint") >= 250);

        Skill1.sprite = (playerManager.GetSettingSkillCode(playerManager.GetCharacter(), 0) >= 1) ? characterBase.GetSkillwithIndex(playerManager.GetSettingSkillCode(playerManager.GetCharacter(), 0)).Icon : MainUIDefaults[3];
        Skill2.sprite = (playerManager.GetSettingSkillCode(playerManager.GetCharacter(), 1) >= 1) ? characterBase.GetSkillwithIndex(playerManager.GetSettingSkillCode(playerManager.GetCharacter(), 1)).Icon : MainUIDefaults[3];
        Skill3.sprite = (playerManager.GetSettingSkillCode(playerManager.GetCharacter(), 2) >= 1) ? characterBase.GetSkillwithIndex(playerManager.GetSettingSkillCode(playerManager.GetCharacter(), 2)).Icon : MainUIDefaults[3];
        MoveSkill1.sprite = (playerManager.GetMoveSkillCode(0) >= 0) ? playerManager.GetMoveSkillWithindex(playerManager.GetMoveSkillCode(0)).Icon : MainUIDefaults[2];
        MoveSkill2.sprite = (playerManager.GetMoveSkillCode(1) >= 0) ? playerManager.GetMoveSkillWithindex(playerManager.GetMoveSkillCode(1)).Icon : MainUIDefaults[2];


        if (playerManager.GetSettingSkillCode(playerManager.GetCharacter(), 0) >= 1)
        {
            if (characterBase.GetisSkillDoing(playerManager.GetSettingSkillCode(playerManager.GetCharacter(), 0))) Skill1.transform.GetChild(0).gameObject.SetActive(true);
            else Skill1.transform.GetChild(0).gameObject.SetActive(false);

            if (characterBase.GetSkillCooltime(playerManager.GetSettingSkillCode(playerManager.GetCharacter(), 0), 0) > 0)
            {
                Skill1.transform.GetChild(1).gameObject.SetActive(true);
                Skill1.transform.GetChild(1).GetComponent<Image>().fillAmount = characterBase.GetSkillCooltime(playerManager.GetSettingSkillCode(playerManager.GetCharacter(), 0), 0) / characterBase.GetSkillCooltime(playerManager.GetSettingSkillCode(playerManager.GetCharacter(), 0), 1);
                Skill1.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = characterBase.GetSkillCooltime(playerManager.GetSettingSkillCode(playerManager.GetCharacter(), 0), 0).ToString("F1");
            }
            else Skill1.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            Skill1.transform.GetChild(0).gameObject.SetActive(false);
            Skill1.transform.GetChild(1).gameObject.SetActive(false);
        }

        if (playerManager.GetSettingSkillCode(playerManager.GetCharacter(), 1) >= 1)
        {
            if (characterBase.GetisSkillDoing(playerManager.GetSettingSkillCode(playerManager.GetCharacter(), 1))) Skill2.transform.GetChild(0).gameObject.SetActive(true);
            else Skill2.transform.GetChild(0).gameObject.SetActive(false);

            if (characterBase.GetSkillCooltime(playerManager.GetSettingSkillCode(playerManager.GetCharacter(), 1), 0) > 0)
            {
                Skill2.transform.GetChild(1).gameObject.SetActive(true);
                Skill2.transform.GetChild(1).GetComponent<Image>().fillAmount = characterBase.GetSkillCooltime(playerManager.GetSettingSkillCode(playerManager.GetCharacter(), 1), 0) / characterBase.GetSkillCooltime(playerManager.GetSettingSkillCode(playerManager.GetCharacter(), 1), 1);
                Skill2.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = characterBase.GetSkillCooltime(playerManager.GetSettingSkillCode(playerManager.GetCharacter(), 1), 0).ToString("F1");
            }
            else Skill2.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            Skill2.transform.GetChild(0).gameObject.SetActive(false);
            Skill2.transform.GetChild(1).gameObject.SetActive(false);
        }

        if (playerManager.GetSettingSkillCode(playerManager.GetCharacter(), 2) >= 1)
        {
            if (characterBase.GetisSkillDoing(playerManager.GetSettingSkillCode(playerManager.GetCharacter(), 2))) Skill3.transform.GetChild(0).gameObject.SetActive(true);
            else Skill3.transform.GetChild(0).gameObject.SetActive(false);

            if (characterBase.GetSkillCooltime(playerManager.GetSettingSkillCode(playerManager.GetCharacter(), 2), 0) > 0)
            {
                Skill3.transform.GetChild(1).gameObject.SetActive(true);
                Skill3.transform.GetChild(1).GetComponent<Image>().fillAmount = characterBase.GetSkillCooltime(playerManager.GetSettingSkillCode(playerManager.GetCharacter(), 2), 0) / characterBase.GetSkillCooltime(playerManager.GetSettingSkillCode(playerManager.GetCharacter(), 2), 1);
                Skill3.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = characterBase.GetSkillCooltime(playerManager.GetSettingSkillCode(playerManager.GetCharacter(), 2), 0).ToString("F1");
            }
            else Skill3.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            Skill3.transform.GetChild(0).gameObject.SetActive(false);
            Skill3.transform.GetChild(1).gameObject.SetActive(false);
        }

        if (playerManager.GetMoveSkillCode(0) >= 0)
        {
            if (playerManager.GetMoveSkillsOn(playerManager.GetMoveSkillCode(0))) MoveSkill1.transform.GetChild(0).gameObject.SetActive(true);
            else MoveSkill1.transform.GetChild(0).gameObject.SetActive(false);

            if (playerManager.GetMoveSkillCooltime(playerManager.GetMoveSkillCode(0), 0) > 0)
            {
                MoveSkill1.transform.GetChild(1).gameObject.SetActive(true);
                MoveSkill1.transform.GetChild(1).GetComponent<Image>().fillAmount = playerManager.GetMoveSkillCooltime(playerManager.GetMoveSkillCode(0), 0) / playerManager.GetMoveSkillCooltime(playerManager.GetMoveSkillCode(0), 1);
                MoveSkill1.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = playerManager.GetMoveSkillCooltime(playerManager.GetMoveSkillCode(0), 0).ToString("F1");
            }
            else MoveSkill1.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            MoveSkill1.transform.GetChild(0).gameObject.SetActive(false);
            MoveSkill1.transform.GetChild(1).gameObject.SetActive(false);
        }

        if ((playerManager.GetMoveSkillCode(1) >= 0))
        {
            if (playerManager.GetMoveSkillsOn(playerManager.GetMoveSkillCode(1))) MoveSkill2.transform.GetChild(0).gameObject.SetActive(true);
            else MoveSkill2.transform.GetChild(0).gameObject.SetActive(false);

            if (playerManager.GetMoveSkillCooltime(playerManager.GetMoveSkillCode(1), 0) > 0)
            {
                MoveSkill2.transform.GetChild(1).gameObject.SetActive(true);
                MoveSkill2.transform.GetChild(1).GetComponent<Image>().fillAmount = playerManager.GetMoveSkillCooltime(playerManager.GetMoveSkillCode(1), 0) / playerManager.GetMoveSkillCooltime(playerManager.GetMoveSkillCode(1), 1);
                MoveSkill2.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = playerManager.GetMoveSkillCooltime(playerManager.GetMoveSkillCode(1), 0).ToString("F1");
            }
            else MoveSkill2.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            MoveSkill2.transform.GetChild(0).gameObject.SetActive(false);
            MoveSkill2.transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    private void SkillIconChange() {
        for (int i = 0; i < 12; i++) {
            AttackSkillButtons[i].sprite = characterBase.GetSkillwithIndex(i + 1).Icon;
            if(playerManager.GetSettingSkillCode(playerManager.GetCharacter(), 0) == i + 1 || playerManager.GetSettingSkillCode(playerManager.GetCharacter(), 1) == i + 1 || playerManager.GetSettingSkillCode(playerManager.GetCharacter(), 2) == i + 1) SkillSelects[i].SetActive(true);
            else SkillSelects[i].SetActive(false);
        }

        for (int i = 0; i < 6; i++)
        {
            if (playerManager.GetMoveSkillCode(0) == i || playerManager.GetMoveSkillCode(1) == i) MoveSkillSelects[i].SetActive(true);
            else MoveSkillSelects[i].SetActive(false);
        }
        UltInfoButton.sprite = characterBase.GetSkillwithIndex(0).Icon;
        PasInfoButton.sprite = characterBase.GetPassive().Icon;
    }

    IEnumerator HPBarChange() {
        float changevalue = -1;
        while (changevalue != HPBar.value)
        {
            changevalue = HPBar.value;
            yield return new WaitForSeconds(0.2f);
        }
        float original = HPChangeBar.value;
        for (float i = original; i > changevalue; i -= ((original - changevalue) * 0.1f)) {
            HPChangeBar.value = i;
            yield return new WaitForSeconds(0.01f);
        }
        HPChangeBar.value = changevalue;
        HPBarChanged = false;
    }

    IEnumerator MPBarChange()
    {
        float changevalue = MPBar.value;
        float original = MPChangeBar.value;
        for (float i = original; i > changevalue; i -= ((original - changevalue) * 0.1f))
        {
            MPChangeBar.value = i;
            yield return new WaitForSeconds(0.01f);
        }
        MPChangeBar.value = changevalue;
        MPBarChanged = false;
    }

    IEnumerator BarriorBarChange()
    {
        float changevalue = BarriorBar.value;
        float original = BarriorChangeBar.value;
        for (float i = original; i > changevalue; i -= ((original - changevalue) * 0.1f))
        {
            BarriorChangeBar.value = i;
            yield return new WaitForSeconds(0.01f);
        }
        BarriorChangeBar.value = changevalue;
        BarriorBarChanged = false;
    }

    IEnumerator UTBarChange()
    {
        float changevalue = UltimatePointGage.fillAmount;
        float original = UltimatePointGageChangeBar.fillAmount;
        for (float i = original; i > changevalue; i -= ((original - changevalue) * 0.1f))
        {
            UltimatePointGageChangeBar.fillAmount = i;
            yield return new WaitForSeconds(0.01f);
        }
        UltimatePointGageChangeBar.fillAmount = changevalue;
        UTBarChanged = false;
    }

    //TMP
    public void SkillSelectButton(int code)
    {
        SkillIcon.sprite = characterBase.GetSkillwithIndex(code).Icon;
        SkillName.text = characterBase.GetSkillwithIndex(code).skillName;
        SkillInfo.text = characterBase.GetSkillwithIndex(code).skillInfo;

        for (int i = 0; i < 3; i++) 
        {          
            if (playerManager.GetSettingSkillCode(playerManager.GetCharacter(), i) == code)
            {
                SkillSelects[code - 1].SetActive(false);
                playerManager.SetSettingSkillCode(playerManager.GetCharacter(), i, 0);
                return;
            }
        }
        for (int i = 0; i < 3; i++)
        {
            if (playerManager.GetSettingSkillCode(playerManager.GetCharacter(), i) == 0)
            {
                SkillSelects[code - 1].SetActive(true);
                playerManager.SetSettingSkillCode(playerManager.GetCharacter(), i, code);
                return;
            }
        }
    }
    public void MovingSkillSelectButton(int code)
    {
        SkillIcon.sprite = playerManager.GetMoveSkillWithindex(code).Icon;
        SkillName.text = playerManager.GetMoveSkillWithindex(code).skillName;
        SkillInfo.text = playerManager.GetMoveSkillWithindex(code).skillInfo;

        for (int i = 0; i < 2; i++)
        {
            if (playerManager.GetMoveSkillCode(i) == code)
            {
                playerManager.SetMoveSkillCode(i, -1);
                return;
            }
        }

        for (int i = 0; i < 2; i++)
        {
            if (playerManager.GetMoveSkillCode(i) == -1)
            {
                playerManager.SetMoveSkillCode(i, code);
                return;
            }
        }
    }

    public void ShowSkillInfo(int code)
    {
        SkillIcon.sprite = characterBase.GetSkillwithIndex(code).Icon;
        SkillName.text = characterBase.GetSkillwithIndex(code).skillName;
        SkillInfo.text = characterBase.GetSkillwithIndex(code).skillInfo;
    }

    public void ShowPassiveInfo()
    {
        SkillIcon.sprite = characterBase.GetPassive().Icon;
        SkillName.text = characterBase.GetPassive().Name;
        SkillInfo.text = characterBase.GetPassive().Info;
    }
}
