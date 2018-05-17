using Mapbox.Unity.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is responsible for the spawning of Pocket Pals
// The script is not as sophisticated as required, but is approproate for the prototype and testing

public class PocketPalSpawnManager : MonoBehaviour
{
    // The Pocket Pal prefabs to be spawned
    public GameObject[] PocketPals;

    // How long between each spawn
    public float avgSpawnTime = 2f;

    //Spawn Variance in percent
    [Tooltip("Max percentage change of the avereage spawn time.")]
    public float spawnTimeVariance = 50.0f;
    private float normalisedVariance = 0.0f;

    // An array of spawn points Pocket Pals can spawn from.
    public Transform[] spawnPoints;

    //List of the rarities of the index
    private List<float> rarityList = new List<float>();

    bool shouldSpawn = true;
    
    //Ref to the map, to set the parent of the pocketpals once they spawn
    public GPS gpsMap;

    // The max number of Pocket Pals that can spawn at any one time
    public int maxPocketPals = 20;               

    //List of all the spawned pocketpals
    private List<GameObject> spawnedPocketPals = new List<GameObject>();

	void Start ()
	{
        //Iter throught the PocketPalscripts and set their IDs
        foreach (GameObject o in PocketPals)
        {
            rarityList.Add(o.GetComponent<PocketPalParent>().Rarity);
        }

        //making it percentage based seem easier too understand, but harder to work with.
        normalisedVariance = spawnTimeVariance / 100;

        // Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time
        StartCoroutine(Spawn());
	}

    public void PocketpalCollected(GameObject obj)
    {
        spawnedPocketPals.Remove(obj);
    }

    private GameObject GetWeightedPocketPal()
    {
        int index = Sampler(rarityList);
        return PocketPals[index];
    }

    private float GetSpawnDelay()
    {
        return Random.Range(avgSpawnTime - normalisedVariance, avgSpawnTime + normalisedVariance);
    }

    private IEnumerator Spawn()
    {
        bool StartDelay = true;
        while (shouldSpawn)
        {
            if (StartDelay)
            {
                yield return new WaitForSeconds(GetSpawnDelay());
            }

            // Checks whether the max number of Pocket Pals have been spawned 
            if (spawnedPocketPals.Count >= maxPocketPals || !gpsMap.isInitialised)    
            {
                yield return new WaitForSeconds(GetSpawnDelay());
            }
            else
            {
                // Find a random index between zero and one less than the number of spawn points
                int spawnPointIndex = Random.Range(0, spawnPoints.Length);
                int RandomPocketPal = Random.Range(0, PocketPals.Length);

                //select spawn point's position and rotation
                Vector3 pos = spawnPoints[spawnPointIndex].position;
                Quaternion rot = spawnPoints[spawnPointIndex].rotation;

                // Create an instance of the prefab at select pocketpal via rarity 
                GameObject clone = Instantiate(GetWeightedPocketPal(), pos, rot);
                spawnedPocketPals.Add(clone);

                // Increases the currentPocketPals value by 1
                clone.transform.parent = gpsMap.currentMap.transform;

                yield return new WaitForSeconds(GetSpawnDelay());
            }
        }
    }

    private static int Sampler(List<float> weightings)
    {
        int index = 0;
        float total = 0;

        //get total of floats
        foreach (float f in weightings)
        {
            total += f;
        }

        //if they all have no probability return random
        if (total == 0) return Random.Range(0, weightings.Count);

        //seudo-random sampling
        float accum = 0;
        float tmp = Random.Range(0, total);
        for (int i = 0; i < weightings.Count; i++)
        {
            accum += weightings[i];
            if (accum >= tmp)
            {
                index = i;
                break;
            }
        }

        return index;
    }
}