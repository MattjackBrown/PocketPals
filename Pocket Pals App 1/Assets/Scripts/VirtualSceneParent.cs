using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VirtualSceneParent : MonoBehaviour
{
    public VirtualGardenSpawn[] AnimalObjects;

    public VGUIManager gUIManager;

	// Used to cycle through the virtual garden's PPals
	int currentLookedAtPPalIndex = 0;

    private void OnEnable()
    {
        bool hasAPocketPal = false;

        foreach (VirtualGardenSpawn obj in AnimalObjects)
        {
            //make sure all are inactive and not used, unless check is correct.
			obj.animalObj.SetActive (false);
			obj.Used = false;

            //Check to see if the player has the pocketpal.
            PocketPalData data = LocalDataManager.Instance.TryGetPocketPal(obj.ID);
            if (data != null)
            {
                hasAPocketPal = true;

                //set the animal obj to have the data of the collected pocketpal.
                obj.SetAnimalData(data);

                //we Know the player owns the pocketpal so make used true and make it active in scene
				obj.Used = true;
                obj.animalObj.SetActive(true);
            }
        }
		if (hasAPocketPal)
        { 
			// Initialise the idle camera action variables when no touches
			CameraController.Instance.VGInitLookAtNextPPal (GetNextPPal ());
		}
    }

    public void SetObtained(int id)
    {
        GetGardenSpawn(id).animalObj.SetActive(true);
    }

    public VirtualGardenSpawn GetGardenSpawn(int id)
    {
        foreach (VirtualGardenSpawn vgs in AnimalObjects)
        {
            if (vgs.ID == id) return vgs;
        }
        return null;
    }

	public GameObject GetNextPPal () {

		foreach (VirtualGardenSpawn PPal in AnimalObjects) {
			// If the current index is the end of the array, then set as zero
			if (currentLookedAtPPalIndex >= AnimalObjects.Length - 1)
				currentLookedAtPPalIndex = 0;
			else
				// else increment index
				currentLookedAtPPalIndex++;

			// Look at that index value
			var indexVGS = AnimalObjects [currentLookedAtPPalIndex];

			// Next check whether it is in the inventory
			if (indexVGS.Used)
            {

                //Set the inspect data in the virtual garden UI manager
                gUIManager.SetInspectData(AnimalObjects[currentLookedAtPPalIndex].GetAnimalData());

                // Return the GameObject of that index in the AnimalObjects
                return indexVGS.animalObj;
			}
		}
		return null;
	}

	public GameObject GetPreviousPPal () {

		foreach (VirtualGardenSpawn PPal in AnimalObjects) {
			// If the current index is the start of the array, then set as the last
			if (currentLookedAtPPalIndex == 0)
				currentLookedAtPPalIndex = AnimalObjects.Length - 1;
			else
			// else deccrement index
			currentLookedAtPPalIndex--;

			// Look at that index value
			var indexVGS = AnimalObjects [currentLookedAtPPalIndex];

			// Next check whether it is in the inventory
			if (indexVGS.Used) {

                //Set the inspect data in the virtual garden UI manager
                gUIManager.SetInspectData(AnimalObjects[currentLookedAtPPalIndex].GetAnimalData());

				// Return the GameObject of that index in the AnimalObjects
				return indexVGS.animalObj;
			}
		}
		return null;
	}
}

[Serializable]
public class VirtualGardenSpawn
{
    public int ID;
    public bool Used;
    public GameObject animalObj;
    private PocketPalData animalData;

    public PocketPalData GetAnimalData() { return animalData; }
    public void SetAnimalData(PocketPalData ppd) { animalData = ppd; }
}
