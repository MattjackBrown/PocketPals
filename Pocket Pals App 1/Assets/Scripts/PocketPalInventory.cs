using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[System.Serializable]
public class PocketPalInventory
{
    private List<PocketPalData> myPPals = new List<PocketPalData>();

    public void AddPocketPal(PocketPalParent obj, float multiplier = 1.0f)
    {
        //try and get the animal from an exsisting inventory
        PocketPalData ppd = GetDataFromID(obj.PocketPalID);
        if (ppd == null)
        {
            PocketPalData pd = obj.GetAnimalData();
            pd.FirstSeen = DateTime.Now.ToString("dd/MM/yyyy");
            pd.LastSeen = DateTime.Now.ToString("dd/MM/yyyy");
            myPPals.Add(pd);
        }
        //merge it if its a repeat animal
        else
        {
            Debug.Log("Repeat");

            obj.GetAnimalData().LastSeen = DateTime.Now.ToString("dd/MM/yyyy");
            ppd.MergePocketPal(obj.GetAnimalData(), multiplier);
        }
    }

    public void InitialServerAdd(PocketPalData ppd)
    {
        myPPals.Add(ppd);

    }

    public string GetInventoryJson()
    {
        string str = "";
        foreach (PocketPalData ppd in myPPals)
        {
            str += JsonUtility.ToJson(ppd);
        }
        Debug.Log(str);
        return str;
    }

    public PocketPalData GetDataFromID(int ID)
    {
        foreach (PocketPalData p in myPPals)
        {
            if (p.ID == ID) return p;
        }
        return null;
    }

    public PocketPalData GetRarest()
    {
        PocketPalData rarest = null;
        foreach (PocketPalData ppd in myPPals)
        {
            if (rarest == null || rarest.baseRarity < ppd.baseRarity)
            {
                rarest = ppd;
            }
        }
        return rarest;
    }

    public PocketPalData GetMostCaught()
    {
        PocketPalData mostCaught = null;
        foreach (PocketPalData ppd in myPPals)
        {
            if (mostCaught == null || mostCaught.numberCaught < ppd.numberCaught)
            {
                mostCaught = ppd;
            }
        }
        return mostCaught;
    }

    public PocketPalData GetHighestLevel()
    {
        PocketPalData highestLevel = null;
        foreach (PocketPalData ppd in myPPals)
        {
            if (highestLevel == null || highestLevel.GetExp() < ppd.GetExp())
            {
                highestLevel = ppd;
            }
        }
        return highestLevel;
    }

    public List<PocketPalData> GetMyPocketPals()
    {
        return myPPals;
    }

    public GameObject GetMostRecent()
    {
        if (myPPals.Count < 1) return null;
        //Use the asset manager to look up the gameobject assosiated to that pocketpal ID
        return AssetManager.Instance.GetPocketPalFromID(myPPals[myPPals.Count - 1].ID);
    }

    public PocketPalData GetMostRecentData()
    {
        if (myPPals.Count < 1) return null;
        //Use the asset manager to look up the gameobject assosiated to that pocketpal ID
        return myPPals[myPPals.Count - 1];
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
            Debug.Log("Name: " + ppd.pocketPalName + " ID: " + ppd.ID + " Level: " + ppd.GetLevel()  + " EXP: " + ppd.GetExp());   
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
