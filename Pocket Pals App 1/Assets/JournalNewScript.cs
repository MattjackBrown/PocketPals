using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalNewScript : MonoBehaviour {

	public GameObject mainCamera, cameraJournalPositionObject, cameraLookAtPositionObject, playerModel, playerJournalPositionObject;

	Vector3 cameraJournalPosition, cameraLookAtPosition, playerJournalPosition;

	// Much trial and error to find this. It's late, I don't know if my maths is right
	Vector3 playerModelPositionOffset = new Vector3 (-1.68f, 12.54f, -10.6f);


	void Start () {

		cameraJournalPosition = cameraJournalPositionObject.transform.position;
		cameraLookAtPosition = cameraLookAtPositionObject.transform.position;

//		playerJournalPosition = playerModel.transform.position;
		playerJournalPosition = cameraJournalPosition + playerModelPositionOffset;


		// Temp to test
		mainCamera.transform.position = cameraJournalPosition;
		mainCamera.transform.LookAt (cameraLookAtPosition);

		playerModel.transform.position = playerJournalPosition;
		playerModel.transform.LookAt (cameraJournalPosition);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
