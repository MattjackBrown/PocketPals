using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileHandler : MonoBehaviour
{

    public static PlayerProfileHandler Instance { set; get; }

    public Image progressBar;
    public Text playerLevel;

    public Text mostCaughtText;

    public Text highestLevelText;

    // Use this for initialization
    void Start () {
        Instance = this;
	}

    public void RefreshStats()
    {
        //Try and get the current game data for this player profile
        GameData gd;
        if (LocalDataManager.Instance)
        {
            gd = LocalDataManager.Instance.GetData();
            if (gd == null) return;
        }
        else
        {
            return;
        }


        //Try and get the most caught pocket pal and set the name. 
        //TO DO: Implement a picture of the animal rather than name.
        PocketPalData mostCaught = gd.Inventory.GetMostCaught();
        if (mostCaught != null) mostCaughtText.text = mostCaught.name;
        else mostCaughtText.text = "None";

        //Try and get the highest level pocket pal and set the name. 
        //TO DO: Implement a picture of the animal rather than name.
        PocketPalData highestLevel = gd.Inventory.GetHighestLevel();
        if (highestLevel != null) highestLevelText.text = highestLevel.name;
        else highestLevelText.text = "None";

        //Set the exp part of the canvas.
        playerLevel.text ="Level: " + gd.GetLevel().ToString();
        progressBar.fillAmount = gd.GetPercentageExp();
    }

	// Update is called once per frame
	void Update () {
		
	}
}
