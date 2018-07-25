using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalNewScript : MonoBehaviour {

	public GameObject mainCamera, playerModel, cameraJournalPositionObject, cameraLookAtPositionObject;

	Vector3 cameraJournalPosition, cameraLookAtPosition, playerJournalPosition;
	Vector3 cameraMapPosition;
	Quaternion cameraMapRotation;

	// Much trial and error to find this
	Vector3 playerModelPositionOffset = new Vector3 (-1.82f, 3.85f, 0.0f);


	void Start () {

		cameraJournalPosition = cameraJournalPositionObject.transform.position;
		cameraLookAtPosition = cameraLookAtPositionObject.transform.position;

		playerJournalPosition = cameraJournalPosition + playerModelPositionOffset;
	}

	public void Init () {

		cameraJournalPosition = cameraJournalPositionObject.transform.position;
		cameraLookAtPosition = cameraLookAtPositionObject.transform.position;

		playerJournalPosition = cameraJournalPosition + playerModelPositionOffset;

		// Record the camera position to return back to after the menu
		cameraMapPosition = mainCamera.gameObject.transform.position;
		cameraMapRotation = mainCamera.gameObject.transform.rotation;

		// Position all used assets
		mainCamera.gameObject.transform.position = cameraJournalPosition;
		mainCamera.gameObject.transform.LookAt (cameraLookAtPosition);

		playerModel.SetActive (true);
		playerModel.gameObject.transform.position = playerJournalPosition;
		playerModel.gameObject.transform.LookAt (cameraJournalPosition);

		// Stupid gimble lock
	//	playerModel.transform.Rotate (playerModel.transform.forward, 270.0f);

		// TODO Create new control scheme
		TouchHandler.Instance.MenuControls ();

	}

	public void ExitBackToMap () {

		// Return the Camera
		mainCamera.gameObject.transform.position = cameraMapPosition;
		mainCamera.transform.rotation = cameraMapRotation;

		// Deactivate the player model used for the menu
		playerModel.SetActive (false);

		TouchHandler.Instance.MapControls ();
	}

	void Update () {
		
		// TODO Lerp movement between inspect positions

		// TODO Buttons trigger camMovement lerps - set up a controlScheme
		// TODO controlScheme allows rotating player. (prefab.transform.position.z as on it's side)
		// Message Triss when done to link up to server data, shop bought bool, grey out shop button
		
	}
}
