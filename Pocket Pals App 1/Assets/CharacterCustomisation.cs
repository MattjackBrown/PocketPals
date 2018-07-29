using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCustomisation : MonoBehaviour {

    public static CharacterCustomisation Instance { set; get; }

    public GameObject menuPlayerObject, realPlayerObject;
    public MeshFilter menuHairMesh, realHairMesh;

    public Material
    menuHairMaterial, realHairMaterial,
    menuBagMaterial, realBagMaterial,
    menuShirtMaterial, realShirtMaterial,
    menuShortsMaterial, realShortsMaterial,
    menuSkinMaterial, realSkinMaterial,
    menuBootsMaterial, realBootsMaterial;

    // Poor naming convention of free but I had already created all the references in editor and it took a while.
    // Read 'free' as 'available to choose from' from here on.
    public List<TextureType>
    HairMatChoices,
    BagChoices,
    ShirtChoices,
    ShortsChoices,
    SkinChoices,
    BootsChoices;

    public List<MeshType> HairChoices;

    public bool customisationKitUnlocked = false;
    public CharacterStyleData cData;
    public CharacterStyleData oldData = new CharacterStyleData();

    public Main_character_animator_controller poseController;
    public MenuCharAnimControllerScript animController;

    public Sprite TickImage;
    public Color untickedColour, tickedColour;
    public Image s_Idle, s_HandsHips, s_Star, s_Bins, s_Net, s_Floss, s_Dab;

    public JournalNewScript jns;

    public void Start() {

        Instance = this;

        // Just in case, initialise the var
        cData = new CharacterStyleData();

        UpdateMenuCharacter();
    }

    // Populates the lists further if the add on items have been purchased
    public void LoadPoseChoices()
    {

        // If clothes kit bought, make sure that the lists contain the unlocked items
        if (customisationKitUnlocked)
        {
            switch (cData.m_PoseID) {

                case 0:
                    ChoosePoseNone();
                    break;

                case 1:
                    ChoosePoseHandsHips();
                    break;

                case 2:
                    ChoosePoseStar();
                    break;

                case 3:
                    ChoosePoseBins();
                    break;

                case 4:
                    ChoosePoseNet();
                    break;

                case 5:
                    ChoosePoseFloss();
                    break;

                case 6:
                    ChoosePoseDab();
                    break;
            }
        }

        // Set as idle anim
        animController.DemoIdlePose();
    }

    // Used on menu init. Copies the loadout from the map character onto the temporary menu character
    public void UpdateMenuCharacter() {

        menuHairMesh.mesh = realHairMesh.mesh;
        menuHairMaterial = realHairMaterial;
        menuBagMaterial = realBagMaterial;
        menuShirtMaterial = realShirtMaterial;
        menuShortsMaterial = realShortsMaterial;
        menuSkinMaterial = realSkinMaterial;
        menuBootsMaterial = realBootsMaterial;
    }

    public void Init()
    {
        oldData = cData.Clone();
        UpdateMenuCharacter();
    }

	// Will be used for the button press to exit menu
	public void Exit ()
    {
        if (!cData.IsMatch(oldData))
        {
            NotificationManager.Instance.QuestionNotification("Would You Like To Apply and Save?", ApplyAndSave, CancelSave);
        }
        else
        {
            jns.ExitBackToMap();
        }
	}

    public void ApplyAndSave()
    {
        // Update the map character with any changes made
        UpdateRealCharacter();

		LocalDataManager.Instance.SaveCharStyle(GetCurrentLoadOut());

        jns.ExitBackToMap();
    }

    public void CancelSave()
    {
        cData = oldData;
        UpdateMenuCharacter();
    }

	// To be called when exiting the customisation pages, updating the actual map character from the temporary model
	public void UpdateRealCharacter ()
    {

        realHairMesh.mesh = GetMeshType(HairChoices, cData.m_HairMeshID).mesh.sharedMesh;
        realHairMaterial.mainTexture = GetTextureType(HairMatChoices, cData.m_HairMatID).tex;
        realBagMaterial.mainTexture = GetTextureType(BagChoices, cData.m_BagID).tex;
        realShirtMaterial.mainTexture = GetTextureType(ShirtChoices, cData.m_ShirtID).tex;
        realShortsMaterial.mainTexture = GetTextureType(ShortsChoices, cData.m_ShortsID).tex;
        realSkinMaterial.mainTexture = GetTextureType(SkinChoices, cData.m_SkinID).tex;
        realBootsMaterial.mainTexture = GetTextureType(BootsChoices, cData.m_BootsID).tex;
    }

	// Updates the map character with the passed in load out. Presumably from a saved file
	public void LoadSavedLoadOut (CharacterStyleData savedCharStyleData)
    {
        cData = savedCharStyleData;

        LoadPoseChoices();

        UpdateRealCharacter();

        // Might as  well update the other menu model while we're here
        UpdateMenuCharacter ();
	}

	// To be used by the data handler to get the load out to save. Feel free to change all this @Tris
	public CharacterStyleData GetCurrentLoadOut ()
    {
        return cData;
	}

    public TextureType GetTextureType(List<TextureType> arr , int id)
    {
    
        if (arr.Count > id)
        {
            return arr[id];
        }
        
        return arr[id];
    }

    public MeshType GetMeshType(List<MeshType> arr, int id)
    {
        if (arr.Count > id)
        {
            return arr[id];
        }

        return arr[id];
    }

    private Mesh GetNextValidMesh(List<MeshType> arr, ref int iter, int indexChange)
    {
        Mesh m = null;


        while (m == null)
        {
            iter += indexChange;
            if (iter > arr.Count-1)
            {
                iter = 0;
            }
            else if(iter < 0)
            {
                iter = arr.Count - 1;
            }
            MeshType mt = arr[iter];
            if (!customisationKitUnlocked && mt.IsPaid)
            {
                // Player is has cycled to a mesh they have not got. COuld do some money grubbing
                // stuff here??
            }
            else
            {
                m = mt.mesh.sharedMesh;
            }
        }
        return m;
    }
    private Texture2D GetNextValidTexture(List<TextureType> arr, ref int iter, int indexChange)
    {
        Texture2D t = null;


        while (t == null)
        {
            iter += indexChange;
            if (iter > arr.Count - 1)
            {
                iter = 0;
            }
            else if (iter < 0)
            {
                iter = arr.Count - 1;
            }
            TextureType tt = arr[iter];
            if (!customisationKitUnlocked && tt.IsPaid)
            {
                // Player is has cycled to a mesh they have not got. COuld do some money grubbing
                // stuff here??
            }
            else
            {
                t = tt.tex;
            }
        }
        return t;
    }

    // Hair mesh
    public void SeeNextHairMeshChoice ()
    {
        Debug.Log("B4: " + cData.m_HairMeshID);
		menuHairMesh.mesh = GetNextValidMesh(HairChoices,ref cData.m_HairMeshID, 1);
        Debug.Log("AF: " + cData.m_HairMeshID);
    }

	public void SeePreviousHairMeshChoice () {

        menuHairMesh.mesh = GetNextValidMesh(HairChoices, ref cData.m_HairMeshID, -1);
    }

	// Hair material
	public void SeeNextHairMaterialChoice () {

        menuHairMaterial.mainTexture = GetNextValidTexture(HairMatChoices, ref cData.m_HairMatID, 1);
	}

	public void SeePreviousHairMaterialChoice () {

        menuHairMaterial.mainTexture = GetNextValidTexture(HairMatChoices, ref cData.m_HairMatID, -1);
    }

	// Bag
	public void SeeNextBagChoice ()
    {
        menuBagMaterial.mainTexture = GetNextValidTexture(BagChoices, ref cData.m_BagID, 1);
	}

	public void SeePreviousBagChoice () {
        menuBagMaterial.mainTexture = GetNextValidTexture(BagChoices, ref cData.m_BagID, -1);
    }

	// Shirt
	public void SeeNextShirtChoice () {
		menuShirtMaterial.mainTexture = GetNextValidTexture(ShirtChoices, ref cData.m_ShirtID, 1);
    }

	public void SeePreviousShirtChoice () {

        menuShirtMaterial.mainTexture = GetNextValidTexture(ShirtChoices, ref cData.m_ShirtID, -1);
    }

	// Shorts
	public void SeeNextShortsChoice ()
    {
		menuShortsMaterial.mainTexture = GetNextValidTexture(ShortsChoices, ref cData.m_ShortsID, 1);
	}

	public void SeePreviousShortsChoice () {

        menuShortsMaterial.mainTexture = GetNextValidTexture(ShortsChoices, ref cData.m_ShortsID, -1);
    }

	// Skin
	public void SeeNextSkinChoice ()
    {
        menuSkinMaterial.mainTexture = GetNextValidTexture(SkinChoices, ref cData.m_SkinID, 1);
	}

	public void SeePreviousSkinChoice ()
    {

        menuSkinMaterial.mainTexture = GetNextValidTexture(SkinChoices, ref cData.m_SkinID, -1);
    }

	// Boots
	public void SeeNextBootsChoice ()
    {
		menuBootsMaterial.mainTexture = GetNextValidTexture(BootsChoices, ref cData.m_BootsID, 1);
    }

	public void SeePreviousBootsChoice () {

        menuBootsMaterial.mainTexture = GetNextValidTexture(BootsChoices, ref cData.m_BootsID, -1);
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

            cData.m_PoseID = 0;
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

            cData.m_PoseID = 3;
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

            cData.m_PoseID = 6;
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

            cData.m_PoseID = 5;
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

            cData.m_PoseID = 4;
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

            cData.m_PoseID = 2;

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

            cData.m_PoseID = 1;

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
	public int m_ShortsID=0;
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

    public CharacterStyleData Clone()
    {
        return new CharacterStyleData(m_HairMeshID, m_HairMatID, m_BagID, m_ShirtID, m_ShortsID, m_SkinID, m_BootsID, m_PoseID);
    }

    public bool IsMatch(CharacterStyleData csd)
    {
        if (m_HairMeshID != csd.m_HairMeshID) return false;
        if (m_HairMatID != csd.m_HairMatID) return false;
        if (m_BagID != csd.m_BagID) return false;
        if (m_ShirtID != csd.m_ShirtID) return false;
        if (m_ShortsID != csd.m_ShortsID) return false;
        if (m_SkinID != csd.m_SkinID) return false;
        if (m_BootsID != csd.m_BootsID) return false;
        if (m_PoseID != csd.m_PoseID) return false;
        return true;
    }
}
