using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class itmebtn : MonoBehaviour
{
    public bool issetting;
    public enum btnsetting {
        sellingbtn,
        buyingbtn,
        usingbtn,
    }

    public btnsetting bst;

    public int itemcode;
    public int order;

    public Image btnimg;
    public Text itemnametx;
    public Text iteminfotx;
    public PurchaseEvent pce;
    private InventorySystem inven;
    public inventory inventory_on;

    private ItemData id;

    // Start is called before the first frame update
    void Start()
    {
        id = GameObject.Find("ItemDatas").GetComponent<ItemData>();
        inven = GameObject.Find("GameManager").GetComponent<InventorySystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (itemcode != 0 && !issetting)
        {
            btnimg.sprite = id.data[itemcode].itemicon;
            iteminfotx.text = id.data[itemcode].iteminfo;
            itemnametx.text = id.data[itemcode].itemname;
            if(bst == btnsetting.sellingbtn)
                iteminfotx.text += "|가격 : " + id.data[itemcode].selling;
            else if(bst == btnsetting.buyingbtn)
                iteminfotx.text += "|가격 : " + id.data[itemcode].selling * 2;

            if (bst == btnsetting.sellingbtn)
                GetComponent<Button>().onClick.AddListener(delegate { pce.sellingbtnpush(order); });
            else if (bst == btnsetting.buyingbtn)
                GetComponent<Button>().onClick.AddListener(delegate { pce.buyingbtnpush(order); });
            else
            {
                GetComponent<Button>().onClick.AddListener(delegate { inven.usingItemSetting(order); });
                GetComponent<Button>().onClick.AddListener(delegate { inventory_on.itemselect(order); });
            }

            issetting = true;
        }        
    }
}
