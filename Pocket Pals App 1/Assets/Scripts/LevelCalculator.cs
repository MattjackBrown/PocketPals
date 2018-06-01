
public class LevelCalculator
{
    public const float LevelCoefficent = 1.05f;
    public const float ExpForFirstLevel = 100;

    public static float GetPercentageToNextLevel(float EXP)
    {
        float EXPToNextLevel = GetExpNeeded(EXP);
        float ExpNeeded = EXP + EXPToNextLevel;
        return EXP / ExpNeeded;
    }

    public static int CalculateLevel(float EXP)
    {
        float temp = ExpForFirstLevel;
        int lvl = 1;
        while (EXP > temp)
        {
            temp = temp * lvl * LevelCoefficent;
            lvl++;
        }
        return lvl;
    }

    public static float GetExpNeeded(float EXP)
    {
        float temp = ExpForFirstLevel;
        int lvl = 1;
        while (EXP > temp)
        {
            temp = temp * lvl * LevelCoefficent;
            lvl++;
        }
        return temp - EXP;
    }
}