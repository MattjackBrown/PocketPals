﻿using System.Collections;
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

	float minigameTimer, captureTimer;

	// Thee max time allowed by the minigame before timing out
	float minigameTimeAllowance = 10.0f;
	float timeToCapture = 4.0f;

	float unfocusedDOFDistance = 30.0f;
	float defaultAperture = 0.03f;
	float pocketPalAperture = 1.0f;

	float screenWidth, screenHeight;

	// The targeted pocket pal for this minigame
	PocketPalParent pocketPal;

	bool focussedOnPPal = false;

	// Use this for initialization
	void Start () {
		
		screenWidth = Screen.width;
		screenHeight = Screen.height;
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
	}

	public void BackButtonPressed () {

		// Exit the minigame
		MinigameExit ();
	}

	public void InitMiniGame (PocketPalParent targetPocketPal) {

		// Swap the UI over just with enabled will do
		MapUI.gameObject.SetActive(false);
		MiniGameMenu.gameObject.SetActive (true);

		// Reset the timers
		minigameTimer = 0.0f;
		captureTimer = 0.0f;
		captureMeter.value = 0.0f;

		// Set the target pocketPal for this minigame
		pocketPal = targetPocketPal;

		// A passive control scheme waiting for a button press
		controls.MenuControls ();
	}

	public void UpdateTimer () {

		// If has not timed out yet
		if (minigameTimer < minigameTimeAllowance) {

			// Step the timer
			minigameTimer += Time.deltaTime;

			if (focussedOnPPal) {
				
				// Step the capture timer
				captureTimer += Time.deltaTime / timeToCapture;

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

			MinigameExit ();
		}
	}

	public void UpdateControls (Vector2 touch) {

		// Adjust the touch position by the device's screen dimensions and adjust for the coming anchor position being from the centre of the screen
		float touchX = touch.x / screenWidth - 0.5f;
		float touchY = touch.y / screenHeight - 0.5f;

		// Set the position of the viewFinder image in the viewport to the adjusted touch position
		viewFinder.rectTransform.anchoredPosition = Camera.main.ViewportToScreenPoint (new Vector3 (touchX, touchY));

		// Funtion sets the depth of field using the touched on position's distance away
		AdjustPostProcessing (touch);

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

		// Add to the player's inventory
		pocketPal.Captured();

		// Exit the minigame
		MinigameExit ();
	}

	void MinigameExit () {

		// Swap the UI
		MapUI.gameObject.SetActive(true);
		MiniGameMenu.gameObject.SetActive(false);
		MiniGameUI.gameObject.SetActive(false);

		// Tell the cameraController to zoom out
		controls.cameraController.MapZoomOutInit ();

		// Remove the depth of field component
		controls.cameraController.EnableDepthOfField (false);
	}
}
