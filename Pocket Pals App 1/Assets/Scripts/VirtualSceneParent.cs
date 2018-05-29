using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VirtualSceneParent : MonoBehaviour
{
    public VirtualGardenSpawn[] AnimalObjects;

	// Used to cycle through the virtual garden's PPals
	int currentLookedAtPPalIndex = 0;

    private void OnEnable()
    {
        List<int> obtainedAnimals =  LocalDataManager.Instance.GetInventory().GetUniqueAnimalIDs();
        foreach (VirtualGardenSpawn obj in AnimalObjects)
        {
			obj.animalObj.SetActive (false);
			obj.Used = false;
            if (obtainedAnimals.Contains(obj.ID))
            {
                Debug.Log(obj.ID);
                obj.animalObj.SetActive(true);

				// Re Triss? What's this used for? I'm using it for whether it is in the inventory/whether it should be in the scene for the
				// getNextPPal() and getPrevPPal()
				obj.Used = true;
            }
        }
		if (obtainedAnimals.Count > 0) {
			
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
			if (indexVGS.Used) {
				
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
}
