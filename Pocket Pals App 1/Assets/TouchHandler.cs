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
		VirtualGardenCameraTransition
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
			}
			break;

		case ControlScheme.VirtualGardenCameraTransition:
			{
				cameraController.VGMoveCamToNextPPal ();
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

	public void InitVirtualGardenControls() {
		virtualGardenTouchPlaced = false;
	}

	public void SetStartTouchPosition (Vector2 position) {
		startTouchPosition = position;
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
				Debug.Log("hiu");
				PocketPalParent hitPocketPal = hit.transform.gameObject.GetComponent<PocketPalParent>();
				Debug.Log(hitPocketPal.PocketPalID);
			}
			else if (IsDebug)
			{
				player.GetComponent<GPS>().SetIsDebug(true);
				player.GetComponent<GPS>().SetPlayerMovePoint(hit.transform.position);
			}
		}
	}

	public void CheckForTap() {

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
					}
				}
			}
		}
	}

	void UseMapControls() {

		if (IsDebug && Input.GetMouseButtonDown(0)) DebugTouch();

		// Switch statement behaves differently with different number of touches
		switch (Input.touchCount) { 

		// Check for pinch to zoom control. The only control to use > two touches. Uses fallthrough cases
		case 4:
		case 3:
		case 2:
			cameraController.PinchZoom (Input.GetTouch(0), Input.GetTouch(1));
			break;

			// check for a single touch swiping
		case 1:
			cameraController.RotateMap (Input.GetTouch(0));
			CheckForTap ();
			break;
		}
	}

	void UseVirtualGardenControls() {
		
		if(Input.GetMouseButtonDown(0))Debug.Log("Cannot rotate camera using mouse button try touches,");

		if (Input.touches.Length > 0) {

			// A lot simpler if we only look at touch(0)
			Touch touchZero = Input.GetTouch (0);

			// Controls begin after touch(0) is placed when in virtual garden. Previous touches are ignored
			if (touchZero.phase == TouchPhase.Began) {

				// Update the start touch position
				startTouchPosition = touchZero.position;

				// Virtual garden controls are not reachable until this is true. Ignores previous pre VG touches
				virtualGardenTouchPlaced = true;

			} else if (virtualGardenTouchPlaced) {

				// If the delta touch position.x is above the threshold needed to look at next PPal
				if (touchZero.position.x - startTouchPosition.x > swipeLengthToLookAtNext * Screen.width) {

					// Init VirtualGardenCameraTransition. ControlScheme, Init function in cameraController
					cameraController.VGInitLookAtNextPPal (virtualGarden.GetNextPPal());

				} else if (touchZero.position.x - startTouchPosition.x < -swipeLengthToLookAtNext * Screen.width) {

					cameraController.VGInitLookAtNextPPal (virtualGarden.GetPreviousPPal());

				} else {

					// Allow a rotation movement when not switching between inventory animals, add in small up and down?
					cameraController.VGRotateCamera (touchZero);

				}
			}
		}
	}
}
