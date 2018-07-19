
using UnityEngine;

public class LevelCalculator
{
    public const float LevelCoefficent = 1.3f;
    public const float ExpForFirstLevel = 1000;

    public static float GetPercentageToNextLevel(float EXP)
    {
        float EXPToNextLevel = GetExpNeeded(EXP);
        float ExpNeeded = EXP + EXPToNextLevel;
        return EXP / ExpNeeded;
    }

    public static int CalculateLevel(float EXP, float modifier = 1.0f)
    {
        float temp = ExpForFirstLevel;
        int lvl = 1;
        while (EXP > temp)
        {
            temp = temp * (LevelCoefficent * modifier);
            lvl++;
        }
        return lvl;
    }

    public static float GetExpNeeded(float EXP, float modifier = 1.0f)
    {
        float temp = ExpForFirstLevel;
        int lvl = 1;
        while (EXP > temp)
        {
            temp += temp * LevelCoefficent;
            lvl++;
        }
        return temp - EXP;
    }

    public static void SimulateLevels(float maxEXP, float modifier = 1.0f)
    {
        float temp = ExpForFirstLevel;
        int lvl = 1;
        while (maxEXP > temp)
        {
            temp = temp * (LevelCoefficent * modifier);
            lvl++;
            Debug.Log("Level: " + lvl + " ExpNeeded: " + temp);
        }
    }
}