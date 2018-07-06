using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class ItemData
{
    public string name = "none";
    public int ID = 0;

    public int numberOwned = 1;

    public ItemData(string str, int id)
    {
        name = str;
        ID = id;
    }

    public ItemData(){ }

    public virtual bool UseItem()
    {
        if (numberOwned > 1)
        {
            numberOwned--;
            return true;
        }
        return false;
    }

    public void AnotherOne()
    {
        numberOwned++;
    }

}
