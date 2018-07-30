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

    public TracksInventory TrackInv { set; get; }

    public float EXP;

    public int PocketCoins = 0;

    public int HasCostumePack = 0;

    public int HasAR = 0;

	public bool CanAR = false;

    public int IsFirstLogIn = 1;

	public CharacterStyleData charStyleData = new CharacterStyleData();

    public GameData()
    {
        DistanceTravelled = 0.0f;
        Username = "None";
        EXP = 0;
        ID = "None";
        Inventory = new PocketPalInventory();
        ItemInv = new ItemInventory();
        TrackInv = new TracksInventory();
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
        int b4 = GetLevel();
        EXP += delta;
        int After = GetLevel();

        PopupHandler.Instance.AddPopup(Mathf.RoundToInt(delta), (b4<After));
    }

    public int GetLevel()
    {
        return LevelCalculator.CalculateLevel(EXP, 2);
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
		
	public void SaveCharacterStyle(CharacterStyleData csd)
	{
		charStyleData = csd;
		ServerDataManager.Instance.UpdateCharacterStyleData (csd);
	}

    public bool TryUseCoins(int Quantity)
    {
        if (PocketCoins >= Quantity)
        {
            PocketCoins -= Quantity;
            ServerDataManager.Instance.WriteCoins(this);
            return true;
        }
        return false;
    }

	public bool UseBerry ()
    {
        if (ItemInv.UseItemWithID(GlobalVariables.BerryID))
        {
            return true;
        }
        return false;
    }

    public void ScanInventoryForBadStats()
    {
        foreach (PocketPalData ppd in Inventory.GetMyPocketPals())
        {
            PocketPalParent ppp = AssetManager.Instance.GetPocketPalFromID(ppd.ID).GetComponent<PocketPalParent>();
            ppd.weight = ppp.CheckNewWeight(ppd.weight);
            ppd.length = ppp.CheckNewLength(ppd.length);
        }
    }

}
