using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodeBlockButton : MonoBehaviour
{
    public int parantindex = -1;
    public int index = 0;
    public int kind = 0;
    private Button btn;
    private UIManager uimanager;
    // Start is called before the first frame update
    void Awake()
    {
        uimanager = transform.root.GetChild(0).GetComponent<UIManager>();
        btn = GetComponent<Button>();        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddListonerToBtn() {
        while (btn == null || uimanager == null) continue;
        btn.onClick.AddListener(() => uimanager.CodeBlockClick(new Vector3Int(parantindex, index, kind)));
    }

    public void AddFocusListonerToBtn()
    {
        while (btn == null || uimanager == null) continue;
        btn.onClick.AddListener(() => uimanager.FocusChange(new Vector3Int(parantindex, index, kind)));
    }
}
