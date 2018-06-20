using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityARInterface;

public class PlaceOnPlane : ARBase
{
    [SerializeField]
    private Transform m_ObjectToPlace;

	Camera ARCamera;

//	public GameObject pocketPal;

	void Start () {
		
		ARCamera = GetCamera ();

		// Instantiate the pocketPal that is stored in the static scene Loader
//		pocketPal = Instantiate (GlobalVariables.ARPocketPAl, Vector3.zero, Quaternion.identity, this.transform);

		// Set the PPal as inactive until the initial position is chosen
//		pocketPal.SetActive (false);
		GlobalVariables.ARPocketPAl.SetActive (false);
	}

    void Update ()
	{
		int numTouches = Input.touchCount;

		if (numTouches > 0) {
			for (int i = 0; i < numTouches; i++) {
				
				Touch tempTouch = Input.GetTouch (i);

				if (tempTouch.phase.Equals (TouchPhase.Began)) {

					Ray ray = ARCamera.ScreenPointToRay (tempTouch.position);

					int layerMask = 1 << LayerMask.NameToLayer ("ARGameObject"); // Planes are in layer ARGameObject

					RaycastHit rayHit;
					if (Physics.Raycast (ray, out rayHit, float.MaxValue, layerMask)) {
	//					pocketPal.SetActive (true);
	//					pocketPal.transform.position = rayHit.point;
	//					transform.position = rayHit.point;
	//					SetPPalRotation ();

						GlobalVariables.ARPocketPAl.SetActive (true);
						GlobalVariables.ARPocketPAl.transform.position = rayHit.point;
						SetPPalRotation (GlobalVariables.ARPocketPAl);

						break;
					}
				}
			}
		}
	}

	void SetPPalRotation (GameObject pocketPal) {

		// Set the look at position to the camera position, but at the PPal's height so as to keep it level
		Vector3 PPalLookAtLocation = new Vector3 (
			ARCamera.transform.position.x, 
			pocketPal.transform.position.y, 
			ARCamera.transform.position.z
		);

		pocketPal.transform.LookAt (PPalLookAtLocation);
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
