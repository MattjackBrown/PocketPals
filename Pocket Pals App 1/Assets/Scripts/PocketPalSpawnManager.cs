using Mapbox.Unity.Map;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// This script is responsible for the spawning of Pocket Pals
// The script is not as sophisticated as required, but is approproate for the prototype and testing

public class PocketPalSpawnManager : MonoBehaviour
{

    public static PocketPalSpawnManager Instance { set; get; }

    //Spawn Variance in percent
    [Tooltip("Max percentage change of the avereage spawn time.")]
    public float minSpawnTime = 50.0f;
    public float maxSpawnTime = 0.0f;

    // An array of spawn locations Pocket Pals can spawn from.
    public List<Transform> spawnPoints;

    bool shouldSpawn = true;

    //Used for girl distance and To get the gps map
    public GameObject girl;
    
    //Ref to the map, to set the parent of the pocketpals once they spawn
    private GPS gpsMap;

    // The max number of Pocket Pals that can spawn at any one time
    public int maxPocketPals = 20;

    //distance before pocketpals despawn
	public int maxPocketPalDistance = 20;

	// The minimum distance allowed between spawning pocket pals to avoid overlaps
	float minimumDistanceBetweenSpawns = 4.0f;

    public Dictionary<SpawnType, SpawnList> spawnLists = new Dictionary<SpawnType, SpawnList>();

    //List of all the spawned pocketpals
    private List<GameObject> spawnedPocketPals = new List<GameObject>();

    public bool TrySyncedSpawns = true;

	private ScreenAnalysis screenAnalysis;

	void Start ()
	{
		Instance = this;

		screenAnalysis = new ScreenAnalysis();

        gpsMap = girl.GetComponent<GPS>();

        // Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time

	}

    public void SetSpawnList(SpawnTime time)
    {
        DespawnAll();

        //Add a list of animals from each enironment
        spawnLists.Add(SpawnType.Woodland, new SpawnList(time, SpawnType.Woodland, maxPocketPals)); 
        spawnLists.Add(SpawnType.Wetland,new SpawnList(time, SpawnType.Wetland, maxPocketPals)); 
        spawnLists.Add(SpawnType.Coastal, new SpawnList(time, SpawnType.Coastal, maxPocketPals)); 
    }

    public void PocketpalCollected(GameObject obj)
    {
        LocalDataManager.Instance.AddPocketPal(obj);
        DespawnPocketPal(obj.GetComponentInParent<PocketPalParent>().gameObject);
    }

    private GameObject GetSyncedPocketPal(Vector3 pos)
    {
        if (ContentGenerator.Instance.CheckForNewAnimalSeed("doombar", GPS.Insatance.GetLatLon().x, GPS.Insatance.GetLatLon().y,false))
        {
            DespawnAll();

            foreach (SpawnList list in spawnLists.Values)
            {
                list.NewSetOfAnimalsNeeded(maxPocketPals);
            }
        }

        switch (screenAnalysis.AnalyseSpawnLocation(pos))
        {
            case SpawnType.Wetland:
                return spawnLists[SpawnType.Wetland].GetNextAnimal();
            case SpawnType.Woodland:
                return spawnLists[SpawnType.Woodland].GetNextAnimal();
            case SpawnType.Coastal:
                return spawnLists[SpawnType.Coastal].GetNextAnimal();
        }

        return AssetManager.Instance.GetPocketPalGameObject(0);
    }

    private float GetSpawnDelay()
    {
        return Random.Range(minSpawnTime, maxSpawnTime);
    }

    public IEnumerator Spawn()
    {
        bool StartDelay = true;

        while (shouldSpawn)
        {
            if (StartDelay)
            {
                StartDelay = false;
                yield return new WaitForSeconds(GetSpawnDelay());
            }

            //check to see if any pps are too far away. If so destroy...
            CheckForDespawns();

            // Checks whether the max number of Pocket Pals have been spawned 
            if (spawnedPocketPals.Count >= maxPocketPals || !gpsMap.GetMapInit())    
            {
                yield return new WaitForSeconds(GetSpawnDelay());
            }
            else
            {
                // Find a random index between zero and one less than the number of spawn points

				// Commented this next line out as not used?
//				int RandomPocketPal = Random.Range(0, AssetManager.Instance.WoodlandPocketPals.Length);

				// Even though these are set, the compiler requires them to be initialised here
				Vector3 spawnPosition = Vector3.zero;
                int spawnPointIndex = 0, tempCount = 0;
				bool validSpawnFound = false;

                //randomise the spawn point list
                spawnPoints = spawnPoints.OrderBy(a => System.Guid.NewGuid()).ToList();

                // Find a randomly chosen spawn position that does not overlap an existing pocketPal
                // As a safety measure, include a count to break out if no valid positions can be found
                while (!validSpawnFound && tempCount < spawnPoints.Count)
                {
                    //get spawn point
					spawnPosition = spawnPoints [tempCount].position;

                    // Check if a valid spawn position
                    if (DoesNotOverlapExistingPPal(spawnPosition))
                    {

                        // If does not overlap then set the bool to true, breaking out of the while loop, and allowing spawning
                        validSpawnFound = true;
                    }
                    else Debug.Log("Bad spawn");

					// Increment count
					tempCount++;
				}

				// If valid spawn position found then spawn, otherwise wait and repeat
				if (validSpawnFound)
                {
                    SpawnPocketPal(spawnPosition);
				}

                yield return new WaitForSeconds(GetSpawnDelay());
            }
        }
    }

    public static int Sampler(System.Random r, List<float> weightings)
    {
        int index = 0;
        float total = 0;

        //get total of floats
        foreach (float f in weightings)
        {
            total += 1/f;
        }

        //if they all have no probability return random
        if (total == 0) return r.Next(0, weightings.Count);

        //seudo-random sampling
        double accum = 0;
        double tmp = r.NextDouble()*total;
        for (int i = 0; i < weightings.Count; i++)
        {
            accum += 1/weightings[i];
            if (accum >= tmp)
            {
                index = i;
                break;
            }
        }

        return index;
    }

    public void DespawnPocketPal(GameObject obj)
    {
        if (spawnedPocketPals.Contains(obj))
        {
            int i = spawnedPocketPals.IndexOf(obj);
            spawnedPocketPals.RemoveAt(i);
        }
        Destroy(obj);
    }

    public void DespawnAll()
    {
        for (int i = 0; i < spawnedPocketPals.Count; i++)
        {
            if (!spawnedPocketPals[i].GetComponent<PocketPalParent>().InMinigame)
            {
                DespawnPocketPal(spawnedPocketPals[i]);
            }  
        }
    }

    private void CheckForDespawns()
    {
        for(int i = 0; i < spawnedPocketPals.Count; i++)
        {
            //not sure why this check is makes it work. Just does.
            if (spawnedPocketPals[i] == null || girl == null)
            {
                //check
            }
            else if (spawnedPocketPals[i].GetComponent<PocketPalParent>().InMinigame)
            {
                // check
            }
            else if (Vector3.Magnitude(girl.transform.position - spawnedPocketPals[i].transform.position) > maxPocketPalDistance)
            {
                DespawnPocketPal(spawnedPocketPals[i]);
            }
        }
    }

    public void SpawnPocketPal(Vector3 pos)
    {
        GameObject ppal;
        Quaternion rot = spawnPoints[0].rotation;

        //Try and get a synced animals spawn
        ppal = GetSyncedPocketPal(pos);

        // Create an instance of the prefab at select pocketpal via rarity 
        GameObject clone = Instantiate(ppal, pos, rot);
        // Increases the currentPocketPals value by 1
        clone.transform.parent = gpsMap.currentMap.transform;

        spawnedPocketPals.Add(clone);

        TouchHandler.Instance.Vibrate();
    }

	bool DoesNotOverlapExistingPPal (Vector3 spawnPosition) {

		// Check position against all currently spawned PPal positions
		foreach (GameObject PPal in spawnedPocketPals) {

			// If deemed too close
			if ((spawnPosition - PPal.transform.position).magnitude < minimumDistanceBetweenSpawns) {

				// Break out early returning false
				return false;
			}
		}

		// If all PPal distance checks pass then return true
		return true;
	}
}