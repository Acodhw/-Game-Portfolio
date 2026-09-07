using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PurchaseEvent : MonoBehaviour
{
    [System.Serializable]
    public class SellingItemData {
        public int itemcode;
        public int purchase;
        public Sprite icon;
    }

    public int[] buyingitemcode;

    public string sellerName;
    public Image illust;
    public Sprite shopperface;
    public GameObject sellingitembtns;
    public GameObject buyingitembtns;
    public Button exitbtn;
    public Button buybtn;
    public Button sellbtn;
    public Transform sellingcontents;
    public Transform buyingcontents;
    public GameObject eventcheck;
    public GameObject purchaseboard;
    public GameObject pl_inter;
    private ItemData id;
    private InventorySystem inven;
    public List<int> sellingitemcode;
    public SellingItemData[] sids;
    private SellingItemData[] sid;

    int settingsell = -1;
    int settingbuy = -1;

    public Text nameboard;

    public Text buyingcheck;
    public Text sellingcheck;

    private bool talking_start = false;
    private bool pushedZ;
    private bool cantalk = true;
    private PlayerControl pc;
    private PlayerState ps;


    void pushz()
    {
        if (Input.GetButton("Check") && !pushedZ)
            StartCoroutine("checkingpushz");
    }

    IEnumerator checkingpushz()
    {
        pushedZ = true;
        yield return new WaitForSeconds(0.005f);
        pushedZ = false;
    }

    IEnumerator cantalkcheck()
    {
        cantalk = false;
        yield return new WaitForSeconds(0.1f);
        cantalk = true;
    }

    // Update is called once per frame
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == pc.tag)
        {
            if (!talking_start)
            {
                eventcheck.SetActive(true);
                if (pushedZ && cantalk && !pc.ispause)
                {
                    talking_start = true;
                    eventcheck.SetActive(false);
                    pl_inter.SetActive(false);
                    purchaseboard.SetActive(true);
                    pc.ispause = true;
                    illust.sprite = shopperface;
                    nameboard.text = sellerName;
                    exitbtn.onClick.RemoveAllListeners();
                    exitbtn.onClick.AddListener(delegate { exitbtnpush(); });
                    buybtn.onClick.AddListener(delegate { buying(); });
                    sellbtn.onClick.AddListener(delegate { selling(); });
                    GameObject g;
                    for (int i = 0; i < buyingcontents.childCount; i++) {
                        Destroy(buyingcontents.GetChild(i).gameObject);
                    }
                    for (int i = 0; i < sellingcontents.childCount; i++)
                    {
                        Destroy(sellingcontents.GetChild(i).gameObject);
                    }
                    for (int i = 0; i < sid.Length; i++) {
                        g = Instantiate(buyingitembtns);
                        g.transform.SetParent(buyingcontents);
                        g.GetComponent<itmebtn>().pce = this;
                        g.GetComponent<itmebtn>().itemcode = sid[i].itemcode;
                        g.GetComponent<itmebtn>().order = i;
                    }
                    int j = 0;
                    foreach (int i in sellingitemcode)
                    {
                        g = Instantiate(sellingitembtns);
                        g.transform.SetParent(sellingcontents);
                        g.GetComponent<itmebtn>().pce = this;
                        g.GetComponent<itmebtn>().itemcode = i;
                        g.GetComponent<itmebtn>().order = j;
                        j++;
                    }                   
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == pc.tag)
        {
            if (eventcheck.activeSelf)
            {
                eventcheck.SetActive(false);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        id = GameObject.Find("ItemDatas").GetComponent<ItemData>();
        inven = GameObject.Find("GameManager").GetComponent<InventorySystem>();
        pc = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
        ps = GameObject.Find("GameManager").GetComponent<PlayerState>();
        for (int i = 0; i < buyingitemcode.Length; i++) {
            sids[i].icon = id.data[buyingitemcode[i]].itemicon;
            sids[i].itemcode = buyingitemcode[i];
            sids[i].purchase = id.data[buyingitemcode[i]].selling * 2;
        }
        sid = new SellingItemData[buyingitemcode.Length];
        for (int i = 0; i < buyingitemcode.Length; i++)
        {
            sid[i] = sids[i];
        }
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
        pushz();
        sellingitemcode = inven.itemcodes;
    }

    public void exitbtnpush()
    {
        talking_start = false;
        pl_inter.SetActive(true);
        purchaseboard.SetActive(false);
        pc.ispause = false;
        StartCoroutine("cantalkcheck");
        
    }

    public void sellingbtnpush(int i)
    {
        settingsell = i;        
        sellingcheck.text = id.data[sellingitemcode[settingsell]].itemname + " | 가격 : " + id.data[sellingitemcode[settingsell]].selling;
    }

    public void buyingbtnpush(int i)
    {
        settingbuy = i;
        buyingcheck.text = id.data[sid[settingbuy].itemcode].itemname + " | 가격 : " + (id.data[sid[settingbuy].itemcode].selling * 2);
    }

    public void buying() {
        if (settingbuy != -1)
        {
            if (ps.GetMoney() >= sid[settingbuy].purchase)
            {
                if (inven.itemcodes.Count < inven.maxInven)
                {
                    ps.SetMoney(ps.GetMoney() - sid[settingbuy].purchase);
                    inven.itemcodes.Add(sid[settingbuy].itemcode);
                    buyingcheck.text = "구매가 완료되었습니다.";
                    for (int i = 0; i < sellingcontents.childCount; i++)
                    {
                        Destroy(sellingcontents.GetChild(i).gameObject);
                    }
                    int j = 0;
                    foreach (int i in sellingitemcode)
                    {
                        GameObject g = Instantiate(sellingitembtns);
                        g.transform.SetParent(sellingcontents);
                        g.GetComponent<itmebtn>().pce = this;
                        g.GetComponent<itmebtn>().itemcode = i;
                        g.GetComponent<itmebtn>().order = j;
                        j++;
                    }
                    inven.inventory_on.isonbtn = false;
                }
                else
                {
                    buyingcheck.text = "가방이 모두 찼습니다.";
                }
            }
            else
            {
                buyingcheck.text = "돈이 부족합니다!";
            }
            settingbuy = -1;
        }
    }

    public void selling()
    {
        if (settingsell != -1)
        {
            inven.itemcodes.RemoveAt(settingsell);
            ps.SetMoney(ps.GetMoney() + id.data[sellingcontents.GetChild(settingsell).GetComponent<itmebtn>().itemcode].selling);
            for (int i = 0; i < sellingcontents.childCount; i++)
            {
                Destroy(sellingcontents.GetChild(i).gameObject);
            }
            int j = 0;
            foreach (int i in sellingitemcode)
            {
                GameObject g = Instantiate(sellingitembtns);
                g.transform.SetParent(sellingcontents);
                g.GetComponent<itmebtn>().pce = this;
                g.GetComponent<itmebtn>().itemcode = i;
                g.GetComponent<itmebtn>().order = j;
                j++;
            }
            sellingcheck.text = "판매가 완료되었습니다";
            settingsell = -1;
            inven.inventory_on.isonbtn = false;
        }
    }
}
