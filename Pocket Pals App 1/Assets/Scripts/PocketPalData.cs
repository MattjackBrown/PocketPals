using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class PocketPalData
{
    public string name = "none";
    public int ID = 0;

    public int numberCaught = 1;
    public float EXP = 0;

    public float weight = 0;
    public float length = 0;
    public float baseRarity = 0;

    public int HasChampion = 0;
    public int HasRare = 0;

    public string FirstSeen= "";
    public string LastSeen = "";

    public PocketPalData(string str, int id, float exp, float w, float l,  float rare)
    {
        name = str;
        ID = id;
        EXP = exp;
        weight = w;
        length =  l;
        baseRarity = rare;
    }

    public PocketPalData() { }

    public float GetRoundedWeight()
    {
        return (float)Math.Round(weight, 1);
    }

    public float GetRoundedLength()
    {
        return (float)Math.Round(length, 1);
    }

    public string GetLastSeen()
    {
        if (LastSeen == "")
        {
            LastSeen = DateTime.Now.ToString("dd/MM/yyyy");
            ServerDataManager.Instance.WritePocketPal(LocalDataManager.Instance.GetData(),this);
        }
        return LastSeen;
    }

    public string GetFirstSeen()
    {
        if (FirstSeen == "")
        {
            FirstSeen = DateTime.Now.ToString("dd/MM/yyyy");
            ServerDataManager.Instance.WritePocketPal(LocalDataManager.Instance.GetData(), this);
        }
        return FirstSeen;
    }

    public int GetLevel()
    {
        return LevelCalculator.CalculateLevel(EXP,1);
    }

    public float GetRarity()
    {
        return Mathf.RoundToInt(baseRarity);
    }

    public float GetExpToNextLevel()
    {
        return (float)Math.Round(LevelCalculator.GetExpNeeded(EXP,1));
    }

    public float GetPercentageToNextLevel()
    {
        return LevelCalculator.GetPercentageToNextLevel(EXP,1);
    }

    public void MergePocketPal(PocketPalData ppd, float expMultiplier)
    {
        if (weight > ppd.weight) weight = ppd.weight;
        if (length > ppd.length) length = ppd.length;
        if (name != ppd.name) name = ppd.name;
        if (baseRarity != ppd.baseRarity) baseRarity = ppd.baseRarity;
        if(FirstSeen == "") FirstSeen = DateTime.Now.ToString("dd/MM/yyyy");
        LastSeen = DateTime.Now.ToString("dd/MM/yyyy");
        numberCaught++;
        EXP += ppd.EXP*expMultiplier;
    }

    public float GetExp() { return (float)Math.Round(EXP); }



}
