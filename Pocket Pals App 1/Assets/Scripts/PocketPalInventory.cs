using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PocketPalInventory : MonoBehaviour {

    private List<GameObject> myPPals = new List<GameObject>();

    public PocketPalSpawnManager spawner;

	// Use this for initialization
	void Start ()
    {
		
	}

    public void AddPocketPal(GameObject obj)
    {
        obj.SetActive(false);
        spawner.PocketpalCollected(obj);
        myPPals.Add(obj);
    }

    public List<GameObject> GetMyPocketPals()
    {
        return myPPals;
    }

    public GameObject GetMostRecent()
    {
        return myPPals[myPPals.Count - 1];
    }

    public List<GameObject> GetXMostRecent(int x)
    {
        List<GameObject> rList = new List<GameObject>();

        if (myPPals.Count < x) x = myPPals.Count;

        for (int i = 1; i < x+1; i++)
        {
            rList.Add(myPPals[myPPals.Count - i]);
        }
        return rList;
    }

	// Update is called once per frame
	void Update ()
    {
		
	}
}
