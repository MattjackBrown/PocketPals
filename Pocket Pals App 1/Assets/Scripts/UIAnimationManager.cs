using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimationManager : MonoBehaviour {
	public GameObject startingUI;
	public TouchHandler controls;

    public GameObject ButtonBlocker, TandTUI;
	public TrackAndTrailsHandle tAndTHandle;

    public static UIAnimationManager Instance { set; get; }

	// There is a central animator within this parent that controls specific pairs of UIs
	Animator canvasAnimator;
    public bool CanAnimChange = true;


	void Awake()
	{
		canvasAnimator = GetComponent<Animator> ();
	}

    void Start () {

		canvasAnimator = GetComponent<Animator> ();
        Instance = this;
	}

	IEnumerator TryOverride()
	{
		while (canvasAnimator == null) 
		{
			canvasAnimator = GetComponent<Animator> ();
			Debug.Log ("Im trying");
			yield return new WaitForSeconds (1);
		}
		canvasAnimator.SetBool ("AROverride", true);
	}

	public void ShowSettings (bool show) {
        if (!CanAnimChange) return;
        canvasAnimator.SetBool ("showSettings", show);
	}

    public void OpenLogin()
    {
        if (!CanAnimChange) return;
        canvasAnimator.Play("LoginOpen");
        canvasAnimator.SetBool("closeLogin", false);
    }

	public void OverrideLogin()
	{
		StartCoroutine (TryOverride());
	}

	public void ShowInventory (bool show)
    {
        if (!CanAnimChange) return;
        if (show)InventoryHandler.Instance.Enabled();
		canvasAnimator.SetBool ("showInventory", show);
		canvasAnimator.SetBool ("openShop", false);
	}

	public void ShowMinigameCaptureButton (bool show) {
        if (!CanAnimChange) return;
        canvasAnimator.SetBool ("showMinigameCapture", show);
	}

	public void ShowPhoto (bool show) {
        if (!CanAnimChange) return;
        canvasAnimator.SetBool ("showPhoto", show);
	}

    public void OpenShopFromInventory()
    {
        if (!CanAnimChange) return;
        ServerDataManager.Instance.RefreshCoins(LocalDataManager.Instance.GetData());
        ShopHandler.Instance.RefreshCoins();
        ShopHandler.Instance.UpdateButtons();
        canvasAnimator.SetBool("openShop", true);
    }

	public void ExitShop () {
		if (!CanAnimChange) return;
		canvasAnimator.SetBool("openShop", false);
		canvasAnimator.SetBool ("showInventory", false);

	}

	public void CloseLogin (bool show) {
        if (!CanAnimChange) return;
        canvasAnimator.SetBool ("closeLogin", show);
	}

	public void OpenCreate (bool show) {
        if (!CanAnimChange) return;
        canvasAnimator.SetBool ("openCreate", show);
	}

	public void OpenMainMenu (bool show) {
        if (!CanAnimChange) return;
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
        if (!CanAnimChange) return;
        canvasAnimator.SetBool ("openMinigame", show);
		canvasAnimator.SetBool ("minigameAccepted", false);
		canvasAnimator.SetBool ("minigameDeclined", false);
		canvasAnimator.SetBool ("minigameSuccess", false);
		canvasAnimator.SetBool ("minigameFail", false);
		canvasAnimator.SetBool ("minigameToMap", false);
		canvasAnimator.SetBool ("cameraChosen", false);
		canvasAnimator.SetBool ("quitChooseCamera", false);
	}

	public void MinigameAccepted () {
        if (!CanAnimChange) return;
        canvasAnimator.SetBool ("minigameAccepted", true);
		canvasAnimator.SetBool ("openMinigame", false);
	}

	public void MinigameDeclined () {
        if (!CanAnimChange) return;
        canvasAnimator.SetBool ("minigameDeclined", true);
		canvasAnimator.SetBool ("openMinigame", false);
	}

	public void MinigameSuccess () {
        if (!CanAnimChange) return;
        canvasAnimator.SetBool ("minigameSuccess", true);
		canvasAnimator.SetBool ("minigameAccepted", false);
		canvasAnimator.SetBool ("minigameDeclined", false);
	}

	public void MinigameFail () {
        if (!CanAnimChange) return;
        canvasAnimator.SetBool ("minigameFail", true);
		canvasAnimator.SetBool ("minigameAccepted", false);
		canvasAnimator.SetBool ("minigameDeclined", false);
	}

	public void MinigameToMap () {
        if (!CanAnimChange) return;
        canvasAnimator.SetBool ("minigameToMap", true);
		ResetMinigame ();
	}

	public void ResetMinigame () {
        if (!CanAnimChange) return;
        canvasAnimator.SetBool ("openMinigame", false);
		canvasAnimator.SetBool ("minigameAccepted", false);
		canvasAnimator.SetBool ("minigameDeclined", false);
		canvasAnimator.SetBool ("minigameSuccess", false);
		canvasAnimator.SetBool ("minigameFail", false);
		canvasAnimator.SetBool ("cameraChosen", false);
		canvasAnimator.SetBool ("quitChooseCamera", false);
	}

	public void OpenJournal (bool show)
	{
        if (!CanAnimChange) return;
        // If opening the UI
        if (show) {
			// Check that the camera is not in a transition state, just the resting map controls allow
			if (controls.IsInMapControls ()) {
				
				PlayerProfileHandler.Instance.RefreshStats();
                CharacterCustomisation.Instance.customisationKitUnlocked = LocalDataManager.Instance.HasCP();
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
        if (!CanAnimChange) return;
        // If opening the UI
        if (show) {
			// Check that the camera is not in a transition state, just the resting map controls allow
			if (controls.IsInMapControls ()) {

				TandTUI.SetActive (true);
				tAndTHandle.RefreshCollection ();
				TrackAndTrailsHandle.Instance.RefreshCollection();
				canvasAnimator.SetBool ("openTracks", show);
	//			TandTUI.SetActive (true);
				controls.MenuControls ();
			}
		} else {
			// Removing the UI
			canvasAnimator.SetBool ("openTracks", show);
	//		TandTUI.SetActive (false);
			controls.MapControls ();
		}
	}

	public void OpenShop (bool show)
    {
        if (!CanAnimChange) return;
        ServerDataManager.Instance.RefreshCoins(LocalDataManager.Instance.GetData());
        ShopHandler.Instance.RefreshCoins();
        ShopHandler.Instance.UpdateButtons();
        canvasAnimator.SetBool ("openShop", show);
	}

	public void ShowVG (bool show) {
        if (!CanAnimChange) return;

        VGUIManager.Instance.CheckARButton();
        canvasAnimator.SetBool ("showVG", show);
	}

	public void IntroFinished () {
        if (!CanAnimChange) return;
        canvasAnimator.SetBool ("introFinished", true);
	}

	public void LoadingBarFinished () {
        if (!CanAnimChange) return;
        canvasAnimator.SetBool ("loadingBarFinished", true);
	}

	public void CameraChosen () {
        if (!CanAnimChange) return;
        canvasAnimator.SetBool ("cameraChosen", true);
	}

	public void QuitChooseCamera () {
        if (!CanAnimChange) return;
        canvasAnimator.SetBool ("quitChooseCamera", true);
	}

	public void OpenMapMenu () {
        if (!CanAnimChange) return;
        canvasAnimator.SetBool ("openMapMenu", true);
	}

	public void ReturnToVirtualGarden () {
        if (!CanAnimChange) return;
        canvasAnimator.SetBool ("startInVG", true);
		canvasAnimator.SetBool ("showVG", true);
		canvasAnimator.SetBool ("openMainMenu", true);
	}

    private bool CanAnimate()
    {
        Input.multiTouchEnabled = false;
        if (Input.touchCount > 1) return false;
        return true;
    }
}
