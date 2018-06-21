using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine.UI;

public class GPS : MonoBehaviour
{

    public BasicMap currentMap;
    public GameObject mapGameObject;

    public GameObject originalMap;

    public GameObject girl;
	public GameObject mainCamera;

	public TouchHandler controls;

    public bool Moving = false;

    //Screen to cover the map re-intialising.
    public GameObject loadingScreen;

    //50.172600, -5.126206
    //Hardcoded start lat long, should be up penryn campus
    private float StartLat = 50.172600f;
    private float StartLong = -5.126206f;

    //the last lat long read from the device.
    private float CurrentLat = 0;
    private float CurrentLong = 0;

    //zoom of the map
    private int zoom = 18;

    //movement variables
    public float movementSpeed = 1f;
    public float MovementAccuracy = 0.2f;
    public float rotationSpeed = 2.0f;
    public float resetDistance = 2500.0f;

    //Onscreen debug text
    public Text distanceText;
    public Text latText;
    public Text lonText;

    private float DistanceTravelled = 0;

    //Set active after the map has been spawned
    private bool isInitialised = false;

    private bool HasGps = false;
    private bool IsDebug =false;

    Vector3 destination = new Vector3(0,0,0);

    Vector3 LastDestination = new Vector3(0, 0, 0);

	// Use this for initialization
	void Start ()
    {
        loadingScreen.SetActive(true);
//        DontDestroyOnLoad(gameObject);
        StartCoroutine(StartLocationService());
    }

    private IEnumerator StartLocationService()
    {
        if (!Input.location.isEnabledByUser)
        {
            UpdateMap();
            yield break;
        }

        Input.location.Start(5, 2);
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(2);
            maxWait--;
        }
        if (maxWait <= 0)
        {
            Debug.Log("time out");
            yield break;
        }
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("unable to find location");
            yield break;
        }
        HasGps = true;
        UpdateMap();
    }

    //Destroys and creates a new map at the location of the player. 
    public void UpdateMap()
    {
        //Check to see if this is a midgame update. if so kill all animals
        if (PocketPalSpawnManager.Instance)
        {
            PocketPalSpawnManager.Instance.DespawnAll();
        }

        //Delete the map if there is already one
        if (currentMap != null) Destroy(mapGameObject);

        //Check to see if the loading screen is still active
        if (loadingScreen.activeSelf == true) loadingScreen.SetActive(false);

        //check to see if we have gps, if we do set it to the last gps location rather
        //than the default
        if (HasGps)
        {
            StartLat = Input.location.lastData.latitude;
            StartLong = Input.location.lastData.longitude;
        }

        //Spawn a new map
        mapGameObject = Instantiate(originalMap);
        mapGameObject.SetActive(true);

        //get the instance of the map and initialise it
        currentMap = mapGameObject.GetComponent<BasicMap>();
        currentMap.Initialize(new Vector2d(StartLat, StartLong),zoom);


        isInitialised = true;
    }

    float GetDistanceMeters(float lat1, float lon1, float lat2, float lon2)
    {
        var R = 6378.137; // Radius of earth in KM
        var dLat = lat2 * Mathf.PI / 180 - lat1 * Mathf.PI / 180;
        var dLon = lon2 * Mathf.PI / 180 - lon1 * Mathf.PI / 180;
        var a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
        Mathf.Cos(lat1 * Mathf.PI / 180) * Mathf.Cos(lat2 * Mathf.PI / 180) *
        Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);
        var c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        var d = R * c;
        return (float)d * 1000; // meters
    }

    float Bearing(float lat1, float lon1, float lat2, float lon2)
    {

        float PI = Mathf.PI;
        float dTeta = Mathf.Log(Mathf.Tan((float)((lat2 / 2) + (PI / 4))) / Mathf.Tan((float)((lat1 / 2) + (PI / 4))));
        float dLon = Mathf.Abs(lon1 - lon2);
        float teta = Mathf.Atan2(dLon, dTeta);
        float direction = Mathf.Round(Mathf.Rad2Deg*teta);
        return direction; //direction in degree
    }

    // Update is called once per frame
    void Update ()
    {
        if (HasGps)
        {
            //latitude
            CurrentLat = Input.location.lastData.latitude;
            latText.text = "Lat: " + CurrentLat.ToString();

            //long
            CurrentLong = Input.location.lastData.longitude;
            lonText.text = "lon: " + CurrentLong.ToString();

            //distance
            DistanceTravelled = GetDistanceMeters(StartLat, StartLong, CurrentLat, CurrentLong);
            distanceText.text = "Dist From Strt: " + DistanceTravelled.ToString();
            if (DistanceTravelled > resetDistance) UpdateMap();

            //get the direction the player is heading in
            float dirX = CurrentLong - StartLong;
            float dirZ = CurrentLat - StartLat;

            //switch directions, this probably could be done better.
            if (dirX < 0) dirX = -1;
            else dirX = 1;
            if (dirZ < 0) dirZ = -1;
            else dirZ = 1;

            //create the player location vector
            float xx = dirX * GetDistanceMeters(0, StartLong, 0, CurrentLong) * currentMap.WorldRelativeScale;
            float zz = dirZ * GetDistanceMeters(StartLat, 0, CurrentLat, 0) * currentMap.WorldRelativeScale;
            Vector3 endPoint = new Vector3(xx, 0, zz);

            SetPlayerMovePoint(endPoint);

            //move
            MovePlayer();

        }
        else if (IsDebug)
        {
            MovePlayer();
        }
    }

    public void SetPlayerMovePoint(Vector3 endPoint)
    {
        LastDestination = destination;
        destination = endPoint;
        LocalDataManager.Instance.UpdateDistance(Vector3.Magnitude(destination - LastDestination) / 1000);

        Moving = true;
    }

    private void MovePlayer()
    {
        //CalcDistance
        float distance = Vector3.SqrMagnitude(girl.transform.position - destination);

        //move and rotate
        if (distance > MovementAccuracy)
        {
			// Get the start position of the player before applying any movement
			Vector3 playerStartPosition = girl.transform.position;

			// Apply the movement to the player
            girl.transform.position = Vector3.Lerp(girl.transform.position, destination, movementSpeed * Time.deltaTime);

			// Don't move the camera if in minigame. There is probably a better place to do this
			if (controls.CameraShouldFollowGPS ()) {

				// Apply the delta position to the camera transform
				mainCamera.transform.position += girl.transform.position - playerStartPosition;
			}

            //calc rotation
            if (destination != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(destination - transform.position);

                // Rotate the player model
                girl.transform.rotation = Quaternion.Lerp(girl.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

        }
        else
        {
            Moving = false;
        }
    }

    public bool GetMapInit() { return isInitialised; }

    public void SetIsDebug(bool b) { IsDebug = b; }
}
