using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaptureMiniGame : MonoBehaviour {

	public TouchHandler controls;

	public Canvas MapUI;
//	public Canvas MiniGameMenu;
	public Canvas MiniGameUI;
	public Image viewFinder;
	public Slider captureMeter;
	public Slider berryMeter;
	public Text berryCount;
	public RawImage captureImage;

	public GameObject miniGameEnvironment;
	public List<GameObject> miniGamePlayerPositions;

	Vector3 miniGamePlayerPosition, miniGamePPalPosition;

	float minigameTimer, captureTimer;

	// Thee max time allowed by the minigame before timing out
	float minigameTimeAllowance = 20.0f;
	float timeToCapture = 1.0f;

	float unfocusedDOFDistance = 30.0f;
	float defaultAperture = 0.03f;
	float pocketPalAperture = 1.0f;

	float screenWidth, screenHeight;

	// The targeted pocket pal for this minigame
	PocketPalParent pocketPal;
	GameObject pocketPalGO;
	Vector3 PPalMapPosition, CameraMapPosition;

	bool focussedOnPPal = false;

	Vector2 viewfinderPosition;
	Vector3 viewFinderDefaultScale;

	public List<GameObject> patrolPositions;
	float patrolSpeed = 0.25f;
	float patrolLerp;
	Vector3 previousPosition, nextPosition;
	int patrolIndex;

    int numGoodCamera;
    public Text goodCamNumText;
    int numMedCamera;
    public Text medCamNumText;
    int numStraw;
    public Text strawNumText;

	bool berryUsed;
	int numberOfBerries;
	float berryTimer, berryDuration = 5.0f;

	// Speed multiplier if berry used
	float berrySpeedModifier = 0.3f;

	// Used to see the inventory for number of berries
	GameData playerProfile;

	// TODO Need to Rotate the ppal between movements
	bool needToRotate = false;
	float targetYRotation, currentYRotation, deltaY;

	// For the end game
	bool endGameSequence;
	Vector3 EGStartPos, EGEndPos, EGStartLookAtPos;
	float cutSceneLerp, EGCamSpeed = 10.0f, EGTimeScale = 0.05f;
	CameraController cameraMain;

	public UIAnimationManager animManager;

	float camMovementSpeed = 1.0f;
	float mediumCamMovementSpeed = 20.0f;
	float goodCamMovementSpeed = 100.0f;
	bool specCameraUsed;
	Vector3 touchScreenSpace, touchStartScreenSpace, targetScreenSpace;


	// Use this for initialization
	void Start () {
		
		screenWidth = Screen.width;
		screenHeight = Screen.height;

		// Choose a random patrol point as the starting PPal location
		patrolIndex = Random.Range(0, patrolPositions.Count);
		miniGamePPalPosition = patrolPositions [patrolIndex].transform.position;

		cameraMain = controls.cameraController;
		viewFinderDefaultScale = viewFinder.rectTransform.localScale;
	}

	public void PlayButtonPressed () {

		// Tell the PPal it is the subject for a minigame
		pocketPal.InMinigame = true;

		// Store the map position if the minigame fails and the PPal should reappear back in the pmap
		PPalMapPosition = pocketPal.transform.position;
		CameraMapPosition = controls.cameraController.transform.position;

		// Set the virtual garden environment to be active
		miniGameEnvironment.SetActive (true);

		// Set the Positions for the miniGame
		pocketPal.transform.position = miniGamePPalPosition;
		miniGamePlayerPosition = miniGamePlayerPositions [Random.Range (0, miniGamePlayerPositions.Count)].transform.position;
		controls.cameraController.transform.position = miniGamePlayerPosition;
		controls.cameraController.transform.LookAt (new Vector3(0.0f, miniGamePPalPosition.y, 0.0f));

		// Store the current position as the starting point 'previousPosition'
		previousPosition = pocketPal.transform.position;

		// Set a randomly chosen endpoint
		patrolIndex = Random.Range(0, patrolPositions.Count);
		nextPosition = patrolPositions [patrolIndex].transform.position;
		pocketPal.transform.LookAt (nextPosition);

		// Used to see the inventory for number of berries
		playerProfile = LocalDataManager.Instance.GetData ();

		// Set the number of berries text
		numberOfBerries = playerProfile.NumberOfBerries ();
		berryCount.text = numberOfBerries.ToString ();

        //set number of strawberries
        numStraw = playerProfile.ItemInv.GetItemFromID(GlobalVariables.StrawBerriesID).numberOwned;
        strawNumText.text = numStraw.ToString();

        numGoodCamera = playerProfile.ItemInv.GetItemFromID(GlobalVariables.ProCameraID).numberOwned;
        goodCamNumText.text = numGoodCamera.ToString();

        numMedCamera = playerProfile.ItemInv.GetItemFromID(GlobalVariables.medCameraID).numberOwned;
        medCamNumText.text = numMedCamera.ToString();

        // Hide the berry meter until used
        berryMeter.gameObject.SetActive(false);
//		captureImage.gameObject.SetActive (false);

		// Adjust the UI
//		MiniGameMenu.gameObject.SetActive(false);
		MiniGameUI.gameObject.SetActive(true);

		viewFinder.gameObject.SetActive (true);

		// Initially set the viewport image to the centre of the screen
		viewFinder.rectTransform.anchoredPosition = Camera.main.ViewportToScreenPoint (new Vector3 (0.0f, 0.0f));
		viewFinder.rectTransform.localScale = viewFinderDefaultScale;

		// Enable the depth of field component
		controls.cameraController.EnableDepthOfField (true);

		// Set initial post processing to centre of screen
		AdjustPostProcessing (new Vector2 (screenWidth, screenHeight) / 2.0f);

		// Set the controlScheme in the touchHandler
		controls.MiniGameControls ();

		// Reset the timers
		minigameTimer = 0.0f;
		captureTimer = 0.0f;
		captureMeter.value = 0.0f;

		patrolLerp = 0.0f;

		berryTimer = 0.0f;
		berryUsed = false;

		camMovementSpeed = 1.0f;

		needToRotate = false;
		endGameSequence = false;

		pocketPalGO = pocketPal.gameObject;

		// Change the animation and avatar to the movement style
		pocketPal.SetMoveAnimation ();

		specCameraUsed = false;

		targetScreenSpace = new Vector3 (0.5f, 0.5f);
		touchScreenSpace = targetScreenSpace;
		touchStartScreenSpace = targetScreenSpace;

		animManager.ShowPhoto (false);
	}

	public void BackButtonPressed () {

		// Exit the minigame
		MinigameExit ();

		// Place the uncaptured PPal back in the map
		pocketPal.transform.position = PPalMapPosition;
		pocketPal.InMinigame = false;
	}

	public void InitMiniGame (PocketPalParent targetPocketPal) {
/*
		// Swap the UI over just with enabled will do
		MapUI.gameObject.SetActive(false);
		MiniGameMenu.gameObject.SetActive (true);
*/
		animManager.OpenMinigame (true);

		// Set the target pocketPal for this minigame
		pocketPal = targetPocketPal;

		// A passive control scheme waiting for a button press
		controls.MenuControls ();
	}

	public void UpdateTimer () {

		if (!endGameSequence) {

			// If has not timed out yet
			if (minigameTimer < minigameTimeAllowance) {

				// Step the timer
				minigameTimer += Time.deltaTime;

				if (focussedOnPPal) {
				
					// Step the capture timer
					captureTimer += Time.deltaTime / timeToCapture * 0.5f;

					// Check for winstate
					if (captureMeter.value >= 1.0f)
						MinigameSuccess ();
				
				} else {

					// Deplete the minigame timer
					captureTimer -= (Time.deltaTime / timeToCapture) * 0.1f;

					if (captureTimer < 0.0f)
						captureTimer = 0.0f;
				}

				// Change the slider value
				captureMeter.value = captureTimer;

				// Berry timer
				if (berryUsed) {
					berryTimer += Time.deltaTime;

					// Convert to 0-1 range and set the slider 
					berryMeter.value = 1.0f - berryTimer / berryDuration;

					if (berryTimer > berryDuration) {
						berryMeter.gameObject.SetActive (false);
						berryUsed = false;
						berryTimer = 0.0f;
					}
				}



				// Lerp the viewfinder position towards thetouch location at camMovementSpeed
				viewFinder.rectTransform.anchoredPosition =
					new Vector2(
						Mathf.Clamp (Mathf.Lerp (viewFinder.rectTransform.anchoredPosition.x, targetScreenSpace.x, Time.deltaTime * camMovementSpeed), -screenWidth/2.0f, screenWidth/2.0f),
						Mathf.Clamp (Mathf.Lerp (viewFinder.rectTransform.anchoredPosition.y, targetScreenSpace.y, Time.deltaTime * camMovementSpeed), -screenHeight/2.0f, screenHeight/2.0f));

			} else {
				
			//	MiniGameUI.gameObject.GetComponentInParent<Animator> ().SetBool ("showMinigameCapture", false);
				animManager.MinigameFail ();
			
				// Minigame failed
				MinigameExit ();

				// Place the uncaptured PPal back in the map
				pocketPal.transform.position = PPalMapPosition;
				pocketPal.InMinigame = false;

				// Change the animation and avatar to the rest style
				pocketPal.SetMoveAnimation ();
			}
		} else {
			// End game Sequence

			// Keep the ppal moving
			MovePPal();

			if (cutSceneLerp < 1.0f) {
				
				cutSceneLerp += Time.deltaTime * EGCamSpeed;

				cameraMain.transform.position = Vector3.Lerp (EGStartPos, EGEndPos, cutSceneLerp);
				cameraMain.transform.LookAt (Vector3.Lerp (EGStartLookAtPos, pocketPal.GetLookAtPosition(), cutSceneLerp));

				viewFinder.rectTransform.anchoredPosition = Vector2.Lerp (viewFinder.rectTransform.anchoredPosition, new Vector2 (0.0f, 0.0f), cutSceneLerp);
				viewFinder.rectTransform.localScale = Vector3.Lerp (viewFinder.rectTransform.localScale, new Vector3 (20.0f, 11.0f, 1.0f), cutSceneLerp);

			} else {

				// Follow the PPal still
				cameraMain.transform.LookAt (pocketPal.GetLookAtPosition());
			}

			// Lock the DOF to the PPal
			float distance = Vector3.Distance (cameraMain.transform.position, pocketPal.GetLookAtPosition());
			cameraMain.SetDepthOfFieldAndFocalLength (distance, pocketPalAperture);
		}
	}

	public void SetTouchStartPosition (Vector2 touchPosition) {

		float touchX = touchPosition.x / screenWidth - 0.5f;
		float touchY = touchPosition.y / screenHeight - 0.5f;

		touchStartScreenSpace = Camera.main.ViewportToScreenPoint (new Vector3 (touchX, touchY));
	}

	public void UpdateControls (Vector2 touchPosition) {

		if (!endGameSequence) {

			viewfinderPosition = touchPosition;

			// Adjust the touch position by the device's screen dimensions and adjust for the coming anchor position being from the centre of the screen
			float touchX = viewfinderPosition.x / screenWidth - 0.5f;
			float touchY = viewfinderPosition.y / screenHeight - 0.5f;

			// Set the position of the viewFinder image in the viewport to the adjusted touch position
			var newTouchScreenSpace = Camera.main.ViewportToScreenPoint (new Vector3 (touchX, touchY));

			// Find the delta touchScreenSpace
			var deltatouchScreenSpace = newTouchScreenSpace - touchStartScreenSpace;

			// Apply that delta to the targetScreenSpace
			targetScreenSpace += deltatouchScreenSpace;//Vector3.Lerp (targetScreenSpace, deltatouchScreenSpace, Time.deltaTime * camMovementSpeed);

			touchStartScreenSpace = newTouchScreenSpace;
		}
	}

	public void MovePPal () {

		// If arrived at current target lerp will be >= 1.0f
		if (patrolLerp >= 1.0f) {

			// Reset the lerp float
			patrolLerp = 0.0f;

			// Set the new position values, Don't use old nextPosition as the new start point as it may have overshot that position
			previousPosition = pocketPal.transform.position;

			// Pick a random next patrol position that is not the last one. Do by repeatedly picking a random index until it does not match the current index
			int tempIndex;
			do {
				tempIndex = Random.Range(0, patrolPositions.Count);
			} while (tempIndex == patrolIndex);

			// When found, update the stored current index
			patrolIndex = tempIndex;

			// Get the patrol position from the list at that index
			nextPosition = patrolPositions [patrolIndex].transform.position;



			// Set up the rotation values
			// Store the current rotation we will return to it in just a moment
			Quaternion startRot = pocketPal.transform.rotation;

			// Used to store a local value of the z rotation that is not limited by 0 - 360 bounds like a frame by grame get
			currentYRotation = startRot.eulerAngles.y;

			// Use the look at function to point the PPal at the target direction. We need to do it this way as each PPal prefab has a shitty and
			// different rotation applied to it's shitty import transform
			pocketPal.transform.LookAt (nextPosition);

			// Store the 'looking at' rotation as the targetYRotation
			targetYRotation = pocketPal.transform.rotation.eulerAngles.y;

			// Once we have the values we need, then set the rotation back to where it was
			pocketPal.transform.rotation = startRot;

			// Adjust for going round the 360deg -> 0 degrees point, We only want to rotate clockwise
			while (targetYRotation < currentYRotation)
				targetYRotation += 360.0f;

			// Tell the update driven function to rotate the PPal instead of move it
			needToRotate = true;
		}


		// Rotate or move the PPal
		if (needToRotate) {

			RotatePPal ();

		} else {

			// Adjust for berry use
			if (berryUsed)
				// Step the lerp
				patrolLerp += Time.deltaTime * patrolSpeed * berrySpeedModifier;
			else
				// Step the lerp
				patrolLerp += Time.deltaTime * patrolSpeed;

			// Set the PPal transform
			pocketPal.transform.position = Vector3.Lerp (previousPosition, nextPosition, patrolLerp);
		}

		viewfinderPosition = viewFinder.rectTransform.anchoredPosition;

		// Funtion sets the depth of field using the touched on position's distance away
		AdjustPostProcessing (viewfinderPosition);
	}

	void RotatePPal()
	{
		// The degrees that the rotation will step clockwise in this frame
		float step;

		if (berryUsed)
			// arcTan theta = Opp/Adj, take the patrol speed * Tdt * by the std prefab transform offset of 2.0f
			step = Mathf.Rad2Deg * Mathf.Atan (patrolSpeed * Time.deltaTime * berrySpeedModifier * 2.0f);
		else
			step = Mathf.Rad2Deg * Mathf.Atan (patrolSpeed * Time.deltaTime * 2.0f);
		
		// Check if it will overshoot the target in this frame
		if (currentYRotation + step > targetYRotation) {

			// Finish up
			// Finish the final stage of the rotation
			pocketPal.transform.LookAt (nextPosition);

			// Tell the update driven function to now move the PPal instead of rotate
			needToRotate = false;

		} else {

			// Step the local variable for the rotation (that can exceed 360)
			currentYRotation += step;

			// Use temporary variable method to assign new rotation			
			Vector3 tempRot = pocketPalGO.transform.rotation.eulerAngles;
			tempRot = new Vector3 (tempRot.x, tempRot.y + step, tempRot.z);
			pocketPalGO.transform.rotation = Quaternion.Euler (tempRot);
		}

	}

	void AdjustPostProcessing(Vector2 position) {

		// Don't question this... It works 
		float touchX = position.x / screenWidth + 0.5f;
		float touchY = position.y / screenHeight + 0.5f;

		var pos = new Vector2 (touchX, touchY);

		// Set as false unless ray cast returns true
		focussedOnPPal = false;

		// Create a rayCastHit object
		RaycastHit hit = new RaycastHit ();

		// Raycast from the touch position
		Ray ray = Camera.main.ViewportPointToRay (pos);

		// if hit
		if (Physics.Raycast (ray, out hit)) {

			// If the hit gameObject has a component "PocketPalParent"
			if (hit.transform.gameObject.GetComponentInParent<PocketPalParent>()) {

				// Get the distance from the camera to the pocketPal's gameObject transform position
				float distance = Vector3.Distance (controls.cameraController.getCamera().transform.position, hit.transform.gameObject.transform.position);

				// Update the post processing settings to look at the pocketPal
				controls.cameraController.SetDepthOfFieldAndFocalLength (distance, pocketPalAperture);

				focussedOnPPal = true;

			} else {

				// Update the post processing settings to look at the touched on position
				controls.cameraController.SetDepthOfFieldAndFocalLength (hit.distance, defaultAperture);

				focussedOnPPal = false;
			}

		} else {

			// Update the post processing settings to look at a set far away position
			controls.cameraController.SetDepthOfFieldAndFocalLength (unfocusedDOFDistance, defaultAperture);
		}
	}

	void MinigameSuccess () {
		
		Time.timeScale = EGTimeScale;

		// Animate the UI swap out
//		MiniGameUI.gameObject.GetComponentInParent<Animator> ().SetBool ("showMinigameCapture", true);
		animManager.MinigameSuccess ();

		EGStartPos = cameraMain.transform.position;
		EGStartLookAtPos = EGStartPos + cameraMain.transform.forward;
		EGEndPos = EGStartPos + (pocketPal.transform.position - EGStartPos) / 2.0f;
		cutSceneLerp = 0.0f;

		// Tell the update to call the cut scene
		endGameSequence = true;

		//StartCoroutine(SlowMo());
	}

	// Fuck yeah that's the good stuff
	IEnumerator WaitThenExit ()
	{
		yield return new WaitForSeconds(3.0f);

		// Reset the anim state machine
	//	MiniGameUI.gameObject.GetComponentInParent<Animator> ().SetBool ("showPhoto", false);
		animManager.MinigameToMap ();

		// Exit the minigame
		MinigameExit ();

		// Add to the player's inventory
		pocketPal.Captured();
	}

	void MinigameExit () {

		// Swap the UI
		MapUI.gameObject.SetActive(true);

//		MiniGameMenu.gameObject.SetActive(false);
//		MiniGameUI.gameObject.SetActive(false);
		viewFinder.gameObject.SetActive (false);
		
		// Place the camera back in the map area
		controls.cameraController.transform.position = CameraMapPosition;

		// Tell the cameraController to zoom out
		controls.cameraController.MapZoomOutInit ();

		// Remove the depth of field component
		controls.cameraController.EnableDepthOfField (false);

		// Deactivate the minigame environment
		miniGameEnvironment.SetActive(false);
	}

	public void UseBerry () {

		// If not already using a berry and has a berry to use
		if (!berryUsed)
        {
            if (playerProfile.UseBerry())
            {
                // Set the slider
                berryMeter.gameObject.SetActive(true);
                berryMeter.value = 1.0f;

                // Set the timer
                berryTimer = 0.0f;
                berryUsed = true;

                // Decrease the local count
                numberOfBerries--;

                // Adjust UI
                berryCount.text = playerProfile.NumberOfBerries().ToString();
            }
            else
            {
                NotificationManager.Instance.ItemFailedNotification("You have no berries to use! Try buying some from the shop, or finding them at resource spots");
            }
		}
	}

	public void UseStrawberry () {

		if (!berryUsed)
        {
            if (playerProfile.ItemInv.UseItemWithID(GlobalVariables.StrawBerriesID))
            {
                // Set the slider
                berryMeter.gameObject.SetActive(true);
                berryMeter.value = 1.0f;

                // Set the timer
                berryTimer = 0.0f;
                berryUsed = true;

                // Decrease the local count
                numStraw--;

                // Adjust UI
                strawNumText.text = numStraw.ToString();
            }
            else
            {
                NotificationManager.Instance.ItemFailedNotification("You have no strawberries to use! Try buying some from the shop, or finding them at resource spots");
            }
        }

	}

	public void TakePhotoButtonPressed () {

		ScreenCapture.CaptureScreenshot("screenshot.png");

		Time.timeScale = 1.0f;

		LoadPhotoInOneASecond ();

	}

	public void LoadPhotoInOneASecond () {

		StartCoroutine (WaitOneSecond ());
	}

	IEnumerator WaitOneSecond()
	{
		yield return new WaitForSeconds(1.0f);


		#if !UNITY_EDITOR

		byte[] bytes = System.IO.File.ReadAllBytes (Application.persistentDataPath + "/screenshot.png");

		Texture2D texture = new Texture2D (200, 200);

		texture.LoadImage (bytes);

		captureImage.texture = texture;

		animManager.ShowPhoto (true);

		//MiniGameUI.gameObject.GetComponentInParent<Animator> ().SetBool ("showPhoto", true);

		StartCoroutine(WaitThenExit());

		#endif
	}

	public void UseMediumCamera () {

		if (!specCameraUsed)
        {
            if (playerProfile.ItemInv.UseItemWithID(GlobalVariables.medCameraID))
            {
                camMovementSpeed = mediumCamMovementSpeed;
                specCameraUsed = true;
                numMedCamera--;
                medCamNumText.text = numMedCamera.ToString();
            }
		}
	}

	public void UseGoodCamera () {

		if (!specCameraUsed)
        {
            if (playerProfile.ItemInv.UseItemWithID(GlobalVariables.ProCameraID))
            {
                camMovementSpeed = goodCamMovementSpeed;
                specCameraUsed = true;
                numGoodCamera--;
                goodCamNumText.text = numGoodCamera.ToString();
            }
        }
	}
}
