using Mapbox.Unity.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpotManager : MonoBehaviour
{

    private List<GameObject> ResourceSpots = new List<GameObject>();

    private bool shouldSpawn = true;

    //Ref to the map, to set the parent of the resourceSpots once they spawn
    private GPS gpsMap;

    //Used for girl distance and To get the gps map
    public GameObject girl;

    public GameObject ResourceSpotPrefab;

    public float spread = 1.3f;

    public int number = 20;

    // Use this for initialization
    void Start ()
    {
        gpsMap = girl.GetComponent<GPS>();
        StartCoroutine(Spawn());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(1);
        while (shouldSpawn)
        {
            List<Vector2> LatLonPositions =  ContentGenerator.Instance.GenerateResourceSpots(GPS.Insatance.GetLatLon().x, GPS.Insatance.GetLatLon().y, number, spread);

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
            }

            ContentGenerator.Instance.WipeResouceSpots();

            yield return new WaitForSeconds(10);
            
        }
    }

    private void DespawnAll()
    {
        for (int i = 0; i < ResourceSpots.Count; i++)
        {
            DespawnResourceSpot(ResourceSpots[i]);
        }
    }

    private void DespawnResourceSpot(GameObject obj)
    {
        ResourceSpots.Remove(obj);
        Destroy(obj);
    }
}
