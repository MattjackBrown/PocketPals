using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VirtualSceneParent : MonoBehaviour
{
    public VirtualGardenSpawn[] AnimalObjects;

	// Use this for initialization
	void Start ()
    {
        foreach (VirtualGardenSpawn obj in AnimalObjects)
        {
            if (obj.ID != 0) obj.Used = true;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		
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
