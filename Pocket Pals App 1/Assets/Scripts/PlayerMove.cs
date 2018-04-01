using UnityEngine;

// This script moves the player using the WASD keys
// The script is used for quality testing only and is not a game feature
public class PlayerMove : MonoBehaviour
{
	void Update()
	{
		var x = Input.GetAxis("Horizontal")*0.1f;  // moves the player horizontally
		var z = Input.GetAxis("Vertical")*0.1f;    // moves the player vertically

		transform.Translate(x, 0, z);
	}
}