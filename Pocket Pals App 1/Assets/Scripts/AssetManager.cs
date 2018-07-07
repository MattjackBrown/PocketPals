using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : MonoBehaviour {

    public static AssetManager Instance { set; get; }

    // The Pocket Pal prefabs to be spawned
    public GameObject[] PocketPals;

    public ItemData[] Items;

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

	// Update is called once per frame
	void Update () {
		
	}
}
