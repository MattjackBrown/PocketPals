using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomisation : MonoBehaviour {

	public GameObject menuPlayerObject, realPlayerObject;
	public MeshFilter menuHairMesh, realHairMesh;
	public List<Mesh> freeHairChoices, paidHairChoices;

	public Material
	menuHairMaterial, realHairMaterial,
	menuBagMaterial, realBagMaterial,
	menuShirtMaterial, realShirtMaterial,
	menuShortsMaterial, realShortsMaterial,
	menuSkinMaterial, realSkinMaterial,
	menuBootsMaterial, realBootsMaterial;

	public List<Texture2D>
	freeHairMatChoices, paidHairMatChoices,
	freeBagChoices,
	freeShirtChoices, paidShirtChoices,
	freeShortsChoices,
	freeSkinChoices,
	freeBootsChoices, paidBootsChoices;



	// Hair mesh
	public void SeeNextHairMeshChoice () {

//		menuHairMesh.mesh = 
		
	}

	public void SeePreviousHairMeshChoice () {

	}

	// Hair material
	public void SeeNextHairMaterialChoice () {

	}

	public void SeePreviousHairMaterialChoice () {

	}

	// Bag
	public void SeeNextBagChoice () {
		
	}

	public void SeePreviousBagChoice () {

	}

	// Shirt
	public void SeeNextShirtChoice () {

	}

	public void SeePreviousShirtChoice () {

	}

	// Shorts
	public void SeeNextShortsChoice () {

	}

	public void SeePreviousShortsChoice () {

	}

	// Skin
	public void SeeNextSkinChoice () {

	}

	public void SeePreviousSkinChoice () {

	}

	// Boots
	public void SeeNextBootsChoice () {

	}

	public void SeePreviousBootsChoice () {

	}

	public void UpdateMenuCharacter () {

		menuHairMesh = realHairMesh;
		menuHairMaterial = realHairMaterial;
		menuBagMaterial = realBagMaterial;
		menuShirtMaterial = realShirtMaterial;
		menuShortsMaterial = realShortsMaterial;
		menuSkinMaterial = realSkinMaterial;
		menuBootsMaterial = realBootsMaterial;
	}

	public void UpdateRealCharacter () {

		realHairMesh = menuHairMesh;
		realHairMaterial = menuHairMaterial;
		realBagMaterial = menuBagMaterial;
		realShirtMaterial = menuShirtMaterial;
		realShortsMaterial = menuShortsMaterial;
		realSkinMaterial = menuSkinMaterial;
		realBootsMaterial = menuBootsMaterial;
	}



	public class CharacterStyleData 
	{
		public int HairMeshID = 0;
		public int HairMatID = 0;
		public int BagID = 0;
		public int ShirtID = 0;
		public int ShortsID = 0;
		public int SkinID = 0;
		public int BootsID = 0;
		public int PoseID = 0;

	}
		
}
