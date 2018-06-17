using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class LocalDataManager : MonoBehaviour {

    //used to return an instance of a local data manager
    public static LocalDataManager Instance{set; get;}

    private string dataFileName = "/data.dat";
    private string destination;
    private GameData localData;

	// Use this for initialization
	void Start ()
    {
        //This makes the local datamanager accessible from anywhere
        Instance = this;

        localData = new GameData();

        DontDestroyOnLoad(gameObject);
    }

    private void Awake()
    {
        destination = Application.persistentDataPath + dataFileName;
    }

    public void UpdateName(string newName)
    {
        localData.Username = newName;
        ServerDataManager.Instance.UpdatePlayerName(localData);
    }

   public void UpdateDistance(float delta)
    {
        localData.DistanceTravelled += delta;
        ServerDataManager.Instance.UpdateDistace(localData);
    }


    public void AddPocketPal(GameObject obj)
    {
        //Get the data reference
        PocketPalData ppd = obj.GetComponent<PocketPalParent>().GetAnimalData();

       //Add the pocketPal to the players inventory
        localData.Inventory.AddPocketPal(obj.GetComponent<PocketPalParent>());

        //increas the players EXP
        localData.IncreaseExp(obj.GetComponent<PocketPalParent>().GetAnimalData().GetExp());

        //update the server
        ServerDataManager.Instance.WritePocketPal(localData, localData.Inventory.GetDataFromID(ppd.ID));

    }

    public void ResetLocalData()
    {
        //delete one if already exsists
        if (File.Exists(destination)) File.Delete(destination);

        //create new file and data for use
        localData = new GameData();
    }

    //try and get a players pocketpal. If player does not own Pocket pal with ID returns null
    public PocketPalData TryGetPocketPal(int ID)
    {
        foreach (PocketPalData ppd in localData.Inventory.GetMyPocketPals())
        {
            if (ppd.ID == ID) return ppd;
        }
        return null;
    }

    public PocketPalInventory GetInventory()
    {
        return localData.Inventory;
    }

    public FileStream ResetFile()
    {
        //delete one if already exsists
        if (File.Exists(destination)) File.Delete(destination);

        //create new file and data for use
        localData = new GameData();
        return File.Create(destination);

    }

    public GameData GetData() { return localData; }
}
