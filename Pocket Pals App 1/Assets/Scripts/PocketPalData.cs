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
    public float weight = 0;
    public float EXP = 0;

    public float size = 0;
    public float baseRarity = 0;

    public int HasChampion = 0;
    public int HasRare = 0;

    public string FirstSeen= "";
    public string LastSeen = "";

    public PocketPalData(string str, int id, float exp, float siz, float rare)
    {
        name = str;
        ID = id;
        EXP = exp;
        size = siz;
        baseRarity = rare;
    }

    public PocketPalData() { }

    public float GetSize()
    {
        return (float)Math.Round(size, 1);
    }

    public string GetLastSeen()
    {
        if (LastSeen == "")
        {
            LastSeen = DateTime.Now.ToString("dd/MM/yyyy");
        }
        return LastSeen;
    }

    public string GetFirstSeen()
    {
        if (FirstSeen == "")
        {
            FirstSeen = DateTime.Now.ToString("dd/MM/yyyy");
        }
        return FirstSeen;
    }

    public int GetLevel()
    {
        return LevelCalculator.CalculateLevel(EXP);
    }

    public float GetRarity()
    {
        return Mathf.RoundToInt(baseRarity);
    }

    public float GetExpToNextLevel()
    {
        return (float)Math.Round(LevelCalculator.GetExpNeeded(EXP));
    }

    public float GetPercentageToNextLevel()
    {
        return LevelCalculator.GetPercentageToNextLevel(EXP);
    }

    public void MergePocketPal(PocketPalData ppd, float expMultiplier)
    {
        if (weight > ppd.weight) weight = ppd.weight;
        if (size > ppd.size) size = ppd.size;
        if (name != ppd.name) name = ppd.name;
        if (baseRarity != ppd.baseRarity) baseRarity = ppd.baseRarity;
        if(FirstSeen == "") FirstSeen = DateTime.Now.ToString("dd/MM/yyyy");
        if(LastSeen == "") LastSeen = DateTime.Now.ToString("dd/MM/yyyy");
        numberCaught++;
        EXP += ppd.EXP*expMultiplier;
    }

    public float GetExp() { return (float)Math.Round(EXP); }



}
