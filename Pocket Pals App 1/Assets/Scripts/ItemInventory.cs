using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventory
{
    private List<ItemData> ownedItems = new List<ItemData>();
    public List<ItemData> GetItemDatas() { return ownedItems; }

    public int MaxAmountOfOneItem = 10;

    public void Init()
    {
        foreach (ItemData id in AssetManager.Instance.Items)
        {
            ownedItems.Add(id.CloneWithNumber(0));
        }
    }

    public ItemData AddItem(ItemData data)
    {
        ItemData defaultData = GetItemFromID(data.ID);
        if (defaultData != null)
        {
             defaultData.numberOwned += data.numberOwned;
            if (defaultData.numberOwned > MaxAmountOfOneItem) defaultData.numberOwned = MaxAmountOfOneItem;
            return defaultData;
        }
        else { Debug.Log("Something went really wrong adding an item"); return null; }
    }

    public ItemData GetItemFromID(int ID)
    {
        foreach (ItemData data in ownedItems)
        {
            if (data.ID == ID) return data;
        }
        return null;
    }

    public int GetNumberOfItem(int ID)
    {
        if (GetItemFromID(ID) != null)
        {
            return GetItemFromID(ID).numberOwned;
        }
        else return 0;
    }

    public bool UseItemWithID(int ID)
    {
        if (GetItemFromID(ID) != null)
        {
            return GetItemFromID(ID).UseItem();
        }
        else return false;
    }
}
