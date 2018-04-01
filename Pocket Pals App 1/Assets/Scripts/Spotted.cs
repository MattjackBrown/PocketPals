using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// This script allows the player to collect Pocket Pals
// As the app progresses, this script will evolve
// Currently, the object is destroyed when pressed. It will need to be added to the players collection moving forward

public class Spotted : MonoBehaviour
{
	void OnMouseDown()             // OnMouseDown also works as a touch on an iphone or other mobile device
	{
			Destroy(gameObject);   // Removes Pocket Pal when clicked or touched

		SceneManager.LoadSceneAsync (2);         // Loads the Spotted Scene when called
		}
}