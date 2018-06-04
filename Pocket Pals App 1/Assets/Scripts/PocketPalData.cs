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
    private float agressiveness = 0;
    private float EXP = 0;

    private float size = 0;
    private float baseRarity = 0;

    public PocketPalData(string str, int id, float exp, float siz, float agress, float rare)
    {
        name = str;
        ID = id;
        EXP = exp;
        size = siz;
        agressiveness = agress;
        baseRarity = rare;
    }

    public float GetSize()
    {
        return (float)Math.Round(size, 1);
    }

    public float GetAgression()
    {
        return (float)Math.Round(agressiveness, 1);
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
        if (agressiveness > ppd.agressiveness) agressiveness = ppd.agressiveness;
        if (size > ppd.size) size = ppd.size;
        if (name != ppd.name) name = ppd.name;
        if (baseRarity != ppd.baseRarity) baseRarity = ppd.baseRarity;
        numberCaught++;
        EXP += ppd.EXP*expMultiplier;
    }

    public float GetExp() { return (float)Math.Round(EXP); }



}
