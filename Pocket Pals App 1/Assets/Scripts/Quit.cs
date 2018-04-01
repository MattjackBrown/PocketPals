using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// This script is linked to the Quit button in the main menu
// The game will end when this script is called (i.e. the Quit button is pressed)

public class Quit : MonoBehaviour {

	public void LoadScene() {
	
		SceneManager.LoadSceneAsync (0);         // Loads the Start Screen 

	}
}