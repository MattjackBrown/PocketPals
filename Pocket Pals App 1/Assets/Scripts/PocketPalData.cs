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
    public int level = 0;
    private float EXP = 0;
    private int numberCaught = 0;
    public float weight = 0;
    public float size = 0;
    public float raiting = 0;
    public float agressiveness = 0;

    public PocketPalData(string str, int id, float exp, float siz, float agress)
    {
        name = str;
        ID = id;
        EXP = exp;
        size = siz;
        agressiveness = agress;
    }

    public void MergePocketPal(PocketPalData ppd, float expMultiplier)
    {
        if (weight > ppd.weight) weight = ppd.weight;
        if (agressiveness > ppd.agressiveness) agressiveness = ppd.agressiveness;
        if (size > ppd.size) size = ppd.size;
        EXP += ppd.EXP*expMultiplier;
        level = CalculateLevel(EXP);
    }

    public float GetExp() { return EXP; }

    public static int CalculateLevel(float exp)
    {
        float temp = ExpForFirstLevel;
        int lvl = 1;
        while (exp < temp)
        {
            temp = temp * lvl * LevelCoefficent;
            lvl++;
        }
        return lvl;
    }

}
