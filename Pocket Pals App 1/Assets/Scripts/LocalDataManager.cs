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
    private static GameData localData;

	public bool InAR = false;

	// Use this for initialization
	void Start ()
    {
        //This makes the local datamanager accessible from anywhere
        Instance = this;

        localData = new GameData();

        DontDestroyOnLoad(gameObject);

		// Store this instance reference as a static variable in the Global variables class
		GlobalVariables.localDataManager = this;
    }

    private void Awake()
	{
		// Don't destroy on load does not stop new instances from being instantiated on scene load. This will check and delete
		if (FindObjectsOfType(typeof(LocalDataManager)).Length > 1)
		{
			Debug.Log("Dirty Singleton management. Deleting new instance.");
			DestroyImmediate(gameObject);
		}

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
        if (ServerDataManager.Instance != null)
        {
            ServerDataManager.Instance.UpdateDistace(localData);
        }
    }

	public void SaveCharStyle(CharacterStyleData csd)
	{
		localData.SaveCharacterStyle (csd);
	}

    public void AddPocketPal(GameObject obj)
    {
        //Get the data reference
        PocketPalData ppd = obj.GetComponentInParent<PocketPalParent>().GetAnimalData();
        SyncPPal(ppd, obj.GetComponentInParent<PocketPalParent>());

    }

    private void SyncPPal(PocketPalData ppd, PocketPalParent pp)
    {
        NotificationManager.Instance.CongratsNotification("You have observed a level " + ppd.GetLevel() + " " + ppd.name);

        //Add the pocketPal to the players inventory
        localData.Inventory.AddPocketPal(pp);

        float exp = pp.GetAnimalData().GetExp();

        //increas the players EXP
        localData.IncreaseExp(exp);

        //update the server
        ServerDataManager.Instance.WritePocketPal(localData, localData.Inventory.GetDataFromID(ppd.ID));

        ServerDataManager.Instance.UpdatePlayerExp(localData);
    }

    public void SuccessfulTrack(PocketPalParent pp, TracksAndTrailsPreset ttp, float modifier)
    {
        PocketPalData ppd = pp.GenerateAnimalData(ttp.expMin, ttp.expMax);
        ppd.EXP *= modifier;
        SyncPPal(ppd, pp);
    }

    public void AddItem(ItemData id)
    {
        int maxIter = 10;
        if (id.numberOwned < maxIter) maxIter = id.numberOwned;
        for (int i = 0; i < maxIter; i++)
        {
            PopupHandler.Instance.AddPopup(id.spr);
        }

        ServerDataManager.Instance.WriteItem(localData, localData.ItemInv.AddItem(id));
    }

    public bool TryAddTracks()
    {
        TrackData td = AssetManager.Instance.GetNewTrack();
        if (localData.TrackInv.TryAddTrack(td))
        {
            NotificationManager.Instance.CongratsNotification("You have found a new trail!");
            ServerDataManager.Instance.WriteTrack(localData, td);
            return true;
        }
        else
        {
            NotificationManager.Instance.InteractError("You already have six tracks and trails!");
            return false;
        }
    }

    public void RemoveTracks(TrackData td)
    {
        localData.TrackInv.GetTracks().Remove(td);
        ServerDataManager.Instance.RemoveTrack(localData, td);
    }

    public void ResetLocalData()
    {
        //delete one if already exsists
        if (File.Exists(destination)) File.Delete(destination);

        //create new file and data for use
        localData = new GameData();
    }

    public bool TryBuySomething(int cost)
    {
        return localData.TryUseCoins(cost);
    }

    public void BuyCoins(int num)
    {
        NotificationManager.Instance.CongratsNotification("You have received " + num + " PocketCoins!!");
        ServerDataManager.Instance.AddPocketCoins(localData, num);
    }

    public void BoughtAR()
    {
        localData.HasAR = 1;
        ServerDataManager.Instance.UpdateHasAR(localData.HasAR);
    }

    public void BoughtCP()
    {
        localData.HasCostumePack = 1;
        ServerDataManager.Instance.UpdateHasCostumePack(localData.HasCostumePack);
        CharacterCustomisation.Instance.customisationKitUnlocked = true;
    }

    public bool HasAR()
    {
        if (localData.HasAR == 1) return true;
        return false;
    }

    public bool HasCP()
    {
        if (localData.HasCostumePack == 1) return true;
        return false;
    }

	public bool CanAR()
	{
		return EnvironmentChanger.IsARSupported ();
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

    public ItemInventory GetItemInventory() { return localData.ItemInv; }

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
