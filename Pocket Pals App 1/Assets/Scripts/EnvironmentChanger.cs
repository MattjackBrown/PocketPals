using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentChanger : MonoBehaviour
{
    EnvironmentChanger Instance { set; get; }

    public GameObject[] playerPositions;

    public GameObject[] scenes;

    //Used to reset the camera back in posisition.
    public GameObject player;
    private GPS gps;

    private int activeIndex;
    
	// Use this for initialization
	void Start ()
    {
        Instance = this;
        gps = player.GetComponent<GPS>();

    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}



   public void SceneInit(int index)
    {
        index = activeIndex;

        //deactivate map stuff and stop gps ticking???
        gps.currentMap.gameObject.SetActive(false);
        gps.gameObject.SetActive(false);

        //set positions and acticate scene.
        CameraController.Instance.InitVirtualGardenTour(playerPositions[index].transform.position, playerPositions[index].transform.forward + playerPositions[index].transform.position);
		scenes[index].SetActive(true);
		scenes [index].GetComponent<VirtualSceneParent> ().InitVGTour ();
        
        //to do start custom scripts depending on scene user story.

    }

    public void ResetScene()
    {
        //activate map stuff
        gps.currentMap.gameObject.SetActive(true);
        gps.gameObject.SetActive(true);

        //set scene inactive.
        scenes[activeIndex].SetActive(false);


        CameraController.Instance.ReturnCamToAfterVirtualGarden();
    }

}
