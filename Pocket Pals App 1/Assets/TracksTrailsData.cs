using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TracksTrailsData
{
    public float t =0;
}

[System.Serializable]
public class TrackData
{
    public float distTarget = 1000.0f;
    public float distCurrent = 0.0f;
    public int ID = 0;
}
[System.Serializable]
public class TracksAndTrailsPreset
{
    public int ID = 0;
    public float distMin = 1000.0f;
    public float distMax = 1000.0f;
    public string PocketPalName = "None";
    public float PocketPalID = 0;
    public Sprite identifier;

    public float expMin = 4000.0f;
    public float expMax = 6000.0f;

    public float GetCompletedExp()
    {
        return Random.Range(expMin, expMax);
    }

    public float GetRequiredDistance()
    {
        return Random.Range(distMin, distMax);
    }
}