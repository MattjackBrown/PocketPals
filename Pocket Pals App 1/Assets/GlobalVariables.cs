using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalVariables {

	public enum SceneName {
		Map,
		AR
	}

    //--------------------- Inventory ID Statics ----------------\\
    public static int BerryID = 1;
    public static int NoneID = 0;
    public static int MagnifyingGlassID = 2;
    public static int StrawBerriesID = 3;

    // Used for scene initialisation to determine an entry point. i.e. whether to start the scene in the virtual garden if coming from the AR scene
    public static SceneName currentScene = SceneName.Map;

	// The gameObject to focus on in the AR scene
	public static GameObject ARPocketPAl { get; set; }

	public static bool hasLoggedIn = false;

	public static LocalDataManager localDataManager;
	public static ServerDataManager serverDataManager;

	// Stores the index of the virtual garden current looked at 
	public static int VGCurrentIndex = 0;
}
