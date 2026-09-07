using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [System.Serializable]
    public class ItemCodeSaver
    {
        public List<int> itemcodes;
    }

    public List<int> itemcodes;

    int usingsetting;
    public readonly int maxInven = 50;
    private ItemData id;
    private PlayerState ps;
    public inventory inventory_on;
    public PlayerControl pc;
    public bool itemselected = false;
    public bool cantuse = false;

    // Start is called before the first frame update
    void Start()
    {       
        id = GameObject.Find("ItemDatas").GetComponent<ItemData>();
        ps = GameObject.Find("GameManager").GetComponent<PlayerState>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void usingItemSetting(int order) {
        usingsetting = order;
        itemselected = true;
    }

    public void usingItem()
    {
        if (id.data[inventory_on.invenContents.GetChild(usingsetting).GetComponent<itmebtn>().itemcode].ic == itemcharacterastic.attack)
        {
            itemcodes.RemoveAt(usingsetting);
            pc.attackItem(id.data[inventory_on.invenContents.GetChild(usingsetting).GetComponent<itmebtn>().itemcode].attackcode);
            inventory_on.isonbtn = false;
        }
        else if (id.data[inventory_on.invenContents.GetChild(usingsetting).GetComponent<itmebtn>().itemcode].ic == itemcharacterastic.heal)
        {
            if (ps.getHP() == ps.getMaxHP())
            {
                inventory_on.selectedTx.text = "이미 체력이 모두 회복되어있습니다.";
            }
            else
            {
                itemcodes.RemoveAt(usingsetting);
                ps.heal(id.data[inventory_on.invenContents.GetChild(usingsetting).GetComponent<itmebtn>().itemcode].healgage);
                inventory_on.isonbtn = false;
            }
        }
        else
        {
            return;
        }
        
    }

    public void SaveItem(int fileNum)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/maplemaple" + fileNum + ".dat");
        ItemCodeSaver sv = new ItemCodeSaver();
        sv.itemcodes = itemcodes;
        bf.Serialize(file, sv);
        file.Close();
    }

    public void LoadItem(int fileNum)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/maplemaple" + fileNum + ".dat", FileMode.Open);
        if (file != null && file.Length > 0)
        {
            ItemCodeSaver sv = (ItemCodeSaver)bf.Deserialize(file);
            itemcodes = sv.itemcodes;
        }
        file.Close();
    }

    public void Reset()
    {
        itemcodes = new List<int>();
    }
}
