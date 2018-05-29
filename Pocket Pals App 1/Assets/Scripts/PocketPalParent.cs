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

    public Vector3 Rotation = new Vector3(0, 15, 0);

    public Vector3 Scale = new Vector3(1, 1, 1);

    public Vector3 Offset = new Vector3(0, 0, 0);

    public Sprite boarder;

    public float maxSpawnExp = 1000.0f;

    public float averageSpawnSize = 5.0f;
    public float sizeVariance = 0.5f;

    private PocketPalData pocketPalData;

    [Tooltip("The rarity of the spawn")]
    public float Rarity = 10.0f;
    
	// Use this for initialization
	void Start ()
    {
        transform.position += Offset;
        //Some of the models are very small this will allow you to scale them
        transform.localScale = Vector3.Scale(transform.localScale, Scale);

        boarder  = Instantiate(boarder);

	}

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(collision.gameObject);
    }

    // Update is called once per frame
    void Update ()
    {
        // rotates the object on the x, y and z axis
        transform.Rotate(Rotation * Time.deltaTime); 
    }

    public void Captured()
    {
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

        pocketPalData = new PocketPalData("None", PocketPalID,exp, size, 0 );
    }

    public PocketPalData GetAnimalData(){ return pocketPalData; }

}
