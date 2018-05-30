using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PocketPalData
{
    public const float LevelCoefficent = 0.9f;
    public const float ExpForFirstLevel = 100; 

    public string name = "none";
    public int ID = 0;
    private int level = 0;
    private float EXP = 0;
    public float EXPToNextLevel = 0;
    public int numberCaught = 0;
    public float weight = 0;
    public float size = 0;
    public float rarity = 0;
    public float agressiveness = 0;

    public PocketPalData(string str, int id, float exp, float siz, float agress)
    {
        name = str;
        ID = id;
        EXP = exp;
        size = siz;
        agressiveness = agress;
        rarity = 2;
    }

    public int GetLevel()
    {
        CalculateLevel();
        return level;
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

    public float GetExp() { return EXP; }

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
        while (EXP < temp)
        {
            temp = temp * lvl * LevelCoefficent;
            lvl++;
        }
        EXPToNextLevel = temp - EXP;
        level = lvl;
    }

}
