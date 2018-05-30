using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VGUIManager : MonoBehaviour
{
    public static VGUIManager Instance { set; get; }

    public Canvas InspectCanvas;

    public Canvas RoamCanvas;

    public Text nameText;
    public Text sizeText;
    public Text agressionText;
    public Text caughtText;
    public Text rarityText;
    public Text levelText;
    public Text expText;
    public Text expToNextLevelText;

    public Image progressBar;

    private PocketPalData currentDisplayData;

    private bool isInspecting = false;

    // Use this for initialization
    void Start ()
    {
        Instance = this;
		
	}

	// Update is called once per frame
	void Update () {
		
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
            nameText.text = currentDisplayData.name;
            sizeText.text = currentDisplayData.size.ToString();
            agressionText.text = currentDisplayData.agressiveness.ToString();
            caughtText.text = currentDisplayData.numberCaught.ToString();
            rarityText.text = currentDisplayData.rarity.ToString();
            expText.text = currentDisplayData.GetExp().ToString();
            levelText.text = currentDisplayData.GetLevel().ToString();
            expToNextLevelText.text = currentDisplayData.EXPToNextLevel.ToString();

            progressBar.fillAmount = currentDisplayData.GetPercentageToNextLevel();

        }
    }

}
