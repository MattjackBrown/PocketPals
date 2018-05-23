using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using UnityEngine.PostProcessing;

public class CameraController : MonoBehaviour {

	float minimumCameraDistance = 4.0f;
	float maximumCameraDistance = 15.0f;
	float currentCameraDistance;

	Vector3 playerPosition;

	// The look at position for the camera added to the player position. Roughly shoulder or head position
	Vector3 lookAtPositionPlayerOffset = new Vector3(0.0f, 2.0f, 0.0f);

	// The player gameObject from which to get the player's location and inventory from
	public GameObject player;

	// The ui compass that will rotate with the view rotation
	public GameObject compass;
	public TouchHandler controls;

	// The camera for which to use the world to screen location to determine swipe map rotation direction
	Camera gameCamera;

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

	PostProcessingProfile postProcessing;
	float miniGameFocalLength = 0.1f;


	// Use this for initialization
	void Start () {

		// Set the transform rotation to look at the player + the look at position offset
		transform.LookAt (player.transform.position + lookAtPositionPlayerOffset);

		// Get the Camera component from the parent
		gameCamera = GetComponentInParent<Camera>();

		postProcessing = GetComponentInParent<PostProcessingBehaviour> ().profile;

		// Have to use a temporary variable to set the post processing settings
		var DOFSettings = postProcessing.depthOfField.settings;
		DOFSettings.focalLength = miniGameFocalLength;
		postProcessing.depthOfField.settings = DOFSettings;

		postProcessing.depthOfField.enabled = false;
	}
/*	
	// Update is called once per frame
	void Update () {
		
	}
*/
	public void PinchZoom(Touch touchZero, Touch touchOne) {

		// Get the position of the player
		playerPosition = player.transform.position;

		// Get the distance of the camera from the player at the start of this frame
		currentCameraDistance = (transform.position - playerPosition).magnitude;

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

	public void RotateMap(Touch touchZero) {

		// Get the position of the player
		playerPosition = player.transform.position;

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

	public void CaptureCamInit (GameObject pocketPal) {

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
		controls.DisableControls();
	}

	public void UpdateCaptureCam() {
		
		if (isZoomingIn) {
			MoveCaptureCamToCaptureView ();
		} else {
			MoveCaptureCamToMapView ();
		}
	}

	public void zoomOutInit() {

		isZoomingIn = false;

		zoomLerp = 0.0f;
	}

	void MoveCaptureCamToCaptureView() {
		
		// Check if arrived. Lerp is complete when == 1.0f
		if (zoomLerp >= 1.0f) {

			// Init minigame!!!
			controls.miniGame.InitMiniGame (targetPocketPal.GetComponent<PocketPalParent>());

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
			controls.MapControls();

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

	public Camera getCamera() {
		return gameCamera;
	}

	public void EnableDepthOfField(bool isEnabled) {
		postProcessing.depthOfField.enabled = isEnabled;
	}

	public void SetDepthOfField(float distance) {

		// Have to use a temporary variable to change the post processing settings
		var DOFSettings = postProcessing.depthOfField.settings;
		DOFSettings.focusDistance = distance;
		postProcessing.depthOfField.settings = DOFSettings;
	}

	public void SetDepthOfFieldAndFocalLength(float distance, float aperture) {

		// Have to use a temporary variable to change the post processing settings
		var DOFSettings = postProcessing.depthOfField.settings;
		DOFSettings.focusDistance = distance;
		DOFSettings.aperture = aperture;
		postProcessing.depthOfField.settings = DOFSettings;
	}
}
