using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private PlayerState state;
    private SystemSetting setting;
    private CodeSetting codes;

    [Header("UI")]
    [SerializeField]
    private CanvasGroup mainUI;
    [SerializeField]
    private CanvasGroup menuUI;
    [SerializeField]
    private CanvasGroup settingUI;

    [Header("DefaultUI")]
    [SerializeField]
    private Text Slotnum;
    [SerializeField]
    private bool CanPause = true;

    [Header("mainUI")]
    [SerializeField]
    private Image hpBar;
    [SerializeField]
    private Text hpItemCount;
    [SerializeField]
    private Text buffItemCount;
    [SerializeField]
    private Text MoneyCount;

    [Header("menuUI")]
    [SerializeField]
    private Slider senSlide;
    [SerializeField]
    private Slider mVolSlide;
    [SerializeField]
    private Slider bVolSlide;
    [SerializeField]
    private Slider sVolSlide;

    [Header("codeSetUI")]
    [SerializeField]
    private GameObject[] blocks;
    [SerializeField]
    private GameObject ifBlock;
    [SerializeField]
    private GameObject whileBlock;
    [SerializeField]
    private GameObject slotFull;
    [SerializeField]
    private Transform Contane;
    [SerializeField]
    private Text blockLimitTx;
    [SerializeField]
    private Transform CodeBlockSettingUI;
    [SerializeField]
    private Transform[] CodeBlockSettingUIs;

    [SerializeField] private int focusingIndex = -1;
    private Color focusBeforeColor;

    private bool onMenu = false;
    private bool onSetting = false;
    [SerializeField] private bool isFullon = false;
    // Start is called before the first frame update
    void Awake()
    {
        state = GameObject.Find("GameManager").GetComponent<PlayerState>();
        setting = GameObject.Find("GameManager").GetComponent<SystemSetting>();
        codes = GameObject.Find("GameManager").GetComponent<CodeSetting>();
        senSlide.value = setting.GetSensitvity();
        mVolSlide.value = setting.GetVoleum();
        bVolSlide.value = setting.GetBGMVoleum();
        sVolSlide.value = setting.GetSEVoleum();
        CloseCodeSetUI();
    }

    // Update is called once per frame
    void Update()
    {
        hpItemCount.text = "x" + state.GetHpitem().ToString();
        buffItemCount.text = "x" + state.GetBuffitem().ToString();
        MoneyCount.text = "x" + state.GetMoney().ToString();

        Slotnum.text = "SLOT " + state.GetNowslot().ToString();

        setting.SetSensitvity(senSlide.value);
        setting.SetVoleum(mVolSlide.value);
        setting.SetBGMVoleum(bVolSlide.value);
        setting.SetSEVoleum(sVolSlide.value);

        
        if (!onMenu && !onSetting)
        {
            Time.timeScale = 1;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (onMenu)
        {
            menuUI.gameObject.SetActive(true);
            settingUI.gameObject.SetActive(false);
            mainUI.gameObject.SetActive(false);
        }
        else if (onSetting)
        {
            menuUI.gameObject.SetActive(false);
            settingUI.gameObject.SetActive(true);
            mainUI.gameObject.SetActive(false);
        }
        else {
            menuUI.gameObject.SetActive(false);
            settingUI.gameObject.SetActive(false);
            mainUI.gameObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && CanPause)
        {
            onMenu = true;
        }

        if (Input.GetMouseButtonDown(1) && !onMenu && CanPause)
        {
            onSetting = true;
        }

        hpBar.fillAmount = (float)state.GetHP() / (float)state.GetMaxHP();
        slotFull.SetActive(isFullon);
    }

    public void Continue() {
        onMenu = false;
    }

    public void ToTitle()
    {
        LoadingSceneManager.LoadScene("Title");
    }


    public void SettingFinish()
    {
        onSetting = false;
    }

    public void CodeBlockShow() {
        if (Contane.childCount > 0)
        {
            foreach (var t in Contane.GetComponentsInChildren<Transform>())
            {
                if(t != Contane)Destroy(t.gameObject);
            }
        }
        List<CodeBlocks> ls = codes.GetSlot(state.GetNowslot()).ToList();
        blockLimitTx.text = "LIMIT\n" + codes.GetSlotCount(state.GetNowslot()) + "/" + codes.limit;
        int i;
        float beforelen = 0;
        if (ls.Count > 0)
        {
            for (i = 0; i < ls.Count; i++)
            {
                int index = 0;
                int j = 0;
                beforelen = 0;
                for (j = 0; j < i; j++) {
                    if (ls[j].kind != Codekinds.ifcode && ls[j].kind != Codekinds.whilecode)
                    {
                        beforelen += 250;
                    }
                    else {
                        beforelen += 140;
                        foreach (CodeBlocks w in ls[j].child)
                        {
                            beforelen += 250;
                        }
                    }
                }
                switch (ls[i].kind)
                {
                    case Codekinds.damamge:
                        index = 0;
                        break;
                    case Codekinds.boom:
                        index = 1;
                        break;
                    case Codekinds.speed:
                        index = 2;
                        break;
                    case Codekinds.delay:
                        index = 3;
                        break;
                    case Codekinds.turn:
                        index = 4;
                        break;
                    case Codekinds.value:
                        index = 5;
                        break;
                    case Codekinds.vaccine:
                        index = 6;
                        break;
                    case Codekinds.whilecode:
                        index = 99;
                        break;
                    case Codekinds.ifcode:
                        index = 100;
                        break;
                    default:
                        return;
                }
                if (index < 10)
                {
                    RectTransform rt = Instantiate(blocks[index], Contane).GetComponent<RectTransform>();
                    rt.position = rt.position + Vector3.right * beforelen - Vector3.right * 200;
                    rt.GetComponent<CodeBlockButton>().index = i;
                    rt.GetComponent<CodeBlockButton>().parantindex = -1;
                    rt.GetComponent<CodeBlockButton>().kind = index;
                    rt.GetComponent<CodeBlockButton>().AddListonerToBtn();
                }
                else if (index == 99)
                {
                    RectTransform rt = Instantiate(whileBlock, Contane).GetComponent<RectTransform>();
                    rt.position = rt.position + Vector3.right * beforelen - Vector3.right * 200;
                    rt.GetComponent<CodeBlockButton>().index = i;
                    rt.GetComponent<CodeBlockButton>().parantindex = -1;
                    rt.GetComponent<CodeBlockButton>().kind = index;
                    rt.GetComponent<CodeBlockButton>().AddListonerToBtn();
                    rt.GetComponent<CodeBlockButton>().AddFocusListonerToBtn();
                    for (j = 0; j < ls[i].child.Count; j++)
                    {
                        switch (ls[i].child[j].kind)
                        {
                            case Codekinds.damamge:
                                index = 0;
                                break;
                            case Codekinds.boom:
                                index = 1;
                                break;
                            case Codekinds.speed:
                                index = 2;
                                break;
                            case Codekinds.delay:
                                index = 3;
                                break;
                            case Codekinds.turn:
                                index = 4;
                                break;
                            case Codekinds.value:
                                index = 5;
                                break;
                            case Codekinds.vaccine:
                                index = 6;
                                break;
                            case Codekinds.whilecode:
                                index = 99;
                                break;
                            case Codekinds.ifcode:
                                index = 100;
                                break;
                            default:
                                return;
                        }
                        RectTransform rt2 = Instantiate(blocks[index], rt.GetChild(1)).GetComponent<RectTransform>();
                        rt2.GetComponent<CodeBlockButton>().index = j;
                        rt2.GetComponent<CodeBlockButton>().parantindex = i;
                        rt2.GetComponent<CodeBlockButton>().kind = index;
                        rt2.GetComponent<CodeBlockButton>().AddListonerToBtn();
                    }
                    rt.sizeDelta = new Vector2(729.555f + (ls[i].child.Count > 0 ? (290 + (ls[i].child.Count - 1) * 500) : 0), rt.sizeDelta.y);
                    if (i == focusingIndex) rt.GetComponent<Image>().color = Color.yellow;
                }
                else if (index == 100)
                {
                    RectTransform rt = Instantiate(ifBlock, Contane).GetComponent<RectTransform>();
                    rt.position = rt.position + Vector3.right * beforelen - Vector3.right * 200;
                    rt.GetComponent<CodeBlockButton>().index = i;
                    rt.GetComponent<CodeBlockButton>().parantindex = -1;
                    rt.GetComponent<CodeBlockButton>().kind = index;
                    rt.GetComponent<CodeBlockButton>().AddListonerToBtn();
                    rt.GetComponent<CodeBlockButton>().AddFocusListonerToBtn();
                    for (j = 0; j < ls[i].child.Count; j++)
                    {
                        switch (ls[i].child[j].kind)
                        {
                            case Codekinds.damamge:
                                index = 0;
                                break;
                            case Codekinds.boom:
                                index = 1;
                                break;
                            case Codekinds.speed:
                                index = 2;
                                break;
                            case Codekinds.delay:
                                index = 3;
                                break;
                            case Codekinds.turn:
                                index = 4;
                                break;
                            case Codekinds.value:
                                index = 5;
                                break;
                            case Codekinds.vaccine:
                                index = 6;
                                break;
                            case Codekinds.whilecode:
                                index = 99;
                                break;
                            case Codekinds.ifcode:
                                index = 100;
                                break;
                            default:
                                return;
                        }
                        RectTransform rt2 = Instantiate(blocks[index], rt.GetChild(1)).GetComponent<RectTransform>();
                        rt2.position = rt.position + Vector3.right * i * 120;
                        rt2.GetComponent<CodeBlockButton>().index = j;
                        rt2.GetComponent<CodeBlockButton>().parantindex = i;
                        rt2.GetComponent<CodeBlockButton>().kind = index;
                        rt2.GetComponent<CodeBlockButton>().AddListonerToBtn();
                    }
                    rt.sizeDelta = new Vector2(729.555f + (ls[i].child.Count > 0 ? (290 + (ls[i].child.Count - 1) * 500) : 0), rt.sizeDelta.y);
                    if(i == focusingIndex) rt.GetComponent<Image>().color = Color.yellow;
                }              
            }
        }
        Contane.GetComponent<RectTransform>().sizeDelta = new Vector2(550 + beforelen * 0.9f, Contane.GetComponent<RectTransform>().sizeDelta.y);
        for (i = 0; i < Contane.childCount; i++) Contane.GetChild(i).GetComponent<RectTransform>().position = Contane.GetChild(i).GetComponent<RectTransform>().position - Vector3.right * beforelen * 0.225f;
        
    }

    public void CodeBlockAdd(int kind) 
    {
        if(codes.GetSlotCount(state.GetNowslot()) >= codes.limit)
        {
            if (!isFullon) StartCoroutine("slotFullMessage");
            return;
        }
        
        List<CodeBlocks> ls;
        if (focusingIndex == -1)
            ls = codes.GetSlot(state.GetNowslot());
        else
        {
            ls = codes.GetSlot(state.GetNowslot())[focusingIndex].child;
            if (kind > 10) return;
        }

        CodeBlocks cd = new CodeBlocks();
        cd.values = new string[2];
        cd.nums = new float[2];
        cd.checks = 0;
        switch (kind) {
            case 0:
                cd.kind = Codekinds.damamge;
                break;
            case 1:
                cd.kind = Codekinds.boom;
                break;
            case 2:
                cd.kind = Codekinds.speed;
                break;
            case 3:
                cd.kind = Codekinds.delay;
                break;
            case 4:
                cd.kind = Codekinds.turn;
                break;
            case 5:
                cd.kind = Codekinds.value;
                break;
            case 6:
                cd.kind = Codekinds.vaccine;
                break;
            case 99:
                cd.kind = Codekinds.whilecode;
                cd.child = new List<CodeBlocks>();
                break;
            case 100:
                cd.kind = Codekinds.ifcode;
                cd.child = new List<CodeBlocks>();
                break;
            default:
                return;
        }
        if (ls.Count > 0)
        {
            if ((ls[ls.Count - 1].kind == Codekinds.ifcode || ls[ls.Count - 1].kind == Codekinds.whilecode) && ls[ls.Count- 1].child.Count == 0) {
                ls.RemoveAt(ls.Count - 1);
            }
        }
        ls.Add(cd);
        if (focusingIndex == -1)
            codes.SetSlot(state.GetNowslot(), ls);    
        else
        {

            List<CodeBlocks> ls2 = codes.GetSlot(state.GetNowslot());
            CodeBlocks tmp = ls2[focusingIndex];
            tmp.child = ls;
            ls2[focusingIndex] = tmp;
            codes.SetSlot(state.GetNowslot(), ls2);
        }
        CodeBlockShow();
    }

    IEnumerator slotFullMessage() {
        isFullon = true;
        yield return new WaitForSecondsRealtime(1f);
        isFullon = false;
    }

    public void CodeBlockClick(Vector3Int indexInfo)
    {
        List<CodeBlocks> ls = codes.GetSlot(state.GetNowslot());
        CodeBlockSettingUI.gameObject.SetActive(true);
        int kind = indexInfo.z;
        if (kind == 99) kind = 7;
        else if (kind == 100) kind = 8;

        for (int i = 0; i < CodeBlockSettingUIs.Length; i++) {
            if (i == kind) CodeBlockSettingUIs[i].gameObject.SetActive(true);
            else CodeBlockSettingUIs[i].gameObject.SetActive(false);
        }
        CodeBlockSettingUIs[kind].GetComponent<CodeSetUI>().SetIndexInfo(indexInfo);
    }

    public void FocusChange(Vector3Int indexInfo)
    {
        if (focusingIndex == -1)
        {
            focusingIndex = indexInfo.y;
            focusBeforeColor = Contane.GetChild(focusingIndex).GetComponent<Image>().color;
            Contane.GetChild(focusingIndex).GetComponent<Image>().color = Color.yellow;
        }
        else if(focusingIndex != indexInfo.y)
        {            
            Contane.GetChild(focusingIndex).GetComponent<Image>().color = focusBeforeColor;
            focusingIndex = indexInfo.y;
            focusBeforeColor = Contane.GetChild(focusingIndex).GetComponent<Image>().color;
            Contane.GetChild(focusingIndex).GetComponent<Image>().color = Color.yellow;
        }
        else
        {           
            Contane.GetChild(focusingIndex).GetComponent<Image>().color = focusBeforeColor;
            focusingIndex = -1;
        }

    }

    public void ChangeCode(Vector3Int indexInfo) 
    {
       
        CloseCodeSetUI();
        List<CodeBlocks> ls = codes.GetSlot(state.GetNowslot());
        CodeBlocks cd;
        if (indexInfo.x == -1) cd = ls[indexInfo.y];
        else cd = ls[indexInfo.x].child[indexInfo.y];
        cd.values = new string[2];
        cd.nums = new float[2];
        cd.checks = 0;
        cd.child = new List<CodeBlocks>();
        switch (indexInfo.z)
        {
            case 0:
                cd.kind = Codekinds.damamge;                
                break;
            case 1:
                cd.kind = Codekinds.boom;
                break;
            case 2:
                cd.kind = Codekinds.speed;
                break;
            case 3:
                cd.kind = Codekinds.delay;
                break;
            case 4:
                cd.kind = Codekinds.turn;
                break;
            case 5:
                cd.kind = Codekinds.value;
                break;
            case 6:
                cd.kind = Codekinds.vaccine;
                break;
            default:
                return;
        }

        
        if (indexInfo.x == -1) ls[indexInfo.y] = cd;
        else
        {
            CodeBlocks cds = ls[indexInfo.x];
            cds.child[indexInfo.y] = cd;
            ls[indexInfo.x] = cds;
        }

        codes.SetSlot(state.GetNowslot(), ls);
        CodeBlockShow();     
    }

    public void DeleteCode(Vector3Int indexInfo)
    {
        CloseCodeSetUI();
        List<CodeBlocks> ls = codes.GetSlot(state.GetNowslot());

        if (indexInfo.x == -1)
        {
            ls.RemoveAt(indexInfo.y);
        }
        else
        {
            ls[indexInfo.x].child.RemoveAt(indexInfo.y);
            if((ls[indexInfo.x].kind == Codekinds.ifcode || ls[indexInfo.x].kind == Codekinds.whilecode) && ls[indexInfo.x].child.Count == 0) ls.RemoveAt(indexInfo.x);
        }       
        codes.SetSlot(state.GetNowslot(), ls);
        focusingIndex = -1;
        CodeBlockShow();
    }

    public void CloseCodeSetUI()
    {
        CodeBlockSettingUI.gameObject.SetActive(false);
        for (int i = 0; i < CodeBlockSettingUIs.Length; i++) CodeBlockSettingUIs[i].gameObject.SetActive(false);        
    }
}
