using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopHandler : MonoBehaviour
{
    public static ShopHandler Instance { set; get; }
    public List<ShopData> shopItems;
    public List<GameObject> Menus;
    private int iter = 0;
    private GameObject cMenu;
    public Text myCoins;

    public void ButtonPressed(string buttonID)
    {
        foreach (ShopData sd in shopItems)
        {
            if (sd.ButtonID == buttonID)
            {
                NotificationManager.Instance.QuestionNotification("Are you sure you want to buy " + sd.Quantity + "x " + sd.name, sd.Buy, null);
            }
        }
    }

    public void Up()
    {
        iter++;
        ChangeMenu();
    }

    public void Down()
    {
        iter--;
        ChangeMenu();
    }

    public void ChangeMenu()
    {
        if (iter > Menus.Count-1)
        {
            iter = 0;
        }
        else if (iter < 0)
        {
            iter = Menus.Count-1;
        }
        cMenu.SetActive(false);
        cMenu = Menus[iter];
        cMenu.SetActive(true);
    }

	// Use this for initialization
	void Start ()
    {
        Instance = this;
        Menus[iter].SetActive(true);
        cMenu = Menus[iter];
        foreach (ShopData sd in shopItems)
        {
            sd.SetText();
        }
	}

    public void RefreshCoins()
    {
        myCoins.text = LocalDataManager.Instance.GetData().PocketCoins.ToString();
    }

}

[System.Serializable]
public class ShopData
{
    public string name = "Super StrawBerry";
    public int ItemID;
    public string ButtonID;
    public int PocketCoinsCost = 10;
    public int Quantity = 1;
    public Text costText;

    public void Buy()
    {
        if (!LocalDataManager.Instance.GetItemInventory().CanBuyMore(ItemID))
        {
            NotificationManager.Instance.ErrorNotification("You already have the max amount of this Item!");
        }
        else
        {
            if (!LocalDataManager.Instance.TryBuySomething(PocketCoinsCost))
            {
                NotificationManager.Instance.ErrorNotification("You Dont have enough coins! Please buy more!");
            }
            else
            {
                ItemData id = AssetManager.Instance.GetItemByID(ItemID);
                id.numberOwned = Quantity;
                LocalDataManager.Instance.AddItem(id);
            }
        }
    }

    public void SetText()
    {
        costText.text = PocketCoinsCost.ToString();
    }
}
