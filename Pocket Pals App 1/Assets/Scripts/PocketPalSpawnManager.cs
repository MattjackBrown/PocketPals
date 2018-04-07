using UnityEngine;

// This script is responsible for the spawning of Pocket Pals
// The script is not as sophisticated as required, but is approproate for the prototype and testing

public class PocketPalSpawnManager : MonoBehaviour
{
	public GameObject[] PocketPals;                 // The Pocket Pal prefab to be spawned
	public float spawnTime = 2f;                 // How long between each spawn
	public Transform[] spawnPoints;              // An array of spawn points Pocket Pals can spawn from. There are 20

	public int maxPocketPals = 10;               // The max number of Pocket Pals that can spawn at any one time
	private int currentPocketPals = 0;           // The current number of Pocket Pals that have spawned at one time

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


	void Spawn ()
	{
		if(currentPocketPals >= maxPocketPals)    // Checks whether the max number of Pocket Pals have been spawned 
		{
			return;                               // Exits the function if the above statement is true
		}

		// Find a random index between zero and one less than the number of spawn points
		int spawnPointIndex = Random.Range (0, spawnPoints.Length);
        int RandomPocketPal = Random.Range(0, PocketPals.Length);

        // Create an instance of the prefab at the randomly selected spawn point's position and rotation
        Instantiate (PocketPals[RandomPocketPal], spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
		currentPocketPals++;                      // Increases the currentPocketPals value by 1
	}
}