using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VGUIManager : MonoBehaviour
{
    public static VGUIManager Instance { set; get; }

    public Canvas InspectCanvas;

    public Canvas RoamCanvas;

    public GameObject ARButton;

    public GameObject statBoard;
    public GameObject factBoard;
    public Text weight;
    public Text length;

    public Text nameText;
//    public Text agressionText;
    public Text caughtText;
    public Text rarityText;
    public Text levelText;
    public Text expText;
    public Text expToNextLevelText;
    public Text lastSeen;
    public Text firstSeen;

    public Image progressBar;

    public GameObject MorphButton;
    private bool IsMorphed = false;

    private PocketPalData currentDisplayData;
    private VirtualGardenSpawn currentObj;

    private bool isInspecting = false;

	public Image mainMenu;

    // Use this for initialization
    void Start ()
    {
        Instance = this;
		
	}

	// Update is called once per frame
	void Update () {
		
	}

    public void Up()
    {
        if (!statBoard.activeSelf)
        {
            statBoard.SetActive(true);
        }
        else
        {
            statBoard.SetActive(false);
        }

    }

    public void Down()
    {
        if (!statBoard.activeSelf)
        {
            statBoard.SetActive(true);
        }
        else
        {
            statBoard.SetActive(false);
        }
    }

    public void AnimalCall()
    {
        SoundEffectHandler.Instance.PlayAnimalSound(currentDisplayData.ID.ToString());

		BackgroundMusic.Instance.LowerBackgroundMusic ();
    }

    public void FindOutMore()
    {
        Application.OpenURL("https://www.pocketpalsapp.com/");
    }

    public void ToggleInspect()
    {
        if (isInspecting) SwitchToRoam();
        else SwitchToInspect();
    }

    public void CheckARButton()
    {
        if (LocalDataManager.Instance.HasAR())
        {
            ARButton.SetActive(true);
        }
        else
        {
            ARButton.SetActive(false);
        }
    }

    public void SwitchToRoam()
    {

        InspectCanvas.transform.gameObject.SetActive(false);
        RoamCanvas.transform.gameObject.SetActive(true);
        isInspecting = false;
    }
    public void SwitchToInspect()
    {
        if (currentDisplayData != null)
        {
            InspectCanvas.transform.gameObject.SetActive(true);
            RoamCanvas.transform.gameObject.SetActive(false);
            isInspecting = true;
        }
    }

    public void SetWeight()
    {
        string str = "";
        if (currentDisplayData.weight < 1)
        {
			double tempWeight = Math.Round ((currentDisplayData.weight * 1000), 1);
			if (tempWeight <= 0.1f)
				str = "";
			else
				str = tempWeight.ToString() + "g";
        }
        else
        {
            str = currentDisplayData.GetRoundedWeight().ToString() + "kg";
        }
        weight.text = str;
    }

    public void SetLength()
    {
        string str = "";
        if (currentDisplayData.weight < 1)
        {
            str = Math.Round((currentDisplayData.length * 10), 1).ToString() + "mm";
        }
        else
        {
            str = currentDisplayData.GetRoundedLength().ToString() + "cm";
        }
        length.text = str;
    }

    public void SetInspectData(VirtualGardenSpawn anim)
    {
        PocketPalData ppd = anim.GetAnimalData();
        currentObj = anim;
        currentDisplayData = ppd;
        PocketPalParent ppp = AssetManager.Instance.GetPocketPalFromID(ppd.ID).GetComponent<PocketPalParent>();
        if (ppp.CanRareMorph()) MorphButton.SetActive(true);
        else MorphButton.SetActive(false);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (currentDisplayData != null)
        {
            factBoard.GetComponent<Image>().sprite = AssetManager.Instance.GetFactSheet(currentDisplayData.ID);
            SetWeight();
            SetLength();

            nameText.text = currentDisplayData.pocketPalName;
//            agressionText.text = currentDisplayData.GetAgression().ToString();
            caughtText.text = currentDisplayData.numberCaught.ToString();
            rarityText.text = currentDisplayData.GetRarity().ToString();
            expText.text = currentDisplayData.GetExp().ToString();
            levelText.text = currentDisplayData.GetLevel().ToString();
            expToNextLevelText.text = currentDisplayData.GetExpToNextLevel().ToString();
            firstSeen.text = currentDisplayData.GetFirstSeen();
            lastSeen.text = currentDisplayData.GetLastSeen();
            progressBar.fillAmount = currentDisplayData.GetPercentageToNextLevel();

        }
    }

	public void SwitchUIToMainMenu () {

		mainMenu.rectTransform.anchorMin = new Vector2 (0.0f, 0.0f);
		mainMenu.rectTransform.anchorMax = new Vector2 (1.0f, 1.0f);
		mainMenu.gameObject.SetActive (true);
		this.gameObject.SetActive (false);
	}

    public void TryToggleMorph()
    {
        if (currentDisplayData.HasRare == 0)
        {
            NotificationManager.Instance.CustomHeaderNotification("Sorry!", "You have not found the rare version of this animal.");
        }
        else
        {
            if (IsMorphed)
            {
                IsMorphed = false;

            }
            else
            {
                IsMorphed = true;
            }
            AssetManager.Instance.GetPocketPalFromID(currentDisplayData.ID).GetComponent<PocketPalParent>().ToggleRare(IsMorphed, currentObj.animalObj.GetComponentInChildren<Renderer>());
        }
    }
}
