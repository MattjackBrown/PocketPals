using Mapbox.Unity.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpotManager : MonoBehaviour
{
    static public ResourceSpotManager Instance { set; get; }

    private List<GameObject> ResourceSpots = new List<GameObject>();

    private bool shouldSpawn = true;

    //Ref to the map, to set the parent of the resourceSpots once they spawn
    private GPS gpsMap;

    //Used for girl distance and To get the gps map
    public GameObject girl;

    public GameObject ResourceSpotPrefab;

    public float spread = 1.3f;

    public int number = 15;

    // Use this for initialization
    void Start ()
    {
        gpsMap = girl.GetComponent<GPS>();
    }
    private void Awake()
    {
        Instance = this;
    }

    public IEnumerator Spawn()
    {

        while (shouldSpawn)
        {

            //check to make sure we have refs to the required classes
            if (GPS.Insatance == null || GPS.Insatance.currentMap == null || ContentGenerator.Instance == null) yield return new WaitForSeconds(1);

            List<Vector2> LatLonPositions =  ContentGenerator.Instance.GenerateResourceSpots(GPS.Insatance.GetLatLon().x, GPS.Insatance.GetLatLon().y, number);

            if (LatLonPositions != null)
            {
                DespawnAll();

                foreach (Vector2 v2 in LatLonPositions)
                {

                    Vector3 spawnpos = GPS.Insatance.GetWorldPos(v2.x, v2.y);
                    spawnpos.y = ResourceSpotPrefab.transform.position.y;
                    // Create an instance of the prefab at select pocketpal via rarity 
                    GameObject clone = Instantiate(ResourceSpotPrefab, spawnpos, ResourceSpotPrefab.transform.rotation);
                    // Increases the currentPocketPals value by 1
                    clone.transform.parent = GPS.Insatance.currentMap.transform;

                    ResourceSpots.Add(clone);
                }

                ContentGenerator.Instance.WipeResouceSpots();
            }

            yield return new WaitForSeconds(1);
            
        }
    }

    private void TryDespawnFirstSet()
    {
        if (ResourceSpots.Count > number * 4)
        {
            for (int i = 0; i < number; i++)
            {
                Destroy(ResourceSpots[i]);
                ResourceSpots.Remove(ResourceSpots[i]);              
            }
        }
        ContentGenerator.Instance.RemoveFirstSeed();
    }

    private void DespawnAll()
    {
        for (int i = 0; i < ResourceSpots.Count; i++)
        {
            Destroy(ResourceSpots[i]);
        }
        ResourceSpots.Clear();
    }

}
