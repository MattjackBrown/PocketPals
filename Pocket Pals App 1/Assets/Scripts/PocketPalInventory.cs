using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PocketPalInventory : MonoBehaviour {

    private List<GameObject> pocketPals = new List<GameObject>();

    public PocketPalSpawnManager spawner;

	// Use this for initialization
	void Start ()
    {
		
	}

    public void AddPocketPal(GameObject obj)
    {
        obj.SetActive(false);
        spawner.PocketpalCollected(obj);
        pocketPals.Add(obj);
    }

    public List<GameObject> GetMyPocketPals()
    {
        return pocketPals;
    }

    public GameObject GetMostRecent()
    {
        return pocketPals[pocketPals.Count - 1];
    }

    public List<GameObject> GetXMostRecent(int x)
    {
        List<GameObject> rList = new List<GameObject>();

        if (pocketPals.Count < x) x = pocketPals.Count;

        for (int i = 1; i < x+1; i++)
        {
            rList.Add(pocketPals[pocketPals.Count - i]);
        }
        return rList;
    }

	// Update is called once per frame
	void Update ()
    {
		
	}
}
