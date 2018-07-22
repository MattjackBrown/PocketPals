using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimationManager : MonoBehaviour {

	public GameObject startingUI;
	public TouchHandler controls;

    public static UIAnimationManager Instance { set; get; }

	// There is a central animator within this parent that controls specific pairs of UIs
	Animator canvasAnimator;

    private void Awake()
    {
        canvasAnimator = GetComponent<Animator>();

    }

    void Start () {

		canvasAnimator = GetComponent<Animator> ();
        Instance = this;
	}

	public void ShowSettings (bool show) {

		canvasAnimator.SetBool ("showSettings", show);
	}

	public void ShowInventory (bool show)
    {
        if(show)InventoryHandler.Instance.Enabled();
		canvasAnimator.SetBool ("showInventory", show);
	}

	public void ShowMinigameCaptureButton (bool show) {

		canvasAnimator.SetBool ("showMinigameCapture", show);
	}

	public void ShowPhoto (bool show) {

		canvasAnimator.SetBool ("showPhoto", show);
	}

	public void CloseLogin (bool show) {

		canvasAnimator.SetBool ("closeLogin", show);
	}

	public void OpenCreate (bool show) {

		canvasAnimator.SetBool ("openCreate", show);
	}

	public void OpenMainMenu (bool show) {

		// If opening the mainMenu
		if (show) {
			// Check that the camera is not in a transition state, just the resting map controls allow
			if (controls.IsInMapControls ()) {

				// Stop being able to select map items
				controls.MenuControls ();
				canvasAnimator.SetBool ("openMainMenu", show);
			}
		} else {
			// Removing the mainMenu
			canvasAnimator.SetBool ("openMainMenu", show);
		}
	}

	public void OpenMinigame (bool show) {

		canvasAnimator.SetBool ("openMinigame", show);
		canvasAnimator.SetBool ("minigameAccepted", false);
		canvasAnimator.SetBool ("minigameDeclined", false);
		canvasAnimator.SetBool ("minigameSuccess", false);
		canvasAnimator.SetBool ("minigameFail", false);
		canvasAnimator.SetBool ("minigameToMap", false);
		canvasAnimator.SetBool ("cameraChosen", false);
	}

	public void MinigameAccepted () {

		canvasAnimator.SetBool ("minigameAccepted", true);
		canvasAnimator.SetBool ("openMinigame", false);
	}

	public void MinigameDeclined () {

		canvasAnimator.SetBool ("minigameDeclined", true);
		canvasAnimator.SetBool ("openMinigame", false);
	}

	public void MinigameSuccess () {

		canvasAnimator.SetBool ("minigameSuccess", true);
		canvasAnimator.SetBool ("minigameAccepted", false);
		canvasAnimator.SetBool ("minigameDeclined", false);
	}

	public void MinigameFail () {

		canvasAnimator.SetBool ("minigameFail", true);
		canvasAnimator.SetBool ("minigameAccepted", false);
		canvasAnimator.SetBool ("minigameDeclined", false);
	}

	public void MinigameToMap () {

		canvasAnimator.SetBool ("minigameToMap", true);
		ResetMinigame ();
	}

	public void ResetMinigame () {
		
		canvasAnimator.SetBool ("openMinigame", false);
		canvasAnimator.SetBool ("minigameAccepted", false);
		canvasAnimator.SetBool ("minigameDeclined", false);
		canvasAnimator.SetBool ("minigameSuccess", false);
		canvasAnimator.SetBool ("minigameFail", false);
		canvasAnimator.SetBool ("cameraChosen", false);
	}

	public void OpenJournal (bool show)
	{
		// If opening the UI
		if (show) {
			// Check that the camera is not in a transition state, just the resting map controls allow
			if (controls.IsInMapControls ()) {
				
				PlayerProfileHandler.Instance.RefreshStats();
				canvasAnimator.SetBool ("openJournal", show);
				controls.MenuControls ();
			}
		} else {
			// Removing the UI
			canvasAnimator.SetBool ("openJournal", show);
			controls.MapControls ();
		}
	}

	public void OpenTracks (bool show) {

		// If opening the UI
		if (show) {
			// Check that the camera is not in a transition state, just the resting map controls allow
			if (controls.IsInMapControls ()) {

				TrackAndTrailsHandle.Instance.RefreshCollection();
				canvasAnimator.SetBool ("openTracks", show);
				controls.MenuControls ();
			}
		} else {
			// Removing the UI
			canvasAnimator.SetBool ("openTracks", show);
			controls.MapControls ();
		}
	}

	public void OpenShop (bool show) {

		canvasAnimator.SetBool ("openShop", show);
	}

	public void ShowVG (bool show) {

		canvasAnimator.SetBool ("showVG", show);
	}

	public void IntroFinished () {

		canvasAnimator.SetBool ("introFinished", true);
	}

	public void LoadingBarFinished () {

		canvasAnimator.SetBool ("loadingBarFinished", true);
	}

	public void CameraChosen () {

		canvasAnimator.SetBool ("cameraChosen", true);
	}

	public void OpenMapMenu () {

		canvasAnimator.SetBool ("openMapMenu", true);
	}

	public void ReturnToVirtualGarden () {

		canvasAnimator.SetBool ("startInVG", true);
		canvasAnimator.SetBool ("showVG", true);
		canvasAnimator.SetBool ("openMainMenu", true);
	}
}
