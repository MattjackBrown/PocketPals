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
        //try and get the animal from an exsisting inventory
        PocketPalData ppd = GetDataFromID(obj.PocketPalID);
        if (ppd == null)
        {
            myPPals.Add(obj.GetAnimalData());
        }
        //merge it if its a repeat animal
        else
        {
            ppd.MergePocketPal(obj.GetAnimalData(), 1.0f);
        }
    }

    public PocketPalData GetDataFromID(int ID)
    {
        foreach (PocketPalData p in myPPals)
        {
            if (p.ID == ID) return p;
        }
        return null;
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

    public List<int> GetUniqueAnimalIDs()
    {
        List<int> UniqueIDs = new List<int>();
        foreach (PocketPalData ppd in myPPals)
        {
            if (!UniqueIDs.Contains(ppd.ID)) UniqueIDs.Add(ppd.ID);
        }
        return UniqueIDs;
    }

    public void PrintMyPocketPals()
    {
        foreach (PocketPalData ppd in myPPals)
        {
            Debug.Log("Name: " + ppd.name + " ID: " + ppd.ID + " Level: " + ppd.level + " Size: " + ppd.size + " EXP: " + ppd.GetExp());   
        }
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
