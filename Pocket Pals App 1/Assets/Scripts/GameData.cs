[System.Serializable]
public class GameData
{
    public float DistanceTravelled { set; get; }
    public string Username { set; get; }
    public PocketPalInventory Inventory { set; get; }
    public float EXP { set; get; }
    public GameData()
    {
        DistanceTravelled = 0.0f;
        Username = "None";
        EXP = 0;
        Inventory = new PocketPalInventory();
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
