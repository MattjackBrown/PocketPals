using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchHandler : MonoBehaviour {
	
	public GameObject player;
	public CameraController cameraController;
	public CaptureMiniGame miniGame;
	public VirtualSceneParent virtualGarden;

	// The state machine for the controls
	enum ControlScheme {
		map,
		mapCameraTransition,
		menu,
		miniGame,
		VirtualGarden,
		VirtualGardenCameraTransition,
		VirtualGardenInfo,
		VirtualGardenInfoTransition
	}

	ControlScheme controlScheme;

	public bool IsDebug = false;

	// The max distance allowed by the raycast hit detection
	public float maxCaptureDistance = 15.0f;

	// Used to determine swipe length in VirtualGarden controlScheme
	Vector2 startTouchPosition;
	bool virtualGardenTouchPlaced = false;

	// Portion of screen width needed to swipe to look at next inventory in virtual garden
	float swipeLengthToLookAtNext = 0.2f;



	// Use this for initialization
	void Start () {
		
		// Initialise as the map controls
		controlScheme = ControlScheme.map;
	}
	
	// Update is called once per frame
	void Update ()
    {

		// Choose how to parse the touch controls based on the current control scheme
		switch (controlScheme) {

		case ControlScheme.map:
			{
                UseMapControls ();
			}
			break;

		case ControlScheme.mapCameraTransition:
			{
				cameraController.UpdateCaptureCam();
			}
			break;

		case ControlScheme.miniGame:
			{
				miniGame.UpdateTimer ();

                if (IsDebug && Input.GetMouseButtonDown(0))  miniGame.UpdateControls(Input.mousePosition);
                else if(Input.touches.Length > 0) miniGame.UpdateControls(Input.GetTouch(0).position);
			}
			break;

		case ControlScheme.menu:
			{

			}
			break;

		case ControlScheme.VirtualGarden:
			{
				UseVirtualGardenControls ();

				//CheckForVGTap ();
			}
			break;

		case ControlScheme.VirtualGardenCameraTransition:
			{
				cameraController.VGMoveCamToNextPPal ();
			}
			break;

		case ControlScheme.VirtualGardenInfo:
			{
				// Temp. Just tap anywhere to return
				if (Input.touches.Length > 0 && Input.GetTouch(0).phase == TouchPhase.Began) cameraController.VGInfoZoomOutInit();
			}
			break;

		case ControlScheme.VirtualGardenInfoTransition:
			{
				cameraController.VGUpdateInfoCam ();
			}
			break;
		}
	}

	// Setters for all controlSchemes
	public void MapControls() {
		controlScheme = ControlScheme.map;
	}
	
	public void MapCameraTransition() {
		controlScheme = ControlScheme.mapCameraTransition;
	}

	public void MenuControls() {
		controlScheme = ControlScheme.menu;
	}

	public void MiniGameControls() {
		controlScheme = ControlScheme.miniGame;
	}

	public void VirtualGardenControls() {
		
		// Set a new start position for swipe controls to stop continuous swiping
		if (IsDebug && Input.GetMouseButtonDown (0))
			startTouchPosition = Input.mousePosition;
		else if (Input.touches.Length > 0)
			startTouchPosition = Input.GetTouch (0).position;
		
		controlScheme = ControlScheme.VirtualGarden;
	}

	public void VirtualGardenCameraTransitionControls() {
		controlScheme = ControlScheme.VirtualGardenCameraTransition;
	}

	public void VirtualGardenInfoControls() {
		controlScheme = ControlScheme.VirtualGardenInfo;
	}

	public void VirtualGardenInfoTransitionControls () {
		controlScheme = ControlScheme.VirtualGardenInfoTransition;
	}

	public void InitVirtualGardenControls() {
		virtualGardenTouchPlaced = false;
	}

	public void SetStartTouchPosition (Vector2 position) {
		startTouchPosition = position;
	}

	public bool CameraShouldFollowGPS() {
		if (controlScheme == ControlScheme.miniGame || 
			controlScheme == ControlScheme.VirtualGarden ||
			controlScheme == ControlScheme.VirtualGardenCameraTransition ||
			controlScheme == ControlScheme.VirtualGardenInfo)
			return false;
		else
			return true;
	}

	void DebugTouch()
	{
		//declare a variable of RaycastHit struct
		RaycastHit hit;
		//Create a Ray on the tapped / clicked position
		Ray ray;
		//for unity editor
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		//for touch device

		//Check if the ray hits any collider
		if (Physics.Raycast(ray, out hit))
		{
			if (hit.transform.gameObject.GetComponent<PocketPalParent>())
			{
				cameraController.CaptureCamInit(hit.transform.gameObject);
				PocketPalParent hitPocketPal = hit.transform.gameObject.GetComponent<PocketPalParent>();
			}
			else if (IsDebug)
			{
				player.GetComponent<GPS>().SetIsDebug(true);
				player.GetComponent<GPS>().SetPlayerMovePoint(hit.transform.position);
			}
		}
	}

	public void CheckForMapTap() {

		// Create a rayCastHit object
		RaycastHit hit = new RaycastHit ();

		// Look at each touch
		for (int i = 0; i < Input.touchCount; i++)
		{
			// If touch has just begun
			if (Input.GetTouch (i).phase.Equals (TouchPhase.Began))
			{
				// Raycast from the touch position
				Ray ray = Camera.main.ScreenPointToRay (Input.GetTouch (i).position);

				// if hit
				if (Physics.Raycast (ray, out hit))
				{
					// If the hit gameObject has a component "PocketPalParent" and is within the capture distance from the player
					if (hit.transform.gameObject.GetComponent ("PocketPalParent") && (hit.transform.position - player.transform.position).magnitude < maxCaptureDistance)
					{
						// Initialise the capture cam values
						cameraController.CaptureCamInit (hit.transform.gameObject);

						PocketPalParent hitPocketPal = (PocketPalParent)hit.transform.gameObject.GetComponent ("PocketPalParent");
						Debug.Log (hitPocketPal.PocketPalID);

						// Only need to find one, Don't bother checking other touches after this
						return;
					}
				}
			}
		}
	}

	void UseMapControls() {

        if (IsDebug && Input.GetMouseButtonDown(0))
        {
            DebugTouch();
            return;
        }

		// Switch statement behaves differently with different number of touches
		switch (Input.touchCount) { 

		// Check for pinch to zoom control. The only control to use > two touches. Uses fallthrough cases
		case 4:
		case 3:
		case 2:
			cameraController.MapPinchZoom (Input.GetTouch(0), Input.GetTouch(1));
			break;

			// check for a single touch swiping
		case 1:
			cameraController.MapRotate (Input.GetTouch(0));
			CheckForMapTap ();
			break;
		}
	}

	void CheckForVGTap () {
		
		// Create a rayCastHit object
		RaycastHit hit = new RaycastHit ();

		// Look at each touch
		for (int i = 0; i < Input.touchCount; i++)
		{
			// Store the touch
			Touch touch = Input.GetTouch (i);

			// If touch has just begun
			if (touch.phase.Equals (TouchPhase.Began))
			{
				// Raycast from the touch position
				Ray ray = Camera.main.ScreenPointToRay (touch.position);

				// if hit
				if (Physics.Raycast (ray, out hit))
				{
					Debug.Log ("Hit : " + hit.transform.gameObject);
					// This is not a good way. Need a cast to whatever data structure will be used here
					// Either PocketPalParent, or look for the int ID component of the VirtualGardenSpawn maybe
					if (hit.transform.gameObject.GetComponent("VirtualGardenInfo"))
					{

						Debug.Log ("Inspect");
						// Initialise the info cam values
						cameraController.VGInspectInit (hit.transform.gameObject);

						// Only need to find one, Don't bother checking other touches after this
						return;
					}
				}
			}
		}
	}

	void UseVirtualGardenControls() {
		
		if(Input.GetMouseButtonDown(0))Debug.Log("Cannot rotate camera using mouse button try touches,");

		int numberOfTouches = Input.touches.Length;

		if (numberOfTouches > 1) {

			// Pinch zoom for the virtual garden
			cameraController.VGPinchZoom (Input.GetTouch(0), Input.GetTouch(1));

		} else if (numberOfTouches == 1) {

			Touch touchZero = Input.GetTouch (0);

			// Controls begin after touch(0) is placed when in virtual garden. Previous touches are ignored
			if (touchZero.phase == TouchPhase.Began) {

				// Update the start touch position
				startTouchPosition = touchZero.position;

				// Virtual garden controls are not reachable until this is true. Ignores previous pre VG touches
				virtualGardenTouchPlaced = true;

			// if the last present touch has just ended. No touches present after this
			} else if (touchZero.phase == TouchPhase.Ended) {

				// Init the camera returning to a default direction when no touches present
				cameraController.VGInitReturn ();

			} else if (virtualGardenTouchPlaced) {

				// If the delta touch position.x is above the threshold needed to look at next PPal
				if (touchZero.position.x - startTouchPosition.x > swipeLengthToLookAtNext * Screen.width) {

					GameObject nextPPal = virtualGarden.GetPreviousPPal ();

					if (nextPPal != null) {
						// Init lerp values to look at the previous PPal
						cameraController.VGInitLookAtNextPPal (nextPPal);
					} else {
						// If no PPals in inventory just rotate
						cameraController.VGRotateCamera (touchZero);
					}

				} else if (touchZero.position.x - startTouchPosition.x < -swipeLengthToLookAtNext * Screen.width) {

					GameObject nextPPal = virtualGarden.GetNextPPal ();

					if (nextPPal != null) {
						// Init lerp values to look at the previous PPal
						cameraController.VGInitLookAtNextPPal (nextPPal);
					} else {
						// If no PPals in inventory just rotate
						cameraController.VGRotateCamera (touchZero);
					}

				} else {

					// Allow a rotation movement when not switching between inventory animals, add in small up and down?
					cameraController.VGRotateCamera (touchZero);
				}
			}
		} else {

			// With no touch present return the orientation of the camera to look directly at the targeted inventory PPal
			cameraController.VGReturnCamToTargetedPPal ();
		}
	}
}
