using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

	// Poor naming convention of free but I had already created all the references in editor and it took a while.
	// Read 'free' as 'available to choose from' from here on.
	public List<Texture2D>
	freeHairMatChoices, paidHairMatChoices,
	freeBagChoices,
	freeShirtChoices, paidShirtChoices,
	freeShortsChoices,
	freeSkinChoices,
	freeBootsChoices, paidBootsChoices;

	int hairMeshIndex, hairMatIndex, bagIndex, shirtIndex, shortsIndex, skinIndex, bootsIndex, poseIndex;

	public bool customisationKitUnlocked = false;
	public CharacterStyleData CharStyleData;

	public Main_character_animator_controller poseController;
	public NewCharAnimControllerScript animController;

	public void Start () {

		// Just in case, initialise the var
		CharStyleData = new CharacterStyleData ();

		UpdateMenuCharacter ();
	}

	// Populates the lists further if the add on items have been purchased
	public void InitChoices () {

		// If clothes kit bought, make sure that the lists contain the unlocked items
		if (customisationKitUnlocked) {

			foreach (Mesh mesh in paidHairChoices) {
				if (!freeHairChoices.Contains (mesh)) {
					freeHairChoices.Add (mesh);
				}
			}

			foreach (Texture2D tex in paidHairMatChoices) {
				if (!freeHairMatChoices.Contains (tex)) {
					freeHairMatChoices.Add (tex);
				}
			}

			foreach (Texture2D tex in paidShirtChoices) {
				if (!freeShirtChoices.Contains (tex)) {
					freeShirtChoices.Add (tex);
				}
			}

			foreach (Texture2D tex in paidBootsChoices) {
				if (!freeBootsChoices.Contains (tex)) {
					freeBootsChoices.Add (tex);
				}
			}
		}

		// Just in case, initiate vars
		hairMeshIndex = 0;
		hairMatIndex = 0;
		bagIndex = 0;
		shirtIndex = 0;
		shortsIndex = 0;
		skinIndex = 0;
		bootsIndex = 0;
		poseIndex = 0;

		// Update the indexes to the correct current values for the currently equipped load out
		hairMeshIndex = freeHairChoices.FindIndex (a => a == realHairMesh);
		hairMatIndex = freeHairMatChoices.FindIndex (b => b == realHairMaterial);
		bagIndex = freeBagChoices.FindIndex (c => c == realBagMaterial);
		shirtIndex = freeShirtChoices.FindIndex (d => d == realShirtMaterial);
		shortsIndex = freeShortsChoices.FindIndex (e => e == realShortsMaterial);

		// Copy loadout onto the menu avatar
		UpdateMenuCharacter ();

		// Set as idle anim
		animController.Idle ();
	}

	// Used on menu init. Copies the loadout from the map character onto the temporary menu character
	public void UpdateMenuCharacter () {

		menuHairMesh.mesh = realHairMesh.mesh;
		menuHairMaterial = realHairMaterial;
		menuBagMaterial = realBagMaterial;
		menuShirtMaterial = realShirtMaterial;
		menuShortsMaterial = realShortsMaterial;
		menuSkinMaterial = realSkinMaterial;
		menuBootsMaterial = realBootsMaterial;
	}

	// Will be used for the button press to exit menu
	public void Exit () {

		// Update the map character with any changes made
		UpdateRealCharacter ();

		// Need to swap out the UI, and change controls or anything else
	}

	// To be called when exiting the customisation pages, updating the actual map character from the temporary model
	public void UpdateRealCharacter () {

		realHairMesh.mesh = menuHairMesh.mesh;
		realHairMaterial = menuHairMaterial;
		realBagMaterial = menuBagMaterial;
		realShirtMaterial = menuShirtMaterial;
		realShortsMaterial = menuShortsMaterial;
		realSkinMaterial = menuSkinMaterial;
		realBootsMaterial = menuBootsMaterial;
	}

	// Updates the map character with the passed in load out. Presumably from a saved file
	public void LoadSavedLoadOut (CharacterStyleData savedCharStyleData) {

		// Update the Lists
		InitChoices ();

		realHairMesh.mesh = freeHairChoices [savedCharStyleData.m_HairMeshID];
		realHairMaterial.mainTexture = freeHairMatChoices [savedCharStyleData.m_HairMatID];
		realBagMaterial.mainTexture = freeBagChoices [savedCharStyleData.m_BagID];
		realShirtMaterial.mainTexture = freeShirtChoices [savedCharStyleData.m_ShirtID];
		realShortsMaterial.mainTexture = freeShortsChoices [savedCharStyleData.m_ShortsID];
		realSkinMaterial.mainTexture = freeSkinChoices [savedCharStyleData.m_SkinID];
		realBootsMaterial.mainTexture = freeBootsChoices [savedCharStyleData.m_BootsID];

		// Might as  well update the other menu model while we're here
		UpdateMenuCharacter ();
	}

	// To be used by the data handler to get the load out to save. Feel free to change all this @Tris
	public CharacterStyleData GetCurrentLoadOut () {
		return new CharacterStyleData (hairMeshIndex, hairMatIndex, bagIndex, shirtIndex, shortsIndex, skinIndex, bootsIndex, poseIndex);
	}




	// Hair mesh
	public void SeeNextHairMeshChoice () {

		if (hairMeshIndex == freeHairChoices.Count-1) {
			hairMeshIndex = 0;
		} else {
			hairMeshIndex++;
		}

		menuHairMesh.mesh = freeHairChoices [hairMeshIndex];
	}

	public void SeePreviousHairMeshChoice () {

		if (hairMeshIndex == 0) {
			hairMeshIndex = freeHairChoices.Count-1;
		} else {
			hairMeshIndex--;
		}

		menuHairMesh.mesh = freeHairChoices [hairMeshIndex];
	}

	// Hair material
	public void SeeNextHairMaterialChoice () {

		if (hairMatIndex == freeHairMatChoices.Count-1) {
			hairMatIndex = 0;
		} else {
			hairMatIndex++;
		}

		menuHairMaterial.mainTexture = freeHairMatChoices [hairMatIndex];
	}

	public void SeePreviousHairMaterialChoice () {

		if (hairMatIndex == 0) {
			hairMatIndex = freeHairMatChoices.Count-1;
		} else {
			hairMatIndex--;
		}

		menuHairMaterial.mainTexture = freeHairMatChoices [hairMatIndex];
	}

	// Bag
	public void SeeNextBagChoice () {

		if (bagIndex == freeBagChoices.Count-1) {
			bagIndex = 0;
		} else {
			bagIndex++;
		}

		menuBagMaterial.mainTexture = freeBagChoices [bagIndex];
	}

	public void SeePreviousBagChoice () {

		if (bagIndex == 0) {
			bagIndex = freeBagChoices.Count-1;
		} else {
			bagIndex--;
		}

		menuBagMaterial.mainTexture = freeBagChoices [bagIndex];
	}

	// Shirt
	public void SeeNextShirtChoice () {

		if (shirtIndex == freeShirtChoices.Count-1) {
			shirtIndex = 0;
		} else {
			shirtIndex++;
		}

		menuShirtMaterial.mainTexture = freeShirtChoices [shirtIndex];
	}

	public void SeePreviousShirtChoice () {

		if (shirtIndex == 0) {
			shirtIndex = freeShirtChoices.Count-1;
		} else {
			shirtIndex--;
		}

		menuShirtMaterial.mainTexture = freeShirtChoices [shirtIndex];
	}

	// Shorts
	public void SeeNextShortsChoice () {

		if (shortsIndex == freeShortsChoices.Count-1) {
			shortsIndex = 0;
		} else {
			shortsIndex++;
		}

		menuShortsMaterial.mainTexture = freeShortsChoices [shortsIndex];
	}

	public void SeePreviousShortsChoice () {

		if (shortsIndex == 0) {
			shortsIndex = freeShortsChoices.Count-1;
		} else {
			shortsIndex--;
		}

		menuShortsMaterial.mainTexture = freeShortsChoices [shortsIndex];
	}

	// Skin
	public void SeeNextSkinChoice () {

		if (skinIndex == freeSkinChoices.Count-1) {
			skinIndex = 0;
		} else {
			skinIndex++;
		}

		menuSkinMaterial.mainTexture = freeSkinChoices [skinIndex];
	}

	public void SeePreviousSkinChoice () {

		if (skinIndex == 0) {
			skinIndex = freeSkinChoices.Count-1;
		} else {
			skinIndex--;
		}

		menuSkinMaterial.mainTexture = freeSkinChoices [skinIndex];
	}

	// Boots
	public void SeeNextBootsChoice () {

		if (bootsIndex == freeBootsChoices.Count-1) {
			bootsIndex = 0;
		} else {
			bootsIndex++;
		}

		menuBootsMaterial.mainTexture = freeBootsChoices [bootsIndex];
	}

	public void SeePreviousBootsChoice () {

		if (bootsIndex == 0) {
			bootsIndex = freeBootsChoices.Count-1;
		} else {
			bootsIndex--;
		}

		menuBootsMaterial.mainTexture = freeBootsChoices [bootsIndex];
	}

	// All these just to make the button presses all coordinate through this script
	// TODO Make the green ticks switch over if successful
	public void ChoosePoseNone () {
		if (customisationKitUnlocked) {
			poseController.ChoosePoseNone ();
		} else {
			// Preview? Show message to say buy?
		}

		// Set the avatar's animation
		animController.Idle ();
	}

	public void ChoosePoseBins () {
		if (customisationKitUnlocked) {
			poseController.ChoosePoseBins ();
		} else {
			// Preview? Show message to say buy?
		}

		// Set the avatar's animation
		animController.Bins ();
	}

	public void ChoosePoseDab () {
		if (customisationKitUnlocked) {
			poseController.ChoosePoseDab ();
		} else {
			// Preview? Show message to say buy?
		}

		// Set the avatar's animation
		animController.Dab ();
	}

	public void ChoosePoseFloss () {
		if (customisationKitUnlocked) {
			poseController.ChoosePoseFloss ();
		} else {
			// Preview? Show message to say buy?
		}

		// Set the avatar's animation
		animController.Floss ();
	}

	public void ChoosePoseNet () {
		if (customisationKitUnlocked) {
			poseController.ChoosePoseNet ();
		} else {
			// Preview? Show message to say buy?
		}

		// Set the avatar's animation
		animController.Net ();
	}

	public void ChoosePoseStar () {
		if (customisationKitUnlocked) {
			poseController.ChoosePoseStar ();
		} else {
			// Preview? Show message to say buy?
		}

		// Set the avatar's animation
		animController.Star ();
	}

	public void ChoosePoseHandsHips () {
		if (customisationKitUnlocked) {
			poseController.ChoosePoseHandsHips ();
		} else {
			// Preview? Show message to say buy?
		}

		// Set the avatar's animation
		animController.HandsHips ();
	}
}


// Basically just holds the load out
public class CharacterStyleData 
{
	public int m_HairMeshID = 0;
	public int m_HairMatID = 0;
	public int m_BagID = 0;
	public int m_ShirtID = 0;
	public int m_ShortsID = 0;
	public int m_SkinID = 0;
	public int m_BootsID = 0;
	public int m_PoseID = 0;

	public CharacterStyleData () {
		
	}

	public CharacterStyleData (int hairMeshID, int hairMatID, int bagID, int shirtID, int shortsID, int skinID, int bootsID, int poseID) {
		m_HairMeshID = hairMeshID;
		m_HairMatID = hairMatID;
		m_BagID = bagID;
		m_ShirtID = shirtID;
		m_ShortsID = shortsID;
		m_SkinID = skinID;
		m_BootsID = bootsID;
		m_PoseID = poseID;
	}
}
