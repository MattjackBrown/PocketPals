using UnityEngine;

[System.Serializable]
public class GameData
{
    public float DistanceTravelled;

    public string ID;

    public string Username;
    public PocketPalInventory Inventory { set; get; }
    public float EXP;
    public GameData()
    {
        DistanceTravelled = 0.0f;
        Username = "None";
        EXP = 0;
        ID = "None";
        Inventory = new PocketPalInventory();
    }

    public string GetJson()
    {
        return JsonUtility.ToJson(this);
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
}
