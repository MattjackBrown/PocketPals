using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// This script is linked to the Play button on the Start Screen
// The game will start when this script is called (i.e. the Play button is pressed)

public class StartGame : MonoBehaviour {

	public void LoadScene() {
	
		SceneManager.LoadSceneAsync (1);         // Loads the Map Scene

	}
}