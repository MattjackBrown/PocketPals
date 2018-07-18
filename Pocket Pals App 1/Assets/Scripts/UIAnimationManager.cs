using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimationManager : MonoBehaviour {

	public GameObject startingUI;

	// There is a central animator within this parent that controls specific pairs of UIs
	Animator canvasAnimator;

    private void Awake()
    {
        canvasAnimator = GetComponent<Animator>();

    }

    void Start () {

		canvasAnimator = GetComponent<Animator> ();

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

		canvasAnimator.SetBool ("openMainMenu", show);
	}

	public void OpenMinigame (bool show) {

		canvasAnimator.SetBool ("openMinigame", show);
		canvasAnimator.SetBool ("minigameAccepted", false);
		canvasAnimator.SetBool ("minigameDeclined", false);
		canvasAnimator.SetBool ("minigameSuccess", false);
		canvasAnimator.SetBool ("minigameFail", false);
		canvasAnimator.SetBool ("minigameToMap", false);
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
	}

	public void OpenJournal (bool show)
    {
		canvasAnimator.SetBool ("openJournal", show);
        PlayerProfileHandler.Instance.RefreshStats();
	}

	public void OpenTracks (bool show) {

		canvasAnimator.SetBool ("openTracks", show);
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

	public void ReturnToVirtualGarden () {

		canvasAnimator.SetBool ("startInVG", true);
		canvasAnimator.SetBool ("showVG", true);
		canvasAnimator.SetBool ("openMainMenu", true);
	}
}
