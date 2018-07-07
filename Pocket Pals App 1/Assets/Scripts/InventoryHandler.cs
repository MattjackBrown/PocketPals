using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryHandler : MonoBehaviour
{
    public Text MagnifyingGlassText;
    public Text BerriesText;
    public Text StrawberriesText;

    private void OnEnable()
    {
        ItemInventory ii= LocalDataManager.Instance.GetItemInventory();
        UpdateTextValue(ii.GetNumberOfItem(GlobalVariables.BerryID), BerriesText);
        UpdateTextValue(ii.GetNumberOfItem(GlobalVariables.StrawBerriesID), StrawberriesText);
        UpdateTextValue(ii.GetNumberOfItem(GlobalVariables.MagnifyingGlassID), MagnifyingGlassText);
    }

    private void UpdateTextValue(int number, Text tx)
    {
        tx.text = "x" + number.ToString();
    }
}
