/**
 * Author Tristan Barlow, Rich Steele
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PocketPalParent : MonoBehaviour
{

    //this will be the ID for each pocketpal this should be unique to each pocketpal and used to identify it for the inventory system. 
    public int PocketPalID = 0;

    public Vector3 RotationPerSecond = new Vector3(0, 15, 0);

    //used to offset some of the flying creatures
    public Vector3 SpawnOffset = new Vector3(0, 0, 0);

    public float maxSpawnExp = 1000.0f;

    public float averageSpawnSize = 5.0f;
    public float sizeVariance = 0.5f;

    public float averageAgro = 5.0f;
    public float agroVariance = 0.5f;

    public string name = "none";

    public bool InMinigame { set; get; }

    private PocketPalData pocketPalData;

    [Tooltip("The rarity of the spawn")]
    public float Rarity = 10.0f;
    
	// Use this for initialization
	void Start ()
    {
        transform.position += SpawnOffset;
        InMinigame = false;
	}

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(collision.gameObject);
    }

    // Update is called once per frame
    void Update ()
    {
		if (!InMinigame)
        	// rotates the object on the x, y and z axis
        	transform.Rotate(RotationPerSecond * Time.deltaTime); 
    }

    public void Captured()
    {
		if (gameObject != null)
        	PocketPalSpawnManager.Instance.PocketpalCollected(gameObject);
    }

    //called on the pocketPalSpawnManager after a new clone is made of the original asset.
    public void GenerateAnimalData()
    {
        //get spawn exp
        float exp = Random.Range(0,maxSpawnExp);

        //get random size inbetween points using size variance.
        float tSizeVar = averageSpawnSize * sizeVariance;
        float size = Random.Range(averageSpawnSize - tSizeVar, averageSpawnSize + tSizeVar);

        float tAgroVar = averageAgro * agroVariance;
        float agro = Random.Range(averageAgro - tAgroVar, averageAgro + tAgroVar);

        pocketPalData = new PocketPalData(name, PocketPalID,exp, size, agro, Rarity);
    }

    public PocketPalData GetAnimalData(){ return pocketPalData; }

}
