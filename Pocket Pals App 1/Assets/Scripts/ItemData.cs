using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class ItemData
{
    public string name = "none";
    public int ID = 0;
    public Sprite spr;
    public int numberOwned = 1;
    public int rarity = 0;

    public ItemData(string str, int id)
    {
        name = str;
        ID = id;
    }

    public ItemData CloneWithNumber(int number)
    {
        ItemData id = new ItemData(this.name, this.ID);
        id.numberOwned = number;
        id.spr = this.spr;
        return id;
    }

    public ItemData(){ }

    public virtual bool UseItem()
    {
        if (numberOwned >= 1)
        {
            numberOwned--;
            ServerDataManager.Instance.WriteItem(LocalDataManager.Instance.GetData(), this);
            return true;
        }
        return false;
    }

    public void AnotherOne()
    {
        numberOwned++;
        Debug.Log(numberOwned);
    }

}
