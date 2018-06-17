using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityARInterface;

public class PlaceOnPlane : ARBase
{
    [SerializeField]
    private Transform m_ObjectToPlace;

	Camera ARCamera;

	void Start () {
		
		ARCamera = GetCamera ();
	}

    void Update ()
    {
		int numTouches = Input.touchCount;

		if (numTouches > 0) {
			for (int i = 0; i < numTouches; i++) {
				
				Touch tempTouch = Input.GetTouch (i);

				if (tempTouch.phase.Equals(TouchPhase.Began)) {

					Ray ray = ARCamera.ScreenPointToRay (tempTouch.position);

					int layerMask = 1 << LayerMask.NameToLayer ("ARGameObject"); // Planes are in layer ARGameObject

					RaycastHit rayHit;
					if (Physics.Raycast (ray, out rayHit, float.MaxValue, layerMask)) {
						transform.position = rayHit.point;
						//break;
					}
				}
			}
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
}
