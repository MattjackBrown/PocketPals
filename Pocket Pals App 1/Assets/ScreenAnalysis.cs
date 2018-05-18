using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenAnalysis : MonoBehaviour {

	public RenderTexture cameraRender;
	public Camera cameraObject;

	public int NSamples = 10;

	Texture2D cameraTexture = new Texture2D(0, 0);

	// When checking for water around a blue pixel, this is far it will look
	int waterSampleDistance = 20;

	GameObject[] landTypes = new GameObject[]{};
	GameObject[] forestTypes = new GameObject[]{};
	GameObject[] urbanTypes = new GameObject[]{};
	GameObject[] seaTypes = new GameObject[]{};
	GameObject[] riverTypes = new GameObject[]{};
	GameObject[] waterTypes = new GameObject[]{};


	// Use this for initialization
	void Start () {

		// Set the texture2d to be the same size as the renderTexture
		cameraTexture.Resize (cameraRender.width, cameraRender.height);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void UpdateTexture () {
		
		// Copy the renderTexture into the texture2d
		cameraTexture.ReadPixels (new Rect (0.0f, 0.0f, cameraRender.width, cameraRender.height), 0, 0);

		// Applies the read pixels to the texture
		cameraTexture.Apply ();
	}

	// Will this ever be needed? Whole screen analysis as opposed to single pixel analysis
	GameObject[] AnalyseScreen () {

		// Update the cameraTexture to the current camera view image
		UpdateTexture ();
		
		// Get all pixels from the texture
		Color[] screenPixels = cameraTexture.GetPixels ();

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
		return GetAppropriatePalListFromMapColour(averageColour);
	}

	GameObject[] AnalyseSpawnLocation (Vector3 worldSpawnLocation) {
		// Analyse the pixel colour at the world spawn location to decide which land sea etc pocket pal list to spawn from

		// World to screen location returns the pixel coordinates
		Vector3 screenLocation = cameraObject.WorldToScreenPoint (worldSpawnLocation);

		Color pixelColour = cameraTexture.GetPixel ((int)screenLocation.x, (int)screenLocation.y);

		float r = pixelColour.r;
		float g = pixelColour.g;
		float b = pixelColour.b;

		// Check for green
		if (r < 0.2f && g > 0.8f && b < 0.2f) return forestTypes;

		// Check for urban (grey rgb have similar values)
		if (r - g < 0.2f && r - b < 0.2f
			// Also check not pure white or black
			&& r < 0.8f && r > 0.2f) return urbanTypes;

		// Check for blue
		if (r < 0.2f && g < 0.2f && b > 0.8f) {

			// Need to check if sea or inland
			// Will need a null check here or some type of off screen
			Color leftSample = cameraTexture.GetPixel ((int)screenLocation.x - waterSampleDistance, (int)screenLocation.y);
			Color rightSample = cameraTexture.GetPixel ((int)screenLocation.x + waterSampleDistance, (int)screenLocation.y);
			Color upSample = cameraTexture.GetPixel ((int)screenLocation.x, (int)screenLocation.y + waterSampleDistance);
			Color downSample = cameraTexture.GetPixel ((int)screenLocation.x, (int)screenLocation.y - waterSampleDistance);
			
			// If inland then pixels on either side will be land
			if (!isBlue (leftSample) && !isBlue (rightSample)) return riverTypes;
			if (!isBlue (upSample) && !isBlue (downSample)) return riverTypes;

			// Else is sea 
			return seaTypes;
		}

		// else return default land types
		return landTypes;
	}

	bool isBlue(Color colour) {
		if (colour.r < 0.2f && colour.g < 0.2f && colour.b > 0.8f) {
			return true;
		}
		return false;
	}

	GameObject[] GetAppropriatePalListFromMapColour(Vector3 colour) {
		float r = colour.x;
		float g = colour.y;
		float b = colour.z;

		// Check for green
		if (r < 0.2f && g > 0.8f && b < 0.2f) return forestTypes;

		// Check for urban (grey rgb have similar values)
		if (r - g < 0.2f && r - b < 0.2f
			// Also check not pure white or black
			&& r < 0.8f && r > 0.2f) return urbanTypes;

		// Check for blue
		if (r < 0.2f && g < 0.2f && b > 0.8f) return waterTypes;

		// else return default land types
		return landTypes;
	}
}
