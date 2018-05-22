using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PocketPalInventory
{
    private List<PocketPalData> myPPals = new List<PocketPalData>();

    public void AddPocketPal(PocketPalParent obj)
    {
        PocketPalData ppd = new PocketPalData(obj.name, obj.PocketPalID, Guid.NewGuid().ToString(), 0);
        myPPals.Add(ppd);
    }

    public List<PocketPalData> GetMyPocketPals()
    {
        return myPPals;
    }

    public GameObject GetMostRecent()
    {
        //Use the asset manager to look up the gameobject assosiated to that pocketpal ID
        return AssetManager.Instance.GetPocketPalFromID(myPPals[myPPals.Count - 1].ID);
    }

    public string GetPocketPalsID()
    {
        string str = "";
        foreach (PocketPalData ppd in myPPals)
        {
            str += ppd.ID;
        }
        return str;
    }

    public List<GameObject> GetXMostRecent(int x)
    {
        List<GameObject> rList = new List<GameObject>();

        if (myPPals.Count < x) x = myPPals.Count;

        for (int i = 1; i < x+1; i++)
        {
            //Use the asset manager to look up the gameobject assosiated to that pocketpal ID
            rList.Add(AssetManager.Instance.GetPocketPalFromID(myPPals[myPPals.Count - i].ID));
        }
        return rList;
    }
}
