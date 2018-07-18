using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnvironmentChanger : MonoBehaviour
{
    EnvironmentChanger Instance { set; get; }

    public GameObject[] playerPositions;

    public GameObject[] scenes;

	public GameObject StartLoginUI, VGUI, LoadingScreen;
	public VirtualSceneParent VGInfo;
	public UIAnimationManager animManager;

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
				StartSceneInVirtualGarden ();
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

	public void ResetSceneInHalfASecond () {
		
		StartCoroutine (WaitHalfSecond ());
	}

	IEnumerator WaitHalfSecond()
	{
		yield return new WaitForSeconds(0.5f);
		ResetScene ();
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

		if (true/*IsARSupported()*/) {

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
				GlobalVariables.VGCurrentIndex = CameraController.Instance.controls.virtualGarden.GetPPalIndex ();

				SceneManager.LoadScene ("SimpleARScene");
			}
		}
	}

	bool IsARSupported () {

		// Check iOS version
		#if UNITY_IOS

		float iosVersion = 0.0f;

		// Check for minimum iOS software version of 11
		float.TryParse(UnityEngine.iOS.Device.systemVersion, out iosVersion);

		if (iosVersion < 11.0f) {
			return false;
		}

		// Check for minimum device version
		var gen = UnityEngine.iOS.Device.generation;

		if (gen == UnityEngine.iOS.DeviceGeneration.iPhone4 ||
			gen == UnityEngine.iOS.DeviceGeneration.iPhone4S ||
			gen == UnityEngine.iOS.DeviceGeneration.iPhone5 ||
			gen == UnityEngine.iOS.DeviceGeneration.iPhone5C ||
			gen == UnityEngine.iOS.DeviceGeneration.iPhone5S ||
			gen == UnityEngine.iOS.DeviceGeneration.iPhone6 ||
			gen == UnityEngine.iOS.DeviceGeneration.iPhone6Plus ||
			gen == UnityEngine.iOS.DeviceGeneration.iPad1Gen ||
			gen == UnityEngine.iOS.DeviceGeneration.iPad2Gen ||
			gen == UnityEngine.iOS.DeviceGeneration.iPad3Gen ||
			gen == UnityEngine.iOS.DeviceGeneration.iPad4Gen ||
			gen == UnityEngine.iOS.DeviceGeneration.iPadAir1 ||
			gen == UnityEngine.iOS.DeviceGeneration.iPadAir2 ||
			gen == UnityEngine.iOS.DeviceGeneration.iPadMini1Gen ||
			gen == UnityEngine.iOS.DeviceGeneration.iPadMini2Gen ||
			gen == UnityEngine.iOS.DeviceGeneration.iPadMini3Gen ||
			gen == UnityEngine.iOS.DeviceGeneration.iPadMini4Gen ||
			gen == UnityEngine.iOS.DeviceGeneration.iPodTouch1Gen ||
			gen == UnityEngine.iOS.DeviceGeneration.iPodTouch2Gen ||
			gen == UnityEngine.iOS.DeviceGeneration.iPodTouch3Gen ||
			gen == UnityEngine.iOS.DeviceGeneration.iPodTouch4Gen ||
			gen == UnityEngine.iOS.DeviceGeneration.iPodTouch5Gen ||
			gen == UnityEngine.iOS.DeviceGeneration.iPodTouch6Gen) 
		{
			return false;
		}

		// If device and iOS version pass minimum check then return true
		return true;

		#endif

		// Temp just supported through iOS currently
		return false;
	}

	public void StartSceneInVirtualGarden () {

		LoadingScreen.SetActive (false);

		// Returning from the AR scene so deactivate the static PPal gameObject
		GlobalVariables.ARPocketPAl.SetActive (false);

		// Change the static variable
		GlobalVariables.currentScene = GlobalVariables.SceneName.Map;

		// Starts the virtual garden
		// This int argument is not being used I think?
		SceneInit (0);

		// Set the virtual garden ui
//		StartLoginUI.SetActive (false);
//		VGUI.SetActive (true);
		animManager.ReturnToVirtualGarden ();
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