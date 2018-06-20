using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnvironmentChanger : MonoBehaviour
{
    EnvironmentChanger Instance { set; get; }

    public GameObject[] playerPositions;

    public GameObject[] scenes;

	public GameObject StartLoginUI, VGUI;
	public VirtualSceneParent VGInfo;

    //Used to reset the camera back in posisition.
    public GameObject player;
    private GPS gps;

	private int activeIndex;

	bool created = false;

/*
	void Awake()
	{
		// Global variables! Have to stop this being deleted between scenes
		if (!created)
		{
			DontDestroyOnLoad(this.gameObject);
			created = true;
		}
	}
*/

	// Use this for initialization
	void Start ()
    {
        Instance = this;
        gps = player.GetComponent<GPS>();

    }

	// Wait for all references to populate
	void  LateUpdate () {
		
		if (!created) {

			// If scene is being initialised from the AR scene then go straight to virtual garden
			if (GlobalVariables.currentScene == GlobalVariables.SceneName.AR) {
//WIP!!!				StartSceneInVirtualGarden ();
				Debug.Log ("working");
			} else {
				Debug.Log ("not working");
			}
			created = true;
		}
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

	public void ARKitSceneLoad () {

		// Get the current looked at PPal in the virtual garden
		GameObject targetedPPal = VGInfo.GetCurrentPPal ();

		// If there is a valid PPal to look at in AR, i.e. inventory is not empty
		if (targetedPPal != null) {

			// Clone it
			GameObject ARPPal = Instantiate (targetedPPal);

			// Keep it between scenes
			DontDestroyOnLoad (ARPPal);

			// Change the static variable to reference it and load the new scene
			GlobalVariables.ARPocketPAl = ARPPal;
			GlobalVariables.currentScene = GlobalVariables.SceneName.AR;
			SceneManager.LoadScene ("SimpleARScene");
		}
	}

	public void StartSceneInVirtualGarden () {

		// Returning from the AR scene so deactivate the static PPal gameObject
		GlobalVariables.ARPocketPAl.SetActive (false);

		// Change the static variable
		GlobalVariables.currentScene = GlobalVariables.SceneName.Map;

		// Starts the virtual garden
		// This int argument is not being used I think?
		SceneInit (0);

		// Set the virtual garden ui
		StartLoginUI.SetActive (false);
		VGUI.SetActive (true);
	}

}
/*
public static class GlobalVariables
{
	public enum SceneName {
		Map,
		AR
	}

	// Used for scene initialisation to determine an entry point. i.e. whether to start the scene in the virtual garden if coming from the AR scene
	public static SceneName currentScene = SceneName.Map;

	// The gameObject to focus on in the AR scene
	public static GameObject ARPocketPAl { get; set; }
}
*/