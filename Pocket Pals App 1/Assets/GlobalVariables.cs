using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : MonoBehaviour {

	public enum SceneName {
		Map,
		AR
	}

    //--------------------- Inventory ID Statics ----------------\\
    public static int BerryID = 1;
    public static int NoneID = 0;
    public static int MagnifyingGlassID = 3;
    public static int StrawBerriesID = 2;
    public static int ProCameraID = 4;
    public static int medCameraID = 5;

    // Used for scene initialisation to determine an entry point. i.e. whether to start the scene in the virtual garden if coming from the AR scene
    public static SceneName currentScene = SceneName.Map;

	// The gameObject to focus on in the AR scene
	public static GameObject ARPocketPAl { get; set; }

	public static bool hasLoggedIn = false;

	public static LocalDataManager localDataManager;
	public static ServerDataManager serverDataManager;

	// Stores the index of the virtual garden current looked at 
	public static int VGCurrentIndex = 0;

    public static int GetRandom(int basePercent, float iterVar, int maxNumber )
    {
       
        int rand = Random.Range(0, 100);

        if (rand > basePercent) return 1;

        int i;
        for ( i = 2; i < maxNumber; i++)
        {
            float x = basePercent * 1/(i*i*iterVar);
            if (rand > x)
            {
                return i;
            }
        }
        return i;
    }
}
