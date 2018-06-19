using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using UnityEngine.PostProcessing;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { set; get; }

	float minimumCameraDistance = 4.0f;
	float maximumCameraDistance = 15.0f;
	float currentCameraDistance;

	Vector3 playerPosition;

	// The look at position for the camera added to the player position. Roughly shoulder or head position
	Vector3 lookAtPlayerPositionOffset = new Vector3(0.0f, 2.0f, 0.0f);

	// The player gameObject from which to get the player's location and inventory from
	public GameObject player;

	// The ui compass that will rotate with the view rotation
	public GameObject compass;
	public TouchHandler controls;

	// The camera for which to use the world to screen location to determine swipe map rotation direction
	Camera gameCamera;

	// The distance of the camera away from the pocket pal for the minigame
	float captureCamDistance = 4.0f;

	// Zoom in speed for the camera from the game view to the minigame view
	float captureZoomInSpeed = 0.8f;

	// Rotation and movement speed when changing the looked at PPal in the virtual garden
	float virtualGardenMovementSpeed = 1.5f;

	// To store the game view camera position relative to the player, to return to after the minigame
	Vector3 returnCamOffsetAfterCapture;

	// The positions used to lerp the camera position from map to minigame view
	Vector3 cameraTargetPosition, cameraLookAtPoint, cameraStartPosition, cameraLookAtStartPosition, targetPocketPalPosition;

	GameObject targetPocketPal;

	bool isZoomingIn = false;
	bool isMovingToInspect = false;
	float lerp, VGPinchLerp;

	// For the depth of field in the minigame
	PostProcessingProfile postProcessing;
	float miniGameFocalLength = 0.1f;

	// The offset of the camera to the player to return to after viewing a virtual garden
	Vector3 returnCamOffsetAfterGarden;

	// Used as a central point to calculate virtual garden PPal inspect positions
	public GameObject VGCentre;
	Vector3 VGZoomedPosition, VGInfoLookAtPoint, VGInfoCamOffset;

	// Use this for initialization
	void Start () {

        Instance = this;

		// Set the transform rotation to look at the player + the look at position offset
		transform.LookAt (player.transform.position + lookAtPlayerPositionOffset);

		// Get the Camera component from the parent
		gameCamera = GetComponentInParent<Camera>();

		postProcessing = GetComponentInParent<PostProcessingBehaviour> ().profile;

		// Have to use a temporary variable to initialise the post processing settings
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
	public void MapPinchZoom(Touch touchZero, Touch touchOne) {

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
		deltaTouchDifference = 1 + deltaTouchDifference * 2.0f / Screen.width;

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
		transform.LookAt (playerPosition + lookAtPlayerPositionOffset);
	}

	public void MapRotate(Touch touchZero) {

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

		playerPosition = player.transform.position;

		// Store the pocketPal
		targetPocketPal = pocketPal;

		cameraStartPosition = transform.position;
		cameraLookAtStartPosition = playerPosition + lookAtPlayerPositionOffset;

		// Store the starting position from the player to return to after minigame finished
		returnCamOffsetAfterCapture = transform.position - playerPosition;

		// Get the target camera position based on player position and pocketPalPosition
		targetPocketPalPosition = targetPocketPal.transform.position;
		cameraTargetPosition = targetPocketPalPosition + (playerPosition - targetPocketPalPosition).normalized * captureCamDistance;

		cameraTargetPosition = new Vector3 (cameraTargetPosition.x, lookAtPlayerPositionOffset.y, cameraTargetPosition.z);

		// So that Update() knows to zoom in
		isZoomingIn = true;

		// For the Vector3.lerp function
		lerp = 0.0f;

		// Disable the controls while the camera zooms in
		controls.MapCameraTransition();
	}

	public void UpdateCaptureCam() {
		
		if (isZoomingIn) {
			MoveCaptureCamToCaptureView ();
		} else {
			MoveCaptureCamToMapView ();
		}
	}

	public void MapZoomOutInit() {

		// Does this need a position update?

		isZoomingIn = false;
		lerp = 0.0f;
		controls.MapCameraTransition ();
	}

	void MoveCaptureCamToCaptureView() {
		
		// Check if arrived. Lerp is complete when == 1.0f
		if (lerp >= 1.0f) {

			// Init minigame!!!
			controls.miniGame.InitMiniGame (targetPocketPal.GetComponent<PocketPalParent>());

		} else {

			// Advance the lerp float
			lerp += Time.deltaTime * captureZoomInSpeed;

			// Not sure about this bit. It works fine but the lerp may have to be done differently to smooth
			transform.position = Vector3.Lerp (cameraStartPosition, cameraTargetPosition, lerp);

			// Lerp the lookAtPoint
			cameraLookAtPoint = Vector3.Lerp (cameraLookAtStartPosition, targetPocketPalPosition, lerp);

			// Set the look at transform for the camera
			transform.LookAt (cameraLookAtPoint);
		}
	}

	void MoveCaptureCamToMapView() {

		// Check if arrived. Lerp is complete when == 1.0f
		if (lerp >= 1.0f) {

			// Return to map controls
			controls.MapControls();

		} else {

			// Advance the lerp float
			lerp += Time.deltaTime * captureZoomInSpeed;

			// Update the player position
			playerPosition = player.transform.position;

			// Not sure about this bit. It works fine but the lerp may have to be done differently to smooth.
			transform.position = Vector3.Lerp (cameraTargetPosition, returnCamOffsetAfterCapture + playerPosition, lerp);

			// Lerp the lookAtPoint
			cameraLookAtPoint = Vector3.Lerp (targetPocketPalPosition, playerPosition + lookAtPlayerPositionOffset, lerp);

			// Set the look at transform for the camera
			transform.LookAt (cameraLookAtPoint);
		}
	}

	public void SetCameraPosition (Vector3 newPosition) {
		transform.position = newPosition;
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

	public void InitVirtualGardenTour (Vector3 newCameraPosition, Vector3 startingLookAtPoint) {

		// Store the camera offset from the player
		returnCamOffsetAfterGarden = transform.position - player.transform.position;

		// Set the camera to the new location and rotation
		transform.position = newCameraPosition;
		transform.LookAt (startingLookAtPoint);

		// Tell the controls to ignore current touches and wait for a virtual garden control
		controls.InitVirtualGardenControls ();

		// Set the controls to the virtual garden scheme
		controls.VirtualGardenControls ();
	}

	public void ReturnCamToAfterVirtualGarden () {

		// Set the camera position to the player's position + the pre garden tour offset
		transform.position = player.transform.position + returnCamOffsetAfterGarden;

		// Set the transform rotation to look at the player + the look at position offset
		transform.LookAt (playerPosition + lookAtPlayerPositionOffset);

		// Return the controls to the map scheme
		controls.MapControls ();
	}

	public void VGRotateCamera (Touch touch) {

		// Get the delta x and y positions and adjust for screen dimensions and sensitivity
		float yRotation = touch.deltaPosition.x / Screen.width * -60;
		float xRotation = touch.deltaPosition.y / Screen.height * 30;

		// Use temporary variable method to adjust transform
		Quaternion currentLocalRotation = transform.localRotation;

		// Add the new rotation
		Vector3 newLocalRotation = currentLocalRotation.eulerAngles + new Vector3 (xRotation, yRotation, 0.0f);

		// Convert to Quaternian and set
		transform.localRotation = Quaternion.Euler (newLocalRotation);
	}

	public void VGInitLookAtNextPPal (GameObject pocketPal) {

		// Store the pocketPal
		targetPocketPal = pocketPal;

        //Check to make sure there is a target pocketpal
        if (targetPocketPal == null) return;

        // Store the camera start position
        cameraStartPosition = transform.position;

		// Get the current look at point. Anywhere along the current forward vector
		cameraLookAtStartPosition = cameraStartPosition + transform.forward;

		// Get the target camera position
		targetPocketPalPosition = targetPocketPal.transform.position;

		// Better way without hardcoding. Picks a targetPosition based on the direction to the centre and a cam distance
		cameraTargetPosition = controls.virtualGarden.GetViewPosition();

		// For the Vector3.lerp function
		lerp = 0.0f;
		VGPinchLerp = 0.0f;

		// Temp. VGZoomedPosition set as halfway between the camera position and the target PPal
		VGZoomedPosition = cameraTargetPosition + (targetPocketPalPosition - cameraTargetPosition) / 2.0f;

		// Set the controlScheme
		controls.VirtualGardenCameraTransitionControls ();
	}

	public void VGMoveCamToNextPPal () {

		// Check if arrived. Lerp is complete when == 1.0f
		if (lerp >= 1.0f) {

			// Set controls back to standard VG controls
			controls.VirtualGardenControls();

			// Do we still want DOF in the VG?
	//		SetDepthOfField()

			VGPinchLerp = 0.0f;

		} else {

			// Advance the lerp float
			lerp += Time.deltaTime * virtualGardenMovementSpeed;

			// Not sure about this bit. It works fine but the lerp may have to be done differently to smooth
			transform.position = Vector3.Lerp (cameraStartPosition, cameraTargetPosition, lerp);

			// Lerp the lookAtPoint
			cameraLookAtPoint = Vector3.Lerp (cameraLookAtStartPosition, targetPocketPalPosition, lerp);

			// Set the look at transform for the camera
			transform.LookAt (cameraLookAtPoint);
		}
	}

	public void VGInitReturn () {

		// Get the current look at point. Distance doesn't matter, just the direction vector being correct
		cameraLookAtStartPosition = transform.position  + transform.forward;

		// For the Vector3.lerp function
		lerp = 0.0f;
	}

	public void VGReturnCamToTargetedPPal () {

		// Empty inventory check
		if (targetPocketPal == null)
			return;
		
		// Opposite of VGMoveCamToNextPPal
		if (lerp < 1.0f) {

			// Decrease the lerp float (a bit slower than other VG movement)
			lerp += Time.deltaTime * virtualGardenMovementSpeed / 2.0f;

			// Lerp the lookAtPoint
			cameraLookAtPoint = Vector3.Lerp (cameraLookAtStartPosition, targetPocketPalPosition, lerp);

			// Set the look at transform for the camera
			transform.LookAt (cameraLookAtPoint);
		}
	}

	public void VGPinchZoom (Touch touchZero, Touch touchOne) {

		// Only zoom in virtual garden if a target has been set. i.e. if the inventory is empty
		if (targetPocketPal != null) {

			// Find the position in the previous frame of each touch
			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			// Find the magnitude of the vector (the distance) between the touches in each frame
			// Previous frame
			float prevTouchDelta = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			// Current frame
			float currentTouchDelta = (touchZero.position - touchOne.position).magnitude;

			// Find the difference in the distances between each frame
			float deltaTouchDifference = currentTouchDelta - prevTouchDelta;

			// Adjust for different device's screen pixel density
			deltaTouchDifference *= 2.0f / Screen.width;

			// Apply the modifier to the current camera distance
			VGPinchLerp += deltaTouchDifference;

			// Clamp between min and max allowed values
			VGPinchLerp = Mathf.Clamp (VGPinchLerp, 0.0f, 1.0f);

			transform.position = Vector3.Lerp (cameraTargetPosition, VGZoomedPosition, VGPinchLerp);
		}
	}

	public void VGToggleInspect () {

		// Show the info or not
		controls.virtualGarden.gUIManager.ToggleInspect ();

		// Button toggles the Inits on each press
		if (isMovingToInspect)
			VGInfoZoomOutInit ();
		else
			VGInspectInit ();
	}

	public void VGInspectInit () {

		// If targetPP has not been set, i.e. inventory empty
		if (targetPocketPal == null) return;

		// Store the camera start position
		cameraStartPosition = transform.position;

		// Get the current look at point. Anywhere along the current forward vector
		cameraLookAtStartPosition = cameraStartPosition + transform.forward;

		// Get the target camera position
		targetPocketPalPosition = targetPocketPal.transform.position;

		VGInfoLookAtPoint = controls.virtualGarden.GetInspectLookAtPosition ();

		cameraTargetPosition = controls.virtualGarden.GetInspectPosition ();

		// For the Vector3.lerp function
		lerp = 0.0f;
		VGPinchLerp = 0.0f;

		isZoomingIn = true;
		isMovingToInspect = true;

		// Set the controlScheme
		controls.VirtualGardenInfoTransitionControls();
	}

	public void VGInfoZoomOutInit() {

		isZoomingIn = false;
		isMovingToInspect = false;
		cameraStartPosition = controls.virtualGarden.GetViewPosition ();
		lerp = 0.0f;
		VGPinchLerp = 0.0f;
		controls.VirtualGardenInfoTransitionControls ();
	}

	public void VGUpdateInfoCam() {
		
		if (isZoomingIn)
			VGMoveToInspect ();
		else
			VGReturnFromInspect ();
	}

	void VGMoveToInspect () {

		// Check if arrived. Lerp is complete when == 1.0f
		if (lerp >= 1.0f) {

			// Change controlScheme
			controls.VirtualGardenInfoControls();

		//	VGUpdateCameraPositions ();

		} else {

			// Advance the lerp float (reusing cam zoom speed var from elsewhere here)
			lerp += Time.deltaTime * captureZoomInSpeed;

			// Not sure about this bit. It works fine but the lerp may have to be done differently to smooth
			transform.position = Vector3.Lerp (cameraStartPosition, cameraTargetPosition, lerp);

			// Lerp the lookAtPoint
			cameraLookAtPoint = Vector3.Lerp (cameraLookAtStartPosition, VGInfoLookAtPoint, lerp);

			// Set the look at transform for the camera
			transform.LookAt (cameraLookAtPoint);
		}
	}

	void VGReturnFromInspect () {

		// Check if arrived. Lerp is complete when == 1.0f
		if (lerp >= 1.0f) {

			// Return to VG controls
			controls.VirtualGardenControls ();

			VGUpdateCameraPositions ();

		} else {

			// Advance the lerp float
			lerp += Time.deltaTime * captureZoomInSpeed;

			// Not sure about this bit. It works fine but the lerp may have to be done differently to smooth.
			transform.position = Vector3.Lerp (cameraTargetPosition, cameraStartPosition, lerp);

			// Lerp the lookAtPoint
			cameraLookAtPoint = Vector3.Lerp (VGInfoLookAtPoint, targetPocketPalPosition, lerp);

			// Set the look at transform for the camera
			transform.LookAt (cameraLookAtPoint);
		}
	}

	public void VGInspectNextPPal () {

		targetPocketPal = controls.virtualGarden.GetNextPPal ();
		VGUpdateCameraPositions ();
		VGInspectInit ();
	}

	public void VGInspectPrevPPal () {

		targetPocketPal = controls.virtualGarden.GetPreviousPPal ();
		VGUpdateCameraPositions ();
		VGInspectInit ();
	}

	public void VGUpdateCameraPositions () {
		
		// Temp. VGZoomedPosition set as halfway between the camera position and the target PPal
		VGZoomedPosition = cameraTargetPosition + (targetPocketPalPosition - cameraTargetPosition) / 2.0f;
		cameraTargetPosition = controls.virtualGarden.GetViewPosition();
		VGPinchLerp = 0.0f;
	}
}
