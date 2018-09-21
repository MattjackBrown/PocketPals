using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AssetManager : MonoBehaviour {

    public static AssetManager Instance { set; get; }

    // The Pocket Pal prefabs to be spawned
    public GameObject[] WoodlandPocketPals;
	public GameObject[] WetlandPocketPals;
	public GameObject[] CoastalPocketPals;

    public Sprite day, night;

	private Dictionary<int, GameObject> AllPocketPals = new Dictionary<int, GameObject>();

    public AnimalScreenshots[] screenShots;

    public ItemData[] Items;
    private List<float> itemRarities = new List<float>();


    public TracksAndTrailsPreset[] TracksAndTrails;
    private List<float> tntRarities = new List<float>();

    // Use this for initialization
    void Start ()
    {
        Instance = this;

        // Combine all the arrays
        foreach (GameObject obj in WoodlandPocketPals)
        {
            AllPocketPals.Add(obj.GetComponent<PocketPalParent>().PocketPalID, obj);
        }
        foreach (GameObject obj in WetlandPocketPals)
        {
            AllPocketPals.Add(obj.GetComponent<PocketPalParent>().PocketPalID, obj);
        }


        foreach (ItemData id in Items)
        {
            itemRarities.Add(id.rarity);
        }
        foreach (TracksAndTrailsPreset ttp in TracksAndTrails)
        {
            tntRarities.Add(ttp.rarity);
        }

        List<DefaultPocketPalInfo> defaultValues = DatabaseScanner.ScanFileForInfo((TextAsset)Resources.Load("PPalData"));

        foreach (DefaultPocketPalInfo obj in defaultValues)
        {
            PocketPalParent ppal = GetBasePocketPal(obj.ID);
            if (ppal == null)
            {
                Debug.Log("Cant find Info for ppal: " + obj.PPalName);
            }
            else
            {
                ppal.SetBaseData(obj);
            }
        }
	}

    public GameObject GetPocketPalGameObject(int ID)
    {
        if (AllPocketPals.ContainsKey(ID))
        {
            return AllPocketPals[ID];
        }
        return null;
    }

    public PocketPalParent GetBasePocketPal(int ID)
    {
        GameObject obj = GetPocketPalGameObject(ID);

        if (obj != null) return obj.GetComponent<PocketPalParent>();
        else return null;
    }

    public Sprite GetScreenShot(int id)
    {
        foreach (AnimalScreenshots scr in screenShots)
        {
            if (scr.ppalID == id) return scr.spr;
        }
        return null;
    }


	// This is just for the guessing game right? Have left as WoodlandPocketPals for now
    public List<PocketPalParent> GetRandomPocketpals(int num, int ppalID, PPalType filter = PPalType.All)
    {
        List<PocketPalParent> ppals = new List<PocketPalParent>();
        System.Random r = new System.Random(ppalID);
        while (ppals.Count < num)
        {
			PocketPalParent ppp = WoodlandPocketPals[r.Next(0, WoodlandPocketPals.Length)].GetComponent<PocketPalParent>();
            if (!ppals.Contains(ppp) && ppp.PocketPalID != ppalID)
            {
                if(filter == PPalType.All || ppp.GetBaseData().ppalType == filter) ppals.Add(ppp);
            }
        }
        return ppals;
    }

    public ItemData GetItemByName(string name)
    {
        foreach (ItemData id in Items)
        {
            if (name == id.name) return id;
        }
        return null;
    }

    public ItemData GetItemByID(int ID)
    {
        foreach (ItemData id in Items)
        {
            if (ID == id.ID) return id;
        }
        return null;
    }

    public List<ItemData> GetStartItems(int stra, int berr, int cam, int mag)
    {
        List<ItemData> st = new List<ItemData>();
        st.Add(GetItemByID(GlobalVariables.StrawBerriesID).CloneWithNumber(stra));
        st.Add(GetItemByID(GlobalVariables.BerryID).CloneWithNumber(berr));
        st.Add(GetItemByID(GlobalVariables.medCameraID).CloneWithNumber(cam));
        st.Add(GetItemByID(GlobalVariables.MagnifyingGlassID).CloneWithNumber(mag));
        return st;
    }
    public ItemData GetRandomItem(int maxFind)
    {
        ItemData id = Items[UnityEngine.Random.Range(0, Items.Length)];

        return id.CloneWithNumber(UnityEngine.Random.Range(1, maxFind));
    }

    public ItemData GetWeightRandomItem()
    {
        ItemData id = Items[PocketPalSpawnManager.Sampler(new System.Random(Guid.NewGuid().GetHashCode()), itemRarities)];
        id.numberOwned = 1;
        return id;
    }

    public List<GameObject> GetPocketPalsOfType(SpawnType type)
    {
        switch (type)
        {
            case SpawnType.Woodland:
                {
                    return WoodlandPocketPals.ToList() ;
                }
            case SpawnType.Wetland:
                {
                    return WetlandPocketPals.ToList();
                }
			case SpawnType.Coastal:
				{
                    return CoastalPocketPals.ToList();
				}
        }
        return WoodlandPocketPals.ToList();
    }

    public TracksAndTrailsPreset GetTrackByID(int ID)
    {
        foreach (TracksAndTrailsPreset tatp in TracksAndTrails)
        {
            if (ID == tatp.ID) return tatp;
        }
        return TracksAndTrails[0];
    }

    public TrackData GetNewTrack()
    {
        TracksAndTrailsPreset ttp = TracksAndTrails[PocketPalSpawnManager.Sampler(new System.Random(Guid.NewGuid().GetHashCode()), tntRarities)];
        TrackData td = new TrackData(ttp.ID, LocalDataManager.Instance.GetData().DistanceTravelled, UnityEngine.Random.Range(ttp.distMin, ttp.distMax));
        return td;
    }

    public TracksAndTrailsPreset GetRandoTandT()
    {
        return TracksAndTrails[UnityEngine.Random.Range(0, TracksAndTrails.Length)];
    }
	// Update is called once per frame
	void Update () {
		
	}


}

[System.Serializable]
public class AnimalScreenshots
{
    public Sprite spr;
    public int ppalID;
}
