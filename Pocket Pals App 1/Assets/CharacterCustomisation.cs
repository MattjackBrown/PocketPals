using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCustomisation : MonoBehaviour {

    public static CharacterCustomisation Instance { set; get; }

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
	public MenuCharAnimControllerScript animController;

	public Sprite TickImage;
	public Color untickedColour, tickedColour;
	public Image s_Idle, s_HandsHips, s_Star, s_Bins, s_Net, s_Floss, s_Dab;

	public void Start () {

        Instance = this;

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

			switch (poseIndex) {

			case 0:
				ChoosePoseNone ();
				break;

			case 1:
				ChoosePoseHandsHips ();
				break;

			case 2:
				ChoosePoseStar ();
				break;

			case 3:
				ChoosePoseBins ();
				break;

			case 4:
				ChoosePoseNet ();
				break;

			case 5:
				ChoosePoseFloss ();
				break;

			case 6:
				ChoosePoseDab ();
				break;



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
		animController.DemoIdlePose ();
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

        ServerDataManager.Instance.UpdateCharacterStyleData(GetCurrentLoadOut());
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
        InitChoices();


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

		setAllTogglesFalse ();

		if (customisationKitUnlocked) {
			poseController.ChoosePoseNone ();

			s_Idle.sprite = TickImage;
			s_Idle.color = tickedColour;

			// Set the avatar's animation
			animController.DemoIdlePose ();

			poseIndex = 0;
		} else {
			// Preview? Show message to say buy?
		}
	}

	public void ChoosePoseBins () {

		setAllTogglesFalse ();

		if (customisationKitUnlocked) {
			poseController.ChoosePoseBins ();

			s_Bins.sprite = TickImage;
			s_Bins.color = tickedColour;

			// Set the avatar's animation
			animController.DemoBinsPose ();

			poseIndex = 3;
		} else {
			// Preview? Show message to say buy?
		}
	}

	public void ChoosePoseDab () {

		setAllTogglesFalse ();

		if (customisationKitUnlocked) {
			poseController.ChoosePoseDab ();

			s_Dab.sprite = TickImage;
			s_Dab.color = tickedColour;

			// Set the avatar's animation
			animController.DemoDabPose ();

			poseIndex = 6;
		} else {
			// Preview? Show message to say buy?
		}
	}

	public void ChoosePoseFloss () {

		setAllTogglesFalse ();

		if (customisationKitUnlocked) {
			poseController.ChoosePoseFloss ();

			s_Floss.sprite = TickImage;
			s_Floss.color = tickedColour;

			// Set the avatar's animation
			animController.DemoFlossPose ();

			poseIndex = 5;
		} else {
			// Preview? Show message to say buy?
		}
	}

	public void ChoosePoseNet () {

		setAllTogglesFalse ();

		if (customisationKitUnlocked) {
			poseController.ChoosePoseNet ();

			s_Net.sprite = TickImage;
			s_Net.color = tickedColour;

			// Set the avatar's animation
			animController.DemoNetPose ();

			poseIndex = 4;
		} else {
			// Preview? Show message to say buy?
		}
	}

	public void ChoosePoseStar () {

		setAllTogglesFalse ();

		if (customisationKitUnlocked) {
			poseController.ChoosePoseStar ();

			s_Star.sprite = TickImage;
			s_Star.color = tickedColour;

			// Set the avatar's animation
			animController.DemoStarPose ();

			poseIndex = 2;

		} else {
			// Preview? Show message to say buy?
		}
	}

	public void ChoosePoseHandsHips () {

		setAllTogglesFalse ();

		if (customisationKitUnlocked) {
			poseController.ChoosePoseHandsHips ();

			s_HandsHips.sprite = TickImage;
			s_HandsHips.color = tickedColour;

			// Set the avatar's animation
			animController.DemoHandsPose ();

			poseIndex = 1;

		} else {
			// Preview? Show message to say buy?
		}
	}

	// Toggle stuff for poses
	void setAllTogglesFalse () {
		s_Idle.sprite = null;
		s_HandsHips.sprite = null;
		s_Star.sprite = null;
		s_Bins.sprite = null;
		s_Net.sprite = null;
		s_Floss.sprite = null;
		s_Dab.sprite = null;

		s_Idle.color = untickedColour;
		s_HandsHips.color = untickedColour;
		s_Star.color = untickedColour;
		s_Bins.color = untickedColour;
		s_Net.color = untickedColour;
		s_Floss.color = untickedColour;
		s_Dab.color = untickedColour;
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

	// Constructors
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
