using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class InventoryItem
{
    public int itemCode;
    public int amount;
}

public class PlayerState : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;
    public float maxStamina = 100f;
    public float currentStamina;
    public int power = 10;

    public bool attackActive;
    public bool[] skillActive;
    public int selectedSkill;

    [HideInInspector]
    public Transform playerTransform; 

    public int quickSlotItemCode = -1;

    public List<InventoryItem> inventory = new List<InventoryItem>();

    void Awake()
    {
        InitializeState();
    }

    void Update()
    {
        EnforceItemCapacity();
    }

    private void EnforceItemCapacity()
    {
        if (ItemList.Instance == null) return;

        foreach (var item in inventory)
        {
            ItemData data = ItemList.Instance.GetItemData(item.itemCode);
            if (data != null && item.amount > data.maxCapacity)
            {
                item.amount = data.maxCapacity;
            }
        }
    }
    public void InitializeState()
    {
        currentHP = maxHP;
        currentStamina = maxStamina;

        attackActive = false;
        skillActive = new bool[3];
        selectedSkill = 0;

        quickSlotItemCode = -1; 
        inventory.Clear();
        InventoryItem ii = new InventoryItem();
        ii.itemCode = 0;
        ii.amount = 10;
        inventory.Add(ii);
    }

    public void TakeDamage(int damage, GameObject effectPrefab = null)
    {
        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;

        if (effectPrefab != null && playerTransform != null)
        {
            Vector3 centerPos = playerTransform.position + Vector3.up * 1.0f;
            Vector3 randomPos = centerPos + Random.insideUnitSphere * 1.25f;

            GameObject effect = Instantiate(effectPrefab, randomPos, Quaternion.identity);
            Destroy(effect, 2f);
        }
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;
    }

    public void AddItem(int code, int amount)
    {
        InventoryItem item = inventory.Find(x => x.itemCode == code);
        if (item != null)
        {
            item.amount += amount;
        }
        else
        {
            inventory.Add(new InventoryItem { itemCode = code, amount = amount });
        }
    }
    public void RemoveItem(int code, int amount)
    {
        InventoryItem item = inventory.Find(x => x.itemCode == code);
        if (item != null)
        {
            item.amount -= amount;
            if (item.amount <= 0)
            {
                inventory.Remove(item);
            }
        }
        CheckQuickSlotItem(); 
    }
    public void CheckQuickSlotItem()
    {
        if (quickSlotItemCode == -1) return;

        InventoryItem item = inventory.Find(x => x.itemCode == quickSlotItemCode);
        if (item == null || item.amount <= 0)
        {
            quickSlotItemCode = -1;
        }
    }
}