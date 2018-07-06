using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventory : MonoBehaviour
{
    private List<ItemData> ownedItems = new List<ItemData>();
    public List<ItemData> GetItemDatas() { return ownedItems; }

    public int MaxAmountOfOneItem = 20;

    public void AddItem(ItemData data)
    {
        ItemData itemD = GetItemFromID(data.ID);
        if (itemD == null)
        {
            ownedItems.Add(data);
        }
        else if (itemD.numberOwned < MaxAmountOfOneItem)
        {
            itemD.AnotherOne();
        }
        else
        {
            //TO DO screen to show that they have too many, Possibly offer a recycle option???
        }
    }

    public ItemData GetItemFromID(int ID)
    {
        foreach (ItemData data in ownedItems)
        {
            if (data.ID == ID) return data;
        }
        return null;
    }
}
