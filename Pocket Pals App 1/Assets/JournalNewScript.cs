using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalNewScript : MonoBehaviour {

	public GameObject mainCamera, playerModel, cameraJournalPositionObject, cameraLookAtPositionObject;

	Vector3 cameraJournalPosition, cameraLookAtPosition, playerJournalPosition;
	Transform cameraMapTransform;

	// Much trial and error to find this
	Vector3 playerModelPositionOffset = new Vector3 (-1.82f, 3.85f, 0.0f);


	void Start () {

		cameraJournalPosition = cameraJournalPositionObject.transform.position;
		cameraLookAtPosition = cameraLookAtPositionObject.transform.position;

		playerJournalPosition = cameraJournalPosition + playerModelPositionOffset;
	}

	public void Init () {

		// Record the camera position to return back to after the menu
		cameraMapTransform = mainCamera.transform;

		// Position all used assets
		mainCamera.transform.position = cameraJournalPosition;
		mainCamera.transform.LookAt (cameraLookAtPosition);

		playerModel.SetActive (true);
		playerModel.transform.position = playerJournalPosition;
		playerModel.transform.LookAt (cameraJournalPosition);

	}

	public void ExitBackToMap () {

		// Return the Camera
		mainCamera.transform.SetPositionAndRotation (cameraMapTransform.position, mainCamera.transform.rotation);

		// Deactivate the player model used for the menu
		playerModel.SetActive (false);

		// TODO Change the control scheme
	}

	void Update () {
		
		// TODO Lerp movement between inspect positions

		// TODO Set up menus with buttons for the functions
		// TODO Buttons trigger camMovement lerps - set up a controlScheme
		// TODO controlScheme allows rotating player. (prefab.transform.position.z as on it's side)
		// Message Triss when done to link up to server data, shop bought bool, grey out shop button
		
	}
}
