using UnityEngine;

// This script moves the player using the WASD keys
// The script is used for quality testing only and is not a game feature
public class PlayerMove : MonoBehaviour
{
    Vector3 Destination = new Vector3(0, 0, 0);

    bool Move = false;

    //Set to true on click. Reset to false on reaching destination
    public bool Moving = false;
    //destination point
    private Vector3 endPoint;
    //alter this to change the speed of the movement of player / gameobject
    public float duration = 100.0f;
    //vertical position of the gameobject
    private float yAxis;

    private Vector3 DirVector;

    float speed = 5;

    float angle = 0;

    void Update()
	{
		var x = Input.GetAxis("Horizontal")*0.1f;  // moves the player horizontally
		var z = Input.GetAxis("Vertical")*0.1f;    // moves the player vertically



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

                if (hit.transform.gameObject.GetComponent("PocketPalParent"))
                {
                    Destroy(hit.transform.gameObject);
                }

                //set a flag to indicate to move the gameobject
                Moving = true;
                //save the click / tap position
                endPoint = hit.point;

                DirVector = (endPoint - this.transform.position);

                angle = Mathf.Atan2(DirVector.y, DirVector.x) * Mathf.Rad2Deg;

                //as we do not want to change the y axis value based on touch position, reset it to original y axis value
                endPoint.y = yAxis;



                Debug.Log(endPoint);
            }
            else
            {
                
                 Moving = true;
                //save the click / tap position
                endPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
                //as we do not want to change the y axis value based on touch position, reset it to original y axis value
                endPoint.y = yAxis;


                Debug.Log(endPoint);
            }
        }
        //check if the flag for movement is true and the current gameobject position is not same as the clicked / tapped position
        if (Moving && !Mathf.Approximately(gameObject.transform.position.magnitude, endPoint.magnitude))
        { //&& !(V3Equal(transform.position, endPoint))){
          //move the gameobject to the desired position
            this.transform.position = Vector3.Lerp(this.transform.position, endPoint, 1 / (duration * (Vector3.Distance(this.transform.position, endPoint))));
            this.transform.localRotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(endPoint, new Vector3(0, 1, 0)), 10*Time.deltaTime);
        }
        //set the movement indicator flag to false if the endPoint and current gameobject position are equal
        else if (Moving && Mathf.Approximately(gameObject.transform.position.magnitude, endPoint.magnitude))
        {
            Moving = false;
            Debug.Log("I am here");
        }

    }
}