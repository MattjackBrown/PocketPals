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
            ownedItems.Add(id);
        }
    }

    public void AddItem(ItemData data)
    {
        if (GetItemFromID(data.ID) != null)
        {
            if (GetItemFromID(data.ID).numberOwned >= MaxAmountOfOneItem) return;
            GetItemFromID(data.ID).numberOwned += data.numberOwned;
        }
        else { Debug.Log("Something went really wrong adding an item"); }
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
}
