using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public string ID;

    public string Username;

    public float DistanceTravelled;

    public PocketPalInventory Inventory { set; get; }

    public ItemInventory ItemInv { set; get; }

    public float EXP;

	// The number of berries in the inventory
	int Berries;

    public GameData()
    {
        DistanceTravelled = 0.0f;
        Username = "None";
        EXP = 0;
        ID = "None";
        Inventory = new PocketPalInventory();
        ItemInv = new ItemInventory();
    }

    public string GetJson()
    {
        return JsonUtility.ToJson(this);
    }

    public string GetInventoryJson()
    {
        return Inventory.GetInventoryJson();
    }

    public void IncreaseExp(float delta)
    {
        EXP += delta;
    }

    public int GetLevel()
    {
        return LevelCalculator.CalculateLevel(EXP);
    }

    public float GetExpToLevel()
    {
        return LevelCalculator.GetExpNeeded(EXP);
    }

    public float GetPercentageExp()
    {
        return LevelCalculator.GetPercentageToNextLevel(EXP);
    }

	public int NumberOfBerries ()
    {
        return ItemInv.GetItemFromID(GlobalVariables.BerryID).numberOwned;

	}

	public bool UseBerry ()
    {
        if (ItemInv.UseItemWithID(GlobalVariables.BerryID))
        {
            ServerDataManager.Instance.WriteItem(this, ItemInv.GetItemFromID(GlobalVariables.BerryID));
            return true;
        }
        return false;
    }
}
