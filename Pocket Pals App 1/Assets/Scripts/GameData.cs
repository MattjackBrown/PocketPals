[System.Serializable]
public class GameData
{
    public float DistanceTravelled { set; get; }
    public string Username { set; get; }
    public PocketPalInventory Inventory { set; get; }
    public GameData()
    {
        DistanceTravelled = 0.0f;
        Username = "None";
        Inventory = new PocketPalInventory();
    }
}
