using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum itemcharacterastic
{
    heal,
    attack,
    other,
}

[System.Serializable]
public class Items
{
    public itemcharacterastic ic;
    public Sprite itemicon;
    public string itemname;
    public string iteminfo;
    public int selling;
    public int healgage;
    public int attackcode;
}


public class ItemData : MonoBehaviour
{   
    public Items[] data;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
