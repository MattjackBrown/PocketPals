
using UnityEngine;

public class LevelCalculator
{
    public const float LevelCoefficent = 2f;
    public const float baseExp = 150;

    public static float GetPercentageToNextLevel(float EXP, float modifer)
    {
        float EXPToNextLevel = GetExpNeeded(EXP, modifer);
        float EXPToLast = exptolastlevel(EXP, modifer);
        float temp = GetExpNeeded(EXP, modifer) - EXPToLast;
        float temp2 = EXP - EXPToLast;
        return temp2/temp;
    }

    public static int CalculateLevel(float EXP, float modifier)
    {
        float temp = baseExp;
        int lvl = 1;
        while (EXP > temp)
        {
            temp += baseExp * (lvl * lvl * 0.2f) * (LevelCoefficent);
            temp *= modifier;
            lvl++;
        }
        return lvl;
    }

    public static float GetExpNeeded(float EXP, float modifier)
    {
        float temp = baseExp;
        float lstexp = 0;
        int lvl = 1;
        while (EXP > temp)
        {
            lstexp = temp;
            
            temp += baseExp * (lvl * lvl * 0.2f) * (LevelCoefficent);
            temp *= modifier;
            lvl++;
        }

        return temp;
    }

    public static float exptolastlevel(float EXP, float modifier)
    {
        float temp = baseExp;
        float lstexp = 0;
        int lvl = 1;
        while (EXP > temp)
        {
            lstexp = temp;

            temp += baseExp * (lvl * lvl * 0.2f) * (LevelCoefficent);
            temp *= modifier;
            lvl++;
        }

        return lstexp;
    }

    public static void SimulateLevels(float maxEXP)
    {
        float temp = baseExp;
        int lvl = 1;
        while (maxEXP > temp)
        {
            temp += baseExp * (lvl*lvl*0.2f)* (LevelCoefficent);
            temp *= 1.1f;
            lvl++;
            Debug.Log("Level " + lvl + " Requires: " + temp + "exp");
        }
    }
}