using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaptureMiniGame : MonoBehaviour {

	public TouchHandler controls;

	public Image viewFinder;
	float minigameTimer = 0.0f;
	float minigameTimeAllowance = 4.0f;
	float captureTimer = 0.0f;

	// The targeted pocket pal for this minigame
	PocketPalParent pocketPal;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void InitMiniGame (PocketPalParent targetPocketPal) {

		// temp
		viewFinder.enabled = true;

		minigameTimer = 0.0f;
		minigameTimeAllowance = 6.0f;
		captureTimer = 0.0f;

		pocketPal = targetPocketPal;

		controls.MiniGameControls ();
	}

	public void UpdateTimer () {

		if (minigameTimer < minigameTimeAllowance) {

			minigameTimer += Time.deltaTime;

		} else {

			// Add to the player's inventory
			pocketPal.Captured();

			viewFinder.enabled = false;

			controls.cameraController.zoomOutInit ();

			controls.DisableControls();
		}
	}

	public void UpdateControls (Touch touch) {

		Vector2 touchPosition = touch.position;

		viewFinder.rectTransform.anchoredPosition = touchPosition;
	}
}
