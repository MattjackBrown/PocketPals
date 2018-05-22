using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {

	public float minimumCameraDistance = 4.0f;
	public float maximumCameraDistance = 15.0f;
	float currentCameraDistance;
	public bool IsDebug = false;

	// The look at position for the camera added to the player position. Roughly shoulder or head position
	Vector3 lookAtPositionPlayerOffset = new Vector3(0.0f, 2.0f, 0.0f);

	// The player gameObject from which to get the player's location and inventory from
	public GameObject player;

	// The ui compass that will rotate with the view rotation
	public GameObject compass;

	// The camera for which to use the world to screen location to determine swipe map rotation direction
	Camera gameCamera;

	// The max distance allowed by the raycast hit detection
	public float maxCaptureDistance = 15.0f;

	// The distance of the camera away from the pocket pal for the minigame
	float captureCamDistance = 3.0f;

	// Zoom in speed for the camera from the game view to the minigame view
	float captureZoomInSpeed = 0.8f;

	// To store the game view camera position relative to the player, to return to after the minigame
	Vector3 returnCamOffsetAfterCapture;

	// The positions used to lerp the camera position from map to minigame view
	Vector3 cameraTargetPosition, cameraLookAtPoint, zoomCamStartPosition, zoomCamLookAtStartPosition, targetPocketPalPosition;

	GameObject targetPocketPal;
	bool isZoomingIn = false;
	float zoomLerp;

	Touch touchZero, touchOne;

	Vector3 playerPosition;

	// The state machine for the controls
	enum ControlScheme {
		disabled,
		map,
		miniGame,
	}

	// Initialise as the map controls
	ControlScheme controlScheme = ControlScheme.map;



	public Image viewFinder;
	float minigameTimer = 0.0f;
	float minigameTimeAllowance = 4.0f;
	float captureTimer = 0.0f;




	// Use this for initialization
	void Start () {

		// Set the transform rotation to look at the player + the look at position offset
		transform.LookAt (player.transform.position + lookAtPositionPlayerOffset);

		// Get the Camera component from the parent
		gameCamera = GetComponentInParent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {

        //used for testing on the pc
        if (IsDebug && (Input.GetMouseButtonDown(0)))
        {
            DebugTouch();
        }

		// Choose how to parse the touch controls based on the current control scheme
		switch (controlScheme) {

		// Allows rotating, zooming, and tapping on pocketPals to initiate capture
		case ControlScheme.map:
			{
				// Switch statement behaves differently with different number of touches
				switch (Input.touchCount) { 

				// Check for pinch to zoom control. The only control to use > two touches. Uses fallthrough cases
				case 4:
				case 3:
				case 2:
					PinchZoom ();
					break;

				// check for a single touch swiping
				case 1:
					RotateMap ();
					CheckForTap ();
					break;
				}
			}
			break;

		case ControlScheme.miniGame:
			{
				UpdateMiniGame (touchZero.position);
			}
			break;

		// 
		case ControlScheme.disabled:
			{
				// Currently controls are only disabled when camera is transitioning between map and minigame views
				if (isZoomingIn) {
					MoveCaptureCamToCaptureView ();
				} else {
					MoveCaptureCamToMapView ();
				}
			}
			break;
		}
	}

	void PinchZoom() {

		// Get the position of the player
		playerPosition = player.transform.position;

		// Get the distance of the camera from the player at the start of this frame
		currentCameraDistance = (transform.position - playerPosition).magnitude;

		// Just store the first two touches, two is all we need
		touchZero = Input.GetTouch (0);
		touchOne = Input.GetTouch (1);

		// Find the position in the previous frame of each touch
		Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
		Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

		// Find the magnitude of the vector (the distance) between the touches in each frame
		// Previous frame
		float prevTouchDelta = (touchZeroPrevPos - touchOnePrevPos).magnitude;
		// Current frame
		float currentTouchDelta = (touchZero.position - touchOne.position).magnitude;

		// Find the difference in the distances between each frame
		float deltaTouchDifference = prevTouchDelta - currentTouchDelta;

		// Adjust for different device's screen pixel density
		deltaTouchDifference = 1 + deltaTouchDifference / Screen.width;

		// Apply the modifier to the current camera distance
		currentCameraDistance *= deltaTouchDifference;

		// Clamp between min and max allowed values
		currentCameraDistance = Mathf.Clamp (currentCameraDistance, minimumCameraDistance, maximumCameraDistance);

		// Calculate the new position of the camera
		// Get the current camera vector
		Vector3 currentCameraVector = (transform.position - playerPosition).normalized;

		// Set the new camera position from the player position + the camera vector * the new distance
		transform.position = playerPosition + currentCameraVector * currentCameraDistance;

		// Set the transform rotation to look at the player + the look at position offset
		transform.LookAt (playerPosition + lookAtPositionPlayerOffset);
	}

	void RotateMap() {

		// Get the position of the player
		playerPosition = player.transform.position;

		// Store the touch
		touchZero = Input.GetTouch (0);

		// Get the horixontal component of the touch's movement
		float deltaTouchX = touchZero.deltaPosition.x;

		// rotation direction reversed if touch is below the player figure
		if (touchZero.position.y < gameCamera.WorldToScreenPoint (playerPosition).y)

			// Adjust for different device's screen widths. Then multiply by the angles that a total screen width swipe will rotate
			deltaTouchX = deltaTouchX / Screen.width * 360;
		else
			deltaTouchX = -deltaTouchX / Screen.width * 360;

		// Repeat for deltaY touch to enable circling movements to circle the map around
		float deltaTouchY = touchZero.deltaPosition.y;

		// Rotation direction reversed if touch is right of the player figure
		if (touchZero.position.x > gameCamera.WorldToScreenPoint (playerPosition).x)

			// Adjust for different device's screen widths. Then multiply by the angles that a total screen width swipe will rotate
			deltaTouchY = deltaTouchY / Screen.height * 180;
		else
			deltaTouchY = -deltaTouchY / Screen.height * 180;

		// Combine both swipe directions into one final value
		float finalRotation = deltaTouchX + deltaTouchY;

		// Apply the rotation to the transform around the player
		transform.RotateAround (playerPosition, Vector3.up, finalRotation);

		// Apply the rotation to the compass ui
		compass.transform.localRotation = Quaternion.Euler (compass.transform.localRotation.eulerAngles + new Vector3 (0.0f, 0.0f, finalRotation));
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
                CaptureCamInit(hit.transform.gameObject);
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

	void CheckForTap() {
		
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
					if (hit.transform.gameObject.GetComponent ("PocketPalParent") && (hit.transform.position - playerPosition).magnitude < maxCaptureDistance)
                    {
						// Initialise the capture cam values
						CaptureCamInit (hit.transform.gameObject);
						
						PocketPalParent hitPocketPal = (PocketPalParent)hit.transform.gameObject.GetComponent ("PocketPalParent");
						Debug.Log (hitPocketPal.PocketPalID);
					}
				}
			}
		}
	}

	void CaptureCamInit (GameObject pocketPal) {

		// Store the pocketPal
		targetPocketPal = pocketPal;

		zoomCamStartPosition = transform.position;
		zoomCamLookAtStartPosition = playerPosition + lookAtPositionPlayerOffset;

		// Store the starting position from the player to return to after minigame finished
		returnCamOffsetAfterCapture = transform.position - player.transform.position;

		// Get the target camera position based on player position and pocketPalPosition
		targetPocketPalPosition = targetPocketPal.transform.position;
		cameraTargetPosition = targetPocketPalPosition + (playerPosition - targetPocketPalPosition).normalized * captureCamDistance;

		cameraTargetPosition = new Vector3 (cameraTargetPosition.x, lookAtPositionPlayerOffset.y, cameraTargetPosition.z);

		// So that Update() knows to zoom in
		isZoomingIn = true;

		// For the Vector3.lerp function
		zoomLerp = 0.0f;

		// Disable the controls while the camera zooms in
		controlScheme = ControlScheme.disabled;
	}

	void MoveCaptureCamToCaptureView() {
		
		// Check if arrived. Lerp is complete when == 1.0f
		if (zoomLerp >= 1.0f) {

			// Init minigame!!!
			InitMiniGame ();

		} else {

			// Advance the lerp float
			zoomLerp += Time.deltaTime * captureZoomInSpeed;

			// Not sure about this bit. It works fine but the lerp may have to be done differently to smooth
			transform.position = Vector3.Lerp (zoomCamStartPosition, cameraTargetPosition, zoomLerp);

			// Lerp the lookAtPoint
			cameraLookAtPoint = Vector3.Lerp (zoomCamLookAtStartPosition, targetPocketPalPosition, zoomLerp);

			// Set the look at transform for the camera
			transform.LookAt (cameraLookAtPoint);
		}
	}

	void MoveCaptureCamToMapView() {

		// Check if arrived. Lerp is complete when == 1.0f
		if (zoomLerp >= 1.0f) {

			// Return to map controls
			controlScheme = ControlScheme.map;

		} else {

			// Advance the lerp float
			zoomLerp += Time.deltaTime * captureZoomInSpeed;

			// Update the player position
			playerPosition = player.transform.position;

			// Not sure about this bit. It works fine but the lerp may have to be done differently to smooth.
			// * 1.1f used as the self calling lerp doesn't ever reach the target. This tells it to overshoot the target
			transform.position = Vector3.Lerp (cameraTargetPosition, returnCamOffsetAfterCapture + playerPosition, zoomLerp);

			// Lerp the lookAtPoint
			cameraLookAtPoint = Vector3.Lerp (targetPocketPalPosition, playerPosition + lookAtPositionPlayerOffset, zoomLerp);

			// Set the look at transform for the camera
			transform.LookAt (cameraLookAtPoint);
		}
	}

	void InitMiniGame () {

		// temp
		viewFinder.enabled = true;

		minigameTimer = 0.0f;
		minigameTimeAllowance = 6.0f;
		captureTimer = 0.0f;

		controlScheme = ControlScheme.miniGame;

		// Set the control scheme to the minigame scheme
//		controlScheme = ControlScheme.miniGame;

		// Set the background image

		// Init gameplay
	}

	void UpdateMiniGame (Vector2 touchLocation) {

		if (minigameTimer < minigameTimeAllowance) {

			minigameTimer += Time.deltaTime;

			viewFinder.rectTransform.anchoredPosition = touchLocation;

		} else {

			// Add to the player's inventory
			player.GetComponent<PocketPalInventory> ().AddPocketPal (targetPocketPal);

			viewFinder.enabled = false;

			isZoomingIn = false;

			zoomLerp = 0.0f;

			// Can probably just do this once somewhere
			controlScheme = ControlScheme.disabled;
		}
	}
}
