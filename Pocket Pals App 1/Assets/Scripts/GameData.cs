[System.Serializable]
public class GameData
{
    public float DistanceTravelled { set; get; }
    public string Username { set; get; }

    public GameData()
    {
        DistanceTravelled = 0.0f;
        Username = "None";
    }
}
