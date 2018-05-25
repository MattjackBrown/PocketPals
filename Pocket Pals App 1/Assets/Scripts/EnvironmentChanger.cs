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

    //Script Behaviours
    
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
        gps.currentMap.gameObject.SetActive(false);
        gps.mainCamera.transform.position = playerPositions[index].transform.position;
        scenes[index].SetActive(true);
    }

}
