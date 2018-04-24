using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine.UI;

public class GPS : MonoBehaviour {

    public static GPS Instance { set; get; }

    public BasicMap basicMap;

    public GameObject girl;

    private float StartLong = 0;
    private float StartLat = 0;


    private float CurrentLat = 0;
    private float CurrentLong = 0;

    public Text text;

    private float DistanceTravelled = 0;

    private bool HasGps = false;
	// Use this for initialization
	void Start ()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
        StartCoroutine(StartLocationService());
        basicMap.Initialize(new Vector2d(StartLat, StartLong), 16);

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
        StartLat = Input.location.lastData.latitude;
        StartLong = Input.location.lastData.longitude;
        
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
            CurrentLat = Input.location.lastData.latitude;
            CurrentLong = Input.location.lastData.longitude;
            DistanceTravelled = (float)GetDistanceMeters(StartLat, StartLong, CurrentLat, CurrentLong);

            float xx = GetDistanceMeters(0, StartLong, 0, CurrentLong);
            float zz = GetDistanceMeters(StartLat, 0, CurrentLat, 0);

            Vector3 pos = new Vector3(xx, 0, zz);

            girl.transform.position = pos;




            print(GetDistanceMeters(StartLat, StartLong, CurrentLat, CurrentLong));
            text.text = DistanceTravelled.ToString();
        }
    }
}
