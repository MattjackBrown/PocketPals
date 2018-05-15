using Mapbox.Unity.Map;
using System.Collections.Generic;
using UnityEngine;

// This script is responsible for the spawning of Pocket Pals
// The script is not as sophisticated as required, but is approproate for the prototype and testing

public class PocketPalSpawnManager : MonoBehaviour
{
	public GameObject[] PocketPals;                 // The Pocket Pal prefab to be spawned
	public float spawnTime = 2f;                 // How long between each spawn
	public Transform[] spawnPoints;              // An array of spawn points Pocket Pals can spawn from. There are 20
    public GPS gpsMap;


	public int maxPocketPals = 20;               // The max number of Pocket Pals that can spawn at any one time
	private int currentPocketPals = 0;           // The current number of Pocket Pals that have spawned at one time
    private List<GameObject> spawnedPocketPals = new List<GameObject>();

	void Start ()
	{
        //Iter throught the PocketPalscripts and set their IDs
        int iter = 0;
        foreach (GameObject o in PocketPals)
        {
            iter++;
            PocketPalParent p = o.GetComponent(typeof(PocketPalParent)) as PocketPalParent;
            p.PocketPalID = iter;
        }

		// Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time
		InvokeRepeating ("Spawn", spawnTime, spawnTime);
	}

    public void PocketpalCollected(GameObject obj)
    {
        spawnedPocketPals.Remove(obj);
    }

    private void Reset()
    {
        foreach (GameObject o in spawnedPocketPals)
        {
            if (o != null)
            {
                GameObject.Destroy(o);
            }
        }
        spawnedPocketPals.Clear();
    }

    void Spawn ()
	{

		if(spawnedPocketPals.Count >= maxPocketPals)    // Checks whether the max number of Pocket Pals have been spawned 
		{
            spawnedPocketPals.Clear();
			return;                               // Exits the function if the above statement is true
		}

		// Find a random index between zero and one less than the number of spawn points
		int spawnPointIndex = Random.Range (0, spawnPoints.Length);
        int RandomPocketPal = Random.Range(0, PocketPals.Length);

        Vector3 pos = spawnPoints[spawnPointIndex].position;

        Quaternion rot = spawnPoints[spawnPointIndex].rotation;

        // Create an instance of the prefab at the randomly selected spawn point's position and rotation
        GameObject clone =  (GameObject)Instantiate(PocketPals[RandomPocketPal],pos, rot );
        spawnedPocketPals.Add(clone);
        clone.transform.parent = gpsMap.basicMap.transform;                   // Increases the currentPocketPals value by 1
	}
}