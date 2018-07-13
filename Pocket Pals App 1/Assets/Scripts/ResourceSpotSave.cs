using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResourceSpotSave
{
    public float lat =0.0f ;
    public float lon = 0.0f;
    public float cooldown = 0.0f;

    public ResourceSpotSave(Vector2 latlon, float cd)
    {
        lat = latlon.x;
        lon = latlon.y;
        cooldown = cd;
    }

    public bool IsMatch(Vector2 latlon, float threshold)
    {
        return Vector2.Distance(latlon, new Vector2(lat, lon)) < threshold;
    }
}
