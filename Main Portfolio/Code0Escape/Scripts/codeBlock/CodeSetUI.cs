using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodeSetUI : MonoBehaviour
{
    private PlayerState state;
    private CodeSetting codes;

    [SerializeField] private InputField[] valueField;
    [SerializeField] private Dropdown checker;
    private Vector3Int indexinfo;
    private UIManager uimanager;
    // Start is called before the first frame update
    void Start()
    {
        state = GameObject.Find("GameManager").GetComponent<PlayerState>();
        codes = GameObject.Find("GameManager").GetComponent<CodeSetting>();
        uimanager = transform.root.GetChild(0).GetComponent<UIManager>();
    }

    public void SetIndexInfo(Vector3Int info) {
        indexinfo = info;
        state = GameObject.Find("GameManager").GetComponent<PlayerState>();
        codes = GameObject.Find("GameManager").GetComponent<CodeSetting>();
        List<CodeBlocks> ls = codes.GetSlot(state.GetNowslot());
        CodeBlocks cd;       
        if (info.x == -1) cd = ls[info.y];
        else cd = ls[info.x].child[info.y];

        
        switch (valueField.Length)
        {
            case 1:
                if (cd.values[0] != null)
                    valueField[0].text = cd.values[0];
                else
                    valueField[0].text = cd.nums[0].ToString();
                break;
            case 2:
                if (cd.values[0] != null)
                    valueField[0].text = cd.values[0];
                else
                    valueField[0].text = cd.nums[0].ToString();

                if (cd.values[1] != null)
                    valueField[1].text = cd.values[1];
                else
                    valueField[1].text = cd.nums[1].ToString();
                checker.value = cd.checks;
                break;
            default:
                break;
        }
    }


    public void ChangeButton(int kind)
    {
        uimanager.ChangeCode(new Vector3Int(indexinfo.x, indexinfo.y, kind));
    }

    public void DeleteButton()
    {
        uimanager.DeleteCode(indexinfo);
    }

    public void FinishButton()
    {
        List<CodeBlocks> ls = codes.GetSlot(state.GetNowslot());
        CodeBlocks cd;
        if (indexinfo.x == -1) cd = ls[indexinfo.y];    
        else cd = ls[indexinfo.x].child[indexinfo.y];
        float a;
        
        switch (valueField.Length) {
            case 1:
                if (float.TryParse(valueField[0].text, out a))
                {
                    cd.nums[0] = a;
                    cd.values[0] = null;
                }
                else if (valueField[0].text == "")
                {
                    cd.nums[0] = 0;
                    cd.values[0] = null;
                }
                else
                    cd.values[0] = valueField[0].text;
                break;
            case 2:
                if (float.TryParse(valueField[0].text, out a))
                {
                    cd.nums[0] = a;
                    cd.values[0] = null;
                }
                else if (valueField[0].text == "")
                {
                    cd.nums[0] = 0;
                    cd.values[0] = null;
                }
                else
                    cd.values[0] = valueField[0].text;

                if (float.TryParse(valueField[1].text, out a))
                {
                    cd.nums[1] = a;
                    cd.values[1] = null;
                }
                else if (valueField[1].text == "")
                {
                    cd.nums[1] = 0;
                    cd.values[1] = null;
                }
                else
                    cd.values[1] = valueField[1].text;

                cd.checks = checker.value;
                break;
            default:
                break;
        }
        if (indexinfo.x == -1) ls[indexinfo.y] = cd;
        else
        {
            CodeBlocks cds = ls[indexinfo.x];
            cds.child[indexinfo.y] = cd;
            ls[indexinfo.x] = cds;
        }
        codes.SetSlot(state.GetNowslot(), ls);
        uimanager.CloseCodeSetUI();
    }

    public void FinishValueSettingButton()
    {
        List<CodeBlocks> ls = codes.GetSlot(state.GetNowslot());
        CodeBlocks cd;
        if (indexinfo.x == -1) cd = ls[indexinfo.y];
        else cd = ls[indexinfo.x].child[indexinfo.y];
        float a;


        cd.values[0] = valueField[0].text;

        if (float.TryParse(valueField[1].text, out a))
        {
            cd.values[1] = null;
            cd.nums[1] = a;
        }
        else if (valueField[1].text == "")
        {
            cd.values[1] = null;
            cd.nums[1] = 0;
        }
        else
            cd.values[1] = valueField[1].text;

        cd.checks = checker.value;
        

        if (indexinfo.x == -1) ls[indexinfo.y] = cd;
        else
        {
            CodeBlocks cds = ls[indexinfo.x];
            cds.child[indexinfo.y] = cd;
            ls[indexinfo.x] = cds;
        }
        codes.SetSlot(state.GetNowslot(), ls);
        uimanager.CloseCodeSetUI();
    }
}
