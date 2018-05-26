using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VirtualSceneParent : MonoBehaviour
{
    public VirtualGardenSpawn[] AnimalObjects;

    private void OnEnable()
    {
        List<int> obtainedAnimals =  LocalDataManager.Instance.GetInventory().GetUniqueAnimalIDs();
        foreach (VirtualGardenSpawn obj in AnimalObjects)
        {
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


}

[Serializable]
public class VirtualGardenSpawn
{
    public int ID;
    public bool Used;
    public GameObject animalObj;
}
