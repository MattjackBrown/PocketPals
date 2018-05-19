using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTouch : MonoBehaviour {

    public PocketPalInventory inventory;

    public bool IsDebug = false;
    GameObject girl;


	// Use this for initialization
	void Start ()
    {
		
	}

    // Update is called once per frame
    void Update()
    {
        //check if the screen is touched / clicked   
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (Input.GetMouseButtonDown(0)))
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
                if (IsDebug)
                {
                    GetComponentInParent<GPS>().IsDebug = true;
                    GetComponentInParent<GPS>().SetPlayerMovePoint(hit.transform.position);
                }
                if (hit.transform.gameObject.GetComponent("PocketPalParent"))
                {
                    PocketPalParent pocketPal = (PocketPalParent)hit.transform.gameObject.GetComponent("PocketPalParent");
                    Debug.Log(pocketPal.PocketPalID);
                    inventory.AddPocketPal(hit.transform.gameObject);

                }
            }
        }
    }
}
