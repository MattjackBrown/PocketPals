using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public float minimumCameraDistance = 4.0f;
	public float maximumCameraDistance = 15.0f;
	float currentCameraDistance;

	// The look at position for the camera added to the player position. Roughly shoulder or head position
	Vector3 lookAtPositionPlayerOffset = new Vector3(0.0f, 2.0f, 0.0f);

	// The player gameObject from which to get the player's location from
	public GameObject player;

	Touch touchZero;
	Touch touchOne;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		// Get the position of the player
		Vector3 playerPosition = player.transform.position;

		// Get the distance of the camera from the player at the start of this frame
		currentCameraDistance = (transform.position - playerPosition).magnitude;

		// Maybe swap all to a switch statement? Zero all variables on default case.
		// Tidier, but does not easily allow for >2 touches doing pinch zooming. Using fallthrough cases
		switch (Input.touchCount) {

		// Check for pinch to zoom control. The only control to use > two touches
		case 4:
		case 3:
		case 2:
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

			break;

			// check for a single touch swiping. Just left to right for now?
		case 1:
			// Store the touch
			touchZero = Input.GetTouch (0);

			// Get the horixontal component of the touch's movement
			float deltaTouchX = touchZero.deltaPosition.x;

			// Adjust for different device's screen widths. Then multiply by the angles that a total screen width swipe will rotate
			deltaTouchX = deltaTouchX / Screen.width * 180;

			// Apply the rotation to the transform around the player
			transform.RotateAround (playerPosition, Vector3.up, deltaTouchX);

			break;

		case 0:
			// Sanity check
//			touchZero = null;
//			touchOne = null;
			break;

		default:
			// Sanity check
//			touchZero = null;
//			touchOne = null;
			break;
		}
	}
}
