using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalVariables {

	public enum SceneName {
		Map,
		AR
	}

	// Used for scene initialisation to determine an entry point. i.e. whether to start the scene in the virtual garden if coming from the AR scene
	public static SceneName currentScene = SceneName.Map;

	// The gameObject to focus on in the AR scene
	public static GameObject ARPocketPAl { get; set; }
}
