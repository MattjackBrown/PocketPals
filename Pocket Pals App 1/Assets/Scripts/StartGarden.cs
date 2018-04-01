using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// This script is linked to the Virtual Garden buttons
// The Virtual Garden scene will load when this script is called

public class StartGarden : MonoBehaviour {

	public void LoadScene() {
	
		SceneManager.LoadSceneAsync (3);         // Loads the Virtual Garden Scene

	}
}