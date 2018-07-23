using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VGUIManager : MonoBehaviour
{
    public static VGUIManager Instance { set; get; }

    public Canvas InspectCanvas;

    public Canvas RoamCanvas;

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

    private PocketPalData currentDisplayData;

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
    

    public void ToggleInspect()
    {
        if (isInspecting) SwitchToRoam();
        else SwitchToInspect();
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

    public void SetInspectData(PocketPalData ppd)
    {
        currentDisplayData = ppd;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (currentDisplayData != null)
        {
            factBoard.GetComponent<Image>().sprite = AssetManager.Instance.GetFactSheet(currentDisplayData.ID);
            weight.text = currentDisplayData.GetWeight().ToString() + "Kg";
            length.text = currentDisplayData.GetLength().ToString() + "cm";

            nameText.text = currentDisplayData.name;
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
}
