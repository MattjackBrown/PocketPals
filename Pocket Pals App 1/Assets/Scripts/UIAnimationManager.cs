using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimationManager : MonoBehaviour {

	public GameObject startingUI;

	// Used just to enable/disable
	public GameObject settingsUI;
	public GameObject inventoryUI;

	// This is all a bit messy, but the different animator components need to be enabled/disabled else they try and overwrite each other
	public Animator mainMenuAnimator;

	GameObject currentUI;

	// There is a central animator within this parent that controls specific pairs of UIs
	Animator coreCanvasAnimator;

	void Start () {

		currentUI = startingUI;

		coreCanvasAnimator = GetComponent<Animator> ();

	}

	public void OpenUI (GameObject nextUI) {

		mainMenuAnimator.enabled = true;

		nextUI.SetActive (true);

		currentUI.GetComponent<Animator> ().SetBool ("isDisplayed", false);
		nextUI.GetComponent<Animator> ().SetBool ("isDisplayed", true);

		currentUI = nextUI;
	}

	public void ShowSettings (bool show) {

		mainMenuAnimator.enabled = false;

		settingsUI.SetActive (true);
		coreCanvasAnimator.SetBool ("showSettings", show);
	}

	public void ShowInventory (bool show) {

		mainMenuAnimator.enabled = false;

		inventoryUI.SetActive (true);
		coreCanvasAnimator.SetBool ("showInventory", show);
	}

	public void ShowMinigameCaptureButton (bool show) {

		coreCanvasAnimator.SetBool ("showMinigameCapture", show);
	}
}
