using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalNewScript : MonoBehaviour {

	public GameObject mainCamera, playerModel, cameraJournalPositionObject, cameraLookAtPositionObject;

	Vector3 cameraJournalPosition, cameraLookAtPosition, playerJournalPosition;

	// Much trial and error to find this. It's late, I don't know if my maths is right
	Vector3 playerModelPositionOffset = new Vector3 (-1.82f, 3.85f, 0.0f);


	void Start () {

		cameraJournalPosition = cameraJournalPositionObject.transform.position;
		cameraLookAtPosition = cameraLookAtPositionObject.transform.position;

		playerModel.SetActive (true);
		playerJournalPosition = cameraJournalPosition + playerModelPositionOffset;
	}
	
	// Update is called once per frame
	void Update () {
		
		// Temp to test
		mainCamera.transform.position = cameraJournalPosition;
		mainCamera.transform.LookAt (cameraLookAtPosition);

		playerModel.transform.position = playerJournalPosition;
		playerModel.transform.LookAt (cameraJournalPosition);
		
	}
}
