using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaptureMiniGame : MonoBehaviour {

	public TouchHandler controls;

	public Canvas MapUI;
	public Canvas MiniGameMenu;
	public Canvas MiniGameUI;
	public Image viewFinder;
	public Slider captureMeter;

	public GameObject miniGameEnvironment;
	public GameObject miniGamePlayerPositionObject;
	public GameObject miniGamePPalPositionObject;

	Vector3 miniGamePlayerPosition, miniGamePPalPosition;

	float minigameTimer, captureTimer;

	// Thee max time allowed by the minigame before timing out
	float minigameTimeAllowance = 15.0f;
	float timeToCapture = 4.0f;

	float unfocusedDOFDistance = 30.0f;
	float defaultAperture = 0.03f;
	float pocketPalAperture = 1.0f;

	float screenWidth, screenHeight;

	// The targeted pocket pal for this minigame
	PocketPalParent pocketPal;
	Vector3 PPalMapPosition, CameraMapPosition;

	bool focussedOnPPal = false;

	Vector2 viewfinderPosition;

	public List<GameObject> patrolPositions;
	float patrolSpeed = 0.5f;
	float patrolLerp;
	Vector3 previousPosition, nextPosition;
	int patrolIndex;

	// Use this for initialization
	void Start () {
		
		screenWidth = Screen.width;
		screenHeight = Screen.height;

		miniGamePlayerPosition = miniGamePlayerPositionObject.transform.position;
		miniGamePPalPosition = miniGamePPalPositionObject.transform.position;
	}

	public void PlayButtonPressed () {

		// Adjust the UI
		MiniGameMenu.gameObject.SetActive(false);
		MiniGameUI.gameObject.SetActive(true);

		// Initially set the viewport image to the centre of the screen
		viewFinder.rectTransform.anchoredPosition = Camera.main.ViewportToScreenPoint (new Vector3 (0.0f, 0.0f));

		// Enable the depth of field component
		controls.cameraController.EnableDepthOfField (true);

		// Set initial post processing to centre of screen
		AdjustPostProcessing (new Vector2 (screenWidth, screenHeight) / 2.0f);

		// Set the controlScheme in the touchHandler
		controls.MiniGameControls ();

		patrolLerp = 0.0f;
	}

	public void BackButtonPressed () {

		// Exit the minigame
		MinigameExit ();

		// Place the uncaptured PPal back in the map
		pocketPal.transform.position = PPalMapPosition;
		pocketPal.InMinigame = false;
	}

	public void InitMiniGame (PocketPalParent targetPocketPal) {

		// Swap the UI over just with enabled will do
		MapUI.gameObject.SetActive(false);
		MiniGameMenu.gameObject.SetActive (true);

		// Reset the timers
		minigameTimer = 0.0f;
		captureTimer = 0.0f;
		captureMeter.value = 0.0f;

		targetPocketPal.InMinigame = true;

		// Set the target pocketPal for this minigame
		pocketPal = targetPocketPal;

		// A passive control scheme waiting for a button press
		controls.MenuControls ();

		// Store the map position if the minigame fails and the PPal should reappear back in the pmap
		PPalMapPosition = targetPocketPal.transform.position;
		CameraMapPosition = controls.cameraController.transform.position;

		// Set the virtual garden environment to be active
		miniGameEnvironment.SetActive (true);

		// Set the Positions for the miniGame
		controls.cameraController.transform.position = miniGamePlayerPosition;
		targetPocketPal.transform.position = miniGamePPalPosition;
		controls.cameraController.transform.LookAt (miniGamePPalPosition);

		previousPosition = pocketPal.transform.position;

		patrolIndex = Random.Range(0, patrolPositions.Count-1);
		nextPosition = patrolPositions [patrolIndex].transform.position;
	}

	public void UpdateTimer () {

		// If has not timed out yet
		if (minigameTimer < minigameTimeAllowance) {

			// Step the timer
			minigameTimer += Time.deltaTime;

			// Move the PPal around
			MovePPal ();

			// Funtion sets the depth of field using the touched on position's distance away
			AdjustPostProcessing (viewfinderPosition);

			if (focussedOnPPal) {
				
				// Step the capture timer
				captureTimer += Time.deltaTime / timeToCapture * 0.5f;

				// Check for winstate
				if (captureMeter.value >= 1.0f)
					MinigameSuccess ();
				
			} else {

				// Deplete the minigame timer
				captureTimer -= Time.deltaTime / timeToCapture * 0.25f;

				if (captureTimer < 0.0f)
					captureTimer = 0.0f;
			}

			// Change the slider value
			captureMeter.value = captureTimer;

		} else {
			
			// Minigame failed
			MinigameExit ();

			// Place the uncaptured PPal back in the map
			pocketPal.transform.position = PPalMapPosition;
			pocketPal.InMinigame = false;
		}
	}

	public void UpdateControls (Vector2 touchPosition) {

		viewfinderPosition = touchPosition;

		// Adjust the touch position by the device's screen dimensions and adjust for the coming anchor position being from the centre of the screen
		float touchX = viewfinderPosition.x / screenWidth - 0.5f;
		float touchY = viewfinderPosition.y / screenHeight - 0.5f;

		// Set the position of the viewFinder image in the viewport to the adjusted touch position
		viewFinder.rectTransform.anchoredPosition = Camera.main.ViewportToScreenPoint (new Vector3 (touchX, touchY));
	}

	void MovePPal () {

		// If arrived at current target lerp will be >= 1.0f
		if (patrolLerp >= 1.0f) {

			// Reset the lerp float
			patrolLerp = 0.0f;

			// Set the new position values, Don't use old nextPosition as the new start point as it may have overshot that position
			previousPosition = pocketPal.transform.position;

			// Pick a random next patrol position that is not the last one. Do by repeatedly picking a random index until it does not match the current index
			int tempIndex;
			do {
				tempIndex = Random.Range(0, patrolPositions.Count-1);
			} while (tempIndex == patrolIndex);

			// When found, update the stored current index
			patrolIndex = tempIndex;

			// Get the patrol position from the list at that index
			nextPosition = patrolPositions [patrolIndex].transform.position;
		}

		// Step the lerp
		patrolLerp += Time.deltaTime * patrolSpeed;

		// Move the PPal
		pocketPal.transform.position = Vector3.Lerp (previousPosition, nextPosition, patrolLerp);

		pocketPal.transform.LookAt (nextPosition);
	}

	void AdjustPostProcessing(Vector2 touchPosition) {

		// Create a rayCastHit object
		RaycastHit hit = new RaycastHit ();

		// Raycast from the touch position
		Ray ray = Camera.main.ScreenPointToRay (touchPosition);

		// if hit
		if (Physics.Raycast (ray, out hit)) {

			// If the hit gameObject has a component "PocketPalParent"
			if (hit.transform.gameObject.GetComponent ("PocketPalParent")) {

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

		// Exit the minigame
		MinigameExit ();

		// Add to the player's inventory
		pocketPal.Captured();
	}

	void MinigameExit () {

		// Swap the UI
		MapUI.gameObject.SetActive(true);
		MiniGameMenu.gameObject.SetActive(false);
		MiniGameUI.gameObject.SetActive(false);

		controls.cameraController.transform.position = CameraMapPosition;

		// Tell the cameraController to zoom out
		controls.cameraController.MapZoomOutInit ();

		// Remove the depth of field component
		controls.cameraController.EnableDepthOfField (false);

		// Deactivate the minigame environment
		miniGameEnvironment.SetActive(false);

		// Place the camera back in the map area
		controls.cameraController.transform.position = CameraMapPosition;

		// Because why not
		controls.Vibrate ();
	}
}
