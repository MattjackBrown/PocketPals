using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PocketPalData
{
    public string name = "none";
    public int ID = 0;
    public string uniqueID = "";
    public float EXP = 0;

    public PocketPalData(){ }

    public PocketPalData(string n, int id, string uID, float exp)
    {
        name = n;
        ID = id;
        uniqueID = uID;
        EXP = exp;
    }
}
