using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : MonoBehaviour {

    public static AssetManager Instance { set; get; }

    // The Pocket Pal prefabs to be spawned
    public GameObject[] PocketPals;

    public ItemData[] Items;

    public TracksAndTrailsPreset[] TracksAndTrails;

    // Use this for initialization
    void Start ()
    {
        Instance = this;
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

    public ItemData GetRandomItem(int maxFind)
    {
        ItemData id = Items[Random.Range(0, Items.Length)];

        return id.CloneWithNumber(Random.Range(1, maxFind));
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

    public TracksAndTrailsPreset GetRandoTandT()
    {
        return TracksAndTrails[Random.Range(0, TracksAndTrails.Length)];
    }
	// Update is called once per frame
	void Update () {
		
	}
}
