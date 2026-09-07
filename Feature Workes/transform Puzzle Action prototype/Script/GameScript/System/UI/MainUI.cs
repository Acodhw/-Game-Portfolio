using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    private GameManager manager;
    private PlayerState pstate;

    [SerializeField]
    private Sprite[] formIcons;
    [SerializeField]
    private Sprite[] skillIcons;

    [SerializeField] 
    private Slider HPbar;
    [SerializeField]
    private Image stateCase;
    [SerializeField]
    private Image staminaIcon;
    [SerializeField]
    private Image maxStaminaIcon;
    [SerializeField]
    private Image playerFormIconSlot;
    [SerializeField]
    private Image skillSlot;
    [SerializeField]
    private Image skillActiveImg;
    [SerializeField]
    private Text skillCooltimeTx;
    [SerializeField]
    private GameObject[] formAbleIcons;
    [SerializeField]
    private GameObject[] formActiveIcons;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        pstate = manager.GetComponent<PlayerState>();
    }

    // Update is called once per frame
    void Update()
    {
        int form = pstate.GetForm();
        playerFormIconSlot.sprite = formIcons[form];
        skillSlot.sprite = skillIcons[form];

        for(int i = 0; i < 4; i++)
        {
            formAbleIcons[i].SetActive(pstate.GetIsFormAble(i + 2));
            formActiveIcons[i].SetActive(pstate.GetIsFormActive(i + 2));
        }

        skillActiveImg.gameObject.SetActive(pstate.GetSkillCooltime(form) > 0);
        
        if (skillActiveImg.gameObject.activeSelf) {
            skillActiveImg.fillAmount = (pstate.GetSkillCooltime(form) / pstate.GetSkillMaxCooltime(form));
            skillCooltimeTx.text = Mathf.CeilToInt(pstate.GetSkillCooltime(form)).ToString();
        }

        HPbar.maxValue = pstate.GetMaxHP();
        HPbar.value = pstate.GetHP();
        maxStaminaIcon.fillAmount = pstate.GetMaxStemina() / 30f;
        staminaIcon.fillAmount = pstate.GetStemina() / 30f;

        RectTransform rf = stateCase.GetComponent<RectTransform>();
        int addLength = (int)(Mathf.Clamp(pstate.GetMaxHP() - 100, 0, 300) * 0.45f);
        rf.sizeDelta = new Vector2(90 + addLength, 39);
        rf.anchoredPosition = new Vector3(51 + addLength * 0.5f, 25f, 0f);
        rf = HPbar.GetComponent<RectTransform>();
        rf.sizeDelta = new Vector2(49 + addLength, 7);
    }
}
