using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class inventory : MonoBehaviour
{
    public Transform invenContents;
    public GameObject usingbtn;
    private ItemData id;
    private InventorySystem inven;

    public Text selectedTx;
    public bool isonbtn = false;

    // Start is called before the first frame update
    void Start()
    {
        id = GameObject.Find("ItemDatas").GetComponent<ItemData>();
        inven = GameObject.Find("GameManager").GetComponent<InventorySystem>();
        inven.inventory_on = this;
        inven.itemselected = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isonbtn)
        {
            for (int i = 0; i < invenContents.childCount; i++)
            {
                Destroy(invenContents.GetChild(i).gameObject);
            }
            int j = 0;
            foreach (int i in inven.itemcodes)
            {
                GameObject g = Instantiate(usingbtn);
                g.transform.SetParent(invenContents);
                g.GetComponent<itmebtn>().inventory_on = this;
                g.GetComponent<itmebtn>().itemcode = i;
                g.GetComponent<itmebtn>().order = j;
                j++;
            }
            selectedTx.text = "";
            isonbtn = true;
        }
    }

    public void itemselect(int order) {        
        selectedTx.text = id.data[invenContents.GetChild(order).GetComponent<itmebtn>().itemcode].itemname + " | " + id.data[invenContents.GetChild(order).GetComponent<itmebtn>().itemcode].iteminfo;
    }

    public void itemusingbtn() {
        if (inven.itemcodes == null || !inven.itemselected)
            selectedTx.text = "선택된 아이템이 없습니다.";
        else
        {
            selectedTx.text = "";
            inven.usingItem();
        }
    }
}
