using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryHandler : MonoBehaviour
{

    public static InventoryHandler Instance { set; get; }

    public Text MagnifyingGlassText;
    public Text BerriesText;
    public Text StrawberriesText;
    public Text GoodCamera;
    public Text StandardCamera;

    private void Start()
    {
        Instance = this;
        LevelCalculator.SimulateLevels(500000);
    }

    public void Enabled()
    {
        ItemInventory ii = LocalDataManager.Instance.GetItemInventory();
        UpdateTextValue(ii.GetNumberOfItem(GlobalVariables.BerryID), BerriesText);
        UpdateTextValue(ii.GetNumberOfItem(GlobalVariables.StrawBerriesID), StrawberriesText);
        UpdateTextValue(ii.GetNumberOfItem(GlobalVariables.MagnifyingGlassID), MagnifyingGlassText);
        UpdateTextValue(ii.GetNumberOfItem(4), GoodCamera);
        UpdateTextValue(ii.GetNumberOfItem(5), StandardCamera);
    }


    private void UpdateTextValue(int number, Text tx)
    {
        tx.text = "x" + number.ToString();
    }
}
