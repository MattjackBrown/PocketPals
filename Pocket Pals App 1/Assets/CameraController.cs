using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public float zoomSpeed = 0.5f;
	public GameObject player;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		// Get the position of the player
		Vector3 playerPosition = player.transform.position;

		// Get the distance of the camera from the player at the start of this frame
		float cameraDistance = (transform.position - playerPosition).magnitude;

		// Maybe swap all to a switch statement. Zero all variables on default case
		switch (Input.touchCount) {
		case 2:
			break;

		case 1:
			break;

		case 0:
			break;

		default:
			break;
		}

		// Check for pinch to zoom control. The only control to use two touches
		if (Input.touchCount == 2) {
			
			// Store both touches.
			Touch touchZero = Input.GetTouch (0);
			Touch touchOne = Input.GetTouch (1);

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

			// Todo: Use this to change the camera location, either += or position along a lerp curve

			// else check for a single touch swiping. Just left to right for now?
		} else if (Input.touchCount == 1) {

			// Store the touch
			Touch touchZero = Input.GetTouch (0);

			// Get the horixontal component of the touch's movement
			float deltaTouchX = touchZero.deltaPosition.x;

			// Adjust for different device's screen widths. Then multiply by the angles that a total screen width swipe will rotate
			deltaTouchX = deltaTouchX / Screen.width * 180;

			// Apply the rotation to the transform around the player position
			transform.RotateAround (playerPosition, Vector3.up, deltaTouchX);

			// Todo: Use this to change the the camera location, or just the look at position and recalculate using distance and
		} else if (Input.touchCount == 0) {

			// No touches, just leaves checking for capture zoom in /zoom out on pocket pal
		}
	}
}
