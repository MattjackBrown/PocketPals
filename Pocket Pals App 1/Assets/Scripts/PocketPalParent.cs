/**
 * Author Tristan Barlow, Rich Steele
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public enum SpawnType
{
    none, 
    a_Woodland,
    d_Woodland,
    n_Woodland,
    a_Wetland,
    d_Wetland,
    n_Wetland
}

public class PocketPalParent : MonoBehaviour
{
//	Animator animator;
//	public RuntimeAnimatorController restAnimController;
//	public RuntimeAnimatorController moveAnimController;
//	public Avatar restAvatar;
//	public Avatar moveAvatar;
	public GameObject PPal;
	public float lookAtPointYOffset = 0.0f;

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

    public SpawnType type = SpawnType.none;

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
//		animator = GetComponent<Animator>();
        GenerateAnimalData();
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
    public void GenerateAnimalData(float expMin = -1, float expMax = -1)
    {
        //get spawn exp
        float exp;
        if (expMin != -1 && expMax != -1)
        {
            exp = Random.Range(expMin, expMax);
        }
        else
        {
            exp = Random.Range(0, maxSpawnExp);
        }

        //get random size inbetween points using size variance.
        float tSizeVar = averageSpawnSize * sizeVariance;
        float size = Random.Range(averageSpawnSize - tSizeVar, averageSpawnSize + tSizeVar);

        float tAgroVar = averageAgro * agroVariance;
        float agro = Random.Range(averageAgro - tAgroVar, averageAgro + tAgroVar);

        pocketPalData = new PocketPalData(name, PocketPalID,exp, size, Rarity);
    }

    public PocketPalData GetAnimalData(){ return pocketPalData; }

	public void SetRestAnimation() {
//		animator.avatar = restAvatar;
//		animator.runtimeAnimatorController = restAnimController;
	}

	public void SetMoveAnimation() {
//		animator.avatar = moveAvatar;
//		animator.runtimeAnimatorController = moveAnimController;
	}

	public Vector3 GetLookAtPosition () {
		return new Vector3 (PPal.transform.position.x, PPal.transform.position.y + lookAtPointYOffset, PPal.transform.position.z);
	}

}
