using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityARInterface;

public class PlaceOnPlane : ARBase
{
    [SerializeField]
    private Transform m_ObjectToPlace;

	Camera ARCamera;

	GameObject PPal;

	float PPalScale;
	float maxPPalScale = 6.0f;
	float minPPalScale = 0.15f;
	float initialRotationOffset = 30.0f;

	float tapTimer = 0.0f;
	float maxTapTime = 0.6f;

	void Start () {
		
		ARCamera = GetCamera ();

		// Get a local reference to the ppal
		PPal = GlobalVariables.ARPocketPAl;

		// Grab the current scale. Uniform scales so any float from the v3 will do
		PPalScale = PPal.transform.localScale.x;

		// Set the PPal as inactive until the initial position is chosen
		PPal.SetActive (false);
	}

    void Update ()
	{
		int numTouches = Input.touchCount;

		// Increment the timer
		if (numTouches > 0)
			tapTimer += Time.deltaTime;

		// Depending on the number of touches perform different actions
		if (numTouches > 1) {

			PPalPinchScale (Input.GetTouch (0), Input.GetTouch (1));

		} else if (numTouches == 1) {
				
			Touch touch = Input.GetTouch (0);

			// Either rotate or place ppal depending on touch phase
			// Running the PPalSetLocation() off a timer now to enable placing a touch to rotate the PPal without setting a new position
			switch (touch.phase) {

			case TouchPhase.Began:
				{
					tapTimer = 0.0f;
				}
				break;

			case TouchPhase.Ended:
				{
					if (tapTimer < maxTapTime && !TouchIsOverUIObject(touch))
						PPalSetLocation (touch);
				}
				break;

			case TouchPhase.Moved:
				{
					PPalRotate (touch);
				}
				break;
			}
		}
	}

	private bool TouchIsOverUIObject(Touch touch)
	{
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);

		eventDataCurrentPosition.position = new Vector2 (touch.position.x, touch.position.y);

		List<RaycastResult> results = new List<RaycastResult>();

		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

		return results.Count > 0;
	}

	void PPalSetLocation (Touch touch) {

		// Raycast out, find the plane, and place and rotate PPal
		Ray ray = ARCamera.ScreenPointToRay (touch.position);

		int layerMask = 1 << LayerMask.NameToLayer ("ARGameObject"); // Planes are in layer ARGameObject

		RaycastHit rayHit;

		if (Physics.Raycast (ray, out rayHit, float.MaxValue, layerMask)) {

			PPal.SetActive (true);
			PPal.transform.position = rayHit.point;
			SetPPalRotation ();
		}
	}

	void PPalPinchScale (Touch touchZero, Touch touchOne) {

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
		deltaTouchDifference = 1.0f + deltaTouchDifference * 2.0f / Screen.width;

		// Apply the modifier to the current scale var
		PPalScale *= deltaTouchDifference;

		// Clamp between min and max allowed values
		PPalScale = Mathf.Clamp (PPalScale, minPPalScale, maxPPalScale);

		// Set the transform scale v3
		PPal.transform.localScale = new Vector3 (PPalScale, PPalScale, PPalScale);
	}

	void PPalRotate(Touch touch) {

		// Get the horixontal component of the touch's movement
		float deltaTouchX = touch.deltaPosition.x;

		// rotation direction reversed if touch is below the player figure
		if (touch.position.y > ARCamera.WorldToScreenPoint (PPal.transform.position).y)

			// Adjust for different device's screen widths. Then multiply by the angles that a total screen width swipe will rotate
			deltaTouchX = deltaTouchX / Screen.width * 360;
		else
			deltaTouchX = -deltaTouchX / Screen.width * 360;

		// Repeat for deltaY touch to enable circling movements to circle the PPal around
		float deltaTouchY = touch.deltaPosition.y;

		// Rotation direction reversed if touch is right of the player figure
		if (touch.position.x < ARCamera.WorldToScreenPoint (PPal.transform.position).x)

			// Adjust for different device's screen widths. Then multiply by the angles that a total screen width swipe will rotate
			deltaTouchY = deltaTouchY / Screen.height * 180;
		else
			deltaTouchY = -deltaTouchY / Screen.height * 180;

		// Combine both swipe directions into one final value
		float finalRotation = deltaTouchX + deltaTouchY; // Consider changing this to sqr(x^2 + y^2) true circling movement?

		// Apply the rotation to the PPal transform
		PPal.transform.Rotate (new Vector3(0.0f, finalRotation, 0.0f));
	}


	void SetPPalRotation () {

		// Set the look at position to the camera position, but at the PPal's height so as to keep it level
		Vector3 PPalLookAtLocation = new Vector3 (
			ARCamera.transform.position.x, 
			PPal.transform.position.y, 
			ARCamera.transform.position.z
		);

		// Set to look directly at camera direction
		PPal.transform.LookAt (PPalLookAtLocation);

		// Then offset it by the desired starting rotation
		PPal.transform.Rotate (new Vector3 (0.0f, initialRotationOffset, 0.0f));
	}



/*
        if (Input.GetMouseButton(0))
        {
            var camera = GetCamera();

            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

			int layerMask = 1 << LayerMask.NameToLayer("ARGameObject"); // Planes are in layer ARGameObject

            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, float.MaxValue, layerMask))
                m_ObjectToPlace.transform.position = rayHit.point;
        }
*/

}
