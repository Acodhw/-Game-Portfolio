using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ItemData
{
    [Header("Item Basic Info")]
    [Tooltip("아이템의 고유 번호 (PlayerState의 inventory와 매칭됨)")]
    public int itemCode;

    [Tooltip("아이템의 이름")]
    public string itemName;

    [Tooltip("인벤토리 UI에 표시될 아이템 설명")]
    [TextArea(2, 4)]
    public string itemDescription;

    [Tooltip("인벤토리에 표시될 아이템 아이콘")]
    public Sprite itemIcon;

    [Header("Item Properties")]
    [Tooltip("사용 가능한 아이템 여부")]
    public bool canUse = true;

    [Tooltip("버리기 가능한 아이템 여부")]
    public bool canDrop = true;

    [Tooltip("인벤토리에 최대로 소지할 수 있는 개수 한계")]
    public int maxCapacity = 99;

    [Header("Item Effect")]
    [Tooltip("아이템 사용 함수(체력 회복, 버프 등)")]
    public UnityEvent onUseEffect;
}

public class ItemList : MonoBehaviour
{
    public static ItemList Instance { get; private set; }

    [Header("Item Database")]
    [Tooltip("게임 아이템 데이터")]
    public List<ItemData> items = new List<ItemData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public ItemData GetItemData(int code)
    {
        return items.Find(x => x.itemCode == code);
    }
}