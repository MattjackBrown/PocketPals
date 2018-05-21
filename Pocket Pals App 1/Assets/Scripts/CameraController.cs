using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;

public class CameraController : MonoBehaviour {

	public float minimumCameraDistance = 4.0f;
	public float maximumCameraDistance = 15.0f;
	float currentCameraDistance;
	bool IsDebug = false;

	// The look at position for the camera added to the player position. Roughly shoulder or head position
	Vector3 lookAtPositionPlayerOffset = new Vector3(0.0f, 2.0f, 0.0f);

	// The player gameObject from which to get the player's location and inventory from
	public GameObject player;

	// The ui compass that will rotate with the view rotation
	public GameObject compass;

	// The camera for which to use the world to screen location to determine swipe map rotation direction
	public Camera gameCamera;

	// The max distance allowed by the raycast hit detection
	public float maxCaptureDistance = 10.0f;

	Touch touchZero;
	Touch touchOne;
	Vector3 playerPosition;

	// Use this for initialization
	void Start () {

		// Set the transform rotation to look at the player + the look at position offset
		transform.LookAt (player.transform.position + lookAtPositionPlayerOffset);
	}
	
	// Update is called once per frame
	void Update () {

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

	void CheckForTap() {
		
		// Create a rayCastHit object
		RaycastHit hit = new RaycastHit ();

		// Look at each touch
		for (int i = 0; i < Input.touchCount; i++) {

			// If touch has just begun
			if (Input.GetTouch (i).phase.Equals (TouchPhase.Began)) {

				// Raycast from the touch position
				Ray ray = Camera.main.ScreenPointToRay (Input.GetTouch (i).position);

				// if hit
				if (Physics.Raycast (ray, out hit, maxCaptureDistance)) {

					if (IsDebug) {
						GetComponentInParent<GPS> ().SetIsDebug (true);
						GetComponentInParent<GPS> ().SetPlayerMovePoint (hit.transform.position);
						Debug.DrawLine (Input.mousePosition, hit.transform.position, Color.red, 10000, false);
					}
					if (hit.transform.gameObject.GetComponent ("PocketPalParent")) {
						
						PocketPalParent hitPocketPal = (PocketPalParent)hit.transform.gameObject.GetComponent ("PocketPalParent");
						Debug.Log (hitPocketPal.PocketPalID);

						// Add to the player's inventory
						player.GetComponent<PocketPalInventory>().AddPocketPal (hit.transform.gameObject);
					}
				}
			}
		}
	}

	void captureCam (GameObject pocketPal) {

		float captureCamDistance = 5.0f;
		float camZoomInSpeed = 10.0f;

		Vector3 pocketPalPosition = pocketPal.transform.position;

		Vector3 cameraTargetPosition = pocketPalPosition - (transform.position - pocketPalPosition).normalized * captureCamDistance;

		transform.position = Vector3.Lerp (transform.position, cameraTargetPosition, Time.deltaTime * camZoomInSpeed);
	}
}
