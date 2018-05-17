using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenAnalysis : MonoBehaviour {

	public RenderTexture cameraRender;

	public int NSamples = 10;

	Texture2D renderTexture;

	// Use this for initialization
	void Start () {

		// Set the texture2d to be the same size as the renderTexture
		renderTexture.Resize (cameraRender.width, cameraRender.height);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void UpdateTexture () {
		
		// Copy the renderTexture into the texture2d
		renderTexture.ReadPixels (new Rect (0.0f, 0.0f, cameraRender.width, cameraRender.height), 0, 0);

		// Applies the read pixels to the texture
		renderTexture.Apply ();
	}

	void AnalyseScreen () {
		
		// Get all pixels from the texture
		Color[] screenPixels = renderTexture.GetPixels ();

		// Data structure to fill with rgb colour values
		Vector3 accumColour = new Vector4();

		// Use the nSamples value to determine the sampling interval
		int sampleLength = screenPixels.Length;
		int sampleFreq = sampleLength / NSamples;

		// Sample the screenPixels[] at intervals and accumulate. Don't sample every pixel - sounds expensive
		for (int i = 0; i < sampleLength; i += sampleFreq) {
			accumColour += new Vector3(screenPixels [i].r, screenPixels [i].g, screenPixels [i].b);
		}

		// Average the result for an average colour fro the sample space
		Vector3 averageColour = accumColour / NSamples;

		// ToDo: do checks on this average colour to see what environment fills the screen
	}

	void AnalyseSpawnLocation (Vector3 worldSpawnLocation) {
		// Analyse the pixel colour at the world spawn location to decide which land sea etc pocket pal list to spawn from
	}
}
