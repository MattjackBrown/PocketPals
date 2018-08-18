using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine.UI;
using Mapbox.Unity.Utilities;
using System.Linq;
using Mapbox.Unity.MeshGeneration.Factories;
using System;
using Mapbox.Unity.MeshGeneration.Data;
using UnityEngine.SceneManagement;

public class GPS : MonoBehaviour
{
	public static GPS Insatance { set; get; }

	public BasicMap currentMap;
	public GameObject mapGameObject;

	public GameObject originalMap;

	public GameObject girl;
	public GameObject mainCamera;

	public TouchHandler controls;

	public bool Moving = false;

	//50.172600, -5.126206
	//Hardcoded start lat long, should be up penryn campus50.171115, -5.507848
	private float StartLat = 50.12949f;
	private float StartLong = -5.542262f;

	//the last lat long read from the device.
	private float CurrentLat = 0;
	private float CurrentLong = 0;

    //for degbug lat longs-5.542262 50.12949
    public float fakeLat = 50.12949f;
	public float fakeLong = -5.542262f;

	//zoom of the map
	private int zoom = 18;

	//movement variables
	private float movementSpeed = 1f;
	public float MovementAccuracy = 0.01f;
    public float currentSpeed =0.0f;
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
    private bool hasStartedCoroutines = false;

	public Vector3 destination = new Vector3(0,0,0); 
	public Vector3 LastDestination = new Vector3(0, 0, 0);
    public Vector3 DeadReckonPos = new Vector3(0,0,0);

    public float DeadReckonThreshHold = 0.5f;
    public float DeadReckonAmount = 0.1f;
    public float gpsFrequency = 0.0f;
    public float moveAlpha = 0.0f;
    public bool ShouldDeadReckon = false;

    public bool fakeGPs = false;
    public Vector2 fakeLatLonIncrement = new Vector2(0.0001f, 0.0001f);
    public float fakeDelayTime = 1.0f;
    private float oldDist = 0.0f;

    public Material MapMaterial;
    public Color DayFog;
    public Color NightTint;
    public int Sunset;
    public int SunRise;

    public float GetMsSpeed() { return movementSpeed; }

	public Material daySkyBox, nightSkyBox;

    // Use this for initialization
    void Start ()
	{
		Insatance = this;

		StartCoroutine(StartLocationService());
	}

	private IEnumerator StartLocationService()
	{
        Debug.Log("Getting locational Services");
		if (!Input.location.isEnabledByUser)
		{
            Debug.Log("Not enabled");
			yield break;
		}

		Input.location.Start(5, 2);
        Input.compass.enabled = true;
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
        Debug.Log("Found location services");
		HasGps = true;
	}

    private IEnumerator FakeMove()
    {
        while (true)
        {
            if (fakeGPs)
            {
                fakeLat += fakeLatLonIncrement.x;
                fakeLong += fakeLatLonIncrement.y;
            }
            yield return new WaitForSeconds(fakeDelayTime);
        }
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
        if (currentMap != null)
        {
            Destroy(mapGameObject);
            StopCoroutines();
        }

		//check to see if we have gps, if we do set it to the last gps location rather
		//than the default
		if (HasGps)
		{
			StartLat = Input.location.lastData.latitude;
			StartLong = Input.location.lastData.longitude;

            CurrentLat = StartLat;
            CurrentLong = StartLong;

			Debug.Log("GPS Initialised at:" + StartLat.ToString()+ ":" + StartLong.ToString());
		}

        if (IsDebug)
        {
            StartLat = fakeLat;

            StartLong = fakeLong;

            CurrentLat = StartLat;
            CurrentLong = StartLong;
        }

        //Check time and set material
        if(System.DateTime.Now.Hour >= Sunset || System.DateTime.Now.Hour < SunRise)
        {
            MakeNight();
            PocketPalSpawnManager.Instance.SetSpawnList(SpawnType.n_Woodland);
        }
        else
        {
            MakeDay();
            PocketPalSpawnManager.Instance.SetSpawnList(SpawnType.d_Woodland);
        }

        //Spawn a new map
        mapGameObject = Instantiate(originalMap);
		mapGameObject.SetActive(true);

		//get the instance of the map and initialise it
		currentMap = mapGameObject.GetComponent<BasicMap>();
		currentMap.Initialize(new Vector2d(StartLat, StartLong),zoom);

        //Start the resourcespot manager co routine

		isInitialised = true;

        LoadingScreenController.Instance.MapLoaded();

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

	private void UpdateDebugText()
	{
		latText.text = "Lat: " + CurrentLat.ToString();
		lonText.text = "lon: " + CurrentLong.ToString();
		distanceText.text = "Dist From Strt: " + DistanceTravelled.ToString();
	}

    public IEnumerator TryUpdateMap()
    {
        int iter = 0;
        float waitTime = 0.5f;
        float timeOut = 30;
        if (TouchHandler.Instance.IsDebug) IsDebug = true;

        int ticket = LoadingScreenController.Instance.AddLoadingMessage("GPS Location");

        while (!HasGps && !IsDebug)
        {
            iter++;
            if (iter*waitTime > timeOut)
            {
                NotificationManager.Instance.ErrorNotification("GPS could not be found. Please check it is allowed in permissions!");
                ServerDataManager.Instance.LogOut();
                yield break;
            }
            Debug.Log("Waiting for gps");
            yield return new WaitForSeconds(waitTime);
        }
        LoadingScreenController.Instance.TicketLoaded(ticket);
        UpdateMap();
        yield break;
    }

	private Vector3 ManualCalc()
	{
		//latitude
		CurrentLat = Input.location.lastData.latitude;

		//long
		CurrentLong = Input.location.lastData.longitude;

		//distance
		DistanceTravelled = GetDistanceMeters(StartLat, StartLong, CurrentLat, CurrentLong);

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
		return new Vector3(xx, 0, zz);
	}

	public Vector3 GetWorldPos(double lat, double lon)
	{
		Vector2d end2d = Conversions.GeoToWorldPosition(lat, lon, currentMap.CenterMercator, currentMap.WorldRelativeScale);

      

		Vector3 pos = new Vector3((float)end2d.x, 0.0f, (float)end2d.y);
        pos += currentMap.Root.position;
        return pos;
	}

	public Vector2 GetLatLon()
	{
		return new Vector2(CurrentLat, CurrentLong);
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
        if (!isInitialised)
        {
            return;
        }

        if (!hasStartedCoroutines && currentMap != null && isInitialised) StartCoroutines();


        gpsFrequency += Time.deltaTime;


        if (HasGps)
		{
			//Set the current position used for seeding
			CurrentLat = Input.location.lastData.latitude;
			CurrentLong = Input.location.lastData.longitude;

			UpdateDebugText();

			Vector3 altEnd = GetWorldPos(CurrentLat, CurrentLong);

			StartPlayerMove(altEnd);
		}
		else if (IsDebug)
		{

            //used when debuging in editor to simulate player movements
            CurrentLat = fakeLat;
			CurrentLong = fakeLong;

			UpdateDebugText();

			Vector3 altEnd = GetWorldPos(CurrentLat, CurrentLong);

			StartPlayerMove(altEnd);
		}

        if (Moving)MovePlayer();
    }

    public void ToggleDayNight()
    {
        UpdateMap();
    }

    public void MakeDay()
    {
        MapMaterial.SetColor("_Tint", new Color(1, 1, 1, 1));
        RenderSettings.fogColor = DayFog;
		RenderSettings.skybox = daySkyBox;
    }

    public void MakeNight()
    {
        MapMaterial.SetColor("_Tint", NightTint);
        RenderSettings.fogColor = NightTint;
		RenderSettings.skybox = nightSkyBox;
    }

    public void FakeGPSRead(Vector3 worldPos)
    {
        Vector2d latlon = worldPos.GetGeoPosition(currentMap.CenterMercator, currentMap.WorldRelativeScale);
        fakeLat = (float)latlon.x;
        fakeLong = (float)latlon.y;
    }

	public void StartPlayerMove(Vector3 endPoint)
	{
        Vector3 dir = endPoint - destination;

        if (Mathf.Abs(Vector3.Magnitude(dir)) >= DeadReckonThreshHold)
        {
            if (gpsFrequency <= 1) gpsFrequency = 1;
            currentSpeed = Vector3.Magnitude(dir) / gpsFrequency;

            dir = dir.normalized * currentSpeed * DeadReckonAmount;


            DeadReckonPos = endPoint + dir;

            ShouldDeadReckon = true;

            movementSpeed = gpsFrequency + 1;

            gpsFrequency = 0.0f;

            moveAlpha = 0.0f;

            LastDestination = girl.transform.position;
            destination = endPoint;

            
            //Get rough distance
            float nDistance = GetDistanceMeters(StartLat, StartLong, CurrentLat, CurrentLong);
            float delta = Math.Abs(nDistance - oldDist) / 1000;
            oldDist = nDistance;
            LocalDataManager.Instance.UpdateDistance(delta);

            

            Moving = true;

        }
        else
        {
            if (!Moving && Vector3.Magnitude(destination-girl.transform.position) > MovementAccuracy)
            {
                destination = endPoint;
                Moving = true;
                moveAlpha = 0.0f;

            }
        }



	}

	private void MovePlayer()
	{

        Vector3 endPoint;
        Vector3 startPoint;


        if (ShouldDeadReckon)
        {
            endPoint = DeadReckonPos;
            startPoint = LastDestination;
        }

        else
        {
            endPoint = destination;
            startPoint = DeadReckonPos;
        }
        //CalcDistance
        float distance = Vector3.SqrMagnitude(girl.transform.position - endPoint);

		//move and rotate
		if (moveAlpha < 1)
		{
			// Get the start position of the player before applying any movement
			Vector3 playerStartPosition = girl.transform.position;

            moveAlpha += Time.deltaTime/movementSpeed;


            // Apply the movement to the player
            girl.transform.position = Vector3.Lerp(startPoint, endPoint,moveAlpha);

            //Super bad way to set player position in shaders. Consider changing.
            foreach (UnityTile ut in currentMap.MapVisualizer.ActiveTiles.Values)
            {
                ut.UpdateMaterialProperty(girl.transform.position);
            }

            // Don't move the camera if in minigame. There is probably a better place to do this
            if (controls.CameraShouldFollowGPS ()) {

				// Apply the delta position to the camera transform
				mainCamera.transform.position += girl.transform.position - playerStartPosition;
			}

			//calc rotation
			if (endPoint != Vector3.zero)
			{
				Quaternion targetRotation = Quaternion.LookRotation(endPoint - startPoint);

				// Rotate the player model
				girl.transform.rotation = Quaternion.Lerp(girl.transform.rotation, targetRotation,rotationSpeed * moveAlpha);

            }

		}
		else
		{
            ShouldDeadReckon = false;
			Moving = false;
            LastDestination = endPoint;
        }
	}

	public bool GetMapInit() { return isInitialised; }

	public void SetIsDebug(bool b)
    {
        IsDebug = b;
    }

    public void StartCoroutines()
    {
        if (hasStartedCoroutines)
        {
            StopCoroutines();
        }
        if (ResourceSpotManager.Instance != null && PocketPalSpawnManager.Instance != null)
        {
            Debug.Log("Coroutines started");
            StartCoroutine(ResourceSpotManager.Instance.Spawn());
            StartCoroutine(PocketPalSpawnManager.Instance.Spawn());
            hasStartedCoroutines = true;
        }
    }

    public void StopCoroutines()
    {
        if (ResourceSpotManager.Instance != null && PocketPalSpawnManager.Instance != null)
        {
            Debug.Log("Coroutines stoped");
            StopCoroutine(ResourceSpotManager.Instance.Spawn());
            StopCoroutine(PocketPalSpawnManager.Instance.Spawn());
            hasStartedCoroutines = false;
        }
    }
}