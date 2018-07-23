using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalNewScript : MonoBehaviour {

	public GameObject mainCamera, cameraJournalPositionObject, cameraLookAtPositionObject, playerModel, playerJournalPositionObject;

	Vector3 cameraJournalPosition, cameraLookAtPosition, playerJournalPosition;


	void Start () {

		cameraJournalPosition = cameraJournalPositionObject.transform.position;
		cameraLookAtPosition = cameraLookAtPositionObject.transform.position;
		playerJournalPosition = playerModel.transform.position;


		// Temp to test
		mainCamera.transform.position = cameraJournalPosition;
		mainCamera.transform.LookAt (cameraLookAtPosition);
		playerModel.transform.position = playerJournalPosition;
	}
	
	// Update is called once per frame
	void Update () {

		// Temp to test
		mainCamera.transform.position = cameraJournalPosition;
		mainCamera.transform.LookAt (cameraLookAtPosition);
		playerModel.transform.position = playerJournalPosition;
	}
}
