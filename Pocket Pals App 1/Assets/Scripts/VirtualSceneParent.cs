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
            obj.animalObj.SetActive(false);
            if (obtainedAnimals.Contains(obj.ID))
            {
                Debug.Log(obj.ID);
                obj.animalObj.SetActive(true);
            }
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

		// If the current index is the end of the array, then set as zero
		if (currentLookedAtPPalIndex >= AnimalObjects.Length-1)
			currentLookedAtPPalIndex = 0;
		else
			// else increment index
			currentLookedAtPPalIndex++;

		// Return the GameObject of that index in the AnimalObjects
		return AnimalObjects [currentLookedAtPPalIndex].animalObj;
	}

	public GameObject GetPreviousPPal () {

		// If the current index is the start of the array, then set as the last
		if (currentLookedAtPPalIndex == 0)
			currentLookedAtPPalIndex = AnimalObjects.Length-1;
		else
			// else increment index
			currentLookedAtPPalIndex--;

		// Return the GameObject of that index in the AnimalObjects
		return AnimalObjects [currentLookedAtPPalIndex].animalObj;
	}
}

[Serializable]
public class VirtualGardenSpawn
{
    public int ID;
    public bool Used;
    public GameObject animalObj;
}
