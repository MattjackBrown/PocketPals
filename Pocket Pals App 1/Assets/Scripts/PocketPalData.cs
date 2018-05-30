using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class PocketPalData
{
    public const float LevelCoefficent = 1.05f;
    public const float ExpForFirstLevel = 100; 

    public string name = "none";
    public int ID = 0;

    public int numberCaught = 1;
    public float weight = 0;
    private float agressiveness = 0;

    private int level = 0;
    private float EXP = 0;
    private float EXPToNextLevel = 0;

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
        CalculateLevel();
        return level;
    }

    public float GetRarity()
    {
        return Mathf.RoundToInt(baseRarity / PocketPalSpawnManager.AverageRarity);
    }

    public float GetExpToNextLevel()
    {
        CalculateLevel();
        return (float)Math.Round(EXPToNextLevel);
    }

    public void MergePocketPal(PocketPalData ppd, float expMultiplier)
    {
        if (weight > ppd.weight) weight = ppd.weight;
        if (agressiveness > ppd.agressiveness) agressiveness = ppd.agressiveness;
        if (size > ppd.size) size = ppd.size;
        numberCaught++;
        EXP += ppd.EXP*expMultiplier;
        CalculateLevel();
    }

    public float GetExp() { return (float)Math.Round(EXP); }

    public float GetPercentageToNextLevel()
    {
        CalculateLevel();
        float ExpNeeded = EXP + EXPToNextLevel;
        return EXP / ExpNeeded;
    }

    public void CalculateLevel()
    {
        float temp = ExpForFirstLevel;
        int lvl = 1;
        while (EXP > temp)
        {
            temp = temp * lvl * LevelCoefficent;
            lvl++;
        }
        EXPToNextLevel = temp - EXP;
        level = lvl;
    }

}
