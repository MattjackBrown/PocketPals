using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaptureMiniGame : MonoBehaviour {

	public TouchHandler controls;

	public Image viewFinder;
	public Canvas MiniGameUI;
	public Canvas MapUI;

	float minigameTimer = 0.0f;
	float minigameTimeAllowance = 4.0f;
	float captureTimer = 0.0f;

	float unfocusedDOFDistance = 30.0f;
	float defaultAperture = 0.03f;
	float pocketPalAperture = 1.0f;

	float screenWidth, screenHeight;

	// The targeted pocket pal for this minigame
	PocketPalParent pocketPal;

	// Use this for initialization
	void Start () {
		screenWidth = Screen.width;
		screenHeight = Screen.height;
	}
	
	// Update is called once per frame
	void Update () {
		
		// Prob need to update DOF on tick if in minigame
	}

	public void InitMiniGame (PocketPalParent targetPocketPal) {

		// temp
		MapUI.enabled = false;
		MiniGameUI.enabled = true;
		viewFinder.enabled = true;

		minigameTimer = 0.0f;
		minigameTimeAllowance = 10.0f;
		captureTimer = 0.0f;

		pocketPal = targetPocketPal;

		// Initially set the viewport image to the centre of the screen
		viewFinder.rectTransform.anchoredPosition = Camera.main.ViewportToScreenPoint (new Vector3 (0.0f, 0.0f));

		controls.cameraController.EnableDepthOfField (true);

		// Set initial post processing to centre of screen
		AdjustPostProcessing (new Vector2 (screenWidth, screenHeight) / 2.0f);

		controls.MiniGameControls ();
	}

	public void UpdateTimer () {

		if (minigameTimer < minigameTimeAllowance) {

			minigameTimer += Time.deltaTime;

		} else {

			// Add to the player's inventory
			pocketPal.Captured();

			MapUI.enabled = true;
			MiniGameUI.enabled = false;
			viewFinder.enabled = false;

			controls.cameraController.zoomOutInit ();

			controls.DisableControls();

			controls.cameraController.EnableDepthOfField (false);
		}
	}

	public void UpdateControls (Vector2 touch) {

		//	Vector2 touchPosition = touch.position;

		float touchX = touch.x / screenWidth - 0.5f;
		float touchY = touch.y / screenHeight - 0.5f;

		viewFinder.rectTransform.anchoredPosition = Camera.main.ViewportToScreenPoint (new Vector3 (touchX, touchY));

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

				float distance = Vector3.Distance (controls.cameraController.getCamera().transform.position, hit.transform.gameObject.transform.position);

				controls.cameraController.SetDepthOfFieldAndFocalLength (distance, pocketPalAperture);
				
				captureTimer += Time.deltaTime;

			} else {

				controls.cameraController.SetDepthOfFieldAndFocalLength (hit.distance, defaultAperture);
			}

		} else {
			
			controls.cameraController.SetDepthOfFieldAndFocalLength (unfocusedDOFDistance, defaultAperture);
		}
	}
}
