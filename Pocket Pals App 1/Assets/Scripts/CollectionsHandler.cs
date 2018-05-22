using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionsHandler : MonoBehaviour {

    public Image[] AvailableFrames;

    public void OnEnable()
    {
        RefreshImages();
    }

    public void RefreshImages()
    {
        int iter = 0;
        foreach (GameObject obj in LocalDataManager.Instance.GetInventory().GetXMostRecent(AvailableFrames.Length))
        {
            PocketPalParent p = (PocketPalParent)obj.GetComponent("PocketPalParent");
            if (p.boarder != null)
            {
                AvailableFrames[iter].overrideSprite = Instantiate(p.boarder);
                Debug.Log("woot");
            }
            Debug.Log("I tried");
            iter++;
        }
    }
}
