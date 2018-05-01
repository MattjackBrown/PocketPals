using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine.UI;

public class GPS : MonoBehaviour {

    public static GPS Instance { set; get; }

    public BasicMap basicMap;

    private BasicMap originalMap;

    public GameObject girl;

    public bool Moving = false;

    private float StartLong = 0;
    private float StartLat = 0;
    private float CurrentLat = 0;
    private float CurrentLong = 0;

    public  int duration = 50;
    public int rotationSpeed = 20;

    public Text distanceText;
    public Text latText;
    public Text lonText;

    private float DistanceTravelled = 0;

    private bool HasGps = false;
	// Use this for initialization
	void Start ()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
        StartCoroutine(StartLocationService());
        originalMap = Object.Instantiate(basicMap);
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
        UpdateLocation();
    }

    public void UpdateLocation()
    {
        Destroy(basicMap.gameObject);
        basicMap = Object.Instantiate(originalMap);
        if (HasGps)
        {
            StartLat = Input.location.lastData.latitude;
            StartLong = Input.location.lastData.longitude;
        }
        basicMap.Initialize(new Vector2d(StartLat, StartLong), 18);
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

    public void MoveRight()
    {
        basicMap.transform.Rotate(Vector3.up * rotationSpeed);
    }
    public void MoveLeft()
    {
        basicMap.transform.Rotate(-Vector3.up * rotationSpeed);
    }
    // Update is called once per frame
    void Update ()
    {
        Debug.Log(basicMap.WorldRelativeScale);
        if (HasGps)
        {
            //latitude
            CurrentLat = Input.location.lastData.latitude;
            latText.text = "Lat: "+CurrentLat.ToString();

            //long
            CurrentLong = Input.location.lastData.longitude;
            lonText.text = "lon: " + CurrentLong.ToString();

            //distance
            DistanceTravelled = (float)GetDistanceMeters(StartLat, StartLong, CurrentLat, CurrentLong);
            if (DistanceTravelled > 50)
            {
                UpdateLocation();
            }
            distanceText.text = DistanceTravelled.ToString();
            float dirX = CurrentLong- StartLong;
            float dirZ = CurrentLat - StartLat;

            if (dirX < 0) dirX = -1;
            else dirX = 1;
            if (dirZ < 0) dirZ = -1;
            else dirZ = 1;

            float xx = dirX*GetDistanceMeters(0, StartLong, 0, CurrentLong)*basicMap.WorldRelativeScale;
            float zz = dirZ*GetDistanceMeters(StartLat, 0, CurrentLat, 0)*basicMap.WorldRelativeScale;

            Vector3 endPoint = new Vector3(xx, 0, zz);

            //check if the flag for movement is true and the current gameobject position is not same as the clicked / tapped position
            if (!Mathf.Approximately(girl.transform.position.magnitude, endPoint.magnitude))
            {
                Moving = true;
              //move the gameobject to the desired position
                girl.transform.position = Vector3.Lerp(this.transform.position, endPoint, 1 / (duration * (Vector3.Distance(this.transform.position, endPoint))));
            }
            //set the movement indicator flag to false if the endPoint and current gameobject position are equal
            else if (Mathf.Approximately(girl.transform.position.magnitude, endPoint.magnitude))
            {
                Moving = false;
                Debug.Log("I am here");
            }


            print(GetDistanceMeters(StartLat, StartLong, CurrentLat, CurrentLong));
        }
    }
}
