using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionsHandler : MonoBehaviour {

    public PocketPalInventory inventory;

    public Image[] AvailableFrames;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnEnable()
    {
        RefreshImages();
    }

    public void RefreshImages()
    {
        int iter = 0;
        foreach (GameObject obj in inventory.GetXMostRecent(AvailableFrames.Length))
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
