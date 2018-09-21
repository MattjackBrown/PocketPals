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
    Woodland,

    Wetland,

	Coastal
        , none,
    Meadows
}
[System.Serializable]
public enum SpawnTime
{
    day, night, all
}
[System.Serializable]
public enum PPalType
{
    Insect,
    Animal,
    Bird,
    Amphibian,
    Fish,
    Reptile,
    All
}

// Including this script will create a RawImage component
//[RequireComponent(typeof(RawImage))]

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

    private DefaultPocketPalInfo baseData;


    public bool InMinigame { set; get; }

    private PocketPalData pocketPalData;

	public float minPatrolSpeed = 0.0f;
	public float maxPatrolSpeed = 0.0f;

    public Texture rareMorph;
    public Texture defaultTexture;
    public float RareMorphChance = 0.05f;
    public float RareMorphBonusMultiplier = 3.0f;

    private bool isRare = false;


//	public MovieTexture movieTexture;
//	public Texture leafTexture;
//	RawImage leafFlutterRawImage;

    
	// Use this for initialization
	void Start ()
    {
        transform.position += SpawnOffset;
        InMinigame = false;
//		animator = GetComponent<Animator>();
        GenerateAnimalData();


//		leafFlutterRawImage = GetComponent<RawImage> ();


	}

    public DefaultPocketPalInfo GetBaseData()
    {
        return baseData;
    }

    public void SetBaseData(DefaultPocketPalInfo info)
    {
        baseData = info;
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


    public float CheckNewLength(float l)
    {
        if (l > baseData.maxLength ||l < baseData.minLength)
        {
            return Random.Range(baseData.minLength, baseData.maxLength);
        }
        else return l;
    }

    public float CheckNewWeight(float w)
    {
        if (w > baseData.maxWeight || w < baseData.minWeight)
        {
            return Random.Range(baseData.minWeight, baseData.maxWeight);
        }
        else return w;
    }


    //called on the pocketPalSpawnManager after a new clone is made of the original asset.
    public PocketPalData GenerateAnimalData(float expMin = -1, float expMax = -1)
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
        float weight = Random.Range(baseData.minWeight, baseData.maxWeight);

        float length = Random.Range(baseData.minLength, baseData.maxLength);

        pocketPalData = new PocketPalData(baseData.PPalName, PocketPalID, exp, weight ,length, baseData.Rarity);

        TryForRareMorph();

        if (isRare) exp *= RareMorphBonusMultiplier;

        return pocketPalData;
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

    public bool TryForRareMorph()
    {
        if (!CanRareMorph()) return false;
        else
        {
            if (Random.Range(0, 1) < RareMorphChance)
            {
                isRare = true;
                pocketPalData.HasRare = 1;
                ToggleRare(true, GetComponentInChildren<Renderer>());
                
                return true;
            }
            ToggleRare(false, GetComponentInChildren<Renderer>());
            return false;
        }
    }

    public void ToggleRare(bool b, Renderer  r)
    {

        if (b)
        {
            r.material.mainTexture = rareMorph;
        }
        else
        {
            r.material.mainTexture = defaultTexture;
        }

    }

    public bool CanRareMorph()
    {
        return rareMorph != null;
    }
}
