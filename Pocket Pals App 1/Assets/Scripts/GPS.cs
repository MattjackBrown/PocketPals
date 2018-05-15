using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine.UI;

public class GPS : MonoBehaviour
{

    public BasicMap basicMap;

    private BasicMap originalMap;

    public GameObject girl;

    public bool Moving = false;

    //Hardcoded start lat long, should be up penryn UNI
    private float StartLat = 50.171268f;
    private float StartLong = -5.123837f;

    //the last lat long read from the device.
    private float CurrentLat = 0;
    private float CurrentLong = 0;

    //zoom of the map
    private int zoom = 0;

    //movement variables
    public float movementSpeed = 10000f;
    public int rotationSpeed = 20;
    public float MovementAccuracy = 0.2f;

    //Onscreen debug text
    public Text distanceText;
    public Text latText;
    public Text lonText;

    private float DistanceTravelled = 0;

    private bool HasGps = false;

    Vector3 destination = new Vector3(0,0,0);

	// Use this for initialization
	void Start ()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(StartLocationService());
        zoom = basicMap.Zoom;
        originalMap = Object.Instantiate(basicMap);
        UpdateMap();
    }

    private IEnumerator StartLocationService()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Request gps priv");
            yield break;
        }

        Input.location.Start();
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

    public void UpdateMap()
    {
        Destroy(basicMap.gameObject);
        basicMap = Object.Instantiate(originalMap);
        if (HasGps)
        {
            StartLat = Input.location.lastData.latitude;
            StartLong = Input.location.lastData.longitude;
        }
        basicMap.Initialize(new Vector2d(StartLat, StartLong),zoom);
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
        if (Moving)
        {
            MovePlayer();
        }
        if (HasGps)
        {
            //latitude
            CurrentLat = Input.location.lastData.latitude;
            if (!Mathf.Approximately(CurrentLat, StartLat))
            {
                StartLat = CurrentLat;
            }
            latText.text = "Lat: "+CurrentLat.ToString();

            //long
            CurrentLong = Input.location.lastData.longitude;
            if (!Mathf.Approximately(CurrentLong, StartLong))
            {
                StartLong = CurrentLong;
            }
            lonText.text = "lon: " + CurrentLong.ToString();

            //distance
            DistanceTravelled = (float)GetDistanceMeters(StartLat, StartLong, CurrentLat, CurrentLong);
            distanceText.text = "Dist: " + DistanceTravelled.ToString();

            //get the direction the player is heading in
            float dirX = CurrentLong- StartLong;
            float dirZ = CurrentLat - StartLat;

            //switch directions, this probably could be done better.
            if (dirX < 0) dirX = -1;
            else dirX = 1;
            if (dirZ < 0) dirZ = -1;
            else dirZ = 1;

            //create the player location vector
            float xx = dirX*GetDistanceMeters(0, StartLong, 0, CurrentLong)*basicMap.WorldRelativeScale;
            float zz = dirZ*GetDistanceMeters(StartLat, 0, CurrentLat, 0)*basicMap.WorldRelativeScale;
            Vector3 endPoint = new Vector3(xx, 0, zz);

            SetPlayerMovePoint(endPoint);

            Debug.Log("Hit");

            //move
            MovePlayer();

        }
    }

    public void SetPlayerMovePoint(Vector3 endPoint)
    {
        destination = endPoint;
        Moving = true;
    }

    private void MovePlayer()
    {
        //CalcDistance
        float distance = Vector3.SqrMagnitude(girl.transform.position - destination);
        Debug.Log("Hit" + distance);
        //move
        if (distance > MovementAccuracy)
        {
            girl.transform.position = Vector3.Lerp(girl.transform.position, destination, movementSpeed * Time.deltaTime);
            Moving = true;
        }
        else
        {
            Moving = false;
        }
    }
}
