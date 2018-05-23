using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchHandler : MonoBehaviour {
	
	public GameObject player;
	public CameraController cameraController;
	public CaptureMiniGame miniGame;

	// The state machine for the controls
	public enum ControlScheme {
		disabled,
		map,
		menu,
		miniGame,
	}

	public ControlScheme controlScheme;

	public bool IsDebug = false;

	// The max distance allowed by the raycast hit detection
	public float maxCaptureDistance = 15.0f;



	// Use this for initialization
	void Start () {
		
		// Initialise as the map controls
		controlScheme = ControlScheme.map;
	}
	
	// Update is called once per frame
	void Update () {

		//used for testing on the pc
		if (IsDebug && (Input.GetMouseButtonDown(0)) && controlScheme == ControlScheme.map)
		{
			DebugTouch();
		}

		// Choose how to parse the touch controls based on the current control scheme
		switch (controlScheme) {

		case ControlScheme.map:
			{
				UseMapControls ();
			}
			break;

		case ControlScheme.miniGame:
			{
				miniGame.UpdateTimer ();
				miniGame.UpdateControls (Input.GetTouch(0));
			}
			break;

		case ControlScheme.menu:
			{

			}
			break;

		case ControlScheme.disabled:
			{
				// Currently controls are only disabled when camera is transitioning between map and minigame views
				cameraController.UpdateCaptureCam();
			}
			break;
		}
	}

	public void DisableControls() {
		controlScheme = ControlScheme.disabled;
	}

	public void MapControls() {
		controlScheme = ControlScheme.map;
	}

	public void MenuControls() {
		controlScheme = ControlScheme.menu;
	}

	public void MiniGameControls() {
		controlScheme = ControlScheme.miniGame;
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
}
