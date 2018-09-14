using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpawnList
{
    private List<GameObject> activeSpawnList = new List<GameObject>();
    private List<float> activeRarities = new List<float>();

    private List<int> GeneratedAnimalsIDs = new List<int>();

    public SpawnType listType = SpawnType.none;
    private int currentIndex;

    public SpawnList(SpawnTime time, SpawnType type, int maxUniqueSize)
    {
        FillList(time, type, maxUniqueSize);
    }

    public void FillList(SpawnTime time, SpawnType type,int maxUniqueSize)
    {
        activeSpawnList.Clear();
        activeRarities.Clear();
        listType = type;
        if (AssetManager.Instance != null)
        {
            foreach (GameObject obj in AssetManager.Instance.GetPocketPalsOfType(type))
            {
                if (time == obj.GetComponent<PocketPalParent>().time || obj.GetComponent<PocketPalParent>().time == SpawnTime.all)
                {
                    activeSpawnList.Add(obj);
                    activeRarities.Add(obj.GetComponent<PocketPalParent>().Rarity);
                }
            }
            GeneratedAnimalsIDs =  ContentGenerator.Instance.TryGenerateNewAnimalList("doombar", GPS.Insatance.GetLatLon().x, GPS.Insatance.GetLatLon().y, maxUniqueSize, activeRarities, true);
        }
    }

    public void NewSetOfAnimalsNeeded(int maxUniqueSize)
    {
        GeneratedAnimalsIDs = ContentGenerator.Instance.TryGenerateNewAnimalList("doombar", GPS.Insatance.GetLatLon().x, GPS.Insatance.GetLatLon().y, maxUniqueSize, activeRarities, true);
    }


    public GameObject GetNextAnimal()
    {
        currentIndex++;
        if (currentIndex > GeneratedAnimalsIDs.Count - 1)
        {
            currentIndex = 0;
        }
        return activeSpawnList[ GeneratedAnimalsIDs[currentIndex]];
    }

}
