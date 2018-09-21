using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class DefaultPocketPalInfo
{
    public int ID, Rarity;
    public string PPalName, LatinName, PocketFact, Description, order;
    public SpawnTime timeActive;
    public SpawnType spawnType;
    public PPalType ppalType;
    public float minWeight, maxWeight, minLength, maxLength;

    public void ApplyStaticInfo(FactSheetFields fields)
    {
        fields.LatinName.text = LatinName;
        fields.PPalName.text = PPalName;
        fields.PocketFact.text = PocketFact;
        fields.Description.text = Description;
        fields.Order.text = order;

        if (timeActive == SpawnTime.night)
        {
            fields.DayNightImage.sprite = AssetManager.Instance.night;
            fields.DayNightText.text = "Nocturnal";
        }
        else
        {
            fields.DayNightImage.sprite = AssetManager.Instance.day;
            fields.DayNightText.text = "Dinural";
        }
        if (ppalType == PPalType.Bird)
        {
            fields.LengthText.text = "Wingspan";
        }
    }

    public void PrintMe()
    {
        string str = "";

        foreach (var foo in this.GetType().GetFields())
        {
            str += foo.Name + ": " + foo.GetValue(this) + "\n";
        }
        Debug.Log(str);
    }

}
[System.Serializable]
public class FactSheetFields
{
    public Image DayNightImage;
    public Text PPalName, LatinName, Description, PocketFact, Order, DayNightText, LengthText;
}