using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script animates the placeholder objects for the Pocket Pals
// The script will be replaced once the assets and animations have been created by the client

public class PocketPalRotation : MonoBehaviour {

		void Update () 
		{
			transform.Rotate (new Vector3 (15, 30, 45) * Time.deltaTime); // rotates the object on the x, y and z axis
		}
	}