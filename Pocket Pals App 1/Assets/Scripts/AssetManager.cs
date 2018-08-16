using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : MonoBehaviour {

    public static AssetManager Instance { set; get; }

    // The Pocket Pal prefabs to be spawned
    public GameObject[] PocketPals;

    public AnimalScreenshots[] screenShots;

    public ItemData[] Items;
    private List<float> itemRarities = new List<float>();


    public TracksAndTrailsPreset[] TracksAndTrails;
    private List<float> tntRarities = new List<float>();

    // Use this for initialization
    void Start ()
    {
        Instance = this;
        foreach (ItemData id in Items)
        {
            itemRarities.Add(id.rarity);
        }
        foreach (TracksAndTrailsPreset ttp in TracksAndTrails)
        {
            tntRarities.Add(ttp.rarity);
        }
	}

    public GameObject GetPocketPalFromID(int ID)
    {
        foreach (GameObject obj in PocketPals)
        {
            if (obj.GetComponent<PocketPalParent>().PocketPalID == ID)
            {
                return obj;
            }
        }
        return null;
    }

    public Sprite GetScreenShot(int id)
    {
        foreach (AnimalScreenshots scr in screenShots)
        {
            if (scr.ppalID == id) return scr.spr;
        }
        return null;
    }

    public Sprite GetFactSheet(int id)
    {
        GameObject obj = GetPocketPalFromID(id);
        PocketPalParent ppp = obj.GetComponent<PocketPalParent>();
        return ppp.FactSheet;
    }

    public List<PocketPalParent> GetRandomPocketpals(int num, int ppalID, PPalType filter = PPalType.All)
    {
        List<PocketPalParent> ppals = new List<PocketPalParent>();
        System.Random r = new System.Random(ppalID);
        while (ppals.Count < num)
        {
           PocketPalParent ppp = PocketPals[r.Next(0, PocketPals.Length)].GetComponent<PocketPalParent>();
            if (!ppals.Contains(ppp) && ppp.PocketPalID != ppalID)
            {
                if(filter == PPalType.All || ppp.animalType == filter) ppals.Add(ppp);
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
        List<GameObject> pList = new List<GameObject>();
        foreach (GameObject obj in PocketPals)
        {
            if (obj.GetComponent<PocketPalParent>().type == type)
            {
                pList.Add(obj);
            }
            else
            {
                switch (obj.GetComponent<PocketPalParent>().type)
                {
                    case SpawnType.a_Woodland:
                        {
                            if (type == SpawnType.d_Woodland || type == SpawnType.n_Woodland)
                            {
                                pList.Add(obj);
                            }
                            break;
                        }
                    case SpawnType.a_Wetland:
                        {
                            if (type == SpawnType.d_Wetland || type == SpawnType.n_Wetland)
                            {
                                pList.Add(obj);
                            }
                            break;
                        }
                }
            }
        }
        return pList;
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
